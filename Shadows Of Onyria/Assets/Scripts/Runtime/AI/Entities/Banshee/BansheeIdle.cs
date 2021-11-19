using System;
using UnityEngine;

namespace DoaT.AI
{
    public class BansheeIdle : State
    {
        private readonly BansheeController _c;
        private readonly BansheeModel _m;
        
        private bool _cancelAttack = false;
            
        public BansheeIdle(StateManager stateManager, BansheeController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;
            
            _c.OnDamageTaken += OnDamageTakenImpl;
        }

        public override void Awake()
        {
            //DebugManager.Log($"Entering {GetType()}");
        }

        public override void Execute()
        {
            if (_m.IsDead)
            {
                _stateManager.SetState<BansheeDeath>();
                return;
            }
            
            if (_cancelAttack)
            {
                _cancelAttack = false;
                _stateManager.SetState<BansheeStagger>();
                return;
            }
            
            if (_c.ShouldIdle) return;
            
            if (_c.Manager.EnemyDetected)
            {
                if (!_c.IsSummonOnCooldown)
                {
                    _stateManager.SetState<BansheeSummon>();
                    return;
                }
        
                _stateManager.SetState<BansheeMovement>();
                return;
            }
            
            var tuple = new Tuple<float, float>(_m.data.viewDistance,_m.data.viewAngle);

            if (!AIUtility.IsTargetVisible(_m.targetData.Position, _m.RayInitPosition, tuple, _c.Position, _c.transform.forward)) return;
            
            _c.Manager.RaiseEnemyDetection(World.GetPlayer());
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");
        }
        
        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if (!_m.Shielded && _stateManager.IsActualState<BansheeIdle>() && onHealth)
                _cancelAttack = true;
        }
    }
}