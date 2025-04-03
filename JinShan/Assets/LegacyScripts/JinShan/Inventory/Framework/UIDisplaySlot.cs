using JinShan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplaySlot : MonoBehaviour
{
    public TextMeshProUGUI _number;
    public Image _icon;
    public Image _Content;
    public InventorySlot inventorySlot;

    private Vector3 originalScale; // 用于存储原始的缩放比例

    private void Awake()
    {
        _number = transform.FindChildByName("Number").GetComponent<TextMeshProUGUI>();
        _Content = transform.FindChildByName("Content").GetComponent<Image>();
        _icon = transform.FindChildByName("Icon").GetComponent<Image>();
        originalScale = transform.localScale; // 保存原始缩放比例
        SetContent(null, 0);
    }

    public void SetContent(Sprite icon, int num)
    {
        _icon.sprite = icon;
        if (_Content != null)
        {
            _Content.sprite = icon;
        }

        _number.text = (num == 1 || num == 0 ? "" : num.ToString());
    }

    public void SetContent(Sprite icon, int num, UIDisplayType displayType, bool IsLocked = false)
    {
        if (displayType == UIDisplayType.skill_node_slot)
        {
            _icon.sprite = icon;
            if (IsLocked)
            {
                GetComponent<Image>().color = new Color(255, 0, 0, 100);
            }
            else
            {
                GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            _number.text = "";
        }
    }

    public void SetContent(InventorySlot inventorySlot)
    {
        this.inventorySlot = inventorySlot;
        if (inventorySlot.item == null || inventorySlot.item.Id == -1)
        {
            _number.text = "";
            _icon.sprite = null;
            return;
        }
        int num = inventorySlot.amount;
        _number.text = (num == 1 || num == 0 ? "" : num.ToString());
        _icon.sprite = inventorySlot.ItemObject.uiDisplay;
        if (inventorySlot.isLocked)
        {
            GetComponent<Image>().color = new Color(255, 0, 0, 100);
        }
        else
        {
            GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
    }

    // 设置高亮状态
    public void SetHighlight(bool isHighlighted)
    {
        if (isHighlighted)
        {
            // 缩放为1.1倍
            transform.localScale = originalScale * 1.2f;
        }
        else
        {
            // 恢复原始缩放比例
            transform.localScale = originalScale;
        }
    }
}

public enum UIDisplayType
{
    skill_cd_slot,
    skill_node_slot
}
