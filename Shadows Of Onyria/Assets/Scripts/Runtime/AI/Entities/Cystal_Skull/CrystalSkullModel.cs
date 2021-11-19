using DoaT.Attributes;
using UnityEngine;

namespace DoaT.AI
{
    public class CrystalSkullModel : MonoBehaviour, IEnemyModel
    {
        public CrystalSkullModelData data;
        public PlayerStreamedData targetData;
        public Pathfinder pathfinder;
        public Animator animator;
        public Health health;
        
        public Vector3 RotationPoint { get; set; }

        public bool IsMoving { get; set; }
        public bool IsDead => health.IsDead;

        public float RotationSpeedCurve =>
            data.rotationCurve.Evaluate(Vector3.Angle(RotationDirection, transform.forward) / 180) * 360 *
            data.rotationSpeed;

        public Vector3 RotationDirectionNormalized => RotationDirection.normalized;
        public Vector3 RayInitPosition => transform.position + new Vector3(0, 0.5f, 0);
        private Vector3 RotationDirection => RotationPoint - transform.position;

        public float speedMultiplier = 1f;
        public float damageTakenMultiplier = 1f;
        public float MovementSpeed => data.movementSpeed * speedMultiplier;

        public Renderer skullsRenderer;

        public bool _shielded;
        public bool Shielded => _shielded;
        
        private void Awake()
        {
            pathfinder = FindObjectOfType<Pathfinder>();
            animator = GetComponentInChildren<Animator>();
            
            _shielded = Random.value <= 0.3f;
        }
    }
}