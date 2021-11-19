using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DoaT
{
    /// <summary>
    /// Will let other scene event components operate if the flag value matches the desired outcome (default is true))
    /// Also if _valueToCheck matches the new value of the flag, it will execute the given triggeredEvent
    /// </summary>
    public sealed class SceneFlagTriggerAndCondition : BaseSceneEventCondition
    {
        [Header("Condition")]
        [Tooltip("The flag that contains the value to be checked for.")]
        [SerializeField] private SceneFlag flag;
        [FormerlySerializedAs("_negate")]
        [Tooltip("Should the flag be compared to 'false'?")]
        [SerializeField] private bool _valueToMatch;
        
        [Header("Trigger")]
        [Tooltip("Event to be triggered if the flag value matches the value to check.")]
        [SerializeField] private SceneEvent triggeredEvent;
        [SerializeField] private bool _valueToCheck = true;

        public override bool CanOperate => _valueToMatch == flag.Value;

        private void Start()
        {
            if (triggeredEvent == null)
            {
                DebugManager.LogWarning($"There is no SceneEvent in the object of type {GetType()} with name {name}.");
                return;
            }
            
            if (flag == null)
            {
                DebugManager.LogWarning($"There is no SceneFlag in the object of type {GetType()} with name {name}.");
            }
            
            flag.OnValueChanged += RaiseSceneEvent;
        }

        private void RaiseSceneEvent(bool value)
        {
            if (value == _valueToCheck)
            {
                EventManager.Raise(triggeredEvent);
            }
        }
    }
}
