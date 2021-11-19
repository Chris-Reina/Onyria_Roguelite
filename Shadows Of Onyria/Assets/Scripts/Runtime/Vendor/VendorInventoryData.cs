using System;
using UnityEngine;

namespace DoaT.Vendor
{
    [Serializable, CreateAssetMenu(fileName = "Vendor Inventory Data", menuName = "Data/Gameplay/Vendor/Inventory Data")]
    public class VendorInventoryData : ScriptableObject
    {
        public Matrix<Item> items = new Matrix<Item>(4, 2);
        public event Action<Vector2Int, Item> OnItemUpdate;

        public void OnItemUpdateEvent(Vector2Int index, Item item) => OnItemUpdate?.Invoke(index, item);

        public void DeleteAt(Vector2Int index)
        {
            items[index.x, index.y] = null;
            OnItemUpdateEvent(index, null);
        }

        public void DeleteAt(int x, int y)
        {
            items[x,y] = null;
            OnItemUpdateEvent(new Vector2Int(x, y), null);
        }
    }
}