using System;
using UnityEngine;
using DoaT.Vendor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DoaT
{
    public class PlayerItemSlot : MonoBehaviour, ISelectableSlot, IPointerClickHandler
    {
        private enum ItemSlot
        {
            First,
            Second,
            Third,
            Fourth,
        }
    
        [SerializeField] private bool _selectable = true;
        [Space]
        [SerializeField] private ItemSlot _slot;
        [SerializeField] private ItemInventoryData _data;
        [SerializeField] private Sprite _transparentIcon;
        [SerializeField] private GameObject _selection;
        
        public bool Selectable => _selectable;

        public int Index => (int) _slot;
        
        public bool IsEmpty
        {
            get
            {
                return _slot switch
                {
                    ItemSlot.First => !_data.FirstSlotOccupied,
                    ItemSlot.Second => !_data.SecondSlotOccupied,
                    ItemSlot.Third => !_data.ThirdSlotOccupied,
                    ItemSlot.Fourth => !_data.FourthSlotOccupied,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        public Item CurrentItem
        {
            get
            {
                var item = _slot switch
                {
                    ItemSlot.First => _data.FirstSlot,
                    ItemSlot.Second => _data.SecondSlot,
                    ItemSlot.Third => _data.ThirdSlot,
                    ItemSlot.Fourth => _data.FourthSlot,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                return item;
            }
        }

        private Image _image;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            _data.OnItemUpdate += SetImage;
            SetImage((int) _slot, CurrentItem);
        }
        
        private void SetImage(int index, Item item)
        {
            if (index != (int) _slot) return;
            
            _image.sprite = IsEmpty ? _transparentIcon : item.icon;
        }
        
        private void OnDestroy()
        {
            _data.OnItemUpdate -= SetImage;
        }
        
        public void Select() => _selection.SetActive(true);
        public void Deselect() => _selection.SetActive(false);
        public void OnPointerClick(PointerEventData eventData)
        {
            if(Selectable) EventManager.Raise(UIEvents.OnVendorSlotSelected, this);
        }
    }
}