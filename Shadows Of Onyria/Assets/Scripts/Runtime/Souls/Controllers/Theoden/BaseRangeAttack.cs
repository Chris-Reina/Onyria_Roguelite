#pragma warning disable CS0067

using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Controller/Base Range Attack", fileName = "Controller_BaseRangeAttack")]
    public class BaseRangeAttack : CharacterController, IClone<BaseRangeAttackBehaviour>
    {
        public SoulType soulType;
        public Attack attack;
        public float chargeDuration;
        public float maxRange;

        public override T GetController<T>()
        {
            var clone = Clone();
            clone.SetAttack(attack)
                 .SetChargeDuration(chargeDuration)
                 .SetMaxRange(maxRange);
            return clone as T;
        }

        public BaseRangeAttackBehaviour Clone()
        {
            return new BaseRangeAttackBehaviour {soulType = new SoulTypeData(soulType)};
        }
    }

    public class BaseRangeAttackBehaviour : IAttackBehaviour
    {
        public SoulTypeData soulType;

        public event Action OnSpendDarkness;
        public event Action<object[]> OnActionCancel;
        public event Action<object[]> OnUpdateRotationRequest;
        
        public event Action<object[]> OnAttackStepBegin;
        public event Action<object[]> OnAttackStepEnd;
        
        public event Action<object[]> OnAttackUpdate;
        public event Action<object[]> OnAttackBegin;
        public event Action<object[]> OnAttackEnd;
        
        private IEntity _attacker;
        private Attack _attack;
        private float _chargeDuration;
        private float _maxRange;
        private Func<AttackInfo> _attackInfoFunc;
        
        private float _ratio;
        private float _size;
        private float _currentChargeDuration;

        public bool IsLocked => false;
        public bool IsOnCooldown { get; private set; }
        public bool NeedsUpdate => false;
        public SoulTypeData SoulType => soulType;
        public AttackInputType AttackInputMask => _attack.AllInputTypes;
        public AttackInputType AttackRequestInputMask => _attack.BeginInputType;
        private AttackInfo AttackInfo => _attackInfoFunc?.Invoke();

        private readonly TimerHandler _timer = new TimerHandler();
        private bool _hasReleased;

        public IAttackBehaviour Initialize(Func<AttackInfo> info, IEntity attacker)
        {
            _attackInfoFunc = info;
            _attacker = attacker;

            return this;
        }

        public void Update()
        {
        }


        public void SendImpulsePress()
        {
            if (_hasReleased) return;
            OnAttackBegin?.Invoke(new object[] {_attack.AnimationSpeed});
        }
        public void SendImpulseSustained()
        {
            if (_hasReleased) return;
            _currentChargeDuration += Time.deltaTime;
            _ratio = Mathf.Min(_currentChargeDuration / _chargeDuration, 1);
            _size = _ratio * _maxRange;

            OnAttackUpdate?.Invoke(new object[] {_size});
            OnUpdateRotationRequest?.Invoke(new object[0]);
        }
        
        public void SendImpulseRelease()
        {
            if (_hasReleased) return;
            
            TimerManager.SetTimer(_timer, () => IsOnCooldown = false, _attack.Duration);
            
            foreach (var effect in _attack.effects)
            {
                TimerManager.SetTimedAction
                (
                    _timer,
                    x => x >= _attack.EffectDurationByEffect(effect),
                    () => ExecuteEffect(effect)
                );
            }

            OnSpendDarkness?.Invoke();
            _hasReleased = true;
        }
        
        private void ExecuteEffect(AttackEffect effect)
        {
            var aInfo = AttackInfo;
            aInfo.damage *= _size;
            
            IsOnCooldown = true;
            effect.Execute(AttackInfo, _attacker, _attack);
            OnAttackEnd?.Invoke(new object[0]);
            
        }
        public void Interrupt()
        {
            if (!_hasReleased) OnActionCancel?.Invoke(new object[0]);
                
            _ratio = 0;
            _size = 0;
            _currentChargeDuration = 0;
            _hasReleased = false;

            if (_timer.IsActive && !_hasReleased) //TODO : Refactor or Implement
            {
                //TimerManager.CancelTimer(_timer);
            }
            
        }
        
        public void Unload(params object[] parameters) { }

        //TODO: Implement
        #region TODO
        public void Unload() { }
        #endregion

        #region Builder Pattern

        public BaseRangeAttackBehaviour SetAttack(Attack value)
        {
            _attack = value;
            return this;
        }
        public BaseRangeAttackBehaviour SetMaxRange(float value)
        {
            _maxRange = value;
            return this;
        }
        public BaseRangeAttackBehaviour SetChargeDuration(float value)
        {
            _chargeDuration = value;
            return this;
        }

        #endregion

        public void OnGamePause() { }

        public void OnGameResume() { }
    }
}
#pragma warning restore CS0067