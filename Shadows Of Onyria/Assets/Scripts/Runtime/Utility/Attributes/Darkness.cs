using System;
using UnityEngine;

namespace DoaT.Attributes
{
    public class Darkness : AttributeManager
    {
#pragma warning disable CS0067
        public event Action OnDarknessFull;
#pragma warning restore CS0067
        
        [SerializeField] private Attribute _darknessAttribute;
        
        public float DarknessAmount => _darknessAttribute.ValueRatio;
        public float CurrentValue => _darknessAttribute.Value;
        public float MaxDarkness => _darknessAttribute.MaxValue;
        
        public override Attribute ManagedAttribute => _darknessAttribute;

        public void SetAttribute(Attribute att)
        {
            _darknessAttribute = att;
            if (att.Name != "Darkness")
                att.Rename("Darkness");
        }

        public Attribute GetAttribute()
        {
            return _darknessAttribute;
        }

        public void Spend(float amount)
        {
            if (CanSpend(amount))
                _darknessAttribute.AddValue(-amount);
            else
                _darknessAttribute.ResetValueToMin();
        }

        public void Gain(float amount)
        {
            _darknessAttribute.AddValue(amount);
        }

        public void Refill()
        {
            _darknessAttribute.ResetValueToMax();
        }

        public void Empty()
        {
            _darknessAttribute.ResetValueToMin();
        }

        public void SetValue(float value)
        {
            _darknessAttribute.Value = value;
        }

        public bool CanSpend(float f)
        {
            return _darknessAttribute.Value - f >= 0;
        }

        public void ChangeMaxDarkness(float value)
        {
            var ratio = DarknessAmount;
            _darknessAttribute.MaxValue = value;
            _darknessAttribute.Value = _darknessAttribute.MaxValue * ratio;
        }
    }
}