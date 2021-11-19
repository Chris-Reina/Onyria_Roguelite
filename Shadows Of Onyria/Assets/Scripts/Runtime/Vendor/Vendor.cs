using System;
using UnityEngine;

namespace DoaT.Vendor
{
    public class Vendor : MonoBehaviourInit
    {
        public VendorInventory inventoryData;

        private void Awake()
        {
            inventoryData.LoadTable();
        }

        private void Start()
        {
            inventoryData.Populate();
        }

        public override float OnInitialization()
        {
            for (int i = 0; i < 8; i++)
            {
                inventoryData.table.Random();
            }
            return 1f;
        }
    }

    [Serializable]
    public class VendorInventory
    {
        public Item[] table;
        public VendorInventoryData data;

        public void LoadTable()
        {
            table = Resources.LoadAll<Item>("");
        }

        public void Populate()
        {
            for (int y = 0; y < data.items.Height; y++)
            {
                for (int x = 0; x < data.items.Width; x++)
                {
                    var rand = table.Random();
                    data.items[x, y] = rand;
                    data.OnItemUpdateEvent(new Vector2Int(x, y), rand);
                }
            }
        }
    }

    public abstract class Item : ScriptableObject, IDynamicTooltip
    {
        public Sprite icon;
        public string header;
        public string description;

        public char beginDynamicDataMarker = '[';
        public char endDynamicDataMarker = ']';

        public int buyAmount;
        public int sellAmount;

        public abstract Tuple<string,int> GetContent();
    }

    public interface IDynamicTooltip
    {
        Tuple<string,int> GetContent();
    }

    public abstract class UsableItem : Item
    {
        public abstract bool UseItem();
    }

    public abstract class PassiveItem : Item
    {
        public abstract void AddPassiveEffect();
    }
}