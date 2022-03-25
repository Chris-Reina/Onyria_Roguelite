using UnityEditor;
using UnityEngine;
using DoaT.UI;
using DoaT.AI;

namespace DoaT.AI
{
    public class TESTUINewGameOld : UIButton
    {
        [SerializeField] private SceneFlagProfile reset;
        
        [SerializeField] private ScenePair thisLevelData;
        [SerializeField] private ScenePair nextLevelData;

        protected override void Awake()
        {
            base.Awake();

            OnInteractionSucceeded += Play;
        }

        private void Play()
        {
            reset.ResetFlags();
            GameManager.BeginLoad(new SceneLoadData(thisLevelData, nextLevelData));
        }
    }
}

#if UNITY_EDITOR


namespace DoaT.UI
{
    [CustomEditor(typeof(TESTUINewGameOld))]
    public class TESTUINewGameOldInspector : Editor
    {
        private TESTUINewGameOld _target;
        
        protected void OnEnable()
        {
            _target = (TESTUINewGameOld)target;
        }
        
        public override void OnInspectorGUI()
        {
            // GUI.enabled = false;
            // EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_target), typeof(UINewGameButton), false);
            // GUI.enabled = true;

            DrawDefaultInspector();
        }
    }
}
#endif