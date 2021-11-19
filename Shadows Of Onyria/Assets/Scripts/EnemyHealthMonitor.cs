using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DoaT.Attributes
{
    public class EnemyHealthMonitor : AttributeMonitor, IUpdate
    {
        [SerializeField] private EnemyEntity _controller;
        [SerializeField] private CanvasGroup _group;

        private bool _showBar = true;
        private Transform _target;
        
        protected override void Start()
        {
            _attribute = manager.ManagedAttribute;
            _lastRatio = 1f;

            base.Start();
            
            _group.alpha = 0;
            _controller.OnDeath += () => _group.alpha = 0;
            _target = World.MainCamera.transform;
            
            ExecutionSystem.AddUpdate(this);
        }

        public void OnUpdate()
        {
            var t = transform;
            t.forward = (_target.position - t.position).normalized;
        }

        protected override void UpdateUI(float ratio)
        {
            base.UpdateUI(ratio);

            if (ratio <= 0f || ratio >= 1f)
            {
                _group.alpha = 0;
            }
            else
            {
                _group.alpha = 1;
            }
        }

        public override void Unload(params object[] parameters)
        {
            base.Unload(parameters);
            ExecutionSystem.RemoveUpdate(this, true);
        }
    }

    public abstract class AttributeMonitor : MonoBehaviour, IUnloadable
    {
        [SerializeField] protected Image barFill;
        [SerializeField] protected Slider slider;
        [SerializeField] protected Color barColor = Color.green;
        [SerializeField] protected Image animatedBarFill;
        [SerializeField] protected AttributeManager manager;
        [SerializeField] protected float animationStartWaitTime = 0.05f;
        [SerializeField] protected float animationLikenessThreshold = 0.0001f;
        [Range(0f, 1f),SerializeField] protected float lerpAnimationSpeed = 0.1f;
        
        protected float _lastRatio;
        protected float _updatedRatio;
        protected Attribute _attribute;
        protected Coroutine _currentCoroutine;

        protected virtual Coroutine CurrentCoroutine
        {
            get => _currentCoroutine;
            set
            {
                if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
                _currentCoroutine = value;
            }
        }
        
        protected virtual void Awake()
        {
            if (slider == null)
                slider = GetComponent<Slider>();
            
            if (barFill == null)
                DebugManager.LogError("There should be a Bar Fill Assigned");
            
            if (slider != null)
                slider.fillRect = barFill.rectTransform;
            else
            {
                slider = gameObject.AddComponent<Slider>();
                slider.transition = Selectable.Transition.None;
                slider.fillRect = barFill.rectTransform;
            }
        }
        
        protected virtual void Start()
        {
            _attribute.OnValueChanged += UpdateUI;
            UpdateUI(_attribute.ValueRatio);
            StartCoroutine(UpdateAnimatedBar());
            
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
        }
        
        protected virtual IEnumerator UpdateAnimatedBar()
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
        
        protected virtual void UpdateUI(float ratio)
        {
            _updatedRatio = ratio;
            CurrentCoroutine = StartCoroutine(UpdateAnimatedBar());
            slider.value = ratio;
        }

        public virtual void Unload(params object[] parameters)
        {
            if(_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
        }
    }
}
