using Inventory.Enums;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class SlotClass
{
    [field: SerializeField] public ItemClass item { get; private set; } = null;
    [field: SerializeField] public int quantity { get; private set; } = 0;
    public SlotType slotType { get; private set; } = SlotType.def;
    
    public SlotClass()
    {
        item = null;
        quantity = 0;
    }

    public SlotClass(SlotType slotType)
    {
        item = null;
        quantity = 0;
        this.slotType = slotType;
    }

    public SlotClass (ItemClass _item, int _quantity)
    {
        item = _item;
        quantity = _quantity;
    }

    public SlotClass (SlotClass slot)
    {
        this.item = slot.item;
        this.quantity = slot.quantity;
    }

    public void Clear()
    {
        this.item = null;
        this.quantity = 0;
    }

/*    public ItemClass GetItem() { return item; }
    public int GetQuantity() { return quantity; }*/
    public void AddQuantity(int _quantity) { quantity += _quantity; }
    public void SubQuantity(int _quantity)
    {
        quantity -= _quantity;
        if (quantity <= 0)
        {
            Clear();
        }    
    }
    public void AddItem(ItemClass item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
    public bool CanAdd(ItemClass item, int quantity = 1)
    {
        return (slotType is SlotType.def || item.slotType == slotType);
    }
}
