using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Scene/Scene", fileName = "Scene_")]
    public class SceneData : ScriptableObject
    {
        public SceneReference scene;
        public List<SceneFlag> temporaryFlags;
    }
}
