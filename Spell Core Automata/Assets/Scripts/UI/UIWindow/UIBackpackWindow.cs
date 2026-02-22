using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WJS;

/// <summary>
/// 背包主窗口，管理背包UI的显示和交互
/// </summary>
public class UIBackpackWindow : UIWindow
{
    [Header("背包格子容器")]
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;
    public int inventorySlotCount = 20;
    
    [Header("装备槽位")]
    public UIEquipmentSlot weaponSlot;
    public UIEquipmentSlot helmSlot;
    public UIEquipmentSlot armorSlot;
    public UIEquipmentSlot trinketSlot;
    
    [Header("角色属性显示")]
    public Text propertyText;
    
    [Header("物品提示")]
    public UIItemTooltip tooltip;
    public GameObject tooltipPrefab;
    
    // 背包格子列表
    private List<UIInventorySlot> inventorySlots = new List<UIInventorySlot>();
    
    // 装备槽字典
    private Dictionary<EquipmentType, UIEquipmentSlot> equipmentSlots;
    
    // 目标角色的背包组件
    [ShowInInspector]
    private UnitBackpack targetBackpack;
    private ChaState targetChaState;

    private void Start()
    {
        InitializeSlots();
        
        // 查找主角的背包
        FindTargetBackpack();
        
        // 创建提示组件（如果没有）
        if (tooltip == null && tooltipPrefab != null)
        {
            GameObject tooltipObj = Instantiate(tooltipPrefab, transform);
            tooltip = tooltipObj.GetComponent<UIItemTooltip>();
        }
        
        // 初始隐藏
        SetVisible(false);
    }

    private void OnEnable()
    {
        if (targetBackpack != null)
        {
            targetBackpack.OnInventoryChanged += OnInventoryChanged;
            targetBackpack.OnEquipmentChanged += OnEquipmentChanged;
        }
    }

    private void OnDisable()
    {
        if (targetBackpack != null)
        {
            targetBackpack.OnInventoryChanged -= OnInventoryChanged;
            targetBackpack.OnEquipmentChanged -= OnEquipmentChanged;
        }
    }

    /// <summary>
    /// 初始化槽位
    /// </summary>
    private void InitializeSlots()
    {
        // 初始化装备槽字典
        equipmentSlots = new Dictionary<EquipmentType, UIEquipmentSlot>();
        
        if (weaponSlot != null)
        {
            weaponSlot.slotType = EquipmentType.weapon;
            weaponSlot.BackpackWindow = this;
            equipmentSlots[EquipmentType.weapon] = weaponSlot;
        }
        
        if (helmSlot != null)
        {
            helmSlot.slotType = EquipmentType.helm;
            helmSlot.BackpackWindow = this;
            equipmentSlots[EquipmentType.helm] = helmSlot;
        }
        
        if (armorSlot != null)
        {
            armorSlot.slotType = EquipmentType.armor;
            armorSlot.BackpackWindow = this;
            equipmentSlots[EquipmentType.armor] = armorSlot;
        }
        
        if (trinketSlot != null)
        {
            trinketSlot.slotType = EquipmentType.trinket;
            trinketSlot.BackpackWindow = this;
            equipmentSlots[EquipmentType.trinket] = trinketSlot;
        }
        
        // 初始化背包格子
        if (inventoryGrid != null && inventorySlotPrefab != null)
        {
            for (int i = 0; i < inventorySlotCount; i++)
            {
                GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryGrid);
                UIInventorySlot slot = slotObj.GetComponent<UIInventorySlot>();
                if (slot != null)
                {
                    slot.SlotIndex = i;
                    slot.BackpackWindow = this;
                    inventorySlots.Add(slot);
                }
            }
        }
    }

    /// <summary>
    /// 查找目标背包
    /// </summary>
    private void FindTargetBackpack()
    {
        if (GameManager.Instance?.mainCharacter != null)
        {
            targetBackpack = GameManager.Instance.mainCharacter.GetComponent<UnitBackpack>();
            targetChaState = GameManager.Instance.mainCharacter.GetComponent<ChaState>();
            
            if (targetBackpack != null)
            {
                RefreshUI();
            }
        }
    }

    /// <summary>
    /// 刷新整个UI
    /// </summary>
    public void RefreshUI()
    {
        if (targetBackpack == null)
        {
            FindTargetBackpack();
            return;
        }
        
        RefreshInventory();
        RefreshEquipment();
        RefreshPropertyDisplay();
    }

    /// <summary>
    /// 刷新背包显示
    /// </summary>
    private void RefreshInventory()
    {
        List<EquipmentObj> items = targetBackpack.GetInventoryItems();
        
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < items.Count)
            {
                inventorySlots[i].SetEquipment(items[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// 刷新装备显示
    /// </summary>
    private void RefreshEquipment()
    {
        foreach (var kvp in equipmentSlots)
        {
            EquipmentObj equip = targetBackpack.GetEquippedItem(kvp.Key);
            if (equip != null)
            {
                kvp.Value.SetEquipment(equip);
            }
            else
            {
                kvp.Value.ClearSlot();
            }
        }
    }

    /// <summary>
    /// 刷新属性显示
    /// </summary>
    private void RefreshPropertyDisplay()
    {
        if (propertyText == null || targetChaState == null) return;
        
        ChaProperty prop = targetChaState.property;
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"生命: {targetChaState.resource.hp}/{prop.hp}");
        sb.AppendLine($"灵力: {targetChaState.resource.mp}/{prop.mp}");
        sb.AppendLine($"攻击: {prop.attack}");
        sb.AppendLine($"暴击倍率: {prop.critic_multiplier}x");
        sb.AppendLine($"暴击率: {prop.critic_rate}%");
        sb.AppendLine($"冷却: {prop.cd_speed}");
        sb.AppendLine($"移速: {prop.moveSpeed:F1}");
        sb.AppendLine($"攻速: {prop.actionSpeed:F2}");
        
        propertyText.text = sb.ToString();
    }

    /// <summary>
    /// 尝试装备物品
    /// </summary>
    public bool TryEquipItem(EquipmentObj item, EquipmentType? specificSlot = null)
    {
        if (targetBackpack == null || item == null) return false;
        
        EquipmentType targetSlot = specificSlot ?? item.model.type;
        
        // 检查装备类型是否匹配
        if (item.model.type != targetSlot)
        {
            Debug.LogWarning($"装备类型不匹配: {item.model.type} != {targetSlot}");
            return false;
        }
        
        bool success = targetBackpack.EquipEquipment(item);
        if (success)
        {
            RefreshUI();
        }
        
        return success;
    }

    /// <summary>
    /// 尝试卸载装备
    /// </summary>
    public bool TryUnequipItem(EquipmentType slotType)
    {
        if (targetBackpack == null) return false;
        
        bool success = targetBackpack.UnequipEquipment(slotType);
        if (success)
        {
            RefreshUI();
        }
        
        return success;
    }

    /// <summary>
    /// 交换背包物品位置
    /// </summary>
    public void SwapInventoryItems(int index1, int index2)
    {
        // 这里可以实现背包排序逻辑
        // 当前实现只是刷新UI
        RefreshInventory();
    }

    /// <summary>
    /// 显示物品提示
    /// </summary>
    public void ShowTooltip(EquipmentObj equipment, Vector3 position)
    {
        if (tooltip != null)
        {
            tooltip.Show(equipment, position);
        }
    }

    /// <summary>
    /// 隐藏物品提示
    /// </summary>
    public void HideTooltip()
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
    }

    /// <summary>
    /// 背包变更回调
    /// </summary>
    private void OnInventoryChanged()
    {
        RefreshInventory();
        RefreshPropertyDisplay();
    }

    /// <summary>
    /// 装备变更回调
    /// </summary>
    private void OnEquipmentChanged()
    {
        RefreshEquipment();
        RefreshPropertyDisplay();
    }

    /// <summary>
    /// 打开背包窗口
    /// </summary>
    public void Open()
    {
        // 如果找不到目标背包，重新查找
        if (targetBackpack == null)
        {
            FindTargetBackpack();
        }
        
        RefreshUI();
        SetVisible(true);
        UIWindowStack.Instance.PushWindow("UIBackpackWindow");
    }

    /// <summary>
    /// 关闭背包窗口
    /// </summary>
    public void Close()
    {
        SetVisible(false);
        HideTooltip();
    }

    public override void SetVisible(bool state, float delay = 0)
    {
        base.SetVisible(state, delay);
        
        if (!state)
        {
            HideTooltip();
        }
    }

    private void Update()
    {
        // 按B键切换背包显示
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (visibleState)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
        
        // 按Escape关闭
        if (visibleState && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }
}
