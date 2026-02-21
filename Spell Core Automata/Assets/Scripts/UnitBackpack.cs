using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WJS;
/// <summary>
/// 角色背包系统，管理角色的装备和背包物品
/// </summary>
public class UnitBackpack : MonoBehaviour
{
    [ShowInInspector]
    private List<EquipmentObj> inventory = new List<EquipmentObj>();
    [ShowInInspector]
    private Dictionary<EquipmentType, EquipmentObj> equippedEquipment = new Dictionary<EquipmentType, EquipmentObj>();
    [ShowInInspector]
    private EquipmentObj equippedWeapon;
    // 背包变更事件
    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;
    public void Init()
    {
        inventory = new List<EquipmentObj>(20);
        equippedEquipment = new Dictionary<EquipmentType, EquipmentObj>();
    }
    /// <summary>
    /// 添加物品到背包
    /// </summary>
    public void AddItem(EquipmentObj item)
    {
        if (item == null) return;
        inventory.Add(item);
        OnInventoryChanged?.Invoke();
    }
    /// <summary>
    /// 从背包移除物品
    /// </summary>
    public bool RemoveItem(EquipmentObj item)
    {
        if (item == null) return false;
        bool result = inventory.Remove(item);
        if (result)
        {
            OnInventoryChanged?.Invoke();
        }
        return result;
    }
    /// <summary>
    /// 获取背包中的所有物品
    /// </summary>
    public List<EquipmentObj> GetInventoryItems()
    {
        return new List<EquipmentObj>(inventory);
    }
    /// <summary>
    /// 装备装备
    /// </summary>
    public bool EquipEquipment(EquipmentObj equip)
    {
        if (equip == null) return false;
        
        EquipmentType slotType = equip.model.type;
        
        // 如果该槽位已有装备，先卸载
        if (equippedEquipment.ContainsKey(slotType) && equippedEquipment[slotType] != null)
        {
            UnequipEquipment(slotType);
        }
        // 从背包移除并装备
        inventory.Remove(equip);
        equippedEquipment[slotType] = equip;
        // 如果是武器，特殊处理
        if (slotType == EquipmentType.weapon)
        {
            equippedWeapon = equip;
        }
        // 应用装备属性
        ApplyEquipmentBuffs(equip, true);
        
        // 刷新角色属性
        ChaState chaState = GetComponent<ChaState>();
        if (chaState != null)
        {
            chaState.AttrRecheck();
        }
        OnInventoryChanged?.Invoke();
        OnEquipmentChanged?.Invoke();
        return true;
    }
    /// <summary>
    /// 卸载指定槽位的装备
    /// </summary>
    public bool UnequipEquipment(EquipmentType slotType)
    {
        if (!equippedEquipment.ContainsKey(slotType) || equippedEquipment[slotType] == null)
            return false;
        EquipmentObj equip = equippedEquipment[slotType];
        
        // 移除装备效果
        ApplyEquipmentBuffs(equip, false);
        // 从装备槽移除并放回背包
        equippedEquipment.Remove(slotType);
        inventory.Add(equip);
        // 如果是武器，特殊处理
        if (slotType == EquipmentType.weapon)
        {
            equippedWeapon = null;
        }
        // 刷新角色属性
        ChaState chaState = GetComponent<ChaState>();
        if (chaState != null)
        {
            chaState.AttrRecheck();
        }
        OnInventoryChanged?.Invoke();
        OnEquipmentChanged?.Invoke();
        return true;
    }
    /// <summary>
    /// 卸载特定装备
    /// </summary>
    public bool UnequipEquipment(EquipmentObj equip)
    {
        if (equip == null) return false;
        
        foreach (var kvp in equippedEquipment)
        {
            if (kvp.Value == equip)
            {
                return UnequipEquipment(kvp.Key);
            }
        }
        return false;
    }
    /// <summary>
    /// 获取指定槽位的装备
    /// </summary>
    public EquipmentObj GetEquippedItem(EquipmentType slotType)
    {
        if (equippedEquipment.ContainsKey(slotType))
            return equippedEquipment[slotType];
        return null;
    }
    /// <summary>
    /// 获取所有已装备的物品
    /// </summary>
    public Dictionary<EquipmentType, EquipmentObj> GetAllEquippedItems()
    {
        return new Dictionary<EquipmentType, EquipmentObj>(equippedEquipment);
    }
    /// <summary>
    /// 检查指定槽位是否已装备
    /// </summary>
    public bool IsSlotEquipped(EquipmentType slotType)
    {
        return equippedEquipment.ContainsKey(slotType) && equippedEquipment[slotType] != null;
    }
    /// <summary>
    /// 获取装备提供的总属性
    /// </summary>
    public ChaProperty GetEquipmentChaProperty()
    {
        ChaProperty chaProperty = ChaProperty.zero;
        foreach (var kvp in equippedEquipment)
        {
            if (kvp.Value != null)
            {
                chaProperty += kvp.Value.model.equipmentProperty;
            }
        }
        return chaProperty;
    }
    /// <summary>
    /// 获取特定类型的已装备列表
    /// </summary>
    public List<EquipmentObj> GetEquipmentByType(EquipmentType type)
    {
        List<EquipmentObj> equipmentList = new List<EquipmentObj>();
        if (equippedEquipment.ContainsKey(type) && equippedEquipment[type] != null)
        {
            equipmentList.Add(equippedEquipment[type]);
        }
        return equipmentList;
    }
    /// <summary>
    /// 切换到上一个武器
    /// </summary>
    public void SwitchToPreviousWeapon()
    {
        // 获取背包中所有武器
        List<EquipmentObj> allWeapons = new List<EquipmentObj>();
        foreach (var item in inventory)
        {
            if (item.model.type == EquipmentType.weapon)
                allWeapons.Add(item);
        }
        
        // 加上当前装备的武器
        if (equippedWeapon != null)
            allWeapons.Add(equippedWeapon);
        if (allWeapons.Count <= 1) return;
        int currentIndex = allWeapons.IndexOf(equippedWeapon);
        if (currentIndex < 0) currentIndex = 0;
        
        int nextIndex = (currentIndex - 1 + allWeapons.Count) % allWeapons.Count;
        
        // 先卸载当前武器
        if (equippedWeapon != null)
        {
            UnequipEquipment(EquipmentType.weapon);
        }
        
        // 装备新武器
        EquipEquipment(allWeapons[nextIndex]);
    }
    /// <summary>
    /// 切换到下一个武器
    /// </summary>
    public void SwitchToNextWeapon()
    {
        // 获取背包中所有武器
        List<EquipmentObj> allWeapons = new List<EquipmentObj>();
        foreach (var item in inventory)
        {
            if (item.model.type == EquipmentType.weapon)
                allWeapons.Add(item);
        }
        
        // 加上当前装备的武器
        if (equippedWeapon != null)
            allWeapons.Add(equippedWeapon);
        if (allWeapons.Count <= 1) return;
        int currentIndex = allWeapons.IndexOf(equippedWeapon);
        if (currentIndex < 0) currentIndex = 0;
        
        int nextIndex = (currentIndex + 1) % allWeapons.Count;
        
        // 先卸载当前武器
        if (equippedWeapon != null)
        {
            UnequipEquipment(EquipmentType.weapon);
        }
        
        // 装备新武器
        EquipEquipment(allWeapons[nextIndex]);
    }
    /// <summary>
    /// 获取当前装备的武器
    /// </summary>
    public EquipmentObj GetEquippedWeapon()
    {
        return equippedWeapon;
    }
    /// <summary>
    /// 应用或移除装备的buff效果
    /// </summary>
    private void ApplyEquipmentBuffs(EquipmentObj equip, bool apply)
    {
        if (equip?.model.buffs == null || equip.model.buffs.Length == 0) return;
        ChaState chaState = GetComponent<ChaState>();
        if (chaState == null) return;
        foreach (var buffInfo in equip.model.buffs)
        {
            if (apply)
            {
                // 应用buff
                AddBuffInfo addBuff = new AddBuffInfo(
                    buffInfo.buffModel,
                    this.gameObject,
                    this.gameObject,
                    buffInfo.addStack,
                    buffInfo.duration,
                    true  // permanent while equipped
                );
                chaState.AddBuff(addBuff);
            }
            else
            {
                // 移除buff - 通过添加负层数
                AddBuffInfo removeBuff = new AddBuffInfo(
                    buffInfo.buffModel,
                    this.gameObject,
                    this.gameObject,
                    0,
                    -999,  // 移除所有层数
                    false
                );
                chaState.AddBuff(removeBuff);
            }
        }
    }
}