namespace DoaT
{
    public class RestoreHealthEventListener : BaseSceneEventListener
    {
        public override void OnEventTriggered(params object[] parameters)
        {
            if (!CanReact) return;
            var health = FindObjectOfType<TheodenModel>();

            if (FindObjectOfType<PersistentData>() != null)
                PersistentData.Health.value = health.health.MaxHealth;

            health.health.RefillHealth();
        }
    }
}