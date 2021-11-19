namespace DoaT
{
    public interface ISceneEventCondition
    {
        bool CanOperate { get; }
    }

    public interface ISceneEventComponent
    {
        void AddConditional(ISceneEventCondition condition);
        void RemoveConditional(ISceneEventCondition condition);
    }

    public interface ISceneEventTrigger : ISceneEventComponent
    {
        bool CanTrigger { get; }
        SceneEvent TriggeredSceneEvent { get; }
        
        void Trigger();
    }
    
    public interface ISceneEventListener : ISceneEventComponent
    {
        SceneEvent ListenedSceneEvent { get; }
        void OnEventTriggered(params object[] parameters);
    }

    public interface ISceneEventListenerDual : ISceneEventListener
    {
        SceneEvent ListenedSceneEventDual { get; }
        void OnEventTriggeredDual(params object[] parameters);
    }
}
