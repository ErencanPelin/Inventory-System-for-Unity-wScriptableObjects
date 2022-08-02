using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Tool/SpawnSword")]
public class SpawnSwordClass : ToolClass
{
    public GameObject spawnObject;
    public override void Use(PlayerController caller)
    {
        base.Use(caller);
        Instantiate(spawnObject, caller.transform.position, Quaternion.identity);
    }
}
