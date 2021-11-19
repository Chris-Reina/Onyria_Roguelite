using DoaT.Save;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DoaT.UI
{
    public class UIContinueButton : UIButton
    {
        protected override void Awake()
        {
            base.Awake();

            OnInteractionSucceeded += Continue;
        }

        protected override void Start()
        {
            base.Start();
            
            interactable = SaveSystem.FileDetected;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            interactable = SaveSystem.FileDetected;
        }

        private void Continue()
        {
            Debug.Log("Continue pressed");
        }
    }
}
    
