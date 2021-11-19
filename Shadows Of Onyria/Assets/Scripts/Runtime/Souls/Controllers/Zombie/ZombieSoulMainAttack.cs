using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Zombie/Main Attack", fileName = "Controller_ZombieMainAttack")]
    public class ZombieSoulMainAttack : CharacterController, IClone<BaseMainAttackBehaviour>
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


