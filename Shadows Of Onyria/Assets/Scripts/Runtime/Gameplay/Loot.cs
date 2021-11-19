using UnityEngine;

namespace DoaT
{
    public class Loot : MonoBehaviour
    {
        //TODO: Expand class to accomodate visual effects

        public SoulType type;

        private void Awake()
        {
            TimerManager.SetTimer(new TimerHandler(), GiveLoot, 3f);
        }

        private void GiveLoot()
        {
            PersistentData.SoulInventory.data.obtainedSouls.Add(new Soul(1, type));
            Destroy(gameObject);
        }
    }
}
