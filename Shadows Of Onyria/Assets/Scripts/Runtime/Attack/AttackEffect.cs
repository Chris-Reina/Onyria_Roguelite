using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public class AttackEffect
    {
        [Range(0.001f, 1f)] public float effectExecutionTime = 1f;
        public AttackDetection detection;

        public List<AttackExecution> executions;
        public void Execute(AttackInfo info, IEntity attacker, Attack attack)
        {
            if (detection == null) return;
            if (executions == null || executions.Count == 0) return;

            var targets = detection.Detect(attacker);

            foreach (var execution in executions)
            {
                execution.Execute(targets, info);
            }
        }
    }

    // public readonly struct AttackEffectParams
    // {
    //     public readonly AttackInfo info;
    //     public readonly IEntity attacker;
    //     public readonly Attack attack;
    //
    //     public AttackEffectParams(AttackInfo info, IEntity attacker, Attack attack)
    //     {
    //         this.info = info;
    //         this.attacker = attacker;
    //         this.attack = attack;
    //     }
    // }
}
