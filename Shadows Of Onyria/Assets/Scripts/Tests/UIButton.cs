using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DoaT.UI
{
    public abstract class UIButton : Button
    {
        public event Action<bool> OnInteraction;
        public event Action<bool> OnSelectionStateChange;
        public event Action<bool> OnInteractableStateChange;

        protected event Action OnInteractionSucceeded;
        
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            OnSelectionStateChange?.Invoke(true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            OnSelectionStateChange?.Invoke(false);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);

            OnInteraction?.Invoke(interactable);
                
            if (interactable) OnInteractionSucceeded?.Invoke();
            
            //EventManager.Raise(UIEvents.OnButtonSubmit, interactable); //TODO: Delete Events, move to Observer Pattern
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            var system = EventSystem.current;
            if (system.currentSelectedGameObject == gameObject) system.SetSelectedGameObject(null);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if(interactable) OnInteractionSucceeded?.Invoke();
            //EventManager.Raise(UIEvents.OnButtonSubmit, interactable);
        }
    }
}