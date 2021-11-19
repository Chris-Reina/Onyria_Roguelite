using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoaT.AI
{
    public class BansheeSummon : State
    {
        private const float WAIT_TIMER_MIN = 0.1f;
        private const float WAIT_TIMER_MAX = 0.25f;
        
        public event Action<float> OnAttackStart;
        public event Action OnAttackEnd;
        
        private BansheeController _c;
        private BansheeModel _m;
        
        private readonly TimerHandler _handler = new TimerHandler();
        private readonly LocalTimer _localTimer = new LocalTimer();

        private bool _cancelAttack = false;
        private bool _isSpawning;

        public BansheeSummon(StateManager stateManager, BansheeController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;
            
            _c.OnDamageTaken += OnDamageTakenImpl;
        }

        public override void Awake()
        {
            TimerManager.SetTimer(_handler, Effect, Random.Range(WAIT_TIMER_MIN, WAIT_TIMER_MAX));

            _m.RotationPoint = _m.targetData.Position;
        }

        public override void Execute()
        {
            _localTimer.Handle();

            if (_m.IsDead)
            {
                _stateManager.SetState<BansheeDeath>();
                return;
            }

            if (_cancelAttack)
            {
                if (_isSpawning)
                {
                    _isSpawning = false;
                    TimerManager.SetTimer(_c.summonCooldownHandler, _m.data.summon.cooldown);
                }
                _cancelAttack = false;
                _stateManager.SetState<BansheeStagger>();
                return;
            }
        }

        public override void Sleep()
        {
            _c.ShouldRotate = true;
            _localTimer.Clear();
            if (_handler.IsActive) TimerManager.CancelTimer(_handler);
        }
        
        private void EndAttack()
        {
            TimerManager.SetTimer(_c.summonCooldownHandler, _m.data.summon.cooldown);
            _stateManager.SetState<BansheeMovement>();
            OnAttackEnd?.Invoke();
            _isSpawning = false;
        }
        
        private void Effect()
        {
             _isSpawning = true;

            var animationLength = _m.data.summon.animation.length;
            OnAttackStart?.Invoke(1f/*/animationLength*/);

            var steps = _m.data.summon.enemyAmount;
            var spawnTime = animationLength / steps;
            
            _localTimer.SetTimer(_handler, EndAttack, animationLength);
            
            for (int i = 1; i < steps; i++)
            { 
                var accum = i;
                _localTimer.SetTimedAction
                (
                    _handler,
                    x => x >= accum * spawnTime,
                    SpawnEnemy
                );
            }
        }
        
        private void SpawnEnemy()
        {
            _c.Manager.SpawnEnemy(_m.data.summon.invokableEnemy.prefab,_c.Position, _m.data.summon.radius);
        }

        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if (!_m.Shielded && _stateManager.IsActualState<BansheeSummon>() && onHealth)
                _cancelAttack = true;
        }
    }
}