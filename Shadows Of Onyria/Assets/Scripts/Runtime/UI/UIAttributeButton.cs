using System;
using UnityEngine;

public class UIAttributeButton : MonoBehaviour
{
    public event Action OnButtonClicked;

    public void OnButtonClickedEvent()
    {
        OnButtonClicked?.Invoke();
    }
}