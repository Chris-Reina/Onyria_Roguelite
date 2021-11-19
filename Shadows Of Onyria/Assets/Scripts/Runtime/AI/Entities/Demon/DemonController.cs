using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT.AI
{
public class DemonController : EnemyEntity, IPath, IUpdate, IPausable
    {
#pragma warning disable CS0067
        public override event Action<IGridEntity> OnPositionChange;
        public override event Action OnDeath;
        
        public event Action OnMoveBegin;
        public event Action OnMoveEnd;
        public event Action<Vector3, bool> OnDamageTaken;
        public event Action OnHeal;
        public event Action<float> OnAttackBegin;
        public event Action OnAttackEnd;
        public event Action OnPathUpdated;
#pragma warning restore CS0067
        
        //[SerializeField] private bool _debugMe;
        [SerializeField] private float neighbourRadiusDetection = 2f;

        private readonly StateManager _stateManager = new StateManager();
        private Coroutine _positionChecker;
        private bool _lastMovingState;

        public int CurrentIndex { get; set; }
        public Path Path { get; set; }
        public PathRequest PathRequest { get; private set; }
        public List<GameObject> Neighbours => GetNeighbours();
        
        public override float Durability => Model.health.HealthAmount;
        public override Vector3 FeedbackDisplacement { get; } = new Vector3(0f, 2f, 0f);
        public override EnemySpawner Manager => _manager;
        public override Rigidbody Rigidbody => null;
        public override bool IsDead => Model.IsDead;
        
        public DemonModel Model { get; private set; }
        public DemonView View { get; private set; }
        public List<SteeringBehaviour> SteeringBehaviours { get; private set; }

        public readonly TimerHandler attackCooldownHandler = new TimerHandler();
        public bool IsAttackOnCooldown => attackCooldownHandler.IsActive;
        public bool ShouldRotate { get; set; } = true;

        
        private void Awake()
        {
            Model = GetComponent<DemonModel>();
            View = GetComponent<DemonView>();

            View.SetRagdollEffect(false);

            var idle = new DemonIdle(_stateManager, this);
            var death = new DemonDeath(_stateManager, this);
            var movement = new DemonMovement(_stateManager, this);
            var attack = new DemonAttack(_stateManager, this);
            var stun = new DemonStagger(_stateManager, this);
            
            attack.OnAttackStart += AttackStartEvent;
            attack.OnAttackEnd += OnAttackEndEvent;
            
            death.OnDeath += OnDeathEvent;
            //stun.OnStun += OnStunEvent;

            var map = new Dictionary<Type, State>
            {
                {typeof(DemonIdle), idle},
                {typeof(DemonDeath), death},
                {typeof(DemonMovement), movement},
                {typeof(DemonAttack), attack},
                {typeof(DemonStagger), stun}
            };

            _stateManager.SetStates(map, map[typeof(DemonIdle)]);

            SteeringBehaviours = GetComponents<SteeringBehaviour>().ToList();
            PathRequest = new PathRequest(SetPath, GetPosition, GetTargetPosition);
        }

        private void Start()
        {
            World.SpatialGrid.AddGridEntity(this);
            ExecutionSystem.AddUpdate(this);
            
            _positionChecker = StartCoroutine(PositionChangeChecker());
        }

        public void OnUpdate()
        {
            CheckMovement(Model.IsMoving);
            
            if (Model.IsMoving)
            {
                OnPositionChange?.Invoke(this);
            }
            _stateManager.Update();
        
            if(ShouldRotate) Rotate(Model.RotationDirectionNormalized);
        }

        private void CheckMovement(bool isMoving)
        {
            switch (_lastMovingState)
            {
                case false when isMoving:
                    OnMoveBeginEvent();
                    break;
                case true when !isMoving:
                    OnMoveStopEvent();
                    break;
            }

            _lastMovingState = isMoving;
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
            if (Model.health.IsInvulnerable) return;
            
            var damage = info.damage.Random();

            // if (Model.shield.CanBlock(damage))
            // {
            //     Model.shield.TakeDamage(damage);
            //     EventManager.Raise(EntityEvents.OnDamageTaken, transform.position, this, damage, false, info);
            //     OnDamageTakenEvent((Position - info.attacker.Position).normalized, false);
            //     return;
            // }

            Model.health.TakeDamage(damage);
            
            EventManager.Raise(EntityEvents.OnDamageTaken, transform.position, this, damage, false, info);
            OnDamageTakenEvent((Position - info.attacker.Position).normalized, true);
        }
        
        private IEnumerator PositionChangeChecker()
        {
            var lastPosition = Position;
            
            while (true)
            {
                var newPosition = Position;
                if(lastPosition != newPosition)
                    OnPositionChange?.Invoke(this);
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        // public void Despawn()
        // {
        //     View..enabled = false;
        //     GetComponent<Rigidbody>().useGravity = false;
        //     UpdateManager.RemoveUpdate(this);
        // }

        private void SetPath(Path p)
        {
            Path = p;
            OnPathUpdated?.Invoke();
        }

        private Vector3 GetPosition() => Position;
        private Vector3 GetTargetPosition() => Model.targetData.Position;
        
        private List<GameObject> GetNeighbours()
        {
            return Physics.OverlapSphere(transform.position, neighbourRadiusDetection, LayersUtility.ENTITY_MASK, QueryTriggerInteraction.Collide)
                          .Where(x => x.gameObject != gameObject)
                          .Select(x => x.gameObject)
                          .ToList();
        }

        public AttackInfo GetAttackInfo()
        {
            var data = Model.data.attack;
            return new AttackInfo(data.damage, data.criticalChance, data.criticalMultiplier, this);
        }

        private void OnMoveBeginEvent() => OnMoveBegin?.Invoke();
        private void OnMoveStopEvent() => OnMoveEnd?.Invoke();
        private void OnDeathEvent() => OnDeath?.Invoke();
        private void OnDamageTakenEvent(Vector3 x, bool y)
        {
            if(!Manager.EnemyDetected) Manager.RaiseEnemyDetection(World.GetPlayer());
            OnDamageTaken?.Invoke(x, y);
        }
        private void OnHealEvent() => OnHeal?.Invoke();
        private void AttackStartEvent(float x) => OnAttackBegin?.Invoke(x);
        private void OnAttackEndEvent() => OnAttackEnd?.Invoke();
        
        private void OnDestroy()
        {
            ExecutionSystem.RemoveUpdate(this, true);
            StopCoroutine(_positionChecker);
            World.SpatialGrid.RemoveGridEntity(this);
        }
        
        

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }
    }
}
