using System;
using UnityEngine;

namespace DoaT.UI
{
    [Serializable]
    public class UINewGameButton : UIButton
    {
        [SerializeField] private UISaveSlotsPanel _saveSlot;
        [SerializeField] private string _behaviourType;
        
        public string BehaviourType
        {
            get => _behaviourType;
            set => _behaviourType = value;
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            OnInteractionSucceeded += OpenSaveSlots;
        }
        
        private void OpenSaveSlots()
        {
            _saveSlot.DisplayPanel((ISaveSlotBehaviour) Activator.CreateInstance(Type.GetType(_behaviourType) ??
                throw new InvalidOperationException("Type not contemplated")));
        }
        
        public UISaveSlotsPanel GetSaveSlotPanel() => _saveSlot;
        public void SetSaveSlotPanel(UISaveSlotsPanel panel) => _saveSlot = panel;
    }
    
    #if UNITY_EDITOR
#endif
    
    
    public interface ISaveSlotBehaviour
    {
        public void Confirm();
        public void Interact(int slot);
    }
    
    public class SaveSlotBehaviourSave : ISaveSlotBehaviour
    {
        public void Confirm()
        {
            throw new NotImplementedException();
        }

        public void Interact(int slot)
        {
            Debug.Log($"executed SaveSlotBehaviourSave Interact with slot {slot}");
        }
    }
    
    public class SaveSlotBehaviourLoad : ISaveSlotBehaviour
    {
        public void Confirm()
        {
            throw new NotImplementedException();
        }
        public void Interact(int slot)
        {
            Debug.Log($"executed SaveSlotBehaviourLoad Interact with slot {slot}");
        }
    }
    
    public class SaveSlotBehaviourCreate : ISaveSlotBehaviour
    {
        public void Confirm()
        {
            throw new NotImplementedException();
        }
        public void Interact(int slot)
        {
            Debug.Log($"executed SaveSlotBehaviourCreate Interact with slot {slot}");
        }
    }
}