using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Scene/Flag Profile", fileName = "FlagProfile_")]
    public class SceneFlagProfile : ScriptableObject
    {
        public List<SceneFlag> flags = new List<SceneFlag>();

        public void ResetFlags()
        {
            if (flags.Count == 0) return;

            for (int i = 0; i < flags.Count; i++)
            {
                flags[i].ResetValue();
            }
        }

        public void SetFlagByName(string flagName)
        {
            //TODO: IMPLEMENT
        }
    }
}
