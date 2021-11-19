using UnityEngine;

namespace DoaT
{
    public interface IAttackable : IEntity
    {
        Vector3 FeedbackDisplacement { get; }
        void TakeDamage(AttackInfo info, DamageType type);
    }
}