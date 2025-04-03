using System.Collections;
using System.Collections.Generic;
using JinShan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public GameObject focus; // 选中框
    public GameObject alertDot; // 提示点

    public Image iconImage; // 物品图标
    public Sprite defaultIcon; // 默认图标
    public TextMeshProUGUI stackCount; // 堆叠数量

    public InventorySlot inventorySlot; // 当前格子

    private SlotState currentSlotState = default;

    private bool isHidingIcon = false;

    // 更新格子状态的方法
    public void SetHideIcon(bool hide)
    {
        isHidingIcon = hide;
    }

    // 枚举定义三种状态
    public enum SlotState
    {
        Default, // 默认
        Focus, // 选中
        Alert // 提示
    }

    void Update()
    {
        UpdateSlotState(currentSlotState);
        UpdateSlotIcon();
    }

    // 更新格子图标的方法
    private void UpdateSlotIcon()
    {
        if (inventorySlot.item != null && inventorySlot.item.itemObject != null)
        {
            iconImage.sprite = inventorySlot.item.itemObject.uiDisplay;
            stackCount.text = inventorySlot.amount.ToString();
            stackCount.enabled = inventorySlot.amount > 1; // 如果堆叠数量大于1，则显示数量
        }
        else
        {
            iconImage.sprite = defaultIcon;
            stackCount.text = "";
            stackCount.enabled = false;
        }

        if (isHidingIcon)
        {
            iconImage.sprite = defaultIcon;
            stackCount.text = "";
            stackCount.enabled = false;
        }
    }

    public void SetSlotState(SlotState state)
    {
        currentSlotState = state;
    }

    // 更新格子状态的方法
    private void UpdateSlotState(SlotState state)
    {
        if (state == currentSlotState)
        {
            return;
        }

        switch (state)
        {
            case SlotState.Default:
                // 默认状态：显示icon和stackCount，隐藏focus和alertDot
                focus.SetActive(false);
                alertDot.SetActive(false);
                iconImage.enabled = inventorySlot.amount >= 0; // 如果有物品则显示图标
                stackCount.enabled = inventorySlot.amount > 1; // 堆叠数量大于1时显示
                break;

            case SlotState.Focus:
                // 选中状态：显示focus，显示icon，隐藏stackCount和alertDot
                focus.SetActive(true);
                alertDot.SetActive(false);
                iconImage.enabled = inventorySlot.amount >= 0; // 如果有物品则显示图标
                stackCount.enabled = false; // 隐藏堆叠数量
                break;

            case SlotState.Alert:
                // 提示状态：隐藏focus，显示icon和stackCount，显示alertDot
                focus.SetActive(false);
                alertDot.SetActive(true);
                iconImage.enabled = inventorySlot.amount >= 0; // 如果有物品则显示图标
                stackCount.enabled = inventorySlot.amount > 1; // 堆叠数量大于1时显示
                break;

            default:
                Debug.LogWarning("未知的背包格子状态！");
                break;
        }
        currentSlotState = state;
    }

    public void SetInventorySlot(InventorySlot newInventorySlot)
    {
        this.inventorySlot = newInventorySlot;

        // 更新图标和堆叠数量显示
        if (inventorySlot != null && inventorySlot.item != null)
        {
            iconImage.sprite = inventorySlot.item.itemObject.uiDisplay;
            stackCount.text = inventorySlot.amount > 1 ? inventorySlot.amount.ToString() : "";
        }
        else
        {
            iconImage.sprite = null;
            stackCount.text = "";
        }
    }
}
