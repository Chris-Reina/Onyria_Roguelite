using System;

namespace DoaT.AI
{
    public class DemonDeath : State
    {
        public event Action OnDeath;

        private readonly DemonController _c;
        private readonly DemonModel _m;
        private readonly DemonView _v;

        //private List<Item> _items;
        private readonly TimerHandler _dissolveTimer = new TimerHandler();
        private float _dissolveDuration = 2f;
        private float _timeUntilDissolve = 5f;

        public DemonDeath(StateManager stateManager, DemonController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;
            _v = controller.View;
        }

        public override void Awake()
        {
            //DebugManager.Log($"Entering {GetType()}");

            //EventManager.Raise(EntityEvents.OnDeath, _zc.Position);
            //_items = _model.lootTable.DropItems();

            EventManager.Raise(GameEvents.SpawnDemonSoul, _c.Position);

            RewardSystem.Gold(_m.data.gold);

            OnDeath?.Invoke();

            _v.SetRagdollEffect(true);
            TimerManager.SetTimer(
                new TimerHandler(),
                () => EventManager.Raise(EntityEvents.OnDeath, _c.Position + _c.Transform.forward * -1.2f),
                _timeUntilDissolve * 0.55f
            );
            TimerManager.SetTimer(_dissolveTimer, () => OnDeath?.Invoke(), _dissolveDuration, _timeUntilDissolve);

            _m.health.IsInvulnerable = true;
        }

        public override void Execute()
        {
            _v.SetDissolveStage(_dissolveTimer.Progress);
        }

        public override void Sleep()
        {
            //DebugManager.Log($"Exiting {GetType()}");
        }
    }

}