using System;
using DoaT.Attributes;
using UnityEngine;

namespace DoaT
{
    public abstract class EnemyEntity : MonoBehaviour, IAttackable
    {
        #pragma warning disable CS0067
        public abstract event Action<IGridEntity> OnPositionChange;
        public abstract event Action OnDeath;
        #pragma warning restore CS0067

        public virtual string Name => name;
        public virtual bool IsEnemy => true;
        public virtual Vector3 Position => transform.position;
        public virtual Vector3 Direction => transform.forward;
        public virtual Transform Transform => transform;
        public virtual GameObject GameObject => gameObject;

        public abstract bool IsDead { get; }
        public abstract float Durability { get; }
        public abstract Rigidbody Rigidbody { get; }
        public abstract Vector3 FeedbackDisplacement { get; }

        public abstract EnemySpawner Manager { get; }
        [SerializeField] protected EnemySpawner _manager;

        public virtual void SetManager(EnemySpawner manager)
        {
            _manager = manager;
        }
        public abstract void TakeDamage(AttackInfo info, DamageType type);
    }
}
