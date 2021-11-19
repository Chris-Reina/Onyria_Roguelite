using System;
using DoaT;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "ZombieModelData", menuName = "Data/Entity/ZombieModelData")]
public class ZombieModelData : ScriptableObject
{
    [Range(0.1f, 2f)] public float rotationSpeed = 1f;
    [Range(0.0001f, 0.1f)] public float movementDetectionThreshold;
    
    public AnimationCurve rotationCurve;
    public float movementSpeed;
    public float nodeDetection;
    public Attack attack;

    public float viewDistance;
    public float viewAngle = 90f;
    public float minDamage;
    public float maxDamage;

    public SoulType soulType;
    public int gold;

    public Tuple<float, float> GetDamageRange()
    {
        return new Tuple<float, float>(minDamage, maxDamage);
    }
}
