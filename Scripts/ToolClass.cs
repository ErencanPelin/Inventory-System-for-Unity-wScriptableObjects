using System.Collections;
using UnityEngine;
using Inventory.Enums;

[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Tool/Tool")]
public class ToolClass : ItemClass
{
    [Header("Tool")] //data specific to tool class
    public ToolType toolType;

    public override void Use(PlayerController caller)
    {
        Debug.Log("Swing Tool: ");
    }
}
