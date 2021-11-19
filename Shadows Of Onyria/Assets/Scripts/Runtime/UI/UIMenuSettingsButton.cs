using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class UIMenuSettingsButton : MonoBehaviour
{
    [SerializeField] private CanvasGroup _managedGroup;
    [SerializeField] private List<UIMenuSettingsButton> _otherGroups = new List<UIMenuSettingsButton>();

    private void Awake()
    {
        GetOtherButtons();
    }

    public void ShowGroup()
    {
        foreach (var gp in _otherGroups)
        {
            gp.HideGroup();
        }

        _managedGroup.Activate();
    }

    public void HideGroup()
    {
        _managedGroup.Deactivate();
    }

    public void GetOtherButtons()
    {
        if (transform.parent == null) return;
        
        var buttons = transform.parent.GetComponentsInChildren<UIMenuSettingsButton>();

        _otherGroups.Clear();
        
        foreach (var button in buttons)
        {
            if (button != this)
                _otherGroups.Add(button);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIMenuSettingsButton))]
public class UIMenuSettingsButtonEditor : Editor
{
    private UIMenuSettingsButton _target;
    
    private void OnEnable()
    {
        _target = (UIMenuSettingsButton)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space(5);

        if (GUILayout.Button(new GUIContent("Get Other Groups*",
            "Only useful for debug instances, will execute on Awake in playtime.")))
        {
            _target.GetOtherButtons();
        }
    }
}
#endif