using UnityEngine;

namespace DoaT
{
    public class UISceneEventListener : BaseSceneEventListener
    {
        public CanvasGroupController groupController;
        public SceneFlag cancelFlag;

        private void Start()
        {
            UIMasterController.OnHideUI += HidePanelKey;
            groupController.OnHideUI += () => cancelFlag.Value = false;
        }

        private void HidePanelKey()
        {
            if (!groupController.IsActive) return;

            OnEventTriggered();
            cancelFlag.Value = false;
        }

        public override void OnEventTriggered(params object[] parameters)
        {
            if (groupController.IsActive)
            {
                groupController.HideUI();
            }
            else
            {
                groupController.ShowUI();
            }
        }
    }
}
