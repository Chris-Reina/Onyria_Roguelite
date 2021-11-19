using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Skull/Range Attack", fileName = "Controller_SkullRangeAttack")]
    public class SkullSoulRangeAttack : CharacterController, IClone<BaseRangeAttackBehaviour>
    {
        public SoulType soulType;
        public Attack attack;
        public float chargeDuration;
        public float maxRange;

        public override T GetController<T>()
        {
            var clone = Clone();
            clone.SetAttack(attack)
                .SetChargeDuration(chargeDuration)
                .SetMaxRange(maxRange);
            return clone as T;
        }

        public BaseRangeAttackBehaviour Clone()
        {
            return new BaseRangeAttackBehaviour {soulType = new SoulTypeData(soulType)};
        }
    }
}