using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCraftingRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipeClass : ScriptableObject
{
    [Header("Crafting Recipe")]
    public SlotClass[] inputItems;
    public SlotClass outputItem;

    public bool CanCraft(InventoryManager inventory)
    {
        //check if we actually have space in our inventory to craft
        if (inventory.isFull())
            return false;

        for (int i = 0; i < inputItems.Length; i++)
        {
            if (!inventory.Contains(inputItems[i].item, inputItems[i].quantity))
                return false;
        }

        //return if inventory has input items
        return true;
    }

    public void Craft(InventoryManager inventory)
    {
        //remove the input items from the inventory
        for (int i = 0; i < inputItems.Length; i++)
        {
            inventory.Remove(inputItems[i].item, inputItems[i].quantity);
        }

        //add the output item to the inventory
        inventory.Add(outputItem.item, outputItem.quantity);
    }
}
