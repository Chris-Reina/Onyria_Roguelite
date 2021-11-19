using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT.Inputs
{
    [Serializable]
    public class KeyInput
    {
        public string keyName;
        public List<KeyCode> keys = new List<KeyCode>();
        public Dictionary<KeyEvent, Action> callback = new Dictionary<KeyEvent, Action>();
        
        public KeyInput(){}
        public KeyInput(string keyName, KeyCode key)
        {
            this.keyName = keyName;
            keys.Add(key);
        }
        public KeyInput(string keyName, IEnumerable<KeyCode> keys)
        {
            this.keyName = keyName;
            this.keys = new List<KeyCode>(keys);
        }

        public void Clear()
        {
            callback.Clear();
        }
    }
}