using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;

public class UnitItemSpawner : MonoBehaviour
{

    public ItemLootTable ItemLootTable;

    private void Awake()
    {
        ItemLootTable = new ItemLootTable();
    }
    public void DropItemPickup()
    {
        Item item = ItemLootTable.GetRandomItem();
        if (item != null)
        {
            GameObject ItemPickup = Instantiate(Resources.Load<GameObject>("Prefabs/ItemPickup/ItemPickup"), transform.position, Quaternion.identity);
            ItemPickup.GetComponent<ItemPickup>().Init(item);
        }
    }
}