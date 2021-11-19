using System;
using DoaT.AI;
using DoaT.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieModel : MonoBehaviour, IEnemyModel
{
    public Action OnDeath;
    
    public ZombieController controller;
    public ZombieView view;

    public ZombieModelData data;

    public Health health;
    public new Rigidbody rigidbody;
    public Pathfinder pathfinder;
    public Animator animator;
    public ZombieAnimationOverrider animationOverrider;
    public Vector3 lastKnownPosition;
    public PlayerStreamedData targetData;
    public new Collider collider;

    private Vector3 _rotationPoint;
    private Vector3 _positionLastFrame;
    

    public Action<float> TriggerAttackCallback;
    public Vector3 RotationPoint
    {
        get => _rotationPoint;
        set =>
            //DebugManager.Log($"Rotation Point Changed from {_rotationPoint} to {value}");
            _rotationPoint = value;
    }

    public bool IsMoving { get; set; }
    public bool IsDead => health.IsDead;
    public float MaxDegreesDelta => data.rotationSpeed * 360f;

    public float RotationSpeedCurve =>
        data.rotationCurve.Evaluate(Vector3.Angle(RotationDirection, transform.forward) / 180) * 360 *
        data.rotationSpeed;
    public Vector3 RotationDirectionNormalized => RotationDirection.normalized;
    public Vector3 RayInitPosition => transform.position + new Vector3(0, 0.5f, 0);
    private Vector3 RotationDirection => RotationPoint - transform.position;

    public bool Dissolve { get; set; } = true;

    public float speedMultiplier = 1f;
    public float damageTakenMultiplier = 1f;
    public float MovementSpeed => data.movementSpeed * speedMultiplier;
    public Renderer zombieRenderer;

    public bool _shielded;
    public bool Shielded => _shielded;
    
    private void Awake()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
        
        _shielded = Random.value <= 0.3f;
    }

    private void LateUpdate()
    {
        _positionLastFrame = transform.position;
    }
}