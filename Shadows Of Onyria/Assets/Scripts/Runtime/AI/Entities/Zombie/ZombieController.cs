using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace DoaT.AI
{
    public class ZombieController : EnemyEntity, IPath, IUpdate, IUnloadable, IPausable
    {
#pragma warning disable
        public override event Action<IGridEntity> OnPositionChange;
        public override event Action OnDeath;
        public event Action<IGridEntity> OnMove;
        public event Action OnPathUpdated;
#pragma warning restore

        public string currentState = "";
        private readonly StateManager _stateManager = new StateManager();

        private ZombieModel _model;

        public int CurrentIndex { get; set; }
        public Path Path { get; set; }
        public PathRequest PathRequest { get; private set; }


        public ZombieModel Model => _model;
        public override float Durability => _model.health.HealthAmount;
        public override Vector3 FeedbackDisplacement { get; } = new Vector3(0f, 2f, 0f);
        public override Rigidbody Rigidbody => _model.rigidbody;
        public override EnemySpawner Manager => _manager;
        
        public override bool IsDead => _model.IsDead;
        public List<GameObject> Neighbours => GetNeighbours();
        public List<SteeringBehaviour> _steeringBehaviours;
        public float neighbourRadiusDetection = 2f;

        public Collider normalCollider;

        //public Rigidbody normalRigidbody;
        public Collider[] ragdollColliders;
        public Rigidbody[] ragdollRigidbodys;
        public Animator animator;
        public bool DebugMe;
        public VisualEffect hitEffectPrefab;
        public Transform effectSpawnPoint;
        public AudioCue damageCue;
        public AudioCue deathCue;
        [HideInInspector] public Material _myMat;

        

        private void Awake()
        {
            _model = GetComponent<ZombieModel>();
            ragdollColliders = GetComponentsInChildren<Collider>().Where(x => x != normalCollider).ToArray();
            ragdollRigidbodys = GetComponentsInChildren<Rigidbody>(); //.Where(x => x != normalRigidbody).ToArray();

            foreach (var rrb in ragdollRigidbodys)
            {
                rrb.isKinematic = true;
            }

            foreach (var rc in ragdollColliders)
            {
                rc.enabled = false;
            }

            var map = new Dictionary<Type, State>
            {
                {typeof(ZombieIdle), new ZombieIdle(_stateManager, this)},
                {typeof(ZombieDeath), new ZombieDeath(_stateManager, this)},
                {typeof(ZombieMovement), new ZombieMovement(_stateManager, this)},
                {typeof(ZombieUseAbility), new ZombieUseAbility(_stateManager, this)}
            };

            _stateManager.SetStates(map, map[typeof(ZombieIdle)]);

            _steeringBehaviours = GetComponents<SteeringBehaviour>().ToList();
            PathRequest = new PathRequest(SetPath, GetPosition, GetTargetPosition);
        }

        private void Start()
        {
            World.SpatialGrid.AddGridEntity(this);
            
            _myMat = _model.zombieRenderer.material;
            _model.OnDeath += () => OnDeath?.Invoke();
            
            ExecutionSystem.AddUpdate(this);
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
        }

        public void OnUpdate()
        {
            if (_model.IsMoving)
                OnMove?.Invoke(this);
            _stateManager.Update();

            Rotate(_model.RotationDirectionNormalized);
        }

        public bool IsTargetVisible()
        {
            if (Vector3.Distance(Model.targetData.Position, Position) > Model.data.viewDistance) return false;
            if (Vector3.Angle(transform.forward, (Model.targetData.Position - Position)) > Model.data.viewAngle / 2) return false;

            var ray = new Ray(Model.RayInitPosition, transform.forward);//(Model.targetData.Position + new Vector3(0, 0.5f, 0)) - Model.RayInitPosition);????

            if (Physics.Raycast(ray, out var hit, Model.data.viewDistance, LayersUtility.PLAYER_DETECTION_SIGHT_MASK))
            {
                if (hit.collider.gameObject.layer == LayersUtility.PLAYER_MASK_INDEX)
                    return true;
            }
            
            return false;
        }


        private void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            var targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up), Vector3.up);


            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                targetRotation,
                Model.RotationSpeedCurve * Time.deltaTime);
        }

        public override void TakeDamage(AttackInfo info, DamageType type)
        {
            if (_model.health.IsInvulnerable) return;

            if (!_manager.EnemyDetected)
            {
                _manager.RaiseEnemyDetection(World.GetPlayer());
            }
            
            var hitEffect = Instantiate(hitEffectPrefab);

            hitEffect.transform.position = effectSpawnPoint.position;
            hitEffect.SetVector3("AttackDirection", (Position - info.attacker.Position).normalized);
            hitEffect.Play();
            AudioSystem.PlayCue(damageCue);
            var damage = info.damage.Random();
            _model.health.TakeDamage(damage);
            EventManager.Raise(EntityEvents.OnDamageTaken, transform.position, this, damage, false, info);
            
            // if(_model.health.IsDead)
            //     OnDeath?.Invoke();
        }

        private List<GameObject> GetNeighbours()
        {
            return Physics.OverlapSphere(transform.position, neighbourRadiusDetection, LayersUtility.ENTITY_MASK,
                    QueryTriggerInteraction.Collide)
                .Where(x => x.gameObject != gameObject)
                .Select(x => x.gameObject)
                .ToList();
        }

        public AttackInfo GetAttackInfo()
        {
            return new AttackInfo(new FloatRange(_model.data.minDamage, _model.data.maxDamage), 0f, 1f, this);
        }
        
        private void SetPath(Path p)
        {
            Path = p;
            OnPathUpdated?.Invoke();
        }

        private Vector3 GetPosition() => Position;
        private Vector3 GetTargetPosition() => _model.targetData.Position;
        public void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, false);
        }

        private void OnDestroy()
        {
            ExecutionSystem.RemoveUpdate(this, true);
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
        }

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }
    }

}