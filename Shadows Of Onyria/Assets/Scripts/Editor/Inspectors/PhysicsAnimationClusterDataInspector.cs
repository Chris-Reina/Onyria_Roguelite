using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicsAnimationClusterData))]
public class PhysicsAnimationClusterDataInspector : Editor
{
    private PhysicsAnimationClusterData _target;

    private bool _unlock;

    private void OnEnable()
    {
        _target = (PhysicsAnimationClusterData) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("CleanupAnimations"))
        {
            _target.CleanupAnimations();
        }

        EditorGUILayout.Space();
        _unlock = EditorGUILayout.Toggle("ActivateClear", _unlock);
        if (!_unlock) GUI.enabled = false;
        if (GUILayout.Button("Clear Animations"))
        {
            _target.ClearAnimations();
        }
        if (!_unlock) GUI.enabled = true;
    }
}
