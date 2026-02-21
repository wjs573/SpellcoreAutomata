using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WJS;

/// <summary>
/// 拖拽数据类型
/// </summary>
public enum DragSourceType
{
    Inventory,      // 来自背包
    Equipment       // 来自装备槽
}

/// <summary>
/// 拖拽数据
/// </summary>
public class DragData
{
    public DragSourceType sourceType;
    public EquipmentObj equipment;
    public int inventoryIndex;          // 背包中的索引
    public EquipmentType equipmentSlot; // 装备槽位类型
    public Sprite icon;
}

/// <summary>
/// UI拖拽控制器，管理所有UI拖拽操作
/// </summary>
public class UIDragController : MonoSingleton<UIDragController>
{
    [Header("拖拽图标")]
    public Image dragIconImage;
    public Canvas dragIconCanvas;
    
    [Header("设置")]
    public float dragThreshold = 10f;   // 开始拖拽的阈值
    
    // 当前拖拽数据
    public DragData CurrentDragData { get; private set; }
    
    // 是否正在拖拽
    public bool IsDragging => CurrentDragData != null;
    
    // 拖拽开始和结束事件
    public event Action<DragData> OnDragStarted;
    public event Action<DragData, bool> OnDragEnded; // bool表示是否成功放置

    protected  void Awake()
    {
        // 创建拖拽图标
        if (dragIconImage == null)
        {
            CreateDragIcon();
        }
    }

    private void CreateDragIcon()
    {
        // 创建一个独立的Canvas用于显示拖拽图标
        GameObject dragCanvasObj = new GameObject("DragIconCanvas");
        dragIconCanvas = dragCanvasObj.AddComponent<Canvas>();
        dragIconCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        dragIconCanvas.sortingOrder = 9999; // 确保在最上层
        
        // 添加CanvasScaler
        CanvasScaler scaler = dragCanvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // 创建拖拽图标Image
        GameObject iconObj = new GameObject("DragIcon");
        iconObj.transform.SetParent(dragCanvasObj.transform);
        dragIconImage = iconObj.AddComponent<Image>();
        
        RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(80, 80);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        dragIconImage.raycastTarget = false;
        dragIconImage.gameObject.SetActive(false);
        
        DontDestroyOnLoad(dragCanvasObj);
    }

    private void Update()
    {
        // 更新拖拽图标位置
        if (IsDragging && dragIconImage != null)
        {
            dragIconImage.transform.position = Input.mousePosition;
        }
        
        // 右键取消拖拽
        if (IsDragging && Input.GetMouseButtonDown(1))
        {
            CancelDrag();
        }
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    public void StartDrag(DragData dragData)
    {
        if (dragData == null || dragData.equipment == null) return;
        
        CurrentDragData = dragData;
        
        // 显示拖拽图标
        if (dragIconImage != null)
        {
            dragIconImage.sprite = dragData.icon;
            dragIconImage.gameObject.SetActive(true);
            dragIconImage.transform.position = Input.mousePosition;
        }
        
        OnDragStarted?.Invoke(dragData);
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    public void EndDrag(bool success)
    {
        if (!IsDragging) return;
        
        OnDragEnded?.Invoke(CurrentDragData, success);
        
        // 隐藏拖拽图标
        if (dragIconImage != null)
        {
            dragIconImage.gameObject.SetActive(false);
        }
        
        CurrentDragData = null;
    }

    /// <summary>
    /// 取消拖拽
    /// </summary>
    public void CancelDrag()
    {
        EndDrag(false);
    }

    /// <summary>
    /// 检查当前拖拽的物品是否可以放入指定槽位
    /// </summary>
    public bool CanEquipToSlot(EquipmentType slotType)
    {
        if (!IsDragging || CurrentDragData.equipment == null) return false;
        
        return CurrentDragData.equipment.model.type == slotType;
    }
}
