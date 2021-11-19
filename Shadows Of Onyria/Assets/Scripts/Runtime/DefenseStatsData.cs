using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Data/Gameplay/Damage/Defense Stats Data", fileName = "Defense Stats Data")]
public class DefenseStatsData : ScriptableObject
{
    public DamageResistance physicalResistance;
    public DamageResistance poisonResistance;

    public List<DamageResistance> GetList()
    {
        var newList = new List<DamageResistance>
        {
            physicalResistance, 
            poisonResistance,
        };

        return newList;
    }
}
