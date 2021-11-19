using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DoaT
{
    public class DamageOverlayFeedback : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _color;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _animationDuration;

        private Coroutine _coroutine;
        private readonly TimerHandler _handler = new TimerHandler();

        private void Awake()
        {
            EventManager.Subscribe(PlayerEvents.OnDamageTaken, PlayAnimation);
        }

        private void PlayAnimation(object[] obj)
        {
            if (_coroutine != null)
            {
                TimerManager.SetTimer(_handler, _animationDuration);
                return;
            }

            TimerManager.SetTimer(_handler, _animationDuration);
            _coroutine = StartCoroutine(ScreenAnimation());
        }

        private IEnumerator ScreenAnimation()
        {
            var color = _color;
            while (_handler.IsActive)
            {
                color.a = _curve.Evaluate(_handler.Progress);
                _image.color = color;
                yield return new WaitForEndOfFrame();
            }

            color.a = 0f;
            _image.color = color;
            _coroutine = null;
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(PlayerEvents.OnDamageTaken, PlayAnimation);
        }
    }

}