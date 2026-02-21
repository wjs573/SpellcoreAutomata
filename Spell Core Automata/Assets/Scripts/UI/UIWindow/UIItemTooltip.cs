using UnityEngine;
using UnityEngine.UI;
using WJS;

/// <summary>
/// 物品提示UI组件
/// </summary>
public class UIItemTooltip : MonoBehaviour
{
    [Header("UI组件")]
    public Text itemNameText;
    public Text itemTypeText;
    public Text itemDescText;
    public Text propertyText;
    public Image iconImage;
    public Image backgroundImage;
    
    [Header("设置")]
    public Vector2 offset = new Vector2(15, -15);
    public float maxWidth = 300f;
    
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        
        // 默认隐藏
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示装备提示
    /// </summary>
    public void Show(EquipmentObj equipment, Vector3 position)
    {
        if (equipment == null) return;
        
        gameObject.SetActive(true);
        
        // 更新信息
        UpdateTooltipInfo(equipment);
        
        // 设置位置
        UpdatePosition(position);
    }

    /// <summary>
    /// 隐藏提示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 更新提示信息
    /// </summary>
    private void UpdateTooltipInfo(EquipmentObj equipment)
    {
        EquipmentModel model = equipment.model;
        
        // 名称
        if (itemNameText != null)
        {
            itemNameText.text = model.name;
        }
        
        // 类型
        if (itemTypeText != null)
        {
            itemTypeText.text = GetEquipmentTypeName(model.type);
        }
        
        // 图标
        if (iconImage != null)
        {
            Sprite sprite = LoadIcon(model.icon);
            iconImage.sprite = sprite;
            iconImage.gameObject.SetActive(sprite != null);
        }
        
        // 属性
        if (propertyText != null)
        {
            propertyText.text = FormatProperty(model.equipmentProperty);
        }
        
        // 描述（这里可以扩展添加描述字段到EquipmentModel）
        if (itemDescText != null)
        {
            if (!string.IsNullOrEmpty(model.id))
            {
                itemDescText.text = $"ID: {model.id}";
            }
            else
            {
                itemDescText.text = "";
            }
        }
    }

    /// <summary>
    /// 格式化属性显示
    /// </summary>
    private string FormatProperty(ChaProperty prop)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        if (prop.hp > 0) sb.AppendLine($"生命: +{prop.hp}");
        if (prop.mp > 0) sb.AppendLine($"灵力: +{prop.mp}");
        if (prop.attack > 0) sb.AppendLine($"攻击: +{prop.attack}");
        if (prop.defence > 0) sb.AppendLine($"防御: +{prop.defence}");
        if (prop.critic_multiplier > 0) sb.AppendLine($"暴击倍率: +{prop.critic_multiplier}x");
        if (prop.critic_rate > 0) sb.AppendLine($"暴击率: +{prop.critic_rate}");
        if (prop.cd_speed > 0) sb.AppendLine($"冷却速度: +{prop.cd_speed}");
        if (prop.moveSpeed > 0) sb.AppendLine($"移速: +{prop.moveSpeed}");
        if (prop.actionSpeed > 0) sb.AppendLine($"攻速: +{prop.actionSpeed}");
        
        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// 获取装备类型名称
    /// </summary>
    private string GetEquipmentTypeName(EquipmentType type)
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
    /// 更新位置，确保不超出屏幕
    /// </summary>
    private void UpdatePosition(Vector3 targetPosition)
    {
        if (rectTransform == null || parentCanvas == null) return;
        
        // 转换为屏幕坐标
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            parentCanvas.worldCamera, targetPosition);
        
        // 应用偏移
        screenPos += offset;
        
        // 确保不超出屏幕边界
        float tooltipWidth = rectTransform.sizeDelta.x;
        float tooltipHeight = rectTransform.sizeDelta.y;
        
        if (screenPos.x + tooltipWidth > Screen.width)
        {
            screenPos.x = targetPosition.x - tooltipWidth - offset.x;
        }
        
        if (screenPos.y - tooltipHeight < 0)
        {
            screenPos.y = targetPosition.y + tooltipHeight - offset.y;
        }
        
        // 设置位置
        rectTransform.position = screenPos;
    }
}
