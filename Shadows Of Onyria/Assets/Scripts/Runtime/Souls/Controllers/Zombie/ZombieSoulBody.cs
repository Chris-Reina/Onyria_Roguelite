using System;
using UnityEngine;

namespace DoaT
{
    [Serializable,
     CreateAssetMenu(fileName = "Modifier_ZombieBodyModifier", menuName = "Abilities/Concrete/Zombie/Body Modifier")]
    public class ZombieSoulBody : CharacterModifier, IAttributeModifier
    {
        public DamageResistance resistance;
        [Range(0f, 1f)] public float percentage;

        public void Modify(TheodenData data)
        {
            var res = data.resistances[resistance];
            data.resistances[resistance] = res - percentage;
        }

        public void Restore(TheodenData data)
        {
            var res = data.resistances[resistance];
            data.resistances[resistance] = res + percentage;
        }
    }
}