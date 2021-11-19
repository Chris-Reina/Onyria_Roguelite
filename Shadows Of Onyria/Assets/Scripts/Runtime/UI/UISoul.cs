using System;
using DoaT;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISoul : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITargetableUI
{
    public event Action<bool, bool> OnPointerInteract;
    
    [SerializeField] private TextMeshProUGUI identifier;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Image icon;
    [SerializeField] private Image border;
    
    private Soul _containedSoul;
    public Soul ContainedSoul => _containedSoul;
    public bool IsSelected => _isSelected || IsInSlot;
    public bool IsInSlot { get; set; }

    private bool _isSelected;

    private void Awake()
    {
        EventManager.Subscribe(UIEvents.OnSoulSelected, OnSoulSelected);
        EventManager.Subscribe(UIEvents.OnSoulDropped, OnSoulDropped);
        EventManager.Subscribe(UIEvents.OnUISoulNeeded, Help);
    }

    private void Help(params object[] parameters)
    {
        var soull = (Soul) parameters[0];
        var slot = (UISoulSlot) parameters[1];
        
        if (soull == _containedSoul)
        {
            slot._containedSoul = this;
            slot.SetupIcon();
        }
    }
    
    private void Start()
    {
        var slotType = _containedSoul.slotID;
        if (slotType == SoulSlotType.None) return;
        
        EventManager.Raise(UIEvents.OnSoulSlotLoad, slotType, this);
        OnPointerInteract?.Invoke(false, true);
    }

    private void OnSoulSelected(params object[] parameters)
    {
        var container = (ITargetableUI) parameters[0];

        if (ReferenceEquals(container, this))
        {
            _isSelected = true;
            OnPointerInteract?.Invoke(false, IsSelected);
        }
    }
    private void OnSoulDropped(params object[] parameters)
    {
        var container = (ITargetableUI) parameters[0];

        if (ReferenceEquals(container, this))
        {
            _isSelected = false;
            OnPointerInteract?.Invoke(false, IsSelected);
        }
    }

    public void Setup(Soul contained)
    {
        _containedSoul = contained;
        identifier.text = contained.type.identifier;
        icon.sprite = contained.type.icon;
        level.text = contained.level.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsSelected) return;
        EventManager.Raise(UIEvents.OnSlotPointerEnter,  this);
        OnPointerInteract?.Invoke(true, IsSelected);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.Raise(UIEvents.OnSlotPointerExit,  this);
        OnPointerInteract?.Invoke(false, IsSelected);
    }

    public void OnClick()
    {
        EventManager.Raise(UIEvents.OnTargetableClicked, this);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(UIEvents.OnSoulSelected, OnSoulSelected);
        EventManager.Unsubscribe(UIEvents.OnSoulDropped, OnSoulDropped);
        EventManager.Unsubscribe(UIEvents.OnUISoulNeeded, Help);
    }
}
