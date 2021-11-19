#pragma warning disable CS0649
using DoaT.Attributes;
using System;
using System.Collections;
using UnityEngine;

namespace DoaT
{
    public sealed class TheodenController : MonoBehaviourInit, IAttackable, IUnloadable, IPausable
    {
        #region Events
        //Movement/Position Events
        public event Action<IGridEntity> OnPositionChange;
        public event Action<bool> OnMove;
        //Dash Events
        public event Action OnDashBegin;
        public event Action OnDashEnd;
        //Main Attack Events
        public event Action<int, float> OnAttackStepBegin;
        public event Action OnAttackStepEnd;
        public event Action OnAttackEnd;
        public event Action OnAttackBegin;
        public event Action OnAttackCancel;
        //Range Attack Events
        public event Action<float> OnRangeAttackBegin; //animationSpeed
        public event Action OnRangeAttackRelease;
        public event Action<float> OnRangeAttackCharging; //size
        public event Action OnRangeChargeReady;
        public event Action OnRangeCancel;
        //Other Events
#pragma warning disable CS0067
        public event Action OnDeath;
        public event Action<Vector3> OnDamageReceived;
#pragma warning restore CS0067
        #endregion
        
        #region Properties
        #region IEntity
        public Vector3 Position => transform.position;
        public Vector3 Direction => transform.forward;
        public Vector3 FeedbackDisplacement => new Vector3(0f, 2f, 0f);
        public Rigidbody Rigidbody => _m.rigidbody;
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;
        public string Name => name;
        public float Durability => _m.health.HealthAmount;
        public bool IsEnemy => false;
        public bool IsDead => _m.health.IsDead;
        #endregion

        public bool IsDashing => _currentController == _dashController;
        
        public Vector3 MovementDirection => UseUpdatedMovementDirection ? CurrentMovementDirection : _input.LastMovementInput;
        public Vector3 AimingDirection => _input.AimInput;

        private bool UseUpdatedMovementDirection => true;
        private Vector3 CurrentMovementDirection => _input.MovementInput;
        #endregion
        
        [SerializeField] private CursorGameSelection _selection;
        [SerializeField] private WallDetector _detector;
        
        private TheodenModel _m;
        private TheodenView _v;
        
        private IController _currentController;
        private IController _defaultController;
        
        private CharacterInput _input;
        private DashController _dashController;
        private MovementController _movementController;
        private MainAttackController _mainAttackController;
        private RangeAttackController _rangeAttackController;

        private bool _disposed = false;

        private void Awake()
        {
            _m = GetComponent<TheodenModel>();
            _v = GetComponent<TheodenView>();
        }
        public override float OnInitialization()
        {
            _input = new CharacterInputPC(_selection, this);
            _dashController = new DashController(this, _detector);
            _movementController = new MovementController(this, _detector);
            _mainAttackController = new MainAttackController(this);
            _rangeAttackController = new RangeAttackController(this, StartCoroutine, StopCoroutine);

            _movementController.OnMove += (x) => OnMove?.Invoke(x);
            
            _dashController.OnDashBegin += () => OnDashBegin?.Invoke();
            _dashController.OnDashEnd += () => OnDashEnd?.Invoke();

            _mainAttackController.OnAttackStepBegin += (x, y) => OnAttackStepBegin?.Invoke(x, y);
            _mainAttackController.OnAttackStepEnd += () => OnAttackStepEnd?.Invoke();
            _mainAttackController.OnAttackEnd += () => OnAttackEnd?.Invoke();
            _mainAttackController.OnAttackBegin += () => OnAttackBegin?.Invoke();
            _mainAttackController.OnAttackCancel += () => OnAttackCancel?.Invoke();

            _rangeAttackController.OnAttackBegin += x => OnRangeAttackBegin?.Invoke(x);
            _rangeAttackController.OnAttackEnd += () => OnRangeAttackRelease?.Invoke();
            _rangeAttackController.OnPreparingFire += x => OnRangeAttackCharging?.Invoke(x);
            _rangeAttackController.OnReadyToFire += () => OnRangeChargeReady?.Invoke();
            _rangeAttackController.OnAttackCancel += () => OnRangeCancel?.Invoke();


            _dashController.Initialize(_m.Data, _input);
            _movementController.Initialize(_m.Data, _input);
            _mainAttackController.Initialize(_m.Data, _input);
            _rangeAttackController.Initialize(_m.Data, _input);

            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
            GameManager.Current.OnLoadingComplete += OnMoveInit;
            GameManager.Current.OnLoadingComplete += GainControlInit;
            
            _currentController = _defaultController = _movementController;
            StartCoroutine(PositionChangeChecker());
            
            _m.health.OnDamageTaken += OnDamageTaken;
            _m.health.OnHeal += OnHeal;
            
            World.SpatialGrid.AddGridEntity(this);

            OnDeath += () => _m.darkness.Empty();
            
            return 1f;
        }
        
        private IEnumerator PositionChangeChecker()
        {
            var lastPosition = Position;
            
            while (true)
            {
                if (ExecutionSystem.Paused)
                {
                    yield return new WaitUntil(() => ExecutionSystem.Paused == false);
                }
                
                GenerateDarkness(_m.darknessPerSecond * Time.deltaTime);
                
                var newPosition = Position;
                if(lastPosition != newPosition)
                    OnPositionChange?.Invoke(this);
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public void Kill() => _m.health.Kill();
        public Health GetHealth() => _m.health;
        public Darkness GetDarkness() => _m.darkness;
        public void SetInvulnerable(bool invulnerable) => _m.health.IsInvulnerable = invulnerable;
        public void TakeDamage(AttackInfo info, DamageType type)
        {
            var amount = info.damage.Random();
            amount = _m.GetProcessedDamage(amount, type);
            _m.health.TakeDamage(amount);

            OnDamageReceived?.Invoke((transform.position - info.attacker.Position).normalized);
        }
        public void SetNewRotationTarget(Vector3 direction, bool shouldSnap, bool forceExecution = false)
        {
            _movementController.CurrentRotation = new RotationData(direction, shouldSnap);
            if (forceExecution)
            {
                _movementController.RotateForced();
            }
        }
        public void GenerateDarkness(float amount)
        {
            if (!SceneContext.UseDarkness) return;
            
            _m.darkness.Gain(amount);
        }
        public void SpendDarkness(float f)
        {
            if (!SceneContext.UseDarkness) return;
            _m.darkness.Spend(f);
        }
        public bool CanSpendDarkness(float f)
        {
            return _m.darkness.CanSpend(f);
        }

        public void RequestControl(IController controller)
        {
            if (controller.IsOnCooldown
                || ReferenceEquals(controller, _currentController)
                || _currentController.IsLocked
                || (!ReferenceEquals(controller, _dashController) && !ReferenceEquals(_currentController, _defaultController))
            ) return;
            
            _currentController.Interrupt();
            _currentController.ControlLost();
            _currentController = controller;
            _currentController.ControlGained();
        }
        public void ControllerFinished(IController controller)
        {
            
            if (!ReferenceEquals(controller, _currentController)) return;
            _currentController.ControlLost();
            
            if (_disposed)
            {
                _currentController = null;
                return;
            }
                
            _currentController = _defaultController;
            _currentController.ControlGained();
        }
        public void InterruptController()
        {
            if (_currentController == null) return;
            _currentController.ControlLost();
            _currentController = _defaultController;
            _currentController.ControlGained();
        }

        #region EventWrappers

        private void OnDamageTaken(float value)
        {
            EventManager.Raise(PlayerEvents.OnDamageTaken, transform.position, value, this);
        }
        private void OnHeal(float value)
        {
            EventManager.Raise(PlayerEvents.OnHeal, transform.position, value, this);
        }
        private void OnMoveInit() => OnMove?.Invoke(false);
        private void GainControlInit() => _currentController.ControlGained();

        #endregion
        
        public void Unload(params object[] objects)
        {
            Dispose();
            StopAllCoroutines();
            
            _input.Unload();
            _dashController.Dispose();
            _movementController.Dispose();
            _mainAttackController.Dispose();
            _rangeAttackController.Dispose();
        }
        private void Dispose(params object[] objects)
        {
            _disposed = true;
            _currentController.ControlLost();
        }
        private void OnDestroy()
        {
            if (!Application.isPlaying) return;
            
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
            GameManager.Current.OnLoadingComplete -= OnMoveInit;
            GameManager.Current.OnLoadingComplete -= GainControlInit;
            StopAllCoroutines();
        }   
        
        public void OnGamePause() { }
        public void OnGameResume() { }

        public bool IsCurrentController(IController controller) => ReferenceEquals(controller, _currentController);



    }
}
#pragma warning restore CS0649