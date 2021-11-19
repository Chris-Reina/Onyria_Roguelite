using System;
using DoaT.Vendor;
using UnityEngine;
using UnityEngine.UI;

namespace DoaT
{
    public class UIVendorManager : CanvasGroupController, IInputUIComponent, IUpdate
    {
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button _buyButton;
        
        [SerializeField] private VendorInventoryData data;
        
        private ISelectableSlot _currentSlot;
        
        private void Start()
        {
            _sellButton.gameObject.SetActive(false);
            _buyButton.gameObject.SetActive(false);
            
            EventManager.Subscribe(UIEvents.OnVendorSlotSelected, SelectSlot);
            ExecutionSystem.AddUpdate(this);
        }
        
        public void OnUpdate()
        {
            switch (_currentSlot)
            {
                case VendorSlot vSlot:
                    _buyButton.interactable = CanBuyItem(vSlot);
                    return;
                case PlayerItemSlot pSlot:
                    _sellButton.interactable = CanSellItem(pSlot);
                    return;
            }
        }

        private bool CanSellItem(PlayerItemSlot slot)
        {
            if (slot == null) return false;
            
            return !slot.IsEmpty;
        }

        private bool CanBuyItem(VendorSlot slot)
        {
            var canSpend = PersistentData.ItemInventory.data.Gold >= slot.GetItemBuyAmount();
            var hasRoom = PersistentData.ItemInventory.data.HasRoom();

            return canSpend && hasRoom;
        }

        private void SelectSlot(object[] obj)
        {
            var slot = (ISelectableSlot) obj[0];

            if (_currentSlot == null)
            {
                _currentSlot = slot;
                slot.Select();
            }
            else
            {
                if (ReferenceEquals(slot, _currentSlot)) return;

                _currentSlot.Deselect();
                _currentSlot = slot;
                _currentSlot.Select();
            }

            switch (_currentSlot)
            {
                case VendorSlot _:
                    _sellButton.gameObject.SetActive(false);
                    _buyButton.gameObject.SetActive(true);
                    return;
                case PlayerItemSlot _:
                    _sellButton.gameObject.SetActive(true);
                    _buyButton.gameObject.SetActive(false);
                    return;
            }
        }

        public override void ShowUI()
        {
            base.ShowUI();
            ExecutionSystem.AddUpdate(this);
        }
        public override void HideUI()
        {
            base.HideUI();
            ExecutionSystem.RemoveUpdate(this, false);
        }
        
        public void OnSelectInput() { }
        public void OnReturnInput()
        {
            _currentSlot.Deselect();
            _currentSlot = null;
            HideUI();
        }
        
        private void OnDestroy()
        {
            ExecutionSystem.RemoveUpdate(this, true);
            EventManager.Unsubscribe(UIEvents.OnVendorSlotSelected, SelectSlot);
        }

        public void TryBuy()
        {   
            var (x, y) = ((VendorSlot) _currentSlot).Index;
            var item = data.items[x, y];

            if (item == null) return;
            
            data.DeleteAt(x,y);
            PersistentData.ItemInventory.data.Gold -= item.buyAmount;
            PersistentData.ItemInventory.data.SetItemInFirstOpenSlot(item);
        }
        public void TrySell()
        { 
            var index = ((PlayerItemSlot) _currentSlot).Index;
            var item = PersistentData.ItemInventory.data.GetItem(index);
            
            if (item == null) return;
            
            PersistentData.ItemInventory.data.Gold += item.sellAmount;
            PersistentData.ItemInventory.data.ClearItem(index);
        }
        public void Confirm()
        {
            HideUI();
        }
    }
}
