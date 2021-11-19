using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT.Inputs
{
    public sealed class InputSystem : MonoBehaviour
    {
        private static InputSystem Current { get; set; }
        
        [SerializeField] private InputData GameplayProfile;
        [SerializeField] private InputData UIProfile;
        
        private readonly Dictionary<InputProfile, InputData> _profileMap = new Dictionary<InputProfile, InputData>();
        
        private InputData _data;
        
        private void Awake()
        {
            if (Current == null)
                Current = this;
            else if (Current != this)
            {
                Destroy(this);
                return;
            }
            
            Clear();

            _profileMap.Add(InputProfile.Gameplay, GameplayProfile);
            _profileMap.Add(InputProfile.UI, UIProfile);
            
            _data = GameplayProfile;
        }

        private void Start()
        {
            GameManager.Current.OnLoadingComplete += EnableInputs;
        }

        private void Update()
        {
            foreach (var aEvent in _data.axisInputs)
            {
                foreach (var callback in aEvent.callback)
                {
                    var value = callback.Key == AxisEvent.Fixed
                        ? Input.GetAxisRaw(aEvent.unityAxisName)
                        : Input.GetAxis(aEvent.unityAxisName);
                    callback.Value?.Invoke(value);
                }
            }

            foreach (var kvp in _data.keyInputs.
                SelectMany(kInput => kInput.callback.Where(x => GetInputByEvent(kInput.keys, x.Key))))
            {
                kvp.Value?.Invoke();
            }
        }
        
        public static void EnableInputs() => EventManager.Raise(PlayerEvents.OnEnableInputs);
        public static void DisableInputs() => EventManager.Raise(PlayerEvents.OnDisableInputs);
        
        public static void SetCurrentProfile(InputProfile value)
        {
            Current._data = Current._profileMap[value];
        }
        
        private static bool GetInputByEvent(IEnumerable<KeyCode> keys, KeyEvent kEvent)
        {
            foreach (var k in keys)
            {
                var inputRegistered = kEvent switch
                {
                    KeyEvent.Press => Input.GetKeyDown(k),
                    KeyEvent.Release => Input.GetKeyUp(k),
                    KeyEvent.Hold => Input.GetKey(k),
                    KeyEvent.Any => Input.GetKeyDown(k) || Input.GetKeyUp(k) || Input.GetKey(k),
                    _ => throw new ArgumentOutOfRangeException(nameof(kEvent), kEvent, null)
                };

                if (inputRegistered) return true;
            }

            return false;
        }

        public static void BindKey(InputProfile profile, string key, KeyEvent eventType, Action callback)
        {
            var input = Current._profileMap[profile].keyInputs.FirstOrDefault(x => x.keyName == key);

            if(input != null)
            {
                if (input.callback.ContainsKey(eventType))
                    input.callback[eventType] += callback;
                else
                    input.callback.Add(eventType, callback);
            }
            else
                DebugManager.LogWarning($"Key Binding Failed, no key named {key} was found.");
        }

        public static void BindAxis(InputProfile profile, string axis, AxisEvent eventType, Action<float> callback)
        {
            var input = Current._profileMap[profile].axisInputs.FirstOrDefault(x => x.name == axis);

            if (input != null)
            {
                if (input.callback.ContainsKey(eventType))
                    input.callback[eventType] += callback;
                else
                    input.callback.Add(eventType, callback);
            }
            else
                DebugManager.LogWarning($"Axis Binding Failed, no axis named {axis} was found.");
        }
        
        public static void UnbindKey(InputProfile profile, string key, KeyEvent eventType, Action callback)
        {
            if (!Current._profileMap.ContainsKey(profile)) return;
            var input = Current._profileMap[profile].keyInputs.FirstOrDefault(x => x.keyName == key);

            if (input == null) return;
            
            if (input.callback.ContainsKey(eventType))
                input.callback[eventType] -= callback;
        }

        public static void UnbindAxis(InputProfile profile, string axis, AxisEvent eventType, Action<float> callback)
        {
            if (!Current._profileMap.ContainsKey(profile)) return;
            var input = Current._profileMap[profile].axisInputs.FirstOrDefault(x => x.name == axis);

            if (input == null) return;
            
            if (input.callback.ContainsKey(eventType))
                input.callback[eventType] -= callback;
        }

        public static void Clear()
        {
            foreach (var map in Current._profileMap)
            {
                map.Value.Unload();
            }

            //Current._profileMap.Clear();
        }
    }
}
