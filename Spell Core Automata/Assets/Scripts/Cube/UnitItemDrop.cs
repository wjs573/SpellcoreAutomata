using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitItemDrop : MonoBehaviour
{
    [Header("UI References")]
    public Canvas nameCanvas;          // 世界空间的Canvas
    public TextMeshProUGUI nameText;  // 显示物品名称的TMP
    public Image nameBackground;      // 背景Image

    [Header("Settings")]
    public string itemName = "装备名称";
    public Color textColor = Color.white;
    public Vector2 backgroundPadding = new Vector2(20, 10); // 背景内边距
    public float displayDuration = 3f; // 默认显示时间

    void Start()
    {
        InitializeNameLabel();  // 初始化UI
        ShowNameTemporarily(); // 显示名称（3秒后自动隐藏）
    }

    // 初始化UI（设置名称、颜色、背景大小）
    public void InitializeNameLabel()
    {
        if (nameText != null)
        {
            nameText.text = itemName;
            nameText.color = textColor;
        }

        if (nameBackground != null && nameText != null)
        {
            // 动态调整背景大小（文字宽高 + 内边距）
            float textWidth = nameText.preferredWidth;
            float textHeight = nameText.preferredHeight;
            nameBackground.rectTransform.sizeDelta = new Vector2(
                textWidth + backgroundPadding.x,
                textHeight + backgroundPadding.y
            );
        }

        // 初始状态设为隐藏（等待触发显示）
        HideName();
    }

    // 显示名称（持续一段时间后自动隐藏）
    public void ShowNameTemporarily()
    {
        if (nameCanvas != null)
            nameCanvas.gameObject.SetActive(true);

        // 3秒后自动隐藏
        Invoke("HideName", displayDuration);
    }

    // 直接关闭显示（名称+背景）
    public void HideName()
    {
        if (nameCanvas != null)
            nameCanvas.gameObject.SetActive(false);
    }

    // 玩家靠近时显示名称（可选）
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowNameTemporarily();
        }
    }

    // 确保UI始终面向摄像机
    void LateUpdate()
    {
        if (nameCanvas != null && nameCanvas.gameObject.activeSelf)
        {
            nameCanvas.transform.LookAt(Camera.main.transform);
            nameCanvas.transform.Rotate(0, 180, 0); // 避免文字反向
        }
    }
}
