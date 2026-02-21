
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI通知系统
    /// 用于显示游戏中的通知消息
    /// </summary>
    public class UINotification : MonoSingleton<UINotification>
    {
        [System.Serializable]
        public class NotificationStyle
        {
            public string name;
            public Color backgroundColor;
            public Color textColor;
            public int fontSize;
            public Font font;
            public Sprite icon;
            public float displayDuration = 3f;
            public float fadeDuration = 0.5f;
            public float slideDistance = 50f;
        }

        [SerializeField]
        private GameObject notificationPrefab;

        [SerializeField]
        private List<NotificationStyle> notificationStyles = new List<NotificationStyle>();

        [SerializeField]
        private string defaultStyleName = "Default";

        [SerializeField]
        private int maxNotifications = 5;

        private Queue<GameObject> notificationQueue = new Queue<GameObject>();
        private List<GameObject> activeNotifications = new List<GameObject>();

        protected void Awake()
        {
            InitNotificationSystem();
        }

        private void InitNotificationSystem()
        {
            if (notificationPrefab == null)
            {
                // 创建默认通知预制体
                notificationPrefab = CreateDefaultNotificationPrefab();
            }
        }

        /// <summary>
        /// 创建默认通知预制体
        /// </summary>
        private GameObject CreateDefaultNotificationPrefab()
        {
            GameObject notificationObj = new GameObject("DefaultNotification");

            // 添加背景
            Image background = notificationObj.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.7f);

            // 添加文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(notificationObj.transform);

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

            return notificationObj;
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        /// <param name="message">通知消息</param>
        /// <param name="styleName">样式名称</param>
        public void ShowNotification(string message, string styleName = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            // 创建通知
            GameObject notification = CreateNotification(message, styleName);

            // 添加到队列
            notificationQueue.Enqueue(notification);

            // 处理通知队列
            ProcessNotificationQueue();
        }

        /// <summary>
        /// 创建通知
        /// </summary>
        /// <param name="message">通知消息</param>
        /// <param name="styleName">样式名称</param>
        private GameObject CreateNotification(string message, string styleName)
        {
            // 实例化通知
            GameObject notification = Instantiate(notificationPrefab, UILayerManager.Instance.GetLayer(UILayer.System));
            notification.name = "Notification";

            // 获取组件
            Text notificationText = notification.GetComponentInChildren<Text>();
            Image backgroundImage = notification.GetComponent<Image>();
            RectTransform notificationRect = notification.GetComponent<RectTransform>();

            // 设置文本
            notificationText.text = message;

            // 应用样式
            NotificationStyle style = null;
            if (!string.IsNullOrEmpty(styleName))
            {
                style = notificationStyles.Find(s => s.name == styleName);
            }

            if (style == null && !string.IsNullOrEmpty(defaultStyleName))
            {
                style = notificationStyles.Find(s => s.name == defaultStyleName);
            }

            if (style != null)
            {
                // 应用背景样式
                if (backgroundImage != null)
                {
                    backgroundImage.color = style.backgroundColor;
                }

                // 应用文本样式
                if (notificationText != null)
                {
                    notificationText.color = style.textColor;
                    notificationText.fontSize = style.fontSize;
                    if (style.font != null)
                    {
                        notificationText.font = style.font;
                    }
                }

                // 存储样式数据，用于动画
                notificationRect.GetComponent<NotificationData>()?.SetStyle(style);
            }

            // 添加通知数据组件
            NotificationData data = notification.GetComponent<NotificationData>();
            if (data == null)
            {
                data = notification.AddComponent<NotificationData>();
            }

            if (style != null)
            {
                data.SetStyle(style);
            }

            return notification;
        }

        /// <summary>
        /// 处理通知队列
        /// </summary>
        private void ProcessNotificationQueue()
        {
            // 如果活动通知数量已达上限，不处理队列
            if (activeNotifications.Count >= maxNotifications)
            {
                return;
            }

            // 如果队列为空，不处理
            if (notificationQueue.Count == 0)
            {
                return;
            }

            // 从队列中取出通知
            GameObject notification = notificationQueue.Dequeue();

            // 添加到活动通知列表
            activeNotifications.Add(notification);

            // 播放通知动画
            StartCoroutine(PlayNotificationAnimation(notification));
        }

        /// <summary>
        /// 播放通知动画
        /// </summary>
        private IEnumerator PlayNotificationAnimation(GameObject notification)
        {
            NotificationData data = notification.GetComponent<NotificationData>();
            RectTransform rect = notification.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = notification.AddComponent<CanvasGroup>();
            }

            // 设置初始状态
            float displayDuration = data != null ? data.DisplayDuration : 3f;
            float fadeDuration = data != null ? data.FadeDuration : 0.5f;
            float slideDistance = data != null ? data.SlideDistance : 50f;

            // 设置初始位置
            Vector2 startPosition = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(startPosition.x, startPosition.y + slideDistance);

            // 淡入动画
            float elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / fadeDuration;

                canvasGroup.alpha = progress;
                rect.anchoredPosition = Vector2.Lerp(
                    new Vector2(startPosition.x, startPosition.y + slideDistance),
                    startPosition,
                    progress
                );

                yield return null;
            }

            // 确保完全显示
            canvasGroup.alpha = 1;
            rect.anchoredPosition = startPosition;

            // 等待显示时间
            yield return new WaitForSecondsRealtime(displayDuration);

            // 淡出动画
            elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / fadeDuration;

                canvasGroup.alpha = 1 - progress;
                rect.anchoredPosition = Vector2.Lerp(
                    startPosition,
                    new Vector2(startPosition.x, startPosition.y + slideDistance),
                    progress
                );

                yield return null;
            }

            // 确保完全隐藏
            canvasGroup.alpha = 0;

            // 从活动通知列表中移除
            activeNotifications.Remove(notification);

            // 销毁通知
            Destroy(notification);

            // 处理队列中的下一个通知
            ProcessNotificationQueue();
        }

        /// <summary>
        /// 添加样式
        /// </summary>
        /// <param name="style">样式</param>
        public void AddStyle(NotificationStyle style)
        {
            if (style == null || string.IsNullOrEmpty(style.name))
            {
                return;
            }

            // 检查是否已存在同名样式
            int existingIndex = notificationStyles.FindIndex(s => s.name == style.name);
            if (existingIndex >= 0)
            {
                notificationStyles[existingIndex] = style;
            }
            else
            {
                notificationStyles.Add(style);
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

            notificationStyles.RemoveAll(s => s.name == styleName);
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

            if (notificationStyles.Exists(s => s.name == styleName))
            {
                defaultStyleName = styleName;
            }
        }

        /// <summary>
        /// 清除所有通知
        /// </summary>
        public void ClearAllNotifications()
        {
            // 清除队列中的通知
            notificationQueue.Clear();

            // 清除活动中的通知
            foreach (GameObject notification in activeNotifications)
            {
                if (notification != null)
                {
                    Destroy(notification);
                }
            }

            activeNotifications.Clear();
        }
    }

    /// <summary>
    /// 通知数据组件
    /// </summary>
    public class NotificationData : MonoBehaviour
    {
        private float displayDuration = 3f;
        private float fadeDuration = 0.5f;
        private float slideDistance = 50f;

        public float DisplayDuration => displayDuration;
        public float FadeDuration => fadeDuration;
        public float SlideDistance => slideDistance;

        public void SetStyle(UINotification.NotificationStyle style)
        {
            if (style != null)
            {
                displayDuration = style.displayDuration;
                fadeDuration = style.fadeDuration;
                slideDistance = style.slideDistance;
            }
        }
    }
}
