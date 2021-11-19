using System;
using UnityEngine;

namespace DoaT
{
    public class DeathWall : MonoBehaviour, IEntity
    {
#pragma warning disable
        public event Action<IGridEntity> OnPositionChange;
#pragma warning restore

        public LayerMask mask;

        private void OnTriggerEnter(Collider other)
        {
            if (mask.ContainsLayer(other.gameObject.layer))
            {
                World.GetPlayer().Kill();
            }
        }

        public Vector3 Position => Vector3.zero;
        public Vector3 Direction => Vector3.up;
        public string Name => "DeathWall";
        public Transform Transform => transform;
        public Rigidbody Rigidbody => null;
        public GameObject GameObject => gameObject;
        public float Durability => 1f;
        public bool IsEnemy => false;
        public bool IsDead => false;
    }
}