using UnityEngine;

namespace DoaT.AI
{
    public class CrystalSkullStagger : State
    {
        private static readonly int StunSpeedMultiplier = Animator.StringToHash("StunSpeedMultiplier");
        
        private readonly CrystalSkullController _c;
        private readonly CrystalSkullModel _m;

        private bool _reset;
        private float _timer;

        public CrystalSkullStagger(StateManager stateManager, CrystalSkullController controller) : base(stateManager)
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
                _stateManager.SetState<CrystalSkullDeath>();
                return;
            }
            
            if (_reset)
            {
                _stateManager.ResetState<CrystalSkullStagger>();
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer > 0) return;

            _stateManager.SetState<CrystalSkullMovement>();
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");
            _reset = false;
            _timer = 0;
        }
        
        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if(!_m.Shielded && _stateManager.IsActualState<CrystalSkullStagger>() && onHealth)
                _reset = true;
        }
    }
}