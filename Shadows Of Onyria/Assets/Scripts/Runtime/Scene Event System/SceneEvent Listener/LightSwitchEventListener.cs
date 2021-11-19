using UnityEngine;

namespace DoaT
{
    public class LightSwitchEventListener : BaseSceneEventListenerDual
    {
        public new Light light;
        
        public override void OnEventTriggered(params object[] parameters)
        {
            light.enabled = true;
        }

        public override void OnEventTriggeredDual(params object[] parameters)
        {
            light.enabled = false;
        }
    }
}