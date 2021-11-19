using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoaT.AI
{
    public class DemonAttack : State
    {
        private const float WAIT_TIMER_MIN = 0.1f;
        private const float WAIT_TIMER_MAX = 0.25f;

        public event Action<float> OnAttackStart;
        public event Action OnAttackEnd;

        private readonly DemonController _c;
        private readonly DemonModel _m;

        private readonly TimerHandler _handler = new TimerHandler();
        private readonly LocalTimer _localTimer = new LocalTimer();

        private bool _cancelAttack = false;

        public DemonAttack(StateManager stateManager, DemonController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;

            _c.OnDamageTaken += OnDamageTakenImpl;
        }

        public override void Awake()
        {
            //DebugManager.Log($"Entering {GetType()}");

            TimerManager.SetTimer(_handler, Effect, Random.Range(WAIT_TIMER_MIN, WAIT_TIMER_MAX));

            _m.RotationPoint = _m.targetData.Position;
        }

        public override void Execute()
        {
            _localTimer.Handle();

            if (_m.IsDead)
            {
                _stateManager.SetState<DemonDeath>();
                return;
            }

            if (_cancelAttack)
            {
                _cancelAttack = false;
                _stateManager.SetState<DemonStagger>();
                return;
            }
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");
            _c.ShouldRotate = true;
            _localTimer.Clear();
            if (_handler.IsActive) TimerManager.CancelTimer(_handler);
        }
        
        private void EndAttack()
        {
            TimerManager.SetTimer(_c.attackCooldownHandler, _m.data.attack.cooldown);
            _stateManager.SetState<DemonMovement>();
            OnAttackEnd?.Invoke();
        }
        
        private void Effect()
        {
            var attack = _m.data.attack.attackExecution;
            OnAttackStart?.Invoke(attack.AnimationSpeed);

            _localTimer.SetTimer(_handler, EndAttack, attack.Duration);

            foreach (var effect in attack.effects)
            {
                _localTimer.SetTimedAction
                (
                    _handler,
                    x => x >= attack.EffectDurationByEffect(effect),
                    () => effect.Execute(_c.GetAttackInfo(), _c, attack)
                );
            }
        }

        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if (!_m.Shielded && _stateManager.IsActualState<DemonAttack>() && onHealth)
                _cancelAttack = true;
        }
    }

}