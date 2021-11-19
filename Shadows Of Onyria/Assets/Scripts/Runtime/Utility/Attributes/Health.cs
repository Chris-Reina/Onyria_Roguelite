using System;
using UnityEngine;

namespace DoaT.Attributes
{
    public class Health : AttributeManager
    {
        public event Action OnDeath;
        public event Action OnHealthRefill;
        public event Action<float> OnDamageTaken;
        public event Action<float> OnHeal;

        [SerializeField] private bool _invulnerable = false;
        [SerializeField] private bool _undying = false;
        [SerializeField] private bool _isDead;
        [SerializeField] private Attribute _healthAttribute;

        public override Attribute ManagedAttribute => _healthAttribute;

        public bool IsInvulnerable
        {
            get => _invulnerable;
            set => _invulnerable = value;
        }

        public bool IsUndying
        {
            get => _undying;
            set => _undying = value;
        }

        public bool IsDead
        {
            get
            {
                _isDead = _healthAttribute.ValueIsMinimum;
                return _isDead;
            }
        }

        public float HealthAmount => _healthAttribute.ValueRatio;
        public float CurrentValue => _healthAttribute.Value;
        public float MaxHealth => _healthAttribute.MaxValue;

        public void SetAttribute(Attribute att)
        {
            _healthAttribute = att;
            if (att.Name != "Health")
                att.Rename("Health");
        }

        public Attribute GetAttribute()
        {
            return _healthAttribute;
        }

        public void ChangeMaxHealth(float value)
        {
            var ratio = HealthAmount;
            _healthAttribute.MaxValue = value;
            _healthAttribute.Value = _healthAttribute.MaxValue * ratio;
        }

        public void TakeDamage(float amount)
        {
            if (_invulnerable) return;
            
            _healthAttribute.AddValue(-amount);
            OnDamageTaken?.Invoke(amount);
            
            if (_undying && _healthAttribute.ValueIsMinimum)
                _healthAttribute.Value = 1f;

            if (IsDead) OnDeath?.Invoke();
        }

        public void Heal(float amount)
        {
            _healthAttribute.AddValue(amount);
            OnHeal?.Invoke(amount);
        }

        public void RefillHealth()
        {
            _healthAttribute.ResetValueToMax();
            OnHealthRefill?.Invoke();
        }

        public void Kill()
        {
            _healthAttribute.Value = _healthAttribute.MinValue;
            _isDead = true;
            OnDeath?.Invoke();
        }

        public void SetValue(float value)
        {
            _healthAttribute.Value = value;
        }
    }
}