using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Skull/Main Attack", fileName = "Controller_SkullMainAttack")]
    public class SkullSoulMainAttack : CharacterController, IClone<BaseMainAttackBehaviour>
    {
        public SoulType soulType;
        public List<Attack> attackChain;

        public override T GetController<T>()
        {
            var clone = Clone();
            clone.SetAttackChain(attackChain);
            return clone as T;
        }

        public BaseMainAttackBehaviour Clone()
        {
            return new BaseMainAttackBehaviour {soulType = new SoulTypeData(soulType)};
        }
    }
}