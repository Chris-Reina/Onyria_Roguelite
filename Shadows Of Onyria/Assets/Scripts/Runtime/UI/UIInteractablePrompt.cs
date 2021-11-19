using System;
using DoaT;
using TMPro;
using UnityEngine;

public class UIInteractablePrompt : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        EventManager.Subscribe(UIEvents.OnInteractableUpdate, UpdateDisplay);
    }

    private void UpdateDisplay(params object[] parameters)
    {
        var show = (bool)parameters[1];

        if (show)
        {
            var message = (string)parameters[0];

            _text.text = message;
            _group.Activate();
        }
        else
        {
            _group.Deactivate();
        }
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(UIEvents.OnInteractableUpdate, UpdateDisplay);
    }
}   
