using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DoaT
{
    public class Initializer : MonoBehaviour
    {
        public static Initializer Current { get; private set; }

        public float minimumWaitingTime = 2f;

        public List<MonoBehaviourInit> InitializationProcesses;
        public List<ScriptableObject> awakeList;
        
        public float Progress { get; private set; }
        public bool IsDone => Progress >= 1f;

        private void Awake()
        {
            Current = this;
            
            foreach (var so in awakeList)
            {
                if(so is IAwake awake)
                    awake.OnAwake();
            }
        }

        public void BeginInitialization()
        {
            FindInitializationProcesses();
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            Progress = Mathf.Min(1f / (InitializationProcesses.Count + 1), 0.99f);
            yield return new WaitForSeconds(minimumWaitingTime);
             
            if (InitializationProcesses.Count != 0)
            {
                var overallProgress = 0f;
                for (int i = 0; i < InitializationProcesses.Count; i++)
                {
                    var progress = InitializationProcesses[i].OnInitialization();
                    while (progress < 1f)
                    {
                        yield return null;
                        progress = InitializationProcesses[i].OnInitialization();
                    }

                    overallProgress += 1;
                    Progress = overallProgress / (InitializationProcesses.Count + 1);
                    yield return null;
                }
            }
            
            Progress = 1f;
            //DebugManager.LogWarning("Initialization Complete");
        }

        private void OnDestroy()
        {
            if (Current == this) Current = null;
        }

        public void FindInitializationProcesses()
        {
            InitializationProcesses = FindObjectsOfType<MonoBehaviourInit>().ToList();
        } 
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Initializer))]
    public class InitializationManagerInspector : Editor
    {
        private Initializer _target;

        private void OnEnable()
        {
            _target = (Initializer) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Find Process Classes"))
            {
                _target.FindInitializationProcesses();
            }
        }
    }
#endif
}
