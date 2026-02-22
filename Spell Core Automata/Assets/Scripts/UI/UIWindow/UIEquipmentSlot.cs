using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WJS;
using TMPro;

/// <summary>
/// 装备槽位UI组件
/// </summary>
public class UIEquipmentSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("装备类型")]
    public EquipmentType slotType;
    
    [Header("UI组件")]
    public Image iconImage;
    public Image highlightImage;
    public Image backgroundImage;
    public TMP_Text slotNameText;
    
    [Header("设置")]
    public bool allowDrag = true;
    public Color validDropColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    public Color invalidDropColor = new Color(1f, 0.3f, 0.3f, 0.5f);
    
    // 当前槽位中的装备
    public EquipmentObj CurrentEquipment { get; private set; }
    
    // 父窗口引用
    public UIBackpackWindow BackpackWindow { get; set; }
    
    // 是否为空
    public bool IsEmpty => CurrentEquipment == null;

    private Vector2 dragStartPos;
    private bool isDragging = false;
    private const float DRAG_THRESHOLD = 10f;
    private Color originalHighlightColor;

    private void Awake()
    {
        if (iconImage == null)
            iconImage = transform.Find("Icon")?.GetComponent<Image>();
        if (highlightImage == null)
            highlightImage = transform.Find("Highlight")?.GetComponent<Image>();
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        if (slotNameText == null)
            slotNameText = transform.Find("SlotName")?.GetComponent<TMP_Text>();
            
        if (highlightImage != null)
            originalHighlightColor = highlightImage.color;
            
        UpdateSlotName();
        ClearSlot();
    }

    /// <summary>
    /// 更新槽位名称显示
    /// </summary>
    private void UpdateSlotName()
    {
        if (slotNameText != null)
        {
            slotNameText.text = GetSlotTypeName(slotType);
        }
    }

    /// <summary>
    /// 获取槽位类型名称
    /// </summary>
    private string GetSlotTypeName(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.weapon: return "武器";
            case EquipmentType.helm: return "头盔";
            case EquipmentType.armor: return "盔甲";
            case EquipmentType.trinket: return "饰品";
            default: return type.ToString();
        }
    }

    /// <summary>
    /// 设置槽位中的装备
    /// </summary>
    public void SetEquipment(EquipmentObj equipment)
    {
        CurrentEquipment = equipment;
        UpdateVisuals();
    }

    /// <summary>
    /// 清空槽位
    /// </summary>
    public void ClearSlot()
    {
        CurrentEquipment = null;
        UpdateVisuals();
    }

    /// <summary>
    /// 更新视觉显示
    /// </summary>
    private void UpdateVisuals()
    {
        if (CurrentEquipment == null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.gameObject.SetActive(false);
            }
        }
        else
        {
            if (iconImage != null)
            {
                Sprite icon = LoadIcon(CurrentEquipment.model.icon);
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(true);
            }
        }
        
        SetHighlight(false);
    }

    /// <summary>
    /// 加载图标资源
    /// </summary>
    private Sprite LoadIcon(string iconName)
    {
        if (string.IsNullOrEmpty(iconName)) return null;
        
        Sprite sprite = Resources.Load<Sprite>($"Icons/{iconName}");
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>("Icons/DefaultItem");
        }
        return sprite;
    }

    /// <summary>
    /// 设置高亮状态
    /// </summary>
    public void SetHighlight(bool highlight, bool validDrop = true)
    {
        if (highlightImage == null) return;
        
        if (highlight)
        {
            highlightImage.color = validDrop ? validDropColor : invalidDropColor;
            highlightImage.gameObject.SetActive(true);
        }
        else
        {
            highlightImage.color = originalHighlightColor;
            highlightImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 检查拖拽的物品是否可以放入此槽位
    /// </summary>
    public bool CanAcceptDrop(DragData dragData)
    {
        if (dragData?.equipment == null) return false;
        
        // 检查装备类型是否匹配
        return dragData.equipment.model.type == slotType;
    }

    #region 事件处理

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!allowDrag || IsEmpty) return;
        
        dragStartPos = eventData.position;
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!allowDrag || IsEmpty) return;
        
        if (!isDragging && Vector2.Distance(dragStartPos, eventData.position) > DRAG_THRESHOLD)
        {
            isDragging = true;
            
            DragData dragData = new DragData
            {
                sourceType = DragSourceType.Equipment,
                equipment = CurrentEquipment,
                equipmentSlot = slotType,
                icon = iconImage?.sprite
            };
            
            UIDragController.Instance.StartDrag(dragData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        bool success = false;
        
        // 使用射线检测查找放置目标
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            // 检查是否是背包格子
            UIInventorySlot invSlot = result.gameObject.GetComponent<UIInventorySlot>();
            if (invSlot != null)
            {
                // 卸载装备到背包
                if (BackpackWindow != null)
                {
                    BackpackWindow.TryUnequipItem(slotType);
                    success = true;
                }
                break;
            }
            
            // 检查是否是其他装备槽
            UIEquipmentSlot otherSlot = result.gameObject.GetComponent<UIEquipmentSlot>();
            if (otherSlot != null && otherSlot != this)
            {
                // 检查是否可以装备到目标槽位
                if (otherSlot.slotType == CurrentEquipment.model.type)
                {
                    // 先卸载当前装备到背包，再装备到目标槽
                    if (BackpackWindow != null)
                    {
                        BackpackWindow.TryUnequipItem(slotType);
                        BackpackWindow.TryEquipItem(CurrentEquipment, otherSlot.slotType);
                        success = true;
                    }
                }
                break;
            }
        }
        
        UIDragController.Instance.EndDrag(success);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Enter:{ gameObject.name}");
        // 检查是否有物品正在拖拽
        if (UIDragController.Instance.IsDragging)
        {
            DragData dragData = UIDragController.Instance.CurrentDragData;
            bool canAccept = CanAcceptDrop(dragData);
            SetHighlight(true, canAccept);
        }
        else if (!IsEmpty)
        {
            // 显示提示
            if (BackpackWindow != null)
            {
                BackpackWindow.ShowTooltip(CurrentEquipment, transform.position);
            }
        }
    }

    /// <summary>
    /// 当鼠标指针离开UI元素时触发的事件处理方法
    /// </summary>
    /// <param name="eventData">包含指针事件的数据信息</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        // 取消高亮显示
        SetHighlight(false);
        
        // 隐藏提示
        // 检查背包窗口是否存在，如果存在则调用其隐藏提示的方法
        if (BackpackWindow != null)
        {
            BackpackWindow.HideTooltip();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsEmpty) return;
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 右键点击卸载装备
            if (BackpackWindow != null)
            {
                BackpackWindow.TryUnequipItem(slotType);
            }
        }
    }

    #endregion
}
