#pragma warning disable CS0067

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT 
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Controller/Base Main Attack", fileName = "Controller_BaseMainAttack")]
    public class BaseMainAttack : CharacterController, IClone<BaseMainAttackBehaviour>
    {
        public SoulType soulType;
        public List<Attack> attackChain;

        public override T GetController<T>()
        {
            var clone = Clone();
            clone.SetAttackChain(attackChain);
            return clone as T;
        }

        public BaseMainAttackBehaviour Clone()
        {
            return new BaseMainAttackBehaviour {soulType = new SoulTypeData(soulType)};
        }
    }

    public class BaseMainAttackBehaviour : IAttackBehaviour
    {
        public SoulTypeData soulType;
        public event Action OnSpendDarkness;

        public event Action<object[]> OnActionCancel;
        public event Action<object[]> OnUpdateRotationRequest;
        
        public event Action<object[]> OnAttackStepBegin;
        public event Action<object[]> OnAttackStepEnd;
        
        public event Action<object[]> OnAttackBegin;
        public event Action<object[]> OnAttackUpdate;
        public event Action<object[]> OnAttackEnd;
        
        public SoulTypeData SoulType => soulType;
        public bool IsLocked => false;
        public bool IsComboLocked { get; private set; }
        public bool IsOnCooldown => _cooldownTimer.IsActive;
        public bool NeedsUpdate => true;

        public AttackInputType AttackRequestInputMask => _attackChain[0].BeginInputType;
        public AttackInputType AttackInputMask
        {
            get
            {
                var flag = AttackInputType.None;
                for (int i = 0; i < _attackChain.Count; i++)
                {
                    flag |= _attackChain[i].AllInputTypes;
                }

                return flag;
            }
        }
        
        private List<Attack> _attackChain;
        private Func<AttackInfo> _attackInfoFunc;
        private AttackInfo AttackInfo => _attackInfoFunc?.Invoke();
        private IEntity _attacker;
        private bool _inputRecorded = false;
        private bool _awaitInput;
        private bool _endSequenceNextFrame = false;
        private int _index = 0;

        private readonly TimerHandler _timer = new TimerHandler();
        private readonly TimerHandler _cooldownTimer = new TimerHandler();

        public void SetAttackChain(List<Attack> attackChain)
        {
            _attackChain = attackChain;
        }

        public IAttackBehaviour Initialize(Func<AttackInfo> info, IEntity attacker)
        {
            _attackInfoFunc += info;
            _attacker = attacker;

            OnAttackEnd += (x) => TimerManager.SetTimer(_cooldownTimer, _attackChain[_attackChain.Count - 1].Cooldown);
            OnActionCancel += (x) => TimerManager.SetTimer(_cooldownTimer, _attackChain[_attackChain.Count - 1].Cooldown);

            return this;
        }

        public void SendImpulsePress()
        {
            _inputRecorded = true;
        }

        public void SendImpulseSustained() { }
        public void SendImpulseRelease() { }
        public void Interrupt()
        {
            _index = 0;
            _inputRecorded = false;
            _awaitInput = false;
            if (_timer.IsActive)
            {
                TimerManager.CancelTimer(_timer);
            }

            if (IsComboLocked) IsComboLocked = false;

            OnActionCancel?.Invoke(new object[0]);
        }

        public void Update()
        {
            if (_endSequenceNextFrame)
            {
                _endSequenceNextFrame = false;
                EndSequence();
                return;
            }

            if (_awaitInput && _inputRecorded)
            {
                EndStep();
            }

            if (!_inputRecorded || _timer.IsActive) return;
            _inputRecorded = false;

            OnAttackBegin?.Invoke(new object[0]);
            ExecuteAttack();
        }

        private void EndStep()
        {
            if(!_awaitInput) OnAttackStepEnd?.Invoke(new object[0]);

            if (!_inputRecorded)
            {
                _awaitInput = true;
                return;
            }

            _awaitInput = false;
            _inputRecorded = false;
            _index += 1;

            if (_index >= _attackChain.Count)
            {
                _endSequenceNextFrame = true;
                return;
            }

            ExecuteAttack();
        }

        private void EndSequence()
        {
            _index = 0;
            _inputRecorded = false;
            _awaitInput = false;
            if (_timer.IsActive)
            {
                TimerManager.CancelTimer(_timer);
            }

            if (IsComboLocked) IsComboLocked = false;
            
            OnAttackEnd?.Invoke(new object[0]);
        }

        private void ExecuteAttack()
        {
            var attack = _attackChain[_index];

            if (_index == _attackChain.Count - 1)
            {
                TimerManager.SetTimer(_timer, EndSequence, attack.UnlockComboDuration);
                
                foreach (var effect in attack.effects)
                {
                    TimerManager.SetTimedAction
                    (
                        _timer,
                        x => x >= attack.EffectDurationByEffect(effect),
                        () => effect.Execute(AttackInfo, _attacker, attack)
                    );
                }
                
                TimerManager.SetTimedAction(_timer, x => x >= attack.LockComboDuration, () => IsComboLocked = true);
            }
            else
            {
                TimerManager.SetTimer(_timer, EndSequence, attack.Duration + attack.ComboWaitTime);
                
                foreach (var effect in attack.effects)
                {
                    TimerManager.SetTimedAction
                    (
                        _timer,
                        x => x >= attack.EffectDurationByEffect(effect),
                        () => effect.Execute(AttackInfo, _attacker, attack)
                    );
                }

                TimerManager.SetTimedAction(_timer, x => x >= attack.Duration, EndStep);
                TimerManager.SetTimedAction(_timer, x => x >= attack.LockComboDuration, () => IsComboLocked = true);
                TimerManager.SetTimedAction(_timer, x => x >= attack.UnlockComboDuration, () => IsComboLocked = false);
            }

            OnUpdateRotationRequest?.Invoke(new object[0]);
            OnAttackStepBegin?.Invoke(new object[] {_index, attack.AnimationSpeed});
        }

        //TODO: Implement
        #region TODO
        public void Unload() { }
        #endregion

        public void Unload(params object[] parameters) { }
        public void OnGamePause() { }
        public void OnGameResume() { }
    }
}
#pragma warning restore CS0067