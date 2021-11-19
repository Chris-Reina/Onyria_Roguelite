using System;
using DoaT;
using DoaT.Vendor;
using UnityEngine;

public class TESTItemUsage : MonoBehaviour
{
    private enum ItemSlot
    {
        First,
        Second,
        Third,
        Fourth,
    }
    
    [SerializeField] private KeyCode _key;
    [SerializeField] private ItemSlot _slot;
    [SerializeField] private ItemInventoryData _data;

    public Item CurrentItem
    {
        get
        {
            var item = _slot switch
            {
                ItemSlot.First => _data.FirstSlot,
                ItemSlot.Second => _data.SecondSlot,
                ItemSlot.Third => _data.ThirdSlot,
                ItemSlot.Fourth => _data.FourthSlot,
                _ => throw new ArgumentOutOfRangeException()
            };
                
            return item;
        }
    }
    
    public bool IsEmpty
    {
        get
        {
            return _slot switch
            {
                ItemSlot.First => !_data.FirstSlotOccupied,
                ItemSlot.Second => !_data.SecondSlotOccupied,
                ItemSlot.Third => !_data.ThirdSlotOccupied,
                ItemSlot.Fourth => !_data.FourthSlotOccupied,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    private void Update()
    {
        if (IsEmpty) return;
        if (ExecutionSystem.Paused) return;
        
        if (Input.GetKeyDown(_key) && CurrentItem is UsableItem uItem)
        {
            if(uItem.UseItem())
                _data.ClearItem((int) _slot);
        }
    }
}
