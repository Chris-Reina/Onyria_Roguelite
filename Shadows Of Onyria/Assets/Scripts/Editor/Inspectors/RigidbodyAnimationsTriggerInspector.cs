using DoaT;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RigidbodyAnimationsTrigger))]
public class RigidbodyAnimationsTriggerInspector : Editor
{
    private RigidbodyAnimationsTrigger _target;

    private void OnEnable()
    {
        _target = (RigidbodyAnimationsTrigger) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Play"))
        {
            _target.Play();
        }
        if (GUILayout.Button("Reset"))
        {
            _target.Reset();
        }
    }
}
