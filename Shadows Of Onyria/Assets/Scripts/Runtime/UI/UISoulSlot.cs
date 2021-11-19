using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DoaT
{
    public class UISoulSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITargetableUI
    {
        public event Action<bool, bool> OnPointerInteract;

        [SerializeField] private SoulSlotType _soulSlotType;
        [SerializeField] private Sprite _defaultIcon;
        [SerializeField] private Image _icon;
        
        public UISoul _containedSoul;
        
        public UISoul ContainedSoul => _containedSoul;
        public SoulSlotType SoulSlotType => _soulSlotType;

        private IGroupController _parent;
        private UISoulPanelManager _panel;
        
        private void Awake()
        {
            if (_icon == null) _icon = GetComponent<Image>();
            if (_defaultIcon == null) _defaultIcon = _icon.sprite;
            else
            {
                _icon.sprite = _defaultIcon;
            }
            
            EventManager.Subscribe(UIEvents.OnSoulSlotLoad, SetupSoul);

            _panel = FindObjectOfType<UISoulPanelManager>();
            _parent = GetComponentInParent<IGroupController>();
            _parent.OnUpdateUI += RedrawUI;
            _parent.OnHideUI += ReleaseUIAssets;
            _parent.OnShowUI += DrawUI;
        }

        public void RedrawUI()
        {
            ReleaseUIAssets();
            DrawUI();
        }

        public void ReleaseUIAssets()
        {
            _containedSoul = null;
            SetupIcon();
        }
        public void DrawUI()
        {
            foreach (var soul in  _panel.inventoryAsset.obtainedSouls)
            {
                if (soul.IsInSlot && soul.slotID == _soulSlotType)
                {
                    EventManager.Raise(UIEvents.OnUISoulNeeded, soul, this);
                    return;
                }
            }
        }

        private void SetupSoul(params object[] parameters)
        {
            var type = (SoulSlotType) parameters[0];

            if (type == SoulSlotType.None) return;
            
            if (_soulSlotType == type)
            {
                var uiSoul = (UISoul) parameters[1];
                SetContainedSoul(uiSoul);
            }
        }
        
        public void SetupIcon()
        {
            _icon.sprite = _containedSoul == null ? _defaultIcon : _containedSoul.ContainedSoul.type.icon;
        }
        
        public void SetContainedSoul(UISoul soul)
        {
            _containedSoul = soul;
            _containedSoul.IsInSlot = true;
            SetupIcon();
            if (_soulSlotType == SoulSlotType.Body)
            {
                EventManager.Raise(UIEvents.TESTOnBodySoulAdded);
            }
        }

        public UISoul SwapContainedSoul(UISoul newSoul)
        {
            if (_soulSlotType == SoulSlotType.Body)
            {
                EventManager.Raise(UIEvents.TESTOnBodySoulRemoved);
            }
            var oldSoul = _containedSoul;
            
            _containedSoul = newSoul;
            SetupIcon();
            
            oldSoul.IsInSlot = false;
            oldSoul.ContainedSoul.slotID = SoulSlotType.None;
            _containedSoul.IsInSlot = true;
            if (_soulSlotType == SoulSlotType.Body)
            {
                EventManager.Raise(UIEvents.TESTOnBodySoulAdded);
            }
            return oldSoul;
        }

        public UISoul ReleaseContainedSoul()
        {
            if (_soulSlotType == SoulSlotType.Body)
            {
                EventManager.Raise(UIEvents.TESTOnBodySoulRemoved);
            }
            var oldSoul = _containedSoul;
            _containedSoul = null;
            SetupIcon();
            oldSoul.IsInSlot = false;
            oldSoul.ContainedSoul.slotID = SoulSlotType.None;
            return oldSoul;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventManager.Raise(UIEvents.OnSlotPointerEnter,  this);
            OnPointerInteract?.Invoke(true, false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventManager.Raise(UIEvents.OnSlotPointerExit,  this);
            OnPointerInteract?.Invoke(false, false);
        }

        public void OnClick()
        {
            EventManager.Raise(UIEvents.OnTargetableClicked, this);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(UIEvents.OnSoulSlotLoad, SetupSoul);
        }
    }
}