using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DoaT.Attributes
{
    public class DarknessMonitor : MonoBehaviour, IUnloadable
    {
        public enum BarUpdateType
        {
            FillAmount,
            Slider
        }
        
        public Image barFill;
        public Slider slider;
        public Color barColor = Color.magenta;
        public BarUpdateType updateType;
        public Attribute attribute;
        public Image animatedBarFill;
        [Range(0f, 1f)] public float lerpAnimationSpeed = 0.1f;
        public float animationStartWaitTime = 0.05f;
        public float animationLikenessThreshold = 0.001f;
        
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
            attribute = World.GetPlayer().GetDarkness().GetAttribute();
            attribute.OnValueChanged += UpdateUI;
            UpdateUI(PersistentData.Darkness.value);

            _lastRatio = PersistentData.Darkness.value;
            CurrentCoroutine = StartCoroutine(UpdateAnimatedBar());
        }

        private IEnumerator UpdateAnimatedBar()
        {
            var timer = 0f;
            var ratio = _lastRatio;

            while (true)
            {
                if (timer > animationStartWaitTime)
                {
                    timer = 0f;
                    ratio = _updatedRatio;
                }
                
                if (ratio > _lastRatio)
                {
                    _lastRatio = ratio;
                }
                else if (ratio < _lastRatio)
                {
                    var value = Mathf.Lerp(_lastRatio, ratio, lerpAnimationSpeed);
                    animatedBarFill.fillAmount = value;
                    _lastRatio = value;
                }
                
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } 
        }
        
        private void UpdateUI(float ratio)
        {
            _updatedRatio = ratio;
            
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

        #region Builder

        public DarknessMonitor SetBarFillImage(Image image)
        {
            barFill = image;
            return this;
        }

        public DarknessMonitor SetBarColor(Color color)
        {
            barColor = color;
            return this;
        }

        public DarknessMonitor SetBarUpdateType(BarUpdateType barType)
        {
            updateType = barType;
            return this;
        }

        #endregion

        public void Unload(params object[] parameters)
        {
            if(_currentCoroutine != null) StopCoroutine(_currentCoroutine);
            PersistentData.Darkness.ratio = _lastRatio;
        }
    }
}