
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugSceneFlagTrigger))]
public class DebugSceneFlagTriggerInspector : Editor
{
    private DebugSceneFlagTrigger _target;

    private void OnEnable()
    {
        _target = (DebugSceneFlagTrigger) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying) GUI.enabled = false;
        if (GUILayout.Button("Trigger"))
        {
            _target.Trigger();
        }
        if (!Application.isPlaying) GUI.enabled = true;
    }
}