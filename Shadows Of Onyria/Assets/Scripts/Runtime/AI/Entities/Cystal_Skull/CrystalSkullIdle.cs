using System;

namespace DoaT.AI
{
    public class CrystalSkullIdle : State
    {
        private readonly CrystalSkullController _c;
        private readonly CrystalSkullModel _m;

        public CrystalSkullIdle(StateManager stateManager, CrystalSkullController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;
        }

        public override void Awake()
        {
            //DebugManager.Log($"Entering {GetType()}");
        }

        public override void Execute()
        {
            if (_m.IsDead)
            {
                _stateManager.SetState<CrystalSkullDeath>();
                return;
            }
        
            if (_c.Manager.EnemyDetected)
            {
                _stateManager.SetState<CrystalSkullMovement>();
                return;
            }
            
            var tuple = new Tuple<float, float>(_m.data.viewDistance,_m.data.viewAngle);

            if (!AIUtility.IsTargetVisible(_m.targetData.Position, _m.RayInitPosition, tuple, _c.Position, _c.transform.forward)) return;
            
            _c.Manager.RaiseEnemyDetection(World.GetPlayer());
            _stateManager.SetState<CrystalSkullMovement>();
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");
        }
    }
}