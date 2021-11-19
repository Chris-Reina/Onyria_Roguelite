using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(fileName = "DemonModelData", menuName = "Data/Entity/DemonModelData")]
    public class DemonModelData : ScriptableObject
    {
        [Range(0.1f, 2f)] public float rotationSpeed = 1f;
        [Range(0.0001f, 0.1f)] public float movementDetectionThreshold;
    
        public AnimationCurve rotationCurve;
        public float movementSpeed;
        public float nodeDetection;
        public float stunDuration;
        public AnimationClip stunAnimation;
        
        public DemonAttackParameters attack;
        
        public float viewDistance;
        public float viewAngle = 90f;

        public SoulType soulType;
        public int gold;
    }

    [Serializable]
    public class DemonAttackParameters
    {
        public Attack attackExecution;
        public float cooldown;
        
        public AttackRadialDetection detection;

        public FloatRange damage;
        public float criticalChance;
        public float criticalMultiplier;
    }
}