using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DoaT.Attributes
{
    public class HealthMonitor : MonoBehaviour, IUnloadable
    {
        public enum BarUpdateType
        {
            FillAmount,
            Slider
        }

        public Image barFill;
        public Slider slider;
        public Color barColor = Color.green;
        public BarUpdateType updateType;
        public Attribute attribute;
        public Image animatedBarFill;
        [Range(0f, 1f)] public float lerpAnimationSpeed = 0.1f;
        public float animationStartWaitTime = 0.05f;
        public float animationLikenessThreshold = 0.0001f;
        
        private float _lastRatio;
        private float _updatedRatio;
        private Coroutine _currentCoroutine;

        private Coroutine CurrentCoroutine
        {
            get => _currentCoroutine;
            set
            {
                if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
                _currentCoroutine = value;
            }
        }

        private void Awake()
        {
            if (slider == null)
            {
                slider = GetComponent<Slider>();
            }

            if (barFill == null)
            {
                DebugManager.LogError("There should be a Bar Fill Assigned");
            }

            if (slider != null)
            {
                slider.fillRect = barFill.rectTransform;
            }
        }

        private void Start()
        {
            CheckSlider();
            attribute = World.GetPlayer().GetHealth().GetAttribute();
            attribute.OnValueChanged += UpdateUI;
            UpdateUI(attribute.ValueRatio);

            StartCoroutine(UpdateAnimatedBar());
            _lastRatio = PersistentData.Health.value;
        }

        private IEnumerator UpdateAnimatedBar()
        {
            yield return new WaitForSeconds(animationStartWaitTime);
            
            if(_updatedRatio == _lastRatio) yield break;

            if (_updatedRatio < _lastRatio)
            {
                while (Math.Abs(_updatedRatio - _lastRatio) > animationLikenessThreshold)
                {
                    var value = Mathf.Lerp(_lastRatio, _updatedRatio, lerpAnimationSpeed);
                    animatedBarFill.fillAmount = value;
                    _lastRatio = value;


                    yield return null;
                }
                
                animatedBarFill.fillAmount = _updatedRatio;
            }
            
            _lastRatio = _updatedRatio;
        }
        
        private void UpdateUI(float ratio)
        {
            _updatedRatio = ratio;
            CurrentCoroutine = StartCoroutine(UpdateAnimatedBar());
            
            PersistentData.Player.Health.ratio = ratio;
            switch (updateType)
            {
                case BarUpdateType.FillAmount:
                    barFill.fillAmount = ratio;
                    break;
                case BarUpdateType.Slider:
                    slider.value = ratio;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ChangeBarColor()
        {
            barFill.color = barColor;
        }

        public void CheckSlider()
        {
            if (updateType == BarUpdateType.Slider && slider == null) 
            {
                gameObject.AddComponent<Slider>();
                slider = GetComponent<Slider>();
                slider.transition = Selectable.Transition.None;
                slider.fillRect = barFill.rectTransform;
            }
        }

        public void Unload(params object[] parameters)
        {
            if(_currentCoroutine != null) StopCoroutine(_currentCoroutine);
            PersistentData.Health.ratio = _lastRatio;
        }
    }
}