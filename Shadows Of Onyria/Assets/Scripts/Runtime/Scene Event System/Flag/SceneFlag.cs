using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Scene/Flag",fileName = "Flag_")]
    public class SceneFlag : ScriptableObject, IAwake
    {
        [SerializeField] private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                
                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }
        public event Action<bool> OnValueChanged;
        
        public void OnAwake()
        {
            OnValueChanged = default;
            _value = false;
        }

        public void ResetValue()
        {
            _value = false;
        }
    }
}
