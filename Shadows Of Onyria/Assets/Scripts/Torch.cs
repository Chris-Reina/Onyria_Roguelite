using UnityEngine;
using UnityEngine.VFX;

namespace DoaT
{
    public class Torch : MonoBehaviourInit, IPausable
    {
        [SerializeField] private VisualEffect _fireVFX;

        public override float OnInitialization()
        {
            ExecutionSystem.AddPausable(this);

            return 1f;
        }

        public void OnGamePause()
        {
            _fireVFX.playRate = 0;
        }

        public void OnGameResume()
        {
            _fireVFX.playRate = 1;
        }

        private void OnDestroy()
        {
            ExecutionSystem.RemovePausable(this);
        }
    }
}