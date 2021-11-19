using UnityEngine;

namespace DoaT.Vendor
{
    public class ItemInventory : MonoBehaviour
    {
        private static ItemInventory Instance { get; set; }

        public ItemInventoryData data;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else 
                Destroy(this);
        }
    }
}