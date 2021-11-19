using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [System.Serializable]
    public class TheodenData
    {
        public Dictionary<DamageResistance, float> resistances;
        
        [Header("Data")] public DefenseStatsData defenseData;
        
        [Header("Stats")] 
        public FloatRange baseDamage;
        public float criticalChance;
        public float criticalMultiplier;

        public LocomotionParameters locomotion;

        public DashParameters dash;
        
        public TheodenData() {}

        public TheodenData(TheodenData data)
        {
            defenseData = data.defenseData;
            baseDamage = data.baseDamage;
            criticalChance = data.criticalChance;
            criticalMultiplier = data.criticalMultiplier;

            locomotion = new LocomotionParameters(data.locomotion);
            dash = new DashParameters(data.dash);

            resistances = new Dictionary<DamageResistance, float>();
            foreach (var dmgRes in defenseData.GetList())
            {
                resistances.Add(dmgRes, 1f);
            }
        }

        public float GetMultiplier(DamageResistance dmgRes)
        {
            return resistances[dmgRes];
        }
    }
}
