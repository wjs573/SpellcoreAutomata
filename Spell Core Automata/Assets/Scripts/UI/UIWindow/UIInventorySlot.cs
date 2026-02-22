using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WJS;

/// <summary>
/// 背包格子UI组件
/// </summary>
public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI组件")]
    public Image iconImage;
    public Image highlightImage;
    public Image backgroundImage;
    
    [Header("设置")]
    public bool allowDrag = true;
    
    // 当前格子中的装备
    public EquipmentObj CurrentEquipment { get; private set; }
    
    // 格子索引
    public int SlotIndex { get; set; }
    
    // 父窗口引用
    public UIBackpackWindow BackpackWindow { get; set; }
    
    // 是否为空
    public bool IsEmpty => CurrentEquipment == null;

    private Vector2 dragStartPos;
    private bool isDragging = false;
    private const float DRAG_THRESHOLD = 10f;

    private void Awake()
    {
        if (iconImage == null)
            iconImage = transform.Find("Icon")?.GetComponent<Image>();
        if (highlightImage == null)
            highlightImage = transform.Find("Highlight")?.GetComponent<Image>();
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
            
        ClearSlot();
    }

    /// <summary>
    /// 设置格子中的装备
    /// </summary>
    public void SetEquipment(EquipmentObj equipment)
    {
        CurrentEquipment = equipment;
        UpdateVisuals();
    }

    /// <summary>
    /// 清空格子
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
                // 从资源管理器加载图标
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
        
        // 尝试从Resources加载
        Sprite sprite = Resources.Load<Sprite>($"Icons/{iconName}");
        if (sprite == null)
        {
            // 使用默认图标
            sprite = Resources.Load<Sprite>("Icons/DefaultItem");
        }
        return sprite;
    }

    /// <summary>
    /// 设置高亮状态
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(highlight);
        }
    }

    /// <summary>
    /// 检查拖拽的物品是否可以放入此格子
    /// </summary>
    public bool CanAcceptDrop(DragData dragData)
    {
        // 背包格子可以接受任何装备
        // 但不能放回原位置
        if (dragData.sourceType == DragSourceType.Inventory && 
            dragData.inventoryIndex == SlotIndex)
        {
            return false;
        }
        return true;
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
        
        // 检查是否超过拖拽阈值
        if (!isDragging && Vector2.Distance(dragStartPos, eventData.position) > DRAG_THRESHOLD)
        {
            isDragging = true;
            
            // 开始拖拽
            DragData dragData = new DragData
            {
                sourceType = DragSourceType.Inventory,
                equipment = CurrentEquipment,
                inventoryIndex = SlotIndex,
                icon = iconImage?.sprite
            };
            
            UIDragController.Instance.StartDrag(dragData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // 检查是否放置到有效区域
        bool success = false;
        
        // 使用射线检测查找放置目标
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            // 检查是否是装备槽
            UIEquipmentSlot equipSlot = result.gameObject.GetComponent<UIEquipmentSlot>();
            if (equipSlot != null)
            {
                // 尝试装备
                if (BackpackWindow != null)
                {
                    BackpackWindow.TryEquipItem(CurrentEquipment, equipSlot.slotType);
                    success = true;
                }
                break;
            }
            
            // 检查是否是其他背包格子
            UIInventorySlot otherSlot = result.gameObject.GetComponent<UIInventorySlot>();
            if (otherSlot != null && otherSlot != this)
            {
                // 交换物品
                if (BackpackWindow != null)
                {
                    BackpackWindow.SwapInventoryItems(SlotIndex, otherSlot.SlotIndex);
                    success = true;
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
            if (CanAcceptDrop(dragData))
            {
                SetHighlight(true);
            }
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

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(false);
        
        // 隐藏提示
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
            // 右键点击装备
            if (BackpackWindow != null)
            {
                BackpackWindow.TryEquipItem(CurrentEquipment);
            }
        }
    }

    #endregion
}
