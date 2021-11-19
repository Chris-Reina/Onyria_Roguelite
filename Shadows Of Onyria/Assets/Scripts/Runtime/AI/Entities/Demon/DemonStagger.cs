using UnityEngine;

namespace DoaT.AI
{
    public class DemonStagger : State
    {
        private static readonly int StunSpeedMultiplier = Animator.StringToHash("StunSpeedMultiplier");

        private readonly DemonController _c;
        private readonly DemonModel _m;

        private bool _reset;
        private float _timer;

        public DemonStagger(StateManager stateManager, DemonController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;

            _c.OnDamageTaken += OnDamageTakenImpl;
        }

        public override void Awake()
        {
            //DebugManager.Log($"Entering {GetType()}");
            _m.animator.SetFloat(StunSpeedMultiplier, _m.data.stunAnimation.length / _m.data.stunDuration);
            _timer = _m.data.stunDuration;
        }

        public override void Execute()
        {
            if (_m.IsDead)
            {
                _stateManager.SetState<DemonDeath>();
                return;
            }

            if (_reset)
            {
                _stateManager.ResetState<DemonStagger>();
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer > 0) return;

            _stateManager.SetState<DemonIdle>();
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");
            _reset = false;
            _timer = 0;
        }

        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if (!_m.Shielded && _stateManager.IsActualState<DemonStagger>() && onHealth)
                _reset = true;
        }
    }

}