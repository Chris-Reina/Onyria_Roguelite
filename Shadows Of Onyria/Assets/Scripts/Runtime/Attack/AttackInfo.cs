namespace DoaT
{
    public class AttackInfo
    {
        public readonly IEntity attacker;
        public float criticalChance;
        public float criticalMultiplier;
        public FloatRange damage;

        public AttackInfo(FloatRange damage, float criticalChance, float criticalMultiplier, IEntity attacker)
        {
            this.attacker = attacker;
            this.damage = damage;
            this.criticalChance = criticalChance;
            this.criticalMultiplier = criticalMultiplier;
        }
    }

    public class RangeAttackInfo : AttackInfo
    {
        public float maxDistance;

        public RangeAttackInfo(FloatRange damage, float criticalChance, float criticalMultiplier, IEntity attacker,
            float maxDistance = -1)
            : base(damage, criticalChance, criticalMultiplier, attacker)
        {
            this.maxDistance = maxDistance;
        }

        public RangeAttackInfo(AttackInfo info, float maxDistance = -1)
            : base(info.damage, info.criticalChance, info.criticalMultiplier, info.attacker)
        {
            this.maxDistance = maxDistance;
        }
    }
}
