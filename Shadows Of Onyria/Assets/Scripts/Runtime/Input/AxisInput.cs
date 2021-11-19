using System;
using System.Collections.Generic;

namespace DoaT.Inputs
{
    [Serializable]
    public class AxisInput
    {
        public string name;
        public string unityAxisName;
        public Dictionary<AxisEvent, Action<float>> callback = new Dictionary<AxisEvent, Action<float>>();
        
        public AxisInput() {}
        public AxisInput(string name, string unityAxisName)
        {
            this.name = name;
            this.unityAxisName = unityAxisName;
        }
        
        public void Clear()
        {
            callback.Clear();
        }
    }
}