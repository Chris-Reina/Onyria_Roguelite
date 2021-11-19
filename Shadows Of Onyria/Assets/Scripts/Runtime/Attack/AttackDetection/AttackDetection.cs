using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public abstract class AttackDetection : ScriptableObject
    {
        public abstract HashSet<IGridEntity> Detect(IEntity attacker);
    }
}
