using JinShan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;

    public int Xoffset = 0;
    public int Yoffset = 0;

    public RectTransform rectTransform;

    /// <summary>
    /// tooltip的定位方式，目前有两种
    /// 1.鼠标位置相对于屏幕的比例
    /// 2.手动设置的
    /// </summary>
    public LocationType locationType;

    /// <summary>
    /// 手动设置的位置
    /// </summary>
    public Vector2 position;
    /// <summary>
    /// 手动设置的锚定
    /// </summary>
    public Vector2 rectTransformPivot;



    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        TooltipSystem.Instance.tooltip = this;


        position = new Vector2(0, 0);
        rectTransformPivot = new Vector2(0.5f, 0.5f);
    }


    public void SetText(string content, string header = "")
    {
        locationType = LocationType.MousePosition;
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLenght = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLenght > characterWrapLimit) ? true : false;
    }


    /// <summary>
    /// 读取buffobj的信息，显示在Tooltip上
    /// </summary>
    /// <param name="buffObj">待展示的buffobj</param>
    public void SetText(BuffObj buffObj)
    {
        locationType = LocationType.MousePosition;
        string header = buffObj.model.name;
        string content = $"剩余时间 ：{buffObj.duration:F1}\n持续时间：{buffObj.timeElapsed:F1}\n{buffObj.model.tags}";
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLenght = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLenght > characterWrapLimit) ? true : false;
    }

    /// <summary>
    /// 读取buffobj的信息，显示在Tooltip上
    /// </summary>
    /// <param name="buffObj">待展示的buffobj</param>
    public void SetText(InventorySlot slot)
    {
        locationType = LocationType.ClickSlotPosition;
        string header = slot.ItemObject.name;
        string content = $"{slot.ItemObject.description}\n" + GetTextFromItemBuffs(slot.item);
        //$"剩余时间 ：{buffObj.duration:F1}\n持续时间：{buffObj.timeElapsed:F1}\n{buffObj.model.tags}";
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLenght = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLenght > characterWrapLimit) ? true : false;
    }

    private string GetTextFromItemBuffs(Item item)
    {
        string text = "";
        ItemBuff[] buffs = item.buffs;
        for (int i = 0; i < buffs.Length; i++)
        {
            switch (buffs[i].attribute)
            {
                case Attributes.生命值:
                case Attributes.生命回复值:
                case Attributes.法力值:
                case Attributes.法力回复值:
                case Attributes.护盾值:
                case Attributes.攻击力:
                case Attributes.防御力:
                case Attributes.神魂强度:
                case Attributes.行动速率:
                case Attributes.冷却速率:
                case Attributes.移动速率:
                case Attributes.金灵根:
                case Attributes.木灵根:
                case Attributes.水灵根:
                case Attributes.火灵根:
                case Attributes.土灵根:
                    text += $"{buffs[i].attribute} +{buffs[i].value}\n";
                    break;
                case Attributes.暴击率:
                case Attributes.暴击伤害倍率:
                case Attributes.闪避率:
                    text += $"{buffs[i].attribute} {buffs[i].value:P0}\n"; // 注意这里去掉了+
                    break;
                default:
                    break;
            }
        }
        return text;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int contentLenght = contentField.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentLenght > characterWrapLimit) ? true : false;
        }

        switch (locationType)
        {
            case LocationType.MousePosition:
                Vector2 position = Input.mousePosition;

                float pivotX = (position.x + Xoffset) / Screen.width;
                float pivotY = (position.y + Yoffset) / Screen.height;

                rectTransform.pivot = new Vector2(pivotX, pivotY);

                gameObject.transform.position = position;
                break;

            case LocationType.ClickSlotPosition:
                rectTransform.pivot = this.rectTransformPivot;
                gameObject.transform.position = this.position;
                break;
            default:
                return;
        }

    }
}

public enum LocationType { MousePosition, ClickSlotPosition }
