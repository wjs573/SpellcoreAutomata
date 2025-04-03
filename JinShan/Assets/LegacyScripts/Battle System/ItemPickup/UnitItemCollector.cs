using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;

public class UnitItemCollector : MonoBehaviour
{
    public InventoryObject targetInventory;

    public void AddItem(ItemPickup itemPickup)
    {
        if (targetInventory)
        {
            targetInventory.AddItem(itemPickup.Item, 1);
        }
    }
}