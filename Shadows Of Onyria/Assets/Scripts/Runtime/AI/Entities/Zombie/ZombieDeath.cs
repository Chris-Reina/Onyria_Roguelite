using UnityEngine;

namespace DoaT.AI
{
    public class ZombieDeath : State
    {
        private readonly ZombieController _zc;
        private readonly ZombieModel _model;

        private static readonly int DissolveStage = Shader.PropertyToID("_DissolveStage");

        //private List<Item> _items;
        private readonly TimerHandler _dissolveTimer = new TimerHandler();
        private float _dissolveDuration = 2f;
        private float _timeUntilDissolve = 5f;

        public ZombieDeath(StateManager stateManager, ZombieController controller) : base(stateManager)
        {
            _zc = controller;
            _model = controller.Model;
        }

        public override void Awake()
        {
            if (_zc.DebugMe) DebugManager.Log($"Entering {GetType()}");
            _zc.currentState = GetType().ToString();
            //EventManager.Raise(EntityEvents.OnDeath, _zc.Position);
            //_items = _model.lootTable.DropItems();
            
            //EventManager.Raise(GameEvents.SpawnZombieSoul, _zc.Position);
            
            RewardSystem.Gold(_model.data.gold);

            AudioSystem.PlayCue(_zc.deathCue);
            _model.health.IsInvulnerable = true;
            _zc.normalCollider.enabled = false;
            TimerManager.SetTimer(_dissolveTimer, () => _model.OnDeath?.Invoke(), _dissolveDuration,
                _timeUntilDissolve);
            TimerManager.SetTimer(
                new TimerHandler(),
                () => EventManager.Raise(EntityEvents.OnDeath, _zc.Position + _zc.Transform.forward * -1.2f),
                _timeUntilDissolve * 0.55f
            );

            _zc.animator.enabled = false;
            
            foreach (var rc in _zc.ragdollColliders)
            {
                rc.enabled = true;
            }
            foreach (var rrb in _zc.ragdollRigidbodys)
            {
                rrb.isKinematic = false;
            }
            foreach (var rrb in _zc.ragdollRigidbodys)
            {
                rrb.AddExplosionForce(2000f, _zc.Position + _zc.Transform.forward, 5f);
            }
        }

        public override void Execute()
        {
            if (_dissolveTimer.IsActive)
            {
                _zc._myMat.SetFloat(DissolveStage, _dissolveTimer.Progress);
            }
        }

        public override void Sleep()
        {
            if (_zc.DebugMe) DebugManager.Log($"Exiting {GetType()}");
            _zc.currentState = "";
        }
    }
}
