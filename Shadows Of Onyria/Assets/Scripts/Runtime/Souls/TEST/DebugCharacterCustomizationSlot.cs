using System;
using DoaT;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "DEBUG_", menuName = "Abilities/Concrete/Debug/Print")]
public class DebugCharacterCustomizationSlot : CharacterCustomization
{
    [TextArea] public string message;
    
    private void OnEnable()
    {
        EventManager.Subscribe(UIEvents.OnSoulWindowApply, Print);
    }

    private void Print(object[] p)
    {
        DebugManager.LogWarning(message);
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(UIEvents.OnSoulWindowApply, Print);
    }
}
