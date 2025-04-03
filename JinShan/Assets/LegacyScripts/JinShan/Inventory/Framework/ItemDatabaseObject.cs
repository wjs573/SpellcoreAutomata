using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

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

        [ContextMenu("更新物品数据库")]
        public void UpdateItems()
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

            // 创建一个新的 ItemObjects 列表用于存储更新后的数据
            List<ItemObject> updatedItemObjects = new List<ItemObject>();

            // 遍历 items
            foreach (ItemObject newItem in items)
            {
                bool found = false;

                // 在现有 ItemObjects 中查找是否存在与 newItem 相同的物品
                foreach (ItemObject existingItem in ItemObjects)
                {
                    if (existingItem.data.Id == newItem.data.Id)
                    {
                        found = true;
                        break;
                    }
                }

                // 如果在现有 ItemObjects 中没有找到相同的物品
                if (!found)
                {
                    // 为新增的物品分配新的 Id 值
                    int newId = GenerateNewId();
                    newItem.data.Id = newId;
                    updatedItemObjects.Add(newItem);
                }
            }

            // 将现有 ItemObjects 列表与 updatedItemObjects 列表合并，以保留未更新的数据并添加新的物品
            updatedItemObjects.AddRange(ItemObjects);
            ItemObjects = updatedItemObjects.ToArray();
        }

        [ContextMenu("从零开始 更新物品id")]
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
            int id = 0;
            foreach (ItemObject item in items)
            {
                item.data.Id = id;
                id += 1;
            }
        }

        // 生成新的物品 Id（根据需求可能需要更复杂的逻辑）
        private int GenerateNewId()
        {
            int maxId = 0;
            if (ItemObjects == null || ItemObjects.Length == 0)
            {
                return maxId;
            }
            // 在这里添加生成新 Id 的逻辑，例如查找最大的现有 Id 并加一
            foreach (ItemObject item in ItemObjects)
            {
                if (item.data.Id > maxId)
                {
                    maxId = item.data.Id;
                }
            }
            return maxId + 1;
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