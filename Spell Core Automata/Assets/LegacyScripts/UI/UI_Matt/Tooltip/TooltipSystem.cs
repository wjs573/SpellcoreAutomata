using JinShan;

public class TooltipSystem : MonoSingleton<TooltipSystem>
{
    private static TooltipSystem current;
    public Tooltip tooltip;
    public void Awake()
    {
        current = this;
        Hide();
    }

    public static void Show(string content, string header = "")
    {
        current.tooltip.SetText(content, header);
        current.tooltip.gameObject.SetActive(true);
    }

    /// <summary>
    /// 显示角色身上的buff
    /// </summary>
    /// <param name="buffObj"></param>
    public static void Show(BuffObj buffObj)
    {
        current.tooltip.SetText(buffObj);
        current.tooltip.gameObject.SetActive(true);
    }

    /// <summary>
    /// 显示角色背包的物品
    /// </summary>
    /// <param name="buffObj"></param>
    public static void ShowItem(InventorySlot slot)
    {
        current.tooltip.SetText(slot);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
