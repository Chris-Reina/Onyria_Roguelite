using System;

namespace DoaT
{
    [Serializable]
    public class TimedAction
    {
        private TimerHandler _target;
        private Func<float, bool> _condition;
        private Action _callback;

        public TimedAction(TimerHandler handler, Func<float,bool> condition, Action callback)
        {
            _target = handler;
            _condition = condition;
            _callback = callback;
        }

        public bool TryAction()
        {
            if (!_condition.Invoke(_target.Elapsed)) return false;
            
            _callback.Invoke();
            return true;
        }

        public bool IsTimer(TimerHandler handler)
        {
            return _target == handler;
        }

        public TimedAction SetCondition(Func<float, bool> condition)
        {
            _condition = condition;
            return this;
        }

        public TimedAction SetCallback(Action callback)
        {
            _callback = callback;
            return this;
        }
    }
}
