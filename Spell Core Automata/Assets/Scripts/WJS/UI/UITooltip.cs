
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI提示系统
    /// 用于显示游戏中的提示信息
    /// </summary>
    public class UITooltip : MonoSingleton<UITooltip>
    {
        [System.Serializable]
        public class TooltipStyle
        {
            public string name;
            public Color backgroundColor;
            public Color textColor;
            public int fontSize;
            public Font font;
            public Sprite backgroundSprite;
            public Vector2 padding;
            public float cornerRadius;
        }

        [SerializeField]
        private GameObject tooltipPrefab;

        [SerializeField]
        private List<TooltipStyle> tooltipStyles = new List<TooltipStyle>();

        [SerializeField]
        private string defaultStyleName = "Default";

        private GameObject currentTooltip;
        private Text tooltipText;
        private Image backgroundImage;
        private RectTransform tooltipRect;
        private Coroutine hideTooltipCoroutine;

        protected void Awake()
        {
            InitTooltip();
        }

        private void InitTooltip()
        {
            if (tooltipPrefab == null)
            {
                // 创建默认提示框预制体
                tooltipPrefab = CreateDefaultTooltipPrefab();
            }

            // 创建提示框实例
            currentTooltip = Instantiate(tooltipPrefab, UILayerManager.Instance.GetLayer(UILayer.Tooltip));
            currentTooltip.name = "Tooltip";
            currentTooltip.SetActive(false);

            // 获取组件
            tooltipText = currentTooltip.GetComponentInChildren<Text>();
            backgroundImage = currentTooltip.GetComponent<Image>();
            tooltipRect = currentTooltip.GetComponent<RectTransform>();
        }

        /// <summary>
        /// 创建默认提示框预制体
        /// </summary>
        private GameObject CreateDefaultTooltipPrefab()
        {
            GameObject tooltipObj = new GameObject("DefaultTooltip");

            // 添加背景
            Image background = tooltipObj.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.8f);

            // 添加文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(tooltipObj.transform);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(10, 10);
            textRect.offsetMax = new Vector2(-10, -10);

            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            return tooltipObj;
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="text">提示文本</param>
        /// <param name="position">提示位置</param>
        /// <param name="duration">显示时长，0表示一直显示</param>
        /// <param name="styleName">样式名称</param>
        public void ShowTooltip(string text, Vector3 position, float duration = 0, string styleName = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // 设置文本
            tooltipText.text = text;

            // 应用样式
            if (!string.IsNullOrEmpty(styleName))
            {
                ApplyStyle(styleName);
            }
            else if (!string.IsNullOrEmpty(defaultStyleName))
            {
                ApplyStyle(defaultStyleName);
            }

            // 设置位置
            tooltipRect.position = position;

            // 显示提示框
            currentTooltip.SetActive(true);

            // 如果有隐藏协程，先停止
            if (hideTooltipCoroutine != null)
            {
                StopCoroutine(hideTooltipCoroutine);
            }

            // 如果有持续时间，设置自动隐藏
            if (duration > 0)
            {
                hideTooltipCoroutine = StartCoroutine(HideTooltipAfterDelay(duration));
            }
        }

        /// <summary>
        /// 在指定UI元素附近显示提示
        /// </summary>
        /// <param name="text">提示文本</param>
        /// <param name="target">目标UI元素</param>
        /// <param name="offset">偏移量</param>
        /// <param name="duration">显示时长，0表示一直显示</param>
        /// <param name="styleName">样式名称</param>
        public void ShowTooltipAtUIElement(string text, RectTransform target, Vector2 offset, float duration = 0, string styleName = null)
        {
            if (target == null)
            {
                return;
            }

            // 计算世界位置
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector3 position = (corners[0] + corners[2]) / 2;
            position += (Vector3)offset;

            ShowTooltip(text, position, duration, styleName);
        }

        /// <summary>
        /// 隐藏提示
        /// </summary>
        public void HideTooltip()
        {
            currentTooltip.SetActive(false);

            if (hideTooltipCoroutine != null)
            {
                StopCoroutine(hideTooltipCoroutine);
                hideTooltipCoroutine = null;
            }
        }

        /// <summary>
        /// 延迟隐藏提示
        /// </summary>
        private IEnumerator HideTooltipAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideTooltip();
        }

        /// <summary>
        /// 应用样式
        /// </summary>
        /// <param name="styleName">样式名称</param>
        private void ApplyStyle(string styleName)
        {
            if (string.IsNullOrEmpty(styleName))
            {
                return;
            }

            TooltipStyle style = tooltipStyles.Find(s => s.name == styleName);
            if (style == null)
            {
                return;
            }

            // 应用背景样式
            if (backgroundImage != null)
            {
                backgroundImage.color = style.backgroundColor;
                if (style.backgroundSprite != null)
                {
                    backgroundImage.sprite = style.backgroundSprite;
                }
            }

            // 应用文本样式
            if (tooltipText != null)
            {
                tooltipText.color = style.textColor;
                tooltipText.fontSize = style.fontSize;
                if (style.font != null)
                {
                    tooltipText.font = style.font;
                }
            }

            // 应用内边距
            if (tooltipRect != null)
            {
                tooltipRect.offsetMin = style.padding;
                tooltipRect.offsetMax = -style.padding;
            }
        }

        /// <summary>
        /// 添加样式
        /// </summary>
        /// <param name="style">样式</param>
        public void AddStyle(TooltipStyle style)
        {
            if (style == null || string.IsNullOrEmpty(style.name))
            {
                return;
            }

            // 检查是否已存在同名样式
            int existingIndex = tooltipStyles.FindIndex(s => s.name == style.name);
            if (existingIndex >= 0)
            {
                tooltipStyles[existingIndex] = style;
            }
            else
            {
                tooltipStyles.Add(style);
            }
        }

        /// <summary>
        /// 移除样式
        /// </summary>
        /// <param name="styleName">样式名称</param>
        public void RemoveStyle(string styleName)
        {
            if (string.IsNullOrEmpty(styleName))
            {
                return;
            }

            tooltipStyles.RemoveAll(s => s.name == styleName);
        }

        /// <summary>
        /// 设置默认样式
        /// </summary>
        /// <param name="styleName">样式名称</param>
        public void SetDefaultStyle(string styleName)
        {
            if (string.IsNullOrEmpty(styleName))
            {
                return;
            }

            if (tooltipStyles.Exists(s => s.name == styleName))
            {
                defaultStyleName = styleName;
            }
        }
    }
}
