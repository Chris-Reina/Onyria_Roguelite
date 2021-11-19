using System;
using DoaT;
using UnityEngine;
using UnityEngine.UI;

public class UIPointerSoulIcon : CanvasGroupController
{
    public Image IconImage;

    protected override void Awake()
    {
        base.Awake();
        
        EventManager.Subscribe(UIEvents.OnSoulSelected, OnSoulSelected);
        EventManager.Subscribe(UIEvents.OnSoulDropped, OnSoulDropped);
    }

    private void OnSoulSelected(params object[] parameters)
    {
        var container = (ITargetableUI) parameters[0];

        if (container is UISoul soul)
        {
            IconImage.sprite = soul.ContainedSoul.type.icon;
            ShowUI();
        }
    }
    
    private void OnSoulDropped(params object[] parameters)
    {
        HideUI();
    }

    private void LateUpdate()
    {
        if (!_isShowing) return;
        transform.position = Input.mousePosition;
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(UIEvents.OnSoulSelected, OnSoulSelected);
        EventManager.Unsubscribe(UIEvents.OnSoulDropped, OnSoulDropped);
    }
}



