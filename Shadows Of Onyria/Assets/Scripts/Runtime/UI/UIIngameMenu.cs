using UnityEngine;

namespace DoaT
{
    public class UIIngameMenu : CanvasGroupController, IInputUIComponent
    {
        private static UIIngameMenu Current { get; set; }
        
        public static bool IsShowing => Current.IsActive;
        public static bool CanActivate => UIMasterController.HasInstance;

        protected override void Awake()
        {
            Current = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            _isShowing = _canvasGroup.IsShowing();
        }
        
        public void OnSelectInput() { }
        public void OnReturnInput()
        {
            HideUI();
        }

        public static void ActivateMainMenu()
        {
            Current.ShowUI();
        }

        public static void DeactivateMainMenu()
        {
            Current.HideUI();
        }

        public override void ShowUI()
        {
            base.ShowUI();
            ExecutionSystem.Pause();
        }

        public override void HideUI()
        {
            base.HideUI();
            ExecutionSystem.Resume();
        }
    }
}
