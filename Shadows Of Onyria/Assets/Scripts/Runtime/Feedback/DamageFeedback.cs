using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace DoaT
{
    public class DamageFeedback : MonoBehaviour, IUpdate, IPoolSpawn
    {
        private float Alpha => _alpha.Evaluate(_feedbackTimer.Progress);

        private readonly TimerHandler _feedbackTimer = new TimerHandler();
        private Pool<DamageFeedback> _parentPool;
        private Vector3 _initialPosition;
        private Vector3 _initialScale;

#pragma warning disable 649
        [Header("Reference")] [SerializeField] private TextMeshProUGUI _text;

        [Header("Look")] [SerializeField] private Color _normalColor;
        [SerializeField] private Color _criticalColor;
        [SerializeField] private float _sizeMultiplierMax;
        [SerializeField] private AnimationCurve _sizeMultiplier;
        [SerializeField] private AnimationCurve _alpha;

        [Header("Animation")] [SerializeField] private float _animationDuration;
        [SerializeField] private float _maxDisplacementY = 1f;
        [SerializeField] private float _maxDisplacementX = 1f;
        [SerializeField] private float _maxDisplacementZ = 1f;
        [SerializeField] private AnimationCurve _movementY;
        [SerializeField] private AnimationCurve _movementX;
        [SerializeField] private AnimationCurve _movementZ;
        [SerializeField, Range(0f, 1f)] private float damageAccumulateTimerSnap;
#pragma warning restore 649

        public IAttackable attacked;
        private float _damageAccum;

        public void Initialize(float damage, bool isCritical, IAttackable attackable, SColor fedColor = default)
        {
            if (!fedColor.IsDefault)
            {
                _text.color = fedColor.ToColor();
            }
            else
            {
                _text.color = isCritical ? _criticalColor : _normalColor;
            }

            _text.text = damage.ToString(CultureInfo.CurrentCulture);

            _damageAccum = damage;
            attacked = attackable;

            TimerManager.SetTimer(_feedbackTimer, _animationDuration);
        }

        private void Start()
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();

            var canvas = GetComponentInChildren<Canvas>();
            canvas.worldCamera = World.MainCamera;
            _initialScale = transform.localScale;
        }

        public void OnUpdate()
        {
            var progress = _feedbackTimer.Progress;
            var size = _sizeMultiplier.Evaluate(progress) * _sizeMultiplierMax;
            transform.position = _initialPosition + EvaluatePosition(progress);
            transform.localScale = _initialScale * size;
            _text.alpha = Alpha;

            if (!_feedbackTimer.IsActive)
                Deactivate();
        }

        public void SetParentPool<T>(T parent)
        {
            _parentPool = parent as Pool<DamageFeedback>;
        }

        public void Activate(Vector3 position, Quaternion rotation)
        {
            transform.position = _initialPosition = position;
            ExecutionSystem.AddUpdate(this);
        }

        public void Deactivate()
        {
            if (_feedbackTimer.IsActive) TimerManager.CancelTimer(_feedbackTimer);
            ExecutionSystem.RemoveUpdate(this, false);
            _text.alpha = 0f;
            transform.localScale = _initialScale;
            _parentPool.ReturnObject(this);
        }

        private Vector3 EvaluatePosition(float atTime)
        {
            var y = Vector3.up * (_movementY.Evaluate(atTime) * _maxDisplacementY);
            var z = World.ScreenForward * (_movementZ.Evaluate(atTime) * _maxDisplacementZ);
            var x = World.ScreenRight * (_movementX.Evaluate(atTime) * _maxDisplacementX);

            return x + y + z;
        }

        public void AddDamage(float damage, bool isCritical)
        {
            _text.color = isCritical ? _criticalColor : _normalColor;
            _damageAccum += damage;
            _text.text = _damageAccum.ToString(CultureInfo.InvariantCulture);
            _feedbackTimer.SetTimeElapsed(_animationDuration * damageAccumulateTimerSnap);
        }

        private void OnDestroy()
        {
            ExecutionSystem.RemoveUpdate(this, true);
        }
    }
}