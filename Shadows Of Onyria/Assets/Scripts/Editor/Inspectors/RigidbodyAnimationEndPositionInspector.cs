using DoaT;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RigidbodyAnimationEndPosition))]
public class RigidbodyAnimationEndPositionInspector : Editor
{
    private RigidbodyAnimationEndPosition _target;

    private void OnEnable()
    {
        _target = (RigidbodyAnimationEndPosition) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Get Transforms"))
        {
            _target.GetTransforms();
        }

        if (_target.transforms.Count == 0) GUI.enabled = false;
        if (GUILayout.Button("Set"))
        {
            _target.Set();
        }
        if (_target.transforms.Count == 0) GUI.enabled = true;
    }
}