using System;
using UnityEngine;

namespace DoaT.Vendor
{
    [Serializable, CreateAssetMenu(menuName = "Item/Potion/Darkness", fileName = "DarknessPotion")]
    public class DarknessPotion : UsableItem
    {
        public int amount;
        
        public override bool UseItem() //TODO: Replace temporalImplementation
        {
            if (PersistentData.Darkness.ratio < 1)
            {
                EventManager.Raise(ItemEvents.DarknessPotion, amount);
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