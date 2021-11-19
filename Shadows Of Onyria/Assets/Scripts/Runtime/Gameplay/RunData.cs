using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Gameplay/Run", fileName = "Run Data")]
    public class RunData : ScriptableObject
    {
        public List<Stage> run;
    }
}
