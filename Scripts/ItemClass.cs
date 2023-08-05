using System.Collections;
using UnityEngine;
using Inventory.Enums;

public abstract class ItemClass : ScriptableObject
{
    [Header("Item")] //data shared across every item
    public string itemName;
    public Sprite itemIcon;
    public bool isStackable = true;
    public int stackSize = 64;
    public SlotType slotType;

    public abstract void Use(PlayerController caller);
}