using System;
using UnityEngine;

namespace DoaT
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class CanvasGroupController : MonoBehaviour, IGroupController
    {
#pragma warning disable CS0067
        public event Action OnHideUI;
        public event Action OnShowUI;
        public event Action OnUpdateUI;
#pragma warning restore CS0067
        
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField, Range(0f,1f)] protected float _maxAlpha = 1f;
        [SerializeField] protected bool _forceNonInteractable = false;
        [SerializeField] protected bool _forceRaycastNonBlocker = false;
        [SerializeField] protected bool _forceNotIgnoreParentGroups = false;

        public bool IsActive => _isShowing;
        protected bool _isShowing;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _isShowing = _canvasGroup.IsShowing();

            UIMasterController.OnHideUI += HideUI;
        }

        public virtual void ShowUI()
        {
            _canvasGroup.alpha = _maxAlpha;
            _canvasGroup.interactable = !_forceNonInteractable;
            _canvasGroup.blocksRaycasts= !_forceRaycastNonBlocker;
            _canvasGroup.ignoreParentGroups = !_forceNotIgnoreParentGroups;
            _isShowing = true;
           UIMasterController.AddTrackingUI(this);
           OnShowUI?.Invoke();
        }

        public virtual void HideUI()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts= false;
            _canvasGroup.ignoreParentGroups = false;
            _isShowing = false;
           UIMasterController.RemoveTrackedUI(this);
           OnHideUI?.Invoke();
        }
    }
}