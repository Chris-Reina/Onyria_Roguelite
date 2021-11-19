
namespace DoaT
{
    public class OnStartEventTrigger : BaseSceneEventTrigger
    {
        private void Start()
        {
            if (CanTrigger)
                Trigger();
        }

        public override void Trigger()
        {
            EventManager.Raise(_sceneEvent);
        }
    }
}
