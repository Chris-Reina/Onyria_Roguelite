using System;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public class TimerHandler
    {
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _elapsed = 0f;

        /// <summary>
        /// The Handler is active whilst it's being updated by the TimerManager.
        /// </summary>
        public bool IsActive { get; set; }
        public float Duration => _duration;
        public float Elapsed => _elapsed < 0f ? 0f : _elapsed;
        public float Remaining => _duration - Elapsed;
        public float Progress
        {
            get
            {
                var value = _elapsed / _duration;
                if (value < 0f)
                    value = 0f;
                else if (value > 1f)
                    value = 1f;

                return value;
            }
        }
        /// <summary>
        /// The Handler Is Done AddingTime.
        /// </summary>
        public bool IsDone => _elapsed >= _duration;

        public TimerHandler() { }

        public void Setup(float duration, float delay)
        {
            IsActive = true;
            _duration = duration;
            _elapsed = 0 - delay;
        }

        public void AddDelay(float delay)
        {
            _elapsed -= delay;
        }

        public bool ElapseTime(float deltaTime)
        {
            if (IsDone) return true;

            _elapsed += deltaTime;
            return IsDone;
        }

        public void SetTimeElapsed(float timeElapsed)
        {
            _elapsed = timeElapsed;
        }
    }
}

