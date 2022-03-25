using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace DoaT.UI
{
    [CustomEditor(typeof(UINewGameButton))]
    public class UINewGameButtonInspector : ButtonEditor
    {
        private UINewGameButton _target;
        
        private SerializedProperty _saveSlotPanel;
        
        private Dictionary<string, Type> _behavioursByReflection = new Dictionary<string, Type>();
        
        private string[] _behaviourStrings;
        private string _choiceString = "";
        private int _choiceIndex;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (UINewGameButton)target;
            
            _saveSlotPanel  = serializedObject.FindProperty("_saveSlot");
            
            var type = typeof(ISaveSlotBehaviour);
            _behavioursByReflection = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .ToDictionary(t => t.ToString());
        
            _behaviourStrings = _behavioursByReflection.Select(x => x.Key).ToArray();
        
            if (string.IsNullOrEmpty(_target.BehaviourType))
            {
                Debug.LogWarning("Behaviour shouldn't be null or empty.");
                _choiceIndex = 0;
                _choiceString = _behaviourStrings[0];
                _target.BehaviourType = _behaviourStrings.Length > 0 ? _behavioursByReflection[_behaviourStrings[0]].ToString() : "";
            }
            else
            {
                var targetType = _target.BehaviourType;
                
                if (_behavioursByReflection.ContainsKey(targetType))
                {
                    _choiceString = targetType;
                    _choiceIndex = Array.FindIndex(_behaviourStrings, s => s == _choiceString);
                }
                else
                {
                    Debug.LogError($"There was no string: '{targetType}' inside the reflection dictionary.");
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_target), typeof(UINewGameButton), false);
            GUILayout.Space(10);
            EditorGUILayout.TextField(new GUIContent("Saved String"), _target.BehaviourType);
            GUI.enabled = true;
            
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(_saveSlotPanel);
            serializedObject.ApplyModifiedProperties();
        
            var comparisonIndex = _choiceIndex;
            var choicesDisplay = _behaviourStrings
                .Select(s => StringUtility.GetExtension(s).Replace("SaveSlotBehaviour", ""))
                .ToArray();
            _choiceIndex = EditorGUILayout.Popup("Save Slot Behaviour", _choiceIndex, choicesDisplay);
        
            if (comparisonIndex != _choiceIndex)
            {
                _choiceString = _behaviourStrings[_choiceIndex];
                _target.BehaviourType = _behavioursByReflection[_choiceString].ToString();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Button", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
            base.OnInspectorGUI();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}









//DEBUG - Can Delete after successful testing
// private UINewGameButton _target;
        //
        // private Dictionary<string, Type> _behavioursByReflection = new Dictionary<string, Type>();
        // private string[] _behaviourStrings;
        // private string _choiceString = "";
        // private int _choiceIndex;
        //
        // private readonly Type SaveSlotInterface = typeof(ISaveSlotBehaviour);
        //
        //
        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //
        //     _target = (UINewGameButton) target;
        //
        //     _behavioursByReflection = AppDomain.CurrentDomain.GetAssemblies()
        //         .SelectMany(s => s.GetTypes())
        //         .Where(p => SaveSlotInterface.IsAssignableFrom(p) && !p.IsInterface)
        //         .ToDictionary(t => t.ToString());
        //
        //     _behaviourStrings = _behavioursByReflection.Select(x => x.Key).ToArray();
        //
        //     Debug.Log(_target.BehaviourType);
        //     Debug.Log(_target.GetBackup());
        //
        //     if (string.IsNullOrEmpty(_target.BehaviourType))
        //     {
        //         Debug.LogWarning("Behaviour shouldn't be null or empty.");
        //     }
        //     
        //     var targetType = _target.BehaviourType;
        //     
        //     if (_behavioursByReflection.ContainsKey(targetType))
        //     {
        //         _choiceString = targetType;
        //         _choiceIndex = Array.FindIndex(_behaviourStrings, s => s == _choiceString);
        //     }
        //     else
        //     {
        //         Debug.LogError($"There was no string: '{targetType}' inside the reflection dictionary.");
        //     }
        // }
        //
        // public override void OnInspectorGUI()
        // {
        //     GUI.enabled = false;
        //     EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_target), typeof(UINewGameButton), false);
        //     GUILayout.Space(10);
        //     EditorGUILayout.TextField(new GUIContent("Saved String"), _target.BehaviourType);
        //     GUI.enabled = true;
        //
        //
        //     _target.SetSaveSlotPanel((UISaveSlotsPanel) EditorGUILayout.ObjectField("Save Slot Panel",
        //         _target.GetSaveSlotPanel(), typeof(UISaveSlotsPanel), true));
        //     
        //     var comparisonIndex = _choiceIndex;
        //     var choicesDisplay = _behaviourStrings
        //         .Select(s => StringUtility.GetExtension(s).Replace("SaveSlotBehaviour", ""))
        //         .ToArray();
        //     _choiceIndex = EditorGUILayout.Popup("Save Slot Behaviour", _choiceIndex, choicesDisplay);
        //     
        //     if (comparisonIndex != _choiceIndex)
        //     {
        //         _choiceString = _behaviourStrings[_choiceIndex];
        //         _target.BehaviourType = _behavioursByReflection[_choiceString].ToString();
        //     }
        //     
        //     GUILayout.Space(10);
        //     GUILayout.Label("Button", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
        //
        //     base.OnInspectorGUI();
        //     
        //     
        //     if (GUI.changed)
        //     {
        //         EditorUtility.SetDirty(_target);
        //         EditorSceneManager.MarkSceneDirty(_target.gameObject.scene);
        //     }
        // }