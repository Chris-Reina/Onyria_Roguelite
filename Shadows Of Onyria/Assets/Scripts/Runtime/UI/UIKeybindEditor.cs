using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT.Inputs
{
    public class UIKeybindEditor : MonoBehaviour
    {
        public string keyAction;

        public bool inEditMode = false;
        public InputData data;
        
        
        public void TrySetKey()
        {
            var keys = GetValues<KeyCode>();

            foreach (var key in keys)
            {
                if (!Input.GetKeyDown(key)) continue;

                if (data.CheckKeyAvailability(key))
                {
                        
                }
                else
                {
                    data.RemoveUsedKey(key);
                    data.SetKey(keyAction, key);
                }

                return;
            }
        }
        
        public static IEnumerable<T> GetValues<T>() //where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
