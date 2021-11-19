using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(fileName = "Modifier_SkullBodyModifier", menuName = "Abilities/Concrete/Skull/Body Modifier")]
    public class SkullSoulBody : CharacterModifier, IAttributeModifier
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