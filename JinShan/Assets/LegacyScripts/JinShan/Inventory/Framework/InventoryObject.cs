using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace JinShan
{
    public enum InterfaceType
    {
        Inventory,
        Equipment,
        Skill
    }

    public delegate void EquipHandler(int equippedIndex);


    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : SerializedScriptableObject
    {
        public string savePath;
        public ItemDatabaseObject database;
        public InterfaceType type;
        public Inventory Container;

        public InventoryObject()
        {
            Container = new Inventory();
        }

        public InventorySlot[] GetSlots
        { get { return Container.Slots; } }

        /// <summary>
        /// 获取仓库里物品的角色属性加和值
        /// </summary>
        /// <returns></returns>
        public ChaProperty GetTotalProperty()
        {
            ChaProperty total_property = ChaProperty.zero;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].ItemObject != null)
                {
                    total_property += GetSlots[i].item.property;
                }
            }
            return total_property;
        }

        // 当前装备的索引
        private int currentEquippedIndex = -1;

        public int CurrentEquippedIndex
        {
            get { return currentEquippedIndex; }
            set { currentEquippedIndex = Mathf.Clamp(value, 0, Container.Slots.Length - 1); }
        }

        // 定义委托
        public EquipHandler OnEquip;

        // 切换到下一个装备槽
        public void EquipNext()
        {
            CurrentEquippedIndex = (CurrentEquippedIndex + 1) % Container.Slots.Length;
            EquipCurrent();
        }

        // 切换到上一个装备槽
        public void EquipPrevious()
        {
            CurrentEquippedIndex = (CurrentEquippedIndex - 1 + Container.Slots.Length) % Container.Slots.Length;
            EquipCurrent();
        }

        // 装备当前索引的槽位中的物品
        private void EquipCurrent()
        {
            if (OnEquip != null)
            {
                OnEquip(CurrentEquippedIndex);
            }
        }

        /// <summary>
        /// 给inventorySlot添加物品
        /// </summary>
        /// <param name="_item">待添加的物品</param>
        /// <param name="_amount">添加数量</param>
        /// <param name="_targetSlot">待添加物品的inventorySlot</param>
        /// <returns></returns>
        public bool AddItem(Item _item, int _amount, InventorySlot _targetSlot = null)
        {
            //目标仓库没有空位
            if (EmptySlotCount <= 0)
            {
                return false;
            }
            //如果要添加的是空物品 直接添加成功
            if (_item == null || _item.itemObject == null || _item.Id <= -1)
            {
                return true;
            }

            //尝试在仓库中找到装有这种物品的inventory slot
            InventorySlot slot = FindItemOnInventory(_item);

            //如果设置了待添加的inventorySlot且为空
            if (_targetSlot != null && (_targetSlot.item == null || _targetSlot.item.Id <= -1))
            {
                _targetSlot.UpdateSlot(_item, _amount);
                return true;
            }

            //如果该物品不可堆叠 或者 找不到已经存在该物品的slot
            if (!_item.itemObject.stackable || slot == null)
            {
                SetEmptySlot(_item, _amount);
                return true;
            }

            //如果该物品可堆叠且已经存在一个格子放置该物品
            slot.AddAmount(_amount);
            return true;
        }

        public InventorySlot FindItemOnInventory(Item _item)
        {
            if (_item == null)
            {
                return null;
            }
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item != null && GetSlots[i].item.Id == _item.Id)
                {
                    return GetSlots[i];
                }
            }
            return null;
        }

        public bool HasItemObject(ItemObject itemObject)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item != null && GetSlots[i].item.Id == itemObject.data.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasInventorySlot(InventorySlot inventorySlot)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i] == inventorySlot)
                {
                    return true;
                }
            }
            return false;
        }

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < GetSlots.Length; i++)
                {
                    if (GetSlots[i].item == null || GetSlots[i].item.Id <= -1)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public InventorySlot SetEmptySlot(Item _item, int _amount)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item == null || GetSlots[i].item.Id <= -1)
                {
                    GetSlots[i].UpdateSlot(_item, _amount);
                    return GetSlots[i];
                }
            }
            //set up functionality for full inventory
            return null;
        }

        public void SwapItem(InventorySlot item1, InventorySlot item2)
        {
            if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
            {
                InventorySlot temp = new InventorySlot(item2.item, item2.amount);
                item2.UpdateSlot(item1.item, item1.amount);
                item1.UpdateSlot(temp.item, temp.amount);
            }
        }

        public void RemoveItem(Item _item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item == _item)
                {
                    GetSlots[i].UpdateSlot(null, 0);
                }
            }
        }

        public int GetValueOfAllItems()
        {
            int Value = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].ItemObject != null)
                {
                    Value += GetSlots[i].item.Value * GetSlots[i].amount;
                }
            }
            return Value;
        }

        [ContextMenu("Save")]
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, Container);
            stream.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
                Inventory newContainer = (Inventory)formatter.Deserialize(stream);
                for (int i = 0; i < GetSlots.Length; i++)
                {
                    Container.Slots[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].amount);
                }
                stream.Close();
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            Container.Clear();
        }
    }

    //仓库 包含了许多个插槽
    [System.Serializable]
    public class Inventory
    {
        public InventorySlot[] Slots = new InventorySlot[20];

        public void Clear()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i].RemoveItem();
            }
        }
    }

    public delegate void SlotUpdated(InventorySlot _slot);

    //插槽 一个插槽包括 插槽允许放入的物品类型 该插槽隶属的用户界面 插槽放入的物品 物品的数量
    [System.Serializable]
    public class InventorySlot : IComparer<InventorySlot>
    {
        public ItemType[] AllowedItems = new ItemType[0];//决定每个slot可以放置的物品类型

        [System.NonSerialized]
        public UserInterface parent;

        //[System.NonSerialized]
        public InventoryObject inventory;

        [System.NonSerialized]
        public GameObject slotDisplay;

        [System.NonSerialized]
        public SlotUpdated OnAfterUpdate;

        [System.NonSerialized]
        public SlotUpdated OnBeforeUpdate;

        public bool isLocked = false;

        public Item item = new Item();
        public int amount;

        public ItemObject ItemObject
        {
            get
            {
                if (item != null && item.Id >= 0)
                {
                    return item.itemObject;
                }
                return null;
            }
        }

        public InventorySlot()
        {
            UpdateSlot(new Item(), 0);
        }

        public InventorySlot(Item _item, int _amount)
        {
            UpdateSlot(_item, _amount);
        }

        public void UpdateSlot(Item _item, int _amount)
        {
            if (OnBeforeUpdate != null)
            {
                OnBeforeUpdate.Invoke(this);
            }
            item = _item;
            amount = _amount;
            if (OnAfterUpdate != null)
            {
                OnAfterUpdate.Invoke(this);
            }
        }

        public void UpdateSlot()
        {
            if (OnBeforeUpdate != null)
            {
                OnBeforeUpdate.Invoke(this);
            }
            if (OnAfterUpdate != null)
            {
                OnAfterUpdate.Invoke(this);
            }
        }

        public void AddAmount(int value)
        {
            UpdateSlot(item, amount += value);
        }

        public void RemoveItem()
        {
            UpdateSlot(new Item(), 0);
        }


        public bool SwapInventorySlot(InventorySlot slot)
        {
            bool isSlot1Empty = (this.item == null || this.item.Id <= -1);
            bool isSlot2Empty = (slot.item == null || slot.item.Id <= -1);
            //slot1和slot2都为空
            if (isSlot1Empty && isSlot2Empty)
            {
                return true;
            }
            if (this.isLocked || slot.isLocked)
            {
                return false;
            }

            //二者为相同Inventory Object
            //二者为不同Inventory Object
            //有一个为空
            if (!isSlot1Empty && isSlot2Empty)
            {
                if (!slot.CanPlaceInSlot(this.ItemObject)) return false;
                if (slot.inventory.AddItem(this.item, this.amount, slot))
                {
                    this.RemoveItem();
                    return true;
                }
            }
            if (isSlot1Empty && !isSlot2Empty)
            {
                if (!this.CanPlaceInSlot(slot.ItemObject)) return false;
                if (this.inventory.AddItem(slot.item, slot.amount, this))
                {
                    slot.RemoveItem();
                    return true;
                }
            }

            //二者都不为空
            if (!isSlot1Empty && !isSlot2Empty)
            {
                if (slot.CanPlaceInSlot(this.ItemObject) && this.CanPlaceInSlot(slot.ItemObject))
                {
                    Item tempItem = this.item.Clone();
                    int tempAmount = this.amount;
                    this.UpdateSlot(slot.item, slot.amount);
                    slot.UpdateSlot(tempItem, tempAmount);
                }
            }
            return false;
        }

        public bool CanPlaceInSlot(ItemObject _itemObject)
        {
            //如果没有slot 类型限制 返回true
            if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
            {
                return true;
            }

            //如果有slot类型限制 但类型满足限制条件 返回true
            for (int i = 0; i < AllowedItems.Length; i++)
            {
                if (_itemObject.type == AllowedItems[i])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// inventoryslot的排序：优先级从高到低，有物品大于无物品，物品品级高大于低品级，高价值大于低价值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(InventorySlot x, InventorySlot y)
        {
            if (x.ItemObject != null && y.ItemObject == null)
                return 1;
            else if (x.ItemObject == null && y.ItemObject != null)
                return -1;
            else if (x.ItemObject == null && y.ItemObject == null)
                return 0;
            else
            {
                if (x.ItemObject.data.Rank > y.ItemObject.data.Rank)
                {
                    return 1;
                }
                else if (x.ItemObject.data.Rank < y.ItemObject.data.Rank)
                {
                    return -1;
                }
                else
                {
                    if (x.ItemObject.data.Value > y.ItemObject.data.Value)
                    {
                        return 1;
                    }
                    else if (x.ItemObject.data.Value < y.ItemObject.data.Value)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }
}