using UnityEditor;
using UnityEngine;

namespace DoaT
{
    [CustomEditor(typeof(SceneFlagProfile))]
    public class SceneFlagProfileInspector : Editor
    {
        private SceneFlagProfile _target;

        private void OnEnable()
        {
            _target = (SceneFlagProfile) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_target.flags.Count == 0) GUI.enabled = false;
            if (GUILayout.Button("Reset"))
            {
                _target.ResetFlags();
            }
            if (_target.flags.Count == 0) GUI.enabled = true;
        }
    }
}