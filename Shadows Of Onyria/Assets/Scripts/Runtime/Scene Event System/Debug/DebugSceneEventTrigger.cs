using DoaT;

public class DebugSceneEventTrigger : BaseSceneEventTrigger
{
    public override bool CanTrigger => true;
    public override void Trigger()
    {
        EventManager.Raise(_sceneEvent);
    }
}
