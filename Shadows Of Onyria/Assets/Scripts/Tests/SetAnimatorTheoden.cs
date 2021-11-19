using UnityEngine;

namespace DoaT
{
    public class SetAnimatorTheoden : MonoBehaviour //TEST
    {
        public RuntimeAnimatorController weapon;
        public RuntimeAnimatorController noWeapon;

        public Animator animator;
        public SceneFlag swordPulledFromGround;

        private void Start()
        {
            if (!swordPulledFromGround.Value)
            {
                animator.runtimeAnimatorController = noWeapon;
            }
        }
    }
}
