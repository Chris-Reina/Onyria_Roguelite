using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Attacks/Attack Effect Execution/Darkness Generation",
                                   fileName = "Darkness Generation Execution")]
    public class DarknessGeneratorExecution : AttackExecution
    {
        private enum DarknessGeneration
        {
            PerTarget,
            Global
        }
        
        [SerializeField] private float amount = 5;
        [SerializeField] private DarknessGeneration generationType = DarknessGeneration.PerTarget;
        

        public override void Execute(HashSet<IGridEntity> targets, AttackInfo info)
        {
            if (targets.Count <= 1) return;

            TheodenController controller = default;
            foreach(var target in targets)
            {
                if (target is TheodenController theoden)
                {
                    controller = theoden;
                }
            }
            targets.Remove(controller);

            var total = generationType switch
            {
                DarknessGeneration.PerTarget => targets.Count * amount,
                DarknessGeneration.Global => amount,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            controller.GenerateDarkness(total);
        }
    }
}
