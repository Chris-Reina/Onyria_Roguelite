using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    /// <summary>
    /// Holds the specific data of a Soul, it will be raised by Resources.Load() and then filtered by a LINQ Query
    /// using the SoulType as desired parameter.
    /// </summary>
    [Serializable, CreateAssetMenu(menuName = "Data/Souls/Data", fileName = "Soul Data")]
    public class SoulData : ScriptableObject
    {
        public SoulType type;
        public List<SoulLevelData> levelsData;
    }
}