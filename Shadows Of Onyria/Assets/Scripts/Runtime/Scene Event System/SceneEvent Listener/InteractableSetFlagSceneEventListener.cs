using UnityEngine;

namespace DoaT
{
    public class InteractableSetFlagSceneEventListener : BaseSceneEventListenerDual, IInteractable
    {
        [SerializeField] private string _interactMessage;
        [SerializeField] private SceneFlag _sceneFlag;
        [SerializeField] private bool _negate;
        public string InteractMessage => _interactMessage;
        public bool Interactable => GetConditionalsCanOperateStatus();

        public bool Interact()
        {
            if (!Interactable) return false;
            EventManager.Raise(GameEvents.OnInteractableRemove, this, _interactMessage);
            OnEventTriggeredDual();
            _sceneFlag.Value = !_negate;
            return true;
        }
        
        public override void OnEventTriggered(params object[] parameters)
        {
            if(CanReact)
                EventManager.Raise(GameEvents.OnInteractableAdd, this, _interactMessage);
        }

        public override void OnEventTriggeredDual(params object[] parameters)
        {
            if(CanReact)
                EventManager.Raise(GameEvents.OnInteractableRemove, this, _interactMessage);
        }
    }
}
