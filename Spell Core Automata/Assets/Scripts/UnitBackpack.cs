using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WJS;

public class UnitBackpack : MonoBehaviour
{
    [ShowInInspector]
    private List<EquipmentObj> inventory = new List<EquipmentObj>();
    [ShowInInspector]
    private List<EquipmentObj> equipedEquipment = new List<EquipmentObj>();
    [ShowInInspector]
    private EquipmentObj equipedWeapon;

    public void Init()
    {
        inventory = new List<EquipmentObj>(20);
        equipedEquipment = new List<EquipmentObj>(6);
    }

    public void AddItem(EquipmentObj item)
    {
        inventory.Add(item);
    }

    public void EquipEquipment(EquipmentObj equip)
    {
        inventory.Remove(equip);
        equipedEquipment.Add(equip);
    }

    public ChaProperty GetEquipmentChaProperty()
    {
        ChaProperty chaProperty = ChaProperty.zero;
        foreach (EquipmentObj equipmentObj in equipedEquipment)
        {
            chaProperty += equipmentObj.model.equipmentProperty;
        }
        if (equipedWeapon != null) chaProperty += equipedWeapon.model.equipmentProperty;
        return chaProperty;
    }

    public List<EquipmentObj> GetEquipmentByType(EquipmentType type)
    {
        List<EquipmentObj> equipmentList = new List<EquipmentObj>();
        foreach (EquipmentObj equipmentObj in equipedEquipment)
        {
            if (equipmentObj.model.type == type)
            {
                equipmentList.Add(equipmentObj);
            }
        }
        return equipmentList;
    }

    public void SwitchToPreviousWeapon()
    {
        List<EquipmentObj> weapons = GetEquipmentByType(EquipmentType.weapon);
        if (weapons.Count > 0)
        {
            equipedWeapon ??= weapons[0];
            int currentIndex = weapons.IndexOf(equipedWeapon);
            int nextIndex = (currentIndex + 1) % weapons.Count; // 循环逻辑
            equipedWeapon = weapons[nextIndex];
        }
    }

    public void SwitchToNextWeapon()
    {
        List<EquipmentObj> weapons = GetEquipmentByType(EquipmentType.weapon);
        if (weapons.Count > 0)
        {
            equipedWeapon ??= weapons[0];
            int currentIndex = weapons.IndexOf(equipedWeapon);
            int nextIndex = (currentIndex - 1) % weapons.Count; // 循环逻辑
            equipedWeapon = weapons[nextIndex];
        }
    }
}
