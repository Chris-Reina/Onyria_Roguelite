using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Controller/Base Locomotion", fileName = "Controller_BaseLocomotion")]
    public class BaseLocomotion : CharacterController, IClone<TheodenLocomotionBehaviour>
    {
        public TheodenLocomotionBehaviour Clone()
        {
            return new TheodenLocomotionBehaviour();
        }

        public override T GetController<T>()
        {
            return Clone() as T;
        }
    }
}
