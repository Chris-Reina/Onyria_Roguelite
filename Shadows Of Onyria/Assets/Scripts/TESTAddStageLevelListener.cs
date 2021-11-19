using UnityEngine;

namespace DoaT
{
    public class TESTAddStageLevelListener : BaseSceneEventListener
    {
        public override void OnEventTriggered(params object[] parameters)
        {
            if (!CanReact) return;

            PersistentData.RunGenerationManager.AddToCurrentStage();
            PersistentData.RunGenerationManager.AddToVendorStage();
        }
    }
}
