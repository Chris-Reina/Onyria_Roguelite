using System;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class Spikes : MonoBehaviour, IEntity
{
    public Transform platform;
    public Transform deployedPosition;
    public Transform awaitPosition;
    public LayerMask detectionMask;
    public LayerMask damageMask;

    [Header("Audio Cues")] 
    public AudioCue OnTrigger;
    public AudioCue OnDeploy;
    public AudioCue OnRetreat;

    public TimerHandler _handler = new TimerHandler();
    public TimerHandler _handler2 = new TimerHandler();
    
    public float deployDelay;
    public float deploySpeed;
    public float retreatDelay;
    public float retreatSpeed;
    public DamageType dmgType;

    private readonly HashSet<IAttackable> _damagedEntities = new HashSet<IAttackable>();
    private bool _isDamaging = false;
    private bool _isDeploying = false;
    private bool _isRetracting = false;
    private bool _hasTriggered = false;
    private AudioDurationTracker _source;

    private bool IsMoving => _isDeploying || _isRetracting;

    private void Update()
    {
        if (_isDeploying)
        {
            platform.position = Vector3.Lerp(awaitPosition.position, deployedPosition.position, _handler2.Progress);
        }
        else if (_isRetracting)
        {
            platform.position = Vector3.Lerp(deployedPosition.position, awaitPosition.position, _handler2.Progress);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsMoving) return;
        
        if(!_hasTriggered)
        {
            if (detectionMask.ContainsLayer(other.gameObject.layer))
            {
                AudioSystem.PlayCue(OnTrigger);
                _hasTriggered = true;
                TimerManager.SetTimer(_handler, StartDeployment, deployDelay);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isDamaging) return;

        if (damageMask.ContainsLayer(other.gameObject.layer))
        {
            var attack = other.gameObject.GetComponent<IAttackable>();
            Damage(attack);
        }
    }

    private void Damage(IAttackable attackable)
    {
        if (_damagedEntities.Contains(attackable)) return;
        
        _damagedEntities.Add(attackable);
        
        attackable.TakeDamage(new AttackInfo(new FloatRange(10f,10f), 0f,1f, this), dmgType);
    }

    private void StartDeployment()
    {
        _isDeploying = true;
        _isDamaging = true;
        AudioSystem.PlayCue(OnDeploy);
        TimerManager.SetTimer(_handler2, SetDeployed, deploySpeed);
    }

    private void SetDeployed()
    {
        _isDeploying = false;
        TimerManager.SetTimer(_handler, StartRetraction, retreatDelay);
    }

    private void StartRetraction()
    {
        _isRetracting = true;
        _isDamaging = false;
        _source = AudioSystem.PlayCue(OnRetreat);
        TimerManager.SetTimer(_handler2, SetRetracted, retreatSpeed);
        _damagedEntities.Clear();
    }

    private void SetRetracted()
    {
        _isRetracting = false;
        _hasTriggered = false;
        
        if(_source.gameObject.activeSelf) _source.AudioSource.Stop();
    }    

    #pragma warning disable
    public Vector3 Position => transform.position;
    public Vector3 Direction => transform.forward;
    public event Action<IGridEntity> OnPositionChange;
    public string Name => gameObject.name;
    public Transform Transform => transform;
    public Rigidbody Rigidbody => null;
    public GameObject GameObject => gameObject;
    public float Durability { get; }= 1;
    public bool IsEnemy { get; } = false;
    public bool IsDead { get; } = false;
    #pragma warning restore
}
