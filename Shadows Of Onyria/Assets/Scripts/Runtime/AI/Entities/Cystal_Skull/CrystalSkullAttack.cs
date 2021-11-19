using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoaT.AI
{
    [Serializable]
    public class CrystalSkullAttack : State
    {
        private const float WAIT_TIMER_MIN = 0.3f;
        private const float WAIT_TIMER_MAX = 0.8f;
        
        public event Action OnAttackBegin;
        public event Action<float> OnAttackEffect;
        public event Action OnAttackEnd;
        
        private readonly CrystalSkullController _c;
        private readonly CrystalSkullModel _m;

        public TimerHandler _handler = new TimerHandler();
        public LocalTimer localTimer = new LocalTimer();

        private bool _cancelAttack = false;

        public CrystalSkullAttack(StateManager stateManager, CrystalSkullController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;

            _c.OnDamageTaken += OnDamageTakenImpl;
        }

        private Vector3 _persistentDirection;
        private Vector3 _initialPosition;

        private Action _executableImplementation;
        private static readonly int PreAttackSpeedMultiplier = Animator.StringToHash("PreAttackSpeedMultiplier");

        public override void Awake()
        {
            //DebugManager.Log($"Entering {GetType()}");

            _initialPosition = _c.Position;
            _executableImplementation = WaitingImpl;

            TimerManager.SetTimer(_handler, SetPreAttack, Random.Range(WAIT_TIMER_MIN, WAIT_TIMER_MAX));
            
            _m.RotationPoint = _m.targetData.Position;
        }

        public override void Execute()
        {
            localTimer.Handle();

            if (_m.IsDead)
            {
                _stateManager.SetState<CrystalSkullDeath>();
                return;
            }

            if (_cancelAttack)
            {
                _cancelAttack = false;
                _stateManager.SetState<CrystalSkullStagger>();
                return;
            }
            
            _executableImplementation?.Invoke();
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");
            _c.ShouldRotate = true;
            localTimer.Clear();
            if(_handler.IsActive) TimerManager.CancelTimer(_handler);
        }

        private void SetPreAttack()
        {
            _persistentDirection = (_m.targetData.Position - _c.Position).normalized;
            
            if (Vector3.Distance(_m.targetData.Position , _c.Position) <= _m.data.attack.detection.radius)
            {
                SetAttackEffect();
                return;
            }
            
            
            _executableImplementation = PreAttackImpl;
            OnAttackBegin?.Invoke();
        }
        private void SetAttackMovement()
        {
            _c.ShouldRotate = false;
            _executableImplementation = MovementImpl;
        }
        private void SetAttackEffect()
        {
            _executableImplementation = null;
            EffectImpl();
        }
        private void EndAttack()
        {
            TimerManager.SetTimer(_c.attackCooldownHandler, _m.data.attack.cooldown);
            _stateManager.SetState<CrystalSkullMovement>();
            OnAttackEnd?.Invoke();
        }

        private void WaitingImpl()
        {
            _m.RotationPoint = _m.targetData.Position;
        }
        private void PreAttackImpl()
        {
            if (_handler.IsActive) return;

            TimerManager.SetTimer(_handler, SetAttackMovement, _m.data.attack.prepareAnimation.length / _m.animator.GetFloat(PreAttackSpeedMultiplier));
            _executableImplementation = null;

        }
        private void MovementImpl()
        {
            var t = _c.transform;

            var effectRange = _m.data.attack.detection.radius;
            var tuple = new Tuple<float, float>(effectRange, _m.data.attack.detection.amplitude);
            
            if (AIUtility.IsTargetVisible(_m.targetData.Position, _m.RayInitPosition, tuple, _c.Position, _persistentDirection))
            {
                SetAttackEffect();
                return;
            }
            
            var hits = Physics.SphereCastAll(_c.Position, 0.25f, _persistentDirection, 1.25f,
                LayersUtility.PLAYER_DETECTION_MOVEMENT_MASK, QueryTriggerInteraction.Collide).Length;

            if (hits > 0)
            {
                SetAttackEffect();
                return;
            }

            if (Vector3.Distance(_c.Position, _initialPosition) > _m.data.attack.movementRange)
            {
                SetAttackEffect();
                return;
            }

            t.position += _persistentDirection * (_m.data.attack.speed * Time.deltaTime);
        }
        private void EffectImpl()
        {
            var attack = _m.data.attack.attackExecution;
            OnAttackEffect?.Invoke(attack.AnimationSpeed);
            
            localTimer.SetTimer(_handler, EndAttack, attack.Duration);
                
            foreach (var effect in attack.effects)
            {
                localTimer.SetTimedAction
                (
                    _handler,
                    x => x >= attack.EffectDurationByEffect(effect),
                    () => effect.Execute(_c.GetAttackInfo(), _c, attack)
                );
            }
        }

        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if(!_m.Shielded && _stateManager.IsActualState<CrystalSkullAttack>() && onHealth)
                _cancelAttack = true;
        }
    }
}