namespace DoaT
{
    public class SetSceneFlagEventListener : BaseSceneEventListener
    {
        public SceneFlag flag;
        public bool changeTo = true;
    
        public override void OnEventTriggered(params object[] parameters)
        {
            if (!CanReact) return;

            flag.Value = changeTo;
        }
    }
}