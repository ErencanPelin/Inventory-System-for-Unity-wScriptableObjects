using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InventoryManager inventory;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //use the item
            if (inventory.selectedItem != null)
                inventory.selectedItem.Use(this);
        }
    }
}
