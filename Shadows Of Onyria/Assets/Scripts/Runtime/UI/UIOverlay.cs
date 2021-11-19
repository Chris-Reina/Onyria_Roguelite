using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIOverlay : MonoBehaviour
{
    [SerializeField] private GameObject possibleTarget;
    [SerializeField] private Image _overlayImage;

    [SerializeField, ColorUsage(true)] 
    private Color _hoveredColor;
    [SerializeField, ColorUsage(true)] 
    private Color _normalColor;
    [SerializeField, ColorUsage(true)] 
    private Color _unavailableColor;
    
    private  ITargetableUI _observed;

    private void Awake()
    {
        if (_overlayImage == null)
            _overlayImage = GetComponent<Image>();
        
        if (possibleTarget != null)
        {
            _observed = possibleTarget.GetComponent<ITargetableUI>();
            if(_observed == null)
                Debug.LogError($"The possibleTarget of an object of type {GetType()} of name {name} has no component of type ITargetableUI");
        }
        else
        {
            _observed = GetComponentInParent<ITargetableUI>();
            if(_observed == null)
                Debug.LogError($"There is no component of type ITargetableUI in {name}'s parent.");
        }

        if (_observed != null) _observed.OnPointerInteract += SetupOverlay;
    }
    
    private void SetupOverlay(bool hover, bool isSelected)
    {
        if (!isSelected)
            _overlayImage.color = hover ? _hoveredColor : _normalColor;
        else
            _overlayImage.color = _unavailableColor;
    }
}
