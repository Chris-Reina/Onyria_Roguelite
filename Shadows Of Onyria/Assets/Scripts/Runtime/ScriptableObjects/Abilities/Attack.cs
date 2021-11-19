using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(fileName = "Attack", menuName = "Abilities/Attacks/Attack")]
    public class Attack : ScriptableObject
    {
        public AttackInputType BeginInputType = AttackInputType.None;
        public AttackInputType EndInputType = AttackInputType.None;
        
        public float Duration;
        public float DamageMultiplier = 1f;
        public float Cooldown = 0f;
        public float ComboWaitTime = 0.1f;
        public List<AnimationClip> Animations;
        [Range(0.001f, 1f)] public float ComboLockPercentage;
        [Range(0.001f, 1f)] public float ComboUnlockPercentage;

        public virtual AttackInputType AllInputTypes => BeginInputType | EndInputType;
        public virtual float AnimationSpeed => Animations[0].length / Duration;
        public virtual float LockComboDuration => Duration * ComboLockPercentage;
        public virtual float UnlockComboDuration => Duration * ComboUnlockPercentage;

        public List<AttackEffect> effects = new List<AttackEffect>();

        public virtual float GetAnimationSpeed(int index)
        {
            return Animations[index].length / Duration;
        }

        public virtual float EffectDurationByIndex(int index)
        {
            if (index >= effects.Count)
                throw new IndexOutOfRangeException();

            return Duration * effects[index].effectExecutionTime;
        }

        public virtual float EffectDurationByEffect(AttackEffect effect)
        {
            if (!effects.Contains(effect))
                throw new IndexOutOfRangeException();

            return Duration * effect.effectExecutionTime;
        }
    }

    [Flags]
    public enum AttackInputType
    {
        None = 0,
        Press = 1,
        Sustain = 2,
        Release = 4,
    }
}

