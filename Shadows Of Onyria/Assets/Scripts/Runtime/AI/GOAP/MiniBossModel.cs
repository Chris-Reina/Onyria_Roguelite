using System;
using System.Collections.Generic;
using System.Linq;
using DoaT;
using DoaT.AI;
using UnityEngine;

public enum PlayerLocation
{
    Unknown,
    Known,
    Visible
}

public class MiniBossModel : MonoBehaviour
{
    public Action OnDeath;
    
    [Header("Streamed Data")]
    public PlayerStreamedData targetData;

    private MiniBossController _c;
    
    #pragma warning disable
    public event Action<IGridEntity> OnPositionChange;
    #pragma warning restore
    
    #region GOAP
    
    //BOOL
    public bool IsPlayerAlive => !targetData.IsDead; 
    
    //FLOAT
    public float DistanceToTarget => Vector3.Distance(targetData.Position, Position);
    public float Health => _health;
    public float RangeAttackCooldown => _rangeAttackCooldown;
    public float RechargeManaCooldown => _rechargeManaCooldown;
    
    //INT
    public int Mana => Mathf.FloorToInt(_mana);
    
    
    //STRING/ENUM
    public PlayerLocation PlayerLocation
    {
        get
        {
            if (IsTargetVisible())
                return PlayerLocation.Visible;
            
            if (targetLastKnownPosition != targetLastKnownPositionDefault)
                return PlayerLocation.Known;

            return PlayerLocation.Unknown;
        }
    }

    #endregion
    
    #region IEntity

    public string Name => name;
    public float Durability => _health / maxHealth;
    public Vector3 FeedbackDisplacement { get; } = new Vector3(0f, 3f, 0f);
    public Transform Transform => transform;
    public Rigidbody Rigidbody { get; } = default;
    public GameObject GameObject => gameObject;
    public bool IsEnemy => true;
    public bool IsDead => Math.Abs(_health) < 0.0001f;

    #endregion

    [Header("Fixed Stats")]
    public MiniBossModelData data;
    public float meleeDistance = 2f;
    public float meleeAttackDamage = 10;
    public float rangeDistance = 10f;
    public int rangeAttackManaCost = 7;
    public float rangeAttackCooldownMax = 3f;
    public float rangeAttackDamage = 50;
    public int maxMana = 10;
    public float maxHealth = 100f;
    public float rechargeManaCooldownMax;
    public bool inCombat = false;
    public Vector3 targetLastKnownPositionDefault = new Vector3(-10000, -10000, -10000);
    [HideInInspector] public Vector3 targetLastKnownPosition;

    [Header("Mutable Stats")]
    [SerializeField] private float _mana = default;
    [SerializeField] private float _health = default;
    [SerializeField] private float _rangeAttackCooldown = default;
    [SerializeField] private float _rechargeManaCooldown = default;

    [Header("DebugManager")] 
    public bool debug = false;
    public Color viewDistanceColor;
    public Color meleeDistanceColor;
    public Color rangeDistanceColor;
    
    [Header("Components")]
    public new Collider collider;
    public LootTable lootTable;
    public Pathfinder pathfinder;
    public MiniBossAnimationOverrider animationOverrider;
    public List<SteeringBehaviour> steeringBehaviours;

    private Vector3 _positionLastFrame;

    #region Properties & Events

    public bool Dissolve { get; set; }
    public Vector3 RotationPoint { get; set; }
    public float MaxDegreesDelta => data.rotationSpeed * 360f;

    public float RotationSpeedCurve =>
        data.rotationCurve.Evaluate(Vector3.Angle(RotationDirection, transform.forward) / 180) * 360 *
        data.rotationSpeed;
    public Vector3 RotationDirectionNormalized => RotationDirection.normalized;
    public Vector3 RayInitPosition => transform.position + new Vector3(0, 0.5f, 0);
    private Vector3 RotationDirection => RotationPoint - transform.position;

    public Vector3 Position => transform.position;
    public List<GameObject> Neighbours => GetNeighbours();
    public bool IsAtMaxMana => Math.Abs(_mana - maxMana) < 0.001f;
    
    
    
    #endregion

    private void Awake()
    {
        targetLastKnownPosition = targetLastKnownPositionDefault;
        pathfinder = FindObjectOfType<Pathfinder>();
        steeringBehaviours = GetComponents<SteeringBehaviour>().ToList();
        animationOverrider = GetComponent<MiniBossAnimationOverrider>();
    }

    private void Start()
    {
        //ExecutionSystem.AddUpdate(this);
    }

    public void OnUpdate()
    {
        if (_rangeAttackCooldown > 0 && inCombat) _rangeAttackCooldown -= Time.deltaTime;
        if (_rechargeManaCooldown > 0 && inCombat) _rechargeManaCooldown -= Time.deltaTime;
        
        Rotate(RotationDirectionNormalized);
    }

    private void LateUpdate()
    {
        _positionLastFrame = transform.position;
    }
    
    private void OnDrawGizmos()
    {
        if (!debug || !data) return;

        var pos = transform.position;
        
        Gizmos.color = viewDistanceColor;
        Gizmos.DrawWireSphere(pos, data.viewDistance);
        
        Gizmos.color = meleeDistanceColor;
        Gizmos.DrawWireSphere(pos, meleeDistance);
        
        Gizmos.color = rangeDistanceColor;
        Gizmos.DrawWireSphere(pos, rangeDistance);
    }
    
    public bool IsTargetVisible(out Vector3 position)
    {
        var visible = IsTargetVisible();
        position = visible ? targetData.Position : targetLastKnownPositionDefault;
        return visible;
    }
    public bool IsTargetVisible()
    {
        var tPosition = targetData.Position;
        
        if (Vector3.Distance(tPosition, Position) > data.viewDistance)
        {
            return false;
        }
        
        var ray = new Ray(RayInitPosition,
            (tPosition + new Vector3(0, 0.5f, 0)) - RayInitPosition);

        if (Physics.Raycast(ray, out var hit, data.viewDistance, LayersUtility.PLAYER_DETECTION_SIGHT_MASK))
        {
            if (hit.collider.gameObject.layer == LayersUtility.PLAYER_MASK_INDEX)
            {
                return true;
            }
        }

        return false;
    }
    
    public void TakeDamage(AttackInfo info, DamageType type)
    {
        var amount = info.damage.Random();
        _health = Mathf.Max(0, _health - amount);
        EventManager.Raise(EntityEvents.OnDeath, transform.position, this, amount, false);
    }
    
    private void Rotate(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up), Vector3.up);
        

        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            targetRotation,
            RotationSpeedCurve * Time.deltaTime);
    }
    
    private List<GameObject> GetNeighbours()
    {
        return Physics.OverlapSphere(transform.position, data.neighbourRadiusDetection, LayersUtility.ENTITY_MASK, QueryTriggerInteraction.Collide)
            .Where(x => x.gameObject != gameObject)
            .Select(x => x.gameObject)
            .ToList();
    }
#pragma warning disable
    public event Action OnPathUpdated;
    public int CurrentIndex { get; set; }
    public Path Path { get; set; }
    public PathRequest PathRequest { get; }
#pragma warning restore

    public void SetRangeAttackOnCooldown()
    {
        _rangeAttackCooldown = rangeAttackCooldownMax;
    }
    
    public void SetRechargeOnCooldown()
    {
        _rechargeManaCooldown = rechargeManaCooldownMax;
    }

    public void SetRechargeOnCooldown(float f)
    {
        _rechargeManaCooldown = f;
    }

    public void SpendMana()
    {
        _mana = Mathf.Max(0, _mana - rangeAttackManaCost);
        DebugManager.Log("Mana Spent : "+ _mana);
    }

    public void AddMana()
    {
        _mana = Mathf.Min(maxMana, _mana + 3 * Time.deltaTime);
        DebugManager.Log("Mana Gained : "+ _mana);
    }

    public void Despawn()
    {
        collider.enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        //ExecutionSystem.RemoveUpdate(this);
    }


    public Vector3 Direction { get; }
}