using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugSceneEventTrigger))]
public class DebugSceneEventTriggerInspector : Editor
{
    private DebugSceneEventTrigger _target;

    private void OnEnable()
    {
        _target = (DebugSceneEventTrigger) target;
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
