using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Attacks/Attack Effect Execution/Damage", fileName = "Damage Execution")]
    public class AttackDamageExecution : AttackExecution
    {
        public DamageType damageType;
        public LayerMask targetMask;
        public bool singleTarget = false;

        public override void Execute(HashSet<IGridEntity> targets, AttackInfo info)
        {
            if (!targets.Any()) return;
            if (singleTarget)
            {
                foreach (var target in targets)
                {
                    if (!targetMask.ContainsLayer(target.GameObject.layer)) continue;
                    if (target is IAttackable attackable)
                    {
                        attackable.TakeDamage(info, damageType);
                        break;
                    }
                }
            }
            else
            {
                foreach (var target in targets)
                {
                    if (!targetMask.ContainsLayer(target.GameObject.layer)) continue;
                    if (target is IAttackable attackable)
                    {
                        attackable.TakeDamage(info, damageType);
                    }
                }
            }

        }
    }

}