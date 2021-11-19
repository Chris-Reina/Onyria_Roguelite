using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    /// <summary>
    /// Handles the Subscription and Call of custom events.
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<SceneEvent, Action<object[]>> SceneEventsData =
            new Dictionary<SceneEvent, Action<object[]>>();
        private static readonly Dictionary<string, Action<object[]>> EventsData = 
            new Dictionary<string, Action<object[]>>();

        public static void Subscribe(string eventName, Action<object[]> action)
        {
            if (EventsData.ContainsKey(eventName))
                EventsData[eventName] += action;
            else
                EventsData.Add(eventName, action);
        }

        public static void Unsubscribe(string eventName, Action<object[]> action)
        {
            if (EventsData.ContainsKey(eventName))
                EventsData[eventName] -= action;
            else 
                Debug.LogWarning("Tried to Unsubscribe to something that is not in the Map.");
        }

        public static void Raise(string eventName, params object[] parameters)
        {
            if(EventsData.ContainsKey(eventName))
                EventsData[eventName]?.Invoke(parameters);
        }
        
        public static void Subscribe(SceneEvent eventName, Action<object[]> action)
        {
            if (SceneEventsData.ContainsKey(eventName))
                SceneEventsData[eventName] += action;
            else
                SceneEventsData.Add(eventName, action);
        }

        public static void Unsubscribe(SceneEvent eventName, Action<object[]> action)
        {
            if (SceneEventsData.ContainsKey(eventName))
                SceneEventsData[eventName] -= action;
        }

        public static void Raise(SceneEvent eventName, params object[] parameters)
        {
            if(SceneEventsData.ContainsKey(eventName))
                SceneEventsData[eventName]?.Invoke(parameters);
        }

        // public static void Print()
        // {
        //     foreach (var data in EventsData)
        //     {
        //         Debug.Log($"Event: [{data.Key}]");
        //     }
        //     
        //     foreach (var data in SceneEventsData)
        //     {
        //         Debug.Log($"SceneEvent: [{data.Key}]");
        //     }
        // }
    }
}
