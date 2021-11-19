using System;
using System.Collections.Generic;
using System.Linq;
using DoaT;
using UnityEngine;

public class LocalTimer
{
    private readonly Dictionary<TimerHandler, Action> _timers = new Dictionary<TimerHandler, Action>();
    private readonly List<TimedAction> _timedActions = new List<TimedAction>();

    public void Clear()
    {
        _timers.Clear();
        _timedActions.Clear();
    }
    
    public void Handle()
    {
        if (_timers.Count == 0) return;

        var time = Time.deltaTime;
        var newList = _timers.Where(t => t.Key.ElapseTime(time)).ToList();

        for (var i = 0; i < newList.Count(); i++)
        {
            newList[i].Value?.Invoke();
            newList[i].Key.IsActive = false;
            _timers.Remove(newList[i].Key);
        }

        for (var i = _timedActions.Count-1; i >= 0; i--)
        {
            if (_timedActions[i].TryAction())
            {
                _timedActions.RemoveAt(i);
            }
        }
    }

    private void InternalSetTimer(TimerHandler handler, Action callback, float duration, float delay)
    {
        if (_timers.ContainsKey(handler))
            _timers[handler] = callback;
        else
            _timers.Add(handler, callback);

        handler.Setup(duration, delay);
    }

    private void InternalCancelTimer(TimerHandler handler)
    {
        if (_timers.ContainsKey(handler))
        {
            _timers.Remove(handler);
        }

        for (int i = _timedActions.Count-1; i >= 0 ; i--)
        {
            if (_timedActions[i].IsTimer(handler))
            {
                _timedActions.RemoveAt(i);
            }
        }

        handler.IsActive = false;
    }

    /// <summary>
    /// Sets a timer that will execute the given Action when done.
    /// </summary>
    /// <param name="handler">Handler to take track of the timer.</param>
    /// <param name="duration">Duration of the timer.</param>
    public void SetTimer(TimerHandler handler, float duration)
    {
        if (handler == null) return;

        InternalSetTimer(handler, null, duration, 0);
    }

    /// <summary>
    /// Sets a timer that will execute the given Action when done.
    /// </summary>
    /// <param name="handler">Handler to take track of the timer.</param>
    /// <param name="callback">Action to be executed when timer's done.</param>
    /// <param name="duration">Duration of the timer.</param>
    public void SetTimer(TimerHandler handler, Action callback, float duration)
    {
        if (handler == null || callback == null) return;

        InternalSetTimer(handler, callback, duration, 0);
    }

    /// <summary>
    /// Sets a timer that will execute the given Action when done.
    /// </summary>
    /// <param name="handler">Handler to take track of the timer.</param>
    /// <param name="callback">Action to be executed when timer's done.</param>
    /// <param name="duration">Duration of the timer.</param>
    /// <param name="delay">Optional parameter. Wait time before starting the Timer.</param>
    public void SetTimer(TimerHandler handler, Action callback, float duration, float delay)
    {
        if (handler == null || callback == null) return;

        InternalSetTimer(handler, callback, duration, delay);
    }

    public void CancelTimer(TimerHandler handler)
    {
        if (handler == null)
        {
            DebugManager.LogWarning("TimerHandler was Null.");
            return;
        }

        InternalCancelTimer(handler);
    }

    public TimedAction SetTimedAction(TimerHandler handler, Func<float, bool> condition, Action callback)
    {
        var action = new TimedAction(handler, condition, callback);
        InternalSetTimedAction(action);
        return action;
    }

    private void InternalSetTimedAction(TimedAction timedAction)
    {
        _timedActions.Add(timedAction);
    }
}
