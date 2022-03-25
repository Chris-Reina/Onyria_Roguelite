using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class TESTStringInspector : MonoBehaviour
{
    [SerializeField] private string _privateString;

    public string PropertyString
    {
        get => _privateString;
        set => _privateString = value;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TESTStringInspector))]
public class TESTStringInspectorInspector : Editor
{
    private TESTStringInspector _target;

    private void OnEnable()
    {
        _target = (TESTStringInspector) target;
        
        Debug.Log(_target.PropertyString);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        _target.PropertyString = EditorGUILayout.TextField("TextDisplay", _target.PropertyString);
    }
}

#endif
