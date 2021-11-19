using System;
using UnityEngine;

namespace DoaT.Vendor
{
    [Serializable, CreateAssetMenu(menuName = "Item/Potion/Health", fileName = "HealthPotion")]
    public class HealthPotion : UsableItem
    {
        public int amount;
        
        public override bool UseItem() //TODO: Replace temporalImplementation
        {
            if (PersistentData.Health.ratio < 1)
            {
                EventManager.Raise(ItemEvents.HealthPotion, amount);
                return true;
            }
            
            return false;
        }

        public override Tuple<string, int> GetContent()
        {
            return StringUtility.GetDynamicString(description, beginDynamicDataMarker, endDynamicDataMarker, new []{$"{amount}"});
        }
    }
}