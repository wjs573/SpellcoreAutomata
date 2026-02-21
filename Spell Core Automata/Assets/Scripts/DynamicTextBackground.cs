using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways] // 在编辑模式下实时更新
public class DynamicTextBackground : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Image backgroundImage;
    public Vector2 padding = new Vector2(20, 10); // 背景内边距

    void Update()
    {
        if (textComponent != null && backgroundImage != null)
        {
            // 获取 TextMeshPro 的 preferredWidth/preferredHeight（自动换行后的实际宽高）
            float textWidth = textComponent.preferredWidth;
            float textHeight = textComponent.preferredHeight;

            // 动态调整背景大小（文字宽高 + 内边距）
            backgroundImage.rectTransform.sizeDelta = new Vector2(
                textWidth + padding.x,
                textHeight + padding.y
            );

            // 可选：让背景和文字对齐方式一致
            SyncAlignment();
        }
    }

    // 同步文字和背景的对齐方式（可选）
    void SyncAlignment()
    {
        switch (textComponent.horizontalAlignment)
        {
            case HorizontalAlignmentOptions.Left:
                backgroundImage.rectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case HorizontalAlignmentOptions.Right:
                backgroundImage.rectTransform.pivot = new Vector2(1, 0.5f);
                break;
            case HorizontalAlignmentOptions.Center:
                backgroundImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
    }
}