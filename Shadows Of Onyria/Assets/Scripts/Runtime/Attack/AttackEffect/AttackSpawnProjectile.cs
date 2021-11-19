using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    [CreateAssetMenu(menuName = "Abilities/Attacks/Attack Effect Execution/Spawn Projectile", fileName = "Spawn Projectile")]
    public class AttackSpawnProjectile : AttackExecution
    {
        public enum ProjectileDamageMode
        {
            Absolute,
            Multiplicative
        }
        
        public Projectile prefab;
        public ProjectileDamageMode damageMode;
        public float damageMultiplier = 1f;
        public float damageAbsolute = 15;
        public float speed;
        public float forwardDisplacement;
        public float upDisplacement;
        public float rightDisplacement;
        public bool distanceBased;
        public float maxDistance;
        public Vector3 spawnAngleDisplacement;

        public override void Execute(HashSet<IGridEntity> targets, AttackInfo info)
        {
            switch (damageMode)
            {
                case ProjectileDamageMode.Absolute:
                    info.damage = new FloatRange(damageAbsolute);
                    break;
                case ProjectileDamageMode.Multiplicative:
                    info.damage *= damageMultiplier;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            var position = info.attacker.Position;
            var forward = info.attacker.Transform.forward * forwardDisplacement;
            var up = info.attacker.Transform.up * upDisplacement;
            var right = info.attacker.Transform.right * rightDisplacement;

            var displacement = forward + up + right;
            var spawnPosition = position + displacement;

            var ints = Instantiate(prefab, spawnPosition, Quaternion.identity);
            if (spawnAngleDisplacement != Vector3.zero) forward = Quaternion.Euler(spawnAngleDisplacement) * forward;
            ints.Initialize(forward, speed);
            ints.SetInfo(distanceBased ? new RangeAttackInfo(info, maxDistance) : new RangeAttackInfo(info));
        }
    }
}
