using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public abstract class AttackExecution : ScriptableObject
    {
        public abstract void Execute(HashSet<IGridEntity> targets, AttackInfo info);
    }
}
