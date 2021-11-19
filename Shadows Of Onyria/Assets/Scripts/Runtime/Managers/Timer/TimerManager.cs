using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace DoaT
{
    public static class TimerManager
    {
        private static readonly Dictionary<TimerHandler, Action> Timers = new Dictionary<TimerHandler, Action>();
        private static readonly List<TimedAction> TimedActions = new List<TimedAction>();

        public static void Unload()
        {
            Timers.Clear();
            TimedActions.Clear();
        }
        
        public static void OnUpdate()
        {
            if (Timers.Count == 0) return;

            var time = Time.deltaTime;
            var newList = Timers.Where(t => t.Key.ElapseTime(time)).ToList();

            for (var i = TimedActions.Count - 1; i >= 0; i--)
            {
                if (TimedActions[i].TryAction())
                {
                    TimedActions.RemoveAt(i);
                }
            }
            
            for (var i = 0; i < newList.Count(); i++)
            {
                newList[i].Value?.Invoke();
                newList[i].Key.IsActive = false;
                Timers.Remove(newList[i].Key);
            }
        }

        private static void SetTimerImpl(TimerHandler handler, Action callback, float duration, float delay)
        {
            handler.Setup(duration, delay);
            
            if (Timers.ContainsKey(handler))
                Timers[handler] = callback;
            else
                Timers.Add(handler, callback);
        }


        /// <summary>
        /// Sets a timer that will execute the given Action when done.
        /// </summary>
        /// <param name="handler">Handler to take track of the timer.</param>
        /// <param name="duration">Duration of the timer.</param>
        public static void SetTimer(TimerHandler handler, float duration) => SetTimerImpl(handler, null, duration, 0);

        /// <summary>
        /// Sets a timer that will execute the given Action when done.
        /// </summary>
        /// <param name="handler">Handler to take track of the timer.</param>
        /// <param name="callback">Action to be executed when timer's done.</param>
        /// <param name="duration">Duration of the timer.</param>
        public static void SetTimer(TimerHandler handler, Action callback, float duration) => SetTimerImpl(handler, callback, duration, 0);

        /// <summary>
        /// Sets a timer that will execute the given Action when done.
        /// </summary>
        /// <param name="handler">Handler to take track of the timer.</param>
        /// <param name="callback">Action to be executed when timer's done.</param>
        /// <param name="duration">Duration of the timer.</param>
        /// <param name="delay">Optional parameter. Wait time before starting the Timer.</param>
        public static void SetTimer(TimerHandler handler, Action callback, float duration, float delay) => SetTimerImpl(handler, callback, duration, delay);
        
        public static void CancelTimer(TimerHandler handler)
        {
            if (Timers.ContainsKey(handler))
            {
                Timers.Remove(handler);
            }

            for (int i = TimedActions.Count-1; i >= 0 ; i--)
            {
                if (TimedActions[i].IsTimer(handler))
                {
                    TimedActions.RemoveAt(i);
                }
            }

            handler.IsActive = false;
        }

        public static TimedAction SetTimedAction(TimerHandler handler, Func<float, bool> condition, Action callback)
        {
            var action = new TimedAction(handler, condition, callback);
            TimedActions.Add(action);
            return action;
        }

        
    }
}
