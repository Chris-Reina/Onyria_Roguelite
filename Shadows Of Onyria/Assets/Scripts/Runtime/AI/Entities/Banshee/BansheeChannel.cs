using System;

namespace DoaT.AI
{
    public class BansheeChannel : State
    {
        private const float WAIT_TIMER_MIN = 0.1f;
        private const float WAIT_TIMER_MAX = 0.25f;

        public event Action<float> OnAttackStart;
        public event Action OnAttackEnd;
        
        private BansheeController _c;
        private BansheeModel _m;
        
        public BansheeChannel(StateManager stateManager, BansheeController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;
        }

        public override void Awake()
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void Sleep()
        {
            throw new System.NotImplementedException();
        }
    }
}