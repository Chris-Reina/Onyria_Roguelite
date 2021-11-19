using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT.Inputs
{
    [CreateAssetMenu(fileName = "InputOptions", menuName = "Data/Options/InputData")]
    public class InputData : ScriptableObject
    {
        public List<AxisInput> axisInputs = new List<AxisInput>();
        public List<KeyInput> keyInputs = new List<KeyInput>();

        public void Unload()
        {
            foreach (var axisInput in axisInputs)
            {
                axisInput.Clear();
            }

            foreach (var keyInput in keyInputs)
            {
                keyInput.Clear();
            }
        }

        public bool CheckKeyAvailability(KeyCode key)
        {
            return keyInputs.All(keyInput => !keyInput.keys.Contains(key));
        }

        public void SetKey(string action, KeyCode key)
        {
            foreach (var keyInput in keyInputs)
            {
                if (string.Equals(keyInput.keyName, action))
                {
                    keyInput.keys = new List<KeyCode> {key};
                    return;
                }
            }
        }

        public string RemoveUsedKey(KeyCode key)
        {
            foreach (var keyInput in keyInputs)
            {
                for (int i = 0; i < keyInput.keys.Count; i++)
                {
                    if (keyInput.keys[i] != key) continue;
                    
                    keyInput.keys.RemoveAt(i);
                    return keyInput.keyName;
                }
            }

            return "";
        }
    }
}