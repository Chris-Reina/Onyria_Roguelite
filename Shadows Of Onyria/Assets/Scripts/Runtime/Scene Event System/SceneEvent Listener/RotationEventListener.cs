using System.Collections;
using UnityEngine;

namespace DoaT
{
    public class RotationEventListener : BaseSceneEventListener
    {
        [Header("Animation")]
        public bool snap = false;
        public float duration;
        public AnimationCurve lerpCurve;
        public Vector3 endRotation;

        private readonly TimerHandler _animationTimer = new TimerHandler();
        private Quaternion _initialRotation;
        private Quaternion _endRotation;

        protected override void Awake()
        {
            base.Awake();

            _initialRotation = transform.rotation;
            _endRotation = Quaternion.Euler(endRotation);
        }

        public override void OnEventTriggered(params object[] parameters)
        {
            if (!CanReact) return;
            if (snap)
            {
                StopAllCoroutines();
                transform.rotation = Quaternion.Euler(endRotation);
            }
            else
            {
                StopAllCoroutines();
                TimerManager.SetTimer(_animationTimer, duration);
                StartCoroutine(Animate());
            }
        }

        private IEnumerator Animate()
        {
            _endRotation = Quaternion.Euler(endRotation);
            while (_animationTimer.IsActive)
            {
                transform.rotation = Quaternion.Lerp(_initialRotation, _endRotation, lerpCurve.Evaluate(_animationTimer.Progress));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
