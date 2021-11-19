using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoaT
{
    public class DamageFeedbackManager : MonoBehaviour
    {
        [SerializeField] private ColorDataVariable _playerDamageColor;
        [SerializeField] private ColorDataVariable _playerHealColor;
    
        [SerializeField] private DamageFeedback feedback = default;
        [SerializeField] private BloodDecal blood = default;

        private Pool<DamageFeedback> _feedbackPool;
        private Pool<BloodDecal> _bloodDecalPool;

        private void Awake()
        {
            DamageFeedback FeedbackFactory(object x) => Instantiate((DamageFeedback) x, transform);
            BloodDecal BloodFactory(object x) => Instantiate((BloodDecal) x, transform);

            _feedbackPool = new Pool<DamageFeedback>(feedback, 5, FeedbackFactory, true);
            _bloodDecalPool = new Pool<BloodDecal>(blood, 5, BloodFactory, true);
        }

        private void Start()
        {
            EventManager.Subscribe(PlayerEvents.OnDamageTaken, SpawnDamageFeedbackPlayer);
            EventManager.Subscribe(PlayerEvents.OnHeal, SpawnHealFeedbackPlayer);
            
            EventManager.Subscribe(EntityEvents.OnDamageTaken, SpawnDamageFeedback);
            EventManager.Subscribe(EntityEvents.OnDeath, SpawnBlood);
        }

        private void SpawnHealFeedbackPlayer(object[] obj)
        {
            var location = (Vector3) obj[0];
            var amount = (float) obj[1];
            var attackable = (IAttackable) obj[2];
        }

        private void SpawnDamageFeedbackPlayer(object[] obj)
        {
            var location = (Vector3) obj[0];
            var damage = (float) obj[1];
            var attackable = (IAttackable) obj[2];
         
            var spawn = _feedbackPool.GetObject();

            spawn.Initialize((int) damage, false, attackable, _playerDamageColor.SerializableColor);
            spawn.Activate(location + attackable.FeedbackDisplacement, spawn.transform.rotation);
        }

        private void SpawnDamageFeedback(params object[] parameters)
        {
            var location = (Vector3) parameters[0];
            var target = (IAttackable) parameters[1];
            var damage = (float) parameters[2];
            var isCritical = (bool) parameters[3];
            var attackInfo = (AttackInfo) parameters[4];

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var currentFeedback = _feedbackPool.CurrentActiveObjects
                                               .Where(x => x.attacked == target)
                                               .FirstOrDefault();

            if (currentFeedback != null)
            {
                currentFeedback.AddDamage((int) damage, isCritical);
                return;
            }

            var spawn = _feedbackPool.GetObject();

            spawn.Initialize((int) damage, isCritical, target);
            spawn.Activate(location + target.FeedbackDisplacement, spawn.transform.rotation);
        }

        private void SpawnBlood(params object[] parameters)
        {
            var location = (Vector3) parameters[0];

            var spawn = _bloodDecalPool.GetObject();

            spawn.Initialize();
            spawn.Activate(location + new Vector3(0, Random.Range(0.001f,0.01f), 0), Quaternion.Euler(0, Random.Range(-359f, 359f), 0));
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(PlayerEvents.OnDamageTaken, SpawnDamageFeedbackPlayer);
            EventManager.Unsubscribe(PlayerEvents.OnHeal, SpawnHealFeedbackPlayer);
            
            EventManager.Unsubscribe(EntityEvents.OnDamageTaken, SpawnDamageFeedback);
            EventManager.Unsubscribe(EntityEvents.OnDeath, SpawnBlood);
        }
    }
}
