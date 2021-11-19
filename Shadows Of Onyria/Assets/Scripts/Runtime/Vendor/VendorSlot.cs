using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DoaT.Vendor
{
    public class VendorSlot : MonoBehaviour, ISelectableSlot, IPointerClickHandler
    {
        //[SerializeField] private bool _selectable = true;
        //[Space]
        [SerializeField] private Vector2Int _index;
        [SerializeField] private VendorInventoryData _data;
        [SerializeField] private Sprite _transparentIcon;
        [SerializeField] private GameObject _selection;

        public bool Selectable => true;//_selectable;
        
        private VendorTooltip _tooltip;
        private Image _image;
        private Item _item;

        public bool IsEmpty { get; private set; } = true;
        public Tuple<int, int> Index => new Tuple<int, int>(_index.x, _index.y);
        
        private void Awake()
        {
            _tooltip = GetComponent<VendorTooltip>();
            _image = GetComponent<Image>();
            
            _data.OnItemUpdate += SetItem;
        }

        private void Start()
        {
            SetItem(_index, _data.items[_index.x, _index.y]);
        }

        public int GetItemBuyAmount() => _item == null ? int.MaxValue : _item.buyAmount;
        public int GetItemSellAmount() => _item == null ? 0 : _item.buyAmount;

        public void SetItem(Vector2Int coords, Item item)
        {
            if (_index != coords) return;

            if (item == null)
            {
                IsEmpty = true;
                _item = null;
                _image.sprite = _transparentIcon;
                TooltipSystem.Hide();
                return;
            }

            IsEmpty = false;
            _item = item;
            _image.sprite = item.icon;
            _tooltip.SetTooltip(_item);
        }

        private void OnDestroy()
        {
            _data.OnItemUpdate -= SetItem;
        }

        public void Select() => _selection.SetActive(true);
        public void Deselect() => _selection.SetActive(false);
        public void OnPointerClick(PointerEventData eventData) => EventManager.Raise(UIEvents.OnVendorSlotSelected, this);
    }
}