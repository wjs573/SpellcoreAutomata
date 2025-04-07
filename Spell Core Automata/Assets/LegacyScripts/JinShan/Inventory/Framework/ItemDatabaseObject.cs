using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using System;

namespace JinShan
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObject : SerializedScriptableObject
    {
        public ItemObject[] ItemObjects;
        public ItemObject[] skillItemObjects;
        public ItemObject[] fabaoItemObjects;
        public ItemObject[] shentongItemObjects;
        public ItemObject[] enhanceEffectItemObjects;
        public ItemObject[] triggerItemObjects;
        public ItemObject[] enemyDataItemObjects;

        public List<ItemObject> GetItemObjectsByNameList(string[] nameList)
        {
            ItemObject[] Skills = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/技能");
            ItemObject[] FaBaos = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/法宝");
            ItemObject[] Shentongs = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/神通");
            ItemObject[] enhanceEffectItemObjects = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/技能强化");
            ItemObject[] triggerItemObjects = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/触发器");
            ItemObject[] enemyDataItemObjects = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/敌人");
            ItemObject[] items = Skills.Concat(FaBaos).Concat(Shentongs).Concat(enhanceEffectItemObjects).Concat(triggerItemObjects).Concat(enemyDataItemObjects).ToArray();
            List<ItemObject> itemObjects = new List<ItemObject>();
            for (int i = 0; i < nameList.Length; i++)
            {
                foreach (ItemObject item in items)
                {
                    if (item.data.Name == nameList[i])
                    {
                        itemObjects.Add(item);
                    }
                }
            }
            return itemObjects;
        }

        [ContextMenu("更新物品id")]
        public void UpdateId()
        {
            // 遍历所有创建的物品
            ItemObject[] Skills = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/技能");
            ItemObject[] FaBaos = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/法宝");
            ItemObject[] Shentongs = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/神通");
            ItemObject[] enhanceEffects = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/技能强化");
            ItemObject[] triggers = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/触发器");
            ItemObject[] enemyDatas = Resources.LoadAll<ItemObject>("Inventory/ScriptableObjects/敌人");

            skillItemObjects = Skills;
            fabaoItemObjects = FaBaos;
            shentongItemObjects = Shentongs;
            enhanceEffectItemObjects = enhanceEffects;
            triggerItemObjects = triggers;
            enemyDataItemObjects = enemyDatas;

            ItemObject[] items = Skills.Concat(FaBaos).Concat(Shentongs).Concat(enhanceEffectItemObjects).Concat(triggerItemObjects).Concat(enemyDataItemObjects).ToArray();
            foreach (ItemObject item in items)
            {
                item.data.Id = Guid.NewGuid().ToString(); // 生成新的唯一 Id
            }
        }

        public ItemObject GetItemObjectByName(string name)
        {
            foreach (ItemObject item in ItemObjects)
            {
                if (item.data.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }
}