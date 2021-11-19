using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [System.Serializable, CreateAssetMenu(menuName = "Abilities/Attacks/Attack Effect Detection/Self",fileName = "Self Detection")]
    public class AttackSelfDetection : AttackDetection
    {
        public override HashSet<IGridEntity> Detect(IEntity attacker)
        {
            return new HashSet<IGridEntity> { attacker };
        }
    }
}