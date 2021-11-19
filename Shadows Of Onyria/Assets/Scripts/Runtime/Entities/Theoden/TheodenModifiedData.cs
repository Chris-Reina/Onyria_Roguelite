using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public class TheodenModifiedData
    {
        public HashSet<IAttributeModifier> modifiers = new HashSet<IAttributeModifier>();
        public TheodenData data;

        public TheodenModifiedData(TheodenData source)
        {
            data = new TheodenData(source);
        }

        public void AddModifier(IAttributeModifier modifier)
        {
            if (modifiers.Contains(modifier)) return;
            modifiers.Add(modifier);
            modifier.Modify(data);
        }

        public void RemoveModifier(IAttributeModifier modifier)
        {
            if (!modifiers.Contains(modifier)) return;
            
            modifiers.Remove(modifier);
            modifier.Restore(data);
        }

        public float GetDamage(float amount, DamageType type)
        {
            var list = data.defenseData.GetList();
            foreach (var dmgRes in list)
            {
                if (dmgRes.resistantTo == type)
                {
                    amount *= data.GetMultiplier(dmgRes);
                    break;
                }
            }

            return amount;
        }
    }
}