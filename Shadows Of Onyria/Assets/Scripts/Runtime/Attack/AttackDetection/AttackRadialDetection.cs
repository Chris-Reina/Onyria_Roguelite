using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT
{
    [System.Serializable, CreateAssetMenu(menuName = "Abilities/Attacks/Attack Effect Detection/Radial",fileName = "Radial Detection")]
    public class AttackRadialDetection : AttackDetection
    {
        public float radius = 3f;
        [Range(0f, 360f)] public float amplitude = 90f;
        private float UsableAmplitude => amplitude / 2;
        
        public override HashSet<IGridEntity> Detect(IEntity attacker)
        {
            var a = attacker.Position;
            var a1 = a + new Vector3(-radius, -radius, -radius);
            var a2 = a + new Vector3(radius, radius, radius);

            bool Filter(Vector3 v)
            {
                if ((v - a).sqrMagnitude <= radius * radius)
                {
                    if (Vector3.Angle(attacker.Transform.forward, (v - a).SetY(0)) <= UsableAmplitude)
                    {
                        return true;
                    }
                }
        
                return false;
            }
            
            return World.SpatialGrid.Query(a1, a2, Filter)
                                    .Aggregate(new HashSet<IGridEntity>(), (set, entity) =>
                                    {
                                        if (!set.Contains(entity))
                                            set.Add(entity);
                                        
                                        return set;
                                    });
        }
    }
}
