using System;
using UnityEngine;

namespace DoaT.AI
{
    [Serializable, CreateAssetMenu(fileName = "BansheeModelData", menuName = "Data/Entity/BansheeModelData")]
    public class BansheeModelData : ScriptableObject
    {
        [Range(0.1f, 2f)] public float rotationSpeed = 1f;
        [Range(0.0001f, 0.1f)] public float movementDetectionThreshold;
    
        public AnimationCurve rotationCurve;
        public float movementSpeed;
        public float nodeDetection;
        public float stunDuration;
        public AnimationClip stunAnimation;
        
        public BansheeSummonParameters summon;
        public BansheeChannelParameters channel;
        
        public float viewDistance;
        public float viewAngle = 90f;

        public SoulType soulType;
        public int gold;
    }
    
    [Serializable]
    public class BansheeSummonParameters
    {
        public AnimationClip animation;

        public float cooldown = 30f;
        public float radius = 2f;
        public int enemyAmount = 3;
        public Enemy invokableEnemy;
    }
    
    [Serializable]
    public class BansheeChannelParameters
    {
        public AnimationClip animation;

        public float cooldown = 25f;
        public float duration = 4f;
    }
}