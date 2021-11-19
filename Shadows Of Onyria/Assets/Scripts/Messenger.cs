using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    public class Messenger : MonoBehaviour
    {
        private static Messenger Current { get; set; }

        public static MessageFormat Error => Current._error;

        [SerializeField] private MessageDisplay messageDisplayPrefab;
        [SerializeField] private float _defaultDisplayTime = 1f;
        [SerializeField] private Gradient _defaultGradient;

        [Header("Message Templates")] 
        [SerializeField] private MessageFormat _error;

        private readonly HashSet<MessageDisplay> _currentMessages = new HashSet<MessageDisplay>();
        
        private void Awake()
        {
            if (Current == null)
                Current = this;
            else if (Current != this)
            {
                DebugManager.LogWarning($"Duplicate Singleton of type {GetType()}");
                Destroy(this);
                return;
            }
        }
        
        public static void Display(string message) => Current.DisplayImpl(message, Current._defaultDisplayTime, Current._defaultGradient);
        public static void Display(string message, float displayTime) => Current.DisplayImpl(message, displayTime, Current._defaultGradient);
        public static void Display(string message, Gradient color) => Current.DisplayImpl(message, Current._defaultDisplayTime, color);
        public static void Display(string message, MessageFormat format) => Current.DisplayImpl(message, format.time, format.color);
        public static void Display(string message, float displayTime, Gradient color) => Current.DisplayImpl(message, displayTime, color);
        
        private void DisplayImpl(string message, float displayTime, Gradient color)
        {
            var inst = Instantiate(messageDisplayPrefab, transform);
            inst.Initialize(message, displayTime, color);
        }
        
        public static void AddChildObject(MessageDisplay child)
        {
            if(Current._currentMessages.Contains(child)) return;
            Current._currentMessages.Add(child);
        }
        public static void RemoveChildObject(MessageDisplay child)
        {
            if (!Current._currentMessages.Contains(child)) return;
            Current._currentMessages.Remove(child);
        }
    }

    [Serializable]
    public struct MessageFormat
    {
        public float time;
        public Gradient color;

        public MessageFormat(float time, Gradient color)
        {
            this.time = time;
            this.color = color;
        }
    }
}
