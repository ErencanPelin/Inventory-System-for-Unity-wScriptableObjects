using Inventory.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipeClass> craftingRecipes = new List<CraftingRecipeClass>();

    [SerializeField] private GameObject itemCursor;

    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject hotbarSlotHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [SerializeField] private SlotClass[] startingItems;

    private SlotClass[] items;
    private SlotClass[] armourItems;

    private GameObject[] slots;
    [SerializeField] private GameObject[] armourSlots;
    private GameObject[] hotbarSlots;

    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;
    bool isMovingItem;

    [SerializeField] private GameObject hotbarSelector;
    [SerializeField] private int selectedSlotIndex = 0;
    public ItemClass selectedItem;

    private void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new SlotClass[slots.Length];
        hotbarSlots = new GameObject[hotbarSlotHolder.transform.childCount];
        armourItems = new SlotClass[armourSlots.Length];

        //define slot type for armour slots
        armourItems[0] = new SlotClass(SlotType.helmet);
        armourItems[1] = new SlotClass(SlotType.chest);
        armourItems[2] = new SlotClass(SlotType.legs);
        armourItems[3] = new SlotClass(SlotType.foot);

        for (int i = 0; i < hotbarSlots.Length; i++)
            hotbarSlots[i] = hotbarSlotHolder.transform.GetChild(i).gameObject;
        for (int i = 0; i < items.Length; i++)
            items[i] = new SlotClass();
        for (int i = 0; i < startingItems.Length; i++)
            items[i] = startingItems[i];
        //set all the slots
        for (int i = 0; i < slotHolder.transform.childCount; i++)
            slots[i] = slotHolder.transform.GetChild(i).gameObject;


        RefreshUI();
        Add(itemToAdd, 1);
        Remove(itemToRemove);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) //handle crafting
            Craft(craftingRecipes[0]);

        itemCursor.SetActive(isMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        if (isMovingItem)
            itemCursor.GetComponent<Image>().sprite = movingSlot.item.itemIcon;

        if (Input.GetMouseButtonDown(0)) //we left clicked!
        {
            //find the closest slot (the slot we clicked on)
            if (isMovingItem)
            {
                //end item move
                EndItemMove();
            }
            else
                BeginItemMove();
        }
        else if (Input.GetMouseButtonDown(1)) //we right clicked!
        {
            //find the closest slot (the slot we clicked on)
            if (isMovingItem)
            {
                //end item move
                EndItemMove_Single();
            }
            else
                BeginItemMove_Half();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) //scrolling up
        {
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex + 1, 0, hotbarSlots.Length - 1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) //scrolling down
        {
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex - 1, 0, hotbarSlots.Length - 1);
        }

        hotbarSelector.transform.position = hotbarSlots[selectedSlotIndex].transform.position;
        selectedItem = items[selectedSlotIndex + (hotbarSlots.Length * 3)].item;
    }
    private void Craft(CraftingRecipeClass recipe)
    {
        if (recipe.CanCraft(this))
            recipe.Craft(this);
        else
            //show error msg
            Debug.Log("Can't craft that item!");
    }

    #region Inventory Utils
    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
            RefreshSlot(items[i], slots[i].transform);
        for (int i = 0; i < armourItems.Length; i++)
            RefreshSlot(armourItems[i], armourSlots[i].transform);
        RefreshHotbar();
    }
    private void RefreshSlot(SlotClass slot, Transform slotUI)
    {
        if (slot.item is null)
        {
            //item is null in the slot
            slotUI.transform.GetChild(0).GetComponent<Image>().sprite = null;
            slotUI.transform.GetChild(0).GetComponent<Image>().enabled = false;
            slotUI.transform.GetChild(1).GetComponent<Text>().text = "";
            return;
        }

        //item exists in slot
        slotUI.transform.GetChild(0).GetComponent<Image>().enabled = true;
        slotUI.transform.GetChild(0).GetComponent<Image>().sprite = slot.item.itemIcon;

        if (slot.item.isStackable)
            slotUI.transform.GetChild(1).GetComponent<Text>().text = slot.quantity + "";
        else
            slotUI.transform.GetChild(1).GetComponent<Text>().text = "";
    }
    public void RefreshHotbar() 
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
            RefreshSlot(items[i + (hotbarSlots.Length * 3)], hotbarSlots[i].transform);
    }
    public bool Add(ItemClass item, int quantity)
    {
        //check if inventory contains item
        SlotClass slot = Contains(item);
        if (slot != null && slot.item.isStackable)
            slot.AddQuantity(quantity);
        else
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].item == null) //this is an empty slot
                { 
                    items[i].AddItem(item, quantity);
                    break;
                }
            }
        }

        RefreshUI();
        return true;
    } 
    public bool Remove(ItemClass item, int quantity = 1)
    {
        // items.Remove(item);
        SlotClass temp = Contains(item);
        if (temp != null)
        {
            if (temp.quantity > 1)
                temp.SubQuantity(quantity);
            else
            {
                int slotToRemoveIndex = 0;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].item == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }

                items[slotToRemoveIndex].Clear();
            }
        }
        else
        {
            return false;
        }

        RefreshUI();
        return true;
    }
    public void UseSelected()
    {
        items[selectedSlotIndex + (hotbarSlots.Length * 3)].SubQuantity(1);
        RefreshUI();
    }
    public bool isFull()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].item == null)
                return false;
        }
        return true;
    }
    public SlotClass Contains(ItemClass item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].item == item)
                return items[i];
        }

        return null;
    }
    public bool Contains(ItemClass item, int quantity)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].item == item && items[i].quantity >= quantity)
                return true;
        }

        return false;
    }
    #endregion Inventoy Utils

    #region Moving Stuff
    private bool BeginItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.item == null)
            return false; //there is not item to move!

        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }
    private bool BeginItemMove_Half()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.item == null)
            return false; //there is not item to move!

        movingSlot = new SlotClass(originalSlot.item, Mathf.CeilToInt(originalSlot.quantity / 2f));
        originalSlot.SubQuantity(Mathf.CeilToInt(originalSlot.quantity / 2f));
        if (originalSlot.quantity == 0)
            originalSlot.Clear();

        isMovingItem = true;
        RefreshUI();
        return true;
    }
    private bool EndItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
        {
            Add(movingSlot.item, movingSlot.quantity);
            movingSlot.Clear();
        }
        else //clicked on a slot
        {
            if (originalSlot.CanAdd(movingSlot.item))
            {
                if (originalSlot.item != null)
                {
                    if (originalSlot.item == movingSlot.item) //they're the same item (they should stack)
                    {
                        if (originalSlot.item.isStackable)
                        {
                            originalSlot.AddQuantity(movingSlot.quantity);
                            movingSlot.Clear();
                        }
                        else
                            return false;
                    }
                    else
                    {
                        tempSlot = new SlotClass(originalSlot); //a = b
                        originalSlot.AddItem(movingSlot.item, movingSlot.quantity); //b = c
                        movingSlot.AddItem(tempSlot.item, tempSlot.quantity); //a = c
                        RefreshUI();
                        return true;
                    }
                }
                else //place item as usual
                {

                    originalSlot.AddItem(movingSlot.item, movingSlot.quantity);
                    movingSlot.Clear();
                }
            }
            else
                return false;
        }

        isMovingItem = false;
        RefreshUI();
        return true;
    }
    private bool EndItemMove_Single()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
            return false;
        if (originalSlot.item != null && 
            originalSlot.item != movingSlot.item &&
            originalSlot.CanAdd(movingSlot.item))
            return false;

        movingSlot.SubQuantity(1);
        if (originalSlot.item != null && originalSlot.item == movingSlot.item)
            originalSlot.AddQuantity(1);
        else
            originalSlot.AddItem(movingSlot.item, 1);

        if (movingSlot.quantity < 1)
        {
            isMovingItem = false;
            movingSlot.Clear();
        }
        else
            isMovingItem = true;

        RefreshUI();
        return true;
    }
    private SlotClass GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
                return items[i];
        }
        for (int i = 0; i < armourSlots.Length; i++)
        {
            if (Vector2.Distance(armourSlots[i].transform.position, Input.mousePosition) <= 32)
                return armourItems[i];
        }
        return null;
    }
    #endregion
}
