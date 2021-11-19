using UnityEditor;

namespace DoaT
{
    [CustomEditor(typeof(SceneData))]
    public class SceneDataInspector : Editor
    {
        private SceneData _target;

        private void OnEnable()
        {
            _target = (SceneData) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}