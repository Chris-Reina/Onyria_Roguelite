using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Scene/Level Scene", fileName = "Scene_Level")]
    public class LevelSceneData : SceneData
    {
        public int exitsAmount;
    }
}