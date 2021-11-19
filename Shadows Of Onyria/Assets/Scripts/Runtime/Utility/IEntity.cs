using UnityEngine;

namespace DoaT
{
    public interface IEntity : IGridEntity
    {
        string Name { get; }
        Transform Transform { get; }
        Rigidbody Rigidbody { get; }
        float Durability { get; }
        bool IsEnemy { get; }
        bool IsDead { get; }
    }
}