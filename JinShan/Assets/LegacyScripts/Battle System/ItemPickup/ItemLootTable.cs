using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;

public class ItemLootTable
{
    public class DropInfo
    {
        public string itemId;
        public float probability;
    }

    private List<DropInfo> dropTable;

    public ItemLootTable()
    {
        dropTable = new List<DropInfo>();
    }

    public void AddDrop(string itemId, float probability)
    {
        dropTable.Add(new DropInfo { itemId = itemId, probability = probability });
    }

    public Item GetRandomItem()
    {
        float totalProbability = 0f;
        foreach (DropInfo drop in dropTable)
        {
            totalProbability += drop.probability;
        }

        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue > totalProbability)
        {
            return null;
        }

        randomValue = UnityEngine.Random.Range(0f, totalProbability);

        foreach (DropInfo drop in dropTable)
        {
            if (randomValue < drop.probability)
            {
                return new Item(MainCharacter.Instance.ItemDatabase.GetItemObjectByName(drop.itemId));
            }
            randomValue -= drop.probability;
        }
        return null;
    }
}