using DoaT.Inputs;
using UnityEngine;

namespace DoaT
{
    public class InteractableManager
    {
        private IInteractable _currentInteractable;
        public IInteractable CurrentInteractable
        {
            get => _currentInteractable;
            private set
            {
                if (value == null)
                {
                    EventManager.Raise(UIEvents.OnInteractableUpdate, "", false);
                }
                else
                {
                    EventManager.Raise(UIEvents.OnInteractableUpdate, value.InteractMessage, true);
                }
                _currentInteractable = value;
            }
        }

        public InteractableManager()
        {
            InputSystem.BindKey(InputProfile.Gameplay, "Interact", KeyEvent.Release, TryInteract);
            
            EventManager.Subscribe(GameEvents.OnInteractableAdd, AddInteractable);
            EventManager.Subscribe(GameEvents.OnInteractableRemove, RemoveInteractable);
        }

        private void TryInteract()
        {
            CurrentInteractable?.Interact();
            _currentInteractable = null;
        }

        private void AddInteractable(params object[] parameters)
        {
            var inter = parameters[0] as IInteractable;
            
            CurrentInteractable = inter;
        }

        private void RemoveInteractable(params object[] parameters)
        {
            var inter = parameters[0] as IInteractable;

            if (CurrentInteractable == inter)
                CurrentInteractable = null;
        }

        public void Dispose()
        {
            EventManager.Unsubscribe(GameEvents.OnInteractableAdd, AddInteractable);
            EventManager.Unsubscribe(GameEvents.OnInteractableRemove, RemoveInteractable);
            InputSystem.UnbindKey(InputProfile.Gameplay, "Interact", KeyEvent.Release, TryInteract);
            _currentInteractable = null;
        }
    }
}
