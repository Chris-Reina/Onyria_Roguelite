using System;

namespace DoaT
{
    public interface IAttackBehaviour : IBehaviour
    {
        event Action OnSpendDarkness;
        
        event Action<object[]> OnAttackStepBegin;
        event Action<object[]> OnAttackUpdate;
        event Action<object[]> OnAttackStepEnd;
        event Action<object[]> OnAttackBegin;
        event Action<object[]> OnAttackEnd;
        
        public SoulTypeData SoulType { get; }
        AttackInputType AttackInputMask { get; }
        AttackInputType AttackRequestInputMask { get; }
        bool NeedsUpdate { get; }
        
        IAttackBehaviour Initialize(Func<AttackInfo> info, IEntity attacker);
        void Update();
        
        void SendImpulsePress();
        void SendImpulseSustained();
        void SendImpulseRelease();

        void Interrupt();
    }
}