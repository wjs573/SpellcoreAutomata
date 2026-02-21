
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI对话框系统
    /// 用于显示游戏中的对话框和确认框
    /// </summary>
    public class UIDialog : MonoSingleton<UIDialog>
    {
        [System.Serializable]
        public class DialogStyle
        {
            public string name;
            public Color backgroundColor;
            public Color titleColor;
            public Color messageColor;
            public Color buttonBackgroundColor;
            public Color buttonTextColor;
            public int titleFontSize;
            public int messageFontSize;
            public int buttonFontSize;
            public Font font;
            public Sprite backgroundSprite;
            public Sprite buttonBackgroundSprite;
        }

        [SerializeField]
        private GameObject dialogPrefab;

        [SerializeField]
        private List<DialogStyle> dialogStyles = new List<DialogStyle>();

        [SerializeField]
        private string defaultStyleName = "Default";

        private GameObject currentDialog;
        private Text titleText;
        private Text messageText;
        private Button confirmButton;
        private Button cancelButton;
        private Text confirmButtonText;
        private Text cancelButtonText;

        protected void Awake()
        {
            InitDialogSystem();
        }

        private void InitDialogSystem()
        {
            if (dialogPrefab == null)
            {
                // 创建默认对话框预制体
                dialogPrefab = CreateDefaultDialogPrefab();
            }
        }

        /// <summary>
        /// 创建默认对话框预制体
        /// </summary>
        private GameObject CreateDefaultDialogPrefab()
        {
            GameObject dialogObj = new GameObject("DefaultDialog");

            // 添加背景
            Image background = dialogObj.AddComponent<Image>();
            background.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            RectTransform dialogRect = dialogObj.GetComponent<RectTransform>();
            dialogRect.sizeDelta = new Vector2(400, 200);

            // 添加标题
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(dialogObj.transform);

            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.1f, 0.7f);
            titleRect.anchorMax = new Vector2(0.9f, 0.9f);
            titleRect.sizeDelta = Vector2.zero;

            Text titleText = titleObj.AddComponent<Text>();
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 18;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.text = "Title";

            // 添加消息
            GameObject messageObj = new GameObject("Message");
            messageObj.transform.SetParent(dialogObj.transform);

            RectTransform messageRect = messageObj.AddComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.1f, 0.4f);
            messageRect.anchorMax = new Vector2(0.9f, 0.7f);
            messageRect.sizeDelta = Vector2.zero;

            Text messageText = messageObj.AddComponent<Text>();
            messageText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            messageText.fontSize = 14;
            messageText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.text = "Message";

            // 添加确认按钮
            GameObject confirmButtonObj = new GameObject("ConfirmButton");
            confirmButtonObj.transform.SetParent(dialogObj.transform);

            RectTransform confirmButtonRect = confirmButtonObj.AddComponent<RectTransform>();
            confirmButtonRect.anchorMin = new Vector2(0.3f, 0.1f);
            confirmButtonRect.anchorMax = new Vector2(0.45f, 0.3f);
            confirmButtonRect.sizeDelta = Vector2.zero;

            Image confirmButtonImage = confirmButtonObj.AddComponent<Image>();
            confirmButtonImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);

            Button confirmButton = confirmButtonObj.AddComponent<Button>();

            GameObject confirmButtonTextObj = new GameObject("Text");
            confirmButtonTextObj.transform.SetParent(confirmButtonObj.transform);

            RectTransform confirmButtonTextRect = confirmButtonTextObj.AddComponent<RectTransform>();
            confirmButtonTextRect.anchorMin = Vector2.zero;
            confirmButtonTextRect.anchorMax = Vector2.one;
            confirmButtonTextRect.sizeDelta = Vector2.zero;

            Text confirmButtonText = confirmButtonTextObj.AddComponent<Text>();
            confirmButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            confirmButtonText.fontSize = 14;
            confirmButtonText.color = Color.white;
            confirmButtonText.alignment = TextAnchor.MiddleCenter;
            confirmButtonText.text = "Confirm";

            // 添加取消按钮
            GameObject cancelButtonObj = new GameObject("CancelButton");
            cancelButtonObj.transform.SetParent(dialogObj.transform);

            RectTransform cancelButtonRect = cancelButtonObj.AddComponent<RectTransform>();
            cancelButtonRect.anchorMin = new Vector2(0.55f, 0.1f);
            cancelButtonRect.anchorMax = new Vector2(0.7f, 0.3f);
            cancelButtonRect.sizeDelta = Vector2.zero;

            Image cancelButtonImage = cancelButtonObj.AddComponent<Image>();
            cancelButtonImage.color = new Color(0.9f, 0.3f, 0.3f, 1f);

            Button cancelButton = cancelButtonObj.AddComponent<Button>();

            GameObject cancelButtonTextObj = new GameObject("Text");
            cancelButtonTextObj.transform.SetParent(cancelButtonObj.transform);

            RectTransform cancelButtonTextRect = cancelButtonTextObj.AddComponent<RectTransform>();
            cancelButtonTextRect.anchorMin = Vector2.zero;
            cancelButtonTextRect.anchorMax = Vector2.one;
            cancelButtonTextRect.sizeDelta = Vector2.zero;

            Text cancelButtonText = cancelButtonTextObj.AddComponent<Text>();
            cancelButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            cancelButtonText.fontSize = 14;
            cancelButtonText.color = Color.white;
            cancelButtonText.alignment = TextAnchor.MiddleCenter;
            cancelButtonText.text = "Cancel";

            return dialogObj;
        }

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <param name="onConfirm">确认回调</param>
        /// <param name="onCancel">取消回调</param>
        /// <param name="confirmButtonText">确认按钮文本</param>
        /// <param name="cancelButtonText">取消按钮文本</param>
        /// <param name="styleName">样式名称</param>
        public void ShowConfirmDialog(string title, string message, Action onConfirm = null, Action onCancel = null, 
            string confirmButtonText = "Confirm", string cancelButtonText = "Cancel", string styleName = null)
        {
            // 创建对话框
            CreateDialog(title, message, confirmButtonText, cancelButtonText, styleName, true);

            // 设置按钮事件
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => {
                HideDialog();
                onConfirm?.Invoke();
            });

            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => {
                HideDialog();
                onCancel?.Invoke();
            });
        }

        /// <summary>
        /// 显示消息对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <param name="onConfirm">确认回调</param>
        /// <param name="confirmButtonText">确认按钮文本</param>
        /// <param name="styleName">样式名称</param>
        public void ShowMessageDialog(string title, string message, Action onConfirm = null, 
            string confirmButtonText = "OK", string styleName = null)
        {
            // 创建对话框
            CreateDialog(title, message, confirmButtonText, "", styleName, false);

            // 设置按钮事件
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => {
                HideDialog();
                onConfirm?.Invoke();
            });
        }

        /// <summary>
        /// 创建对话框
        /// </summary>
        private void CreateDialog(string title, string message, string confirmButtonText, string cancelButtonText, 
            string styleName, bool showCancelButton)
        {
            // 如果已有对话框，先隐藏
            if (currentDialog != null)
            {
                HideDialog();
            }

            // 实例化对话框
            currentDialog = Instantiate(dialogPrefab, UILayerManager.Instance.GetLayer(UILayer.Popup));
            currentDialog.name = "Dialog";

            // 获取组件
            Transform titleTransform = currentDialog.transform.Find("Title");
            Transform messageTransform = currentDialog.transform.Find("Message");
            Transform confirmButtonTransform = currentDialog.transform.Find("ConfirmButton");
            Transform cancelButtonTransform = currentDialog.transform.Find("CancelButton");

            if (titleTransform != null)
            {
                titleText = titleTransform.GetComponent<Text>();
            }

            if (messageTransform != null)
            {
                messageText = messageTransform.GetComponent<Text>();
            }

            if (confirmButtonTransform != null)
            {
                confirmButton = confirmButtonTransform.GetComponent<Button>();
                Transform confirmButtonTextTransform = confirmButtonTransform.Find("Text");
                if (confirmButtonTextTransform != null)
                {
                    this.confirmButtonText = confirmButtonTextTransform.GetComponent<Text>();
                }
            }

            if (cancelButtonTransform != null)
            {
                cancelButton = cancelButtonTransform.GetComponent<Button>();
                Transform cancelButtonTextTransform = cancelButtonTransform.Find("Text");
                if (cancelButtonTextTransform != null)
                {
                    this.cancelButtonText = cancelButtonTextTransform.GetComponent<Text>();
                }
            }

            // 设置文本
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (messageText != null)
            {
                messageText.text = message;
            }

            if (this.confirmButtonText != null)
            {
                this.confirmButtonText.text = confirmButtonText;
            }

            if (this.cancelButtonText != null)
            {
                this.cancelButtonText.text = cancelButtonText;
            }

            // 显示/隐藏取消按钮
            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(showCancelButton);
            }

            // 应用样式
            if (!string.IsNullOrEmpty(styleName))
            {
                ApplyStyle(styleName);
            }
            else if (!string.IsNullOrEmpty(defaultStyleName))
            {
                ApplyStyle(defaultStyleName);
            }

            // 显示对话框
            currentDialog.SetActive(true);
        }

        /// <summary>
        /// 隐藏对话框
        /// </summary>
        public void HideDialog()
        {
            if (currentDialog != null)
            {
                currentDialog.SetActive(false);
                Destroy(currentDialog);
                currentDialog = null;
            }
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

            DialogStyle style = dialogStyles.Find(s => s.name == styleName);
            if (style == null)
            {
                return;
            }

            // 应用背景样式
            Image backgroundImage = currentDialog.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = style.backgroundColor;
                if (style.backgroundSprite != null)
                {
                    backgroundImage.sprite = style.backgroundSprite;
                }
            }

            // 应用标题样式
            if (titleText != null)
            {
                titleText.color = style.titleColor;
                titleText.fontSize = style.titleFontSize;
                if (style.font != null)
                {
                    titleText.font = style.font;
                }
            }

            // 应用消息样式
            if (messageText != null)
            {
                messageText.color = style.messageColor;
                messageText.fontSize = style.messageFontSize;
                if (style.font != null)
                {
                    messageText.font = style.font;
                }
            }

            // 应用按钮样式
            if (confirmButton != null)
            {
                Image confirmButtonImage = confirmButton.GetComponent<Image>();
                if (confirmButtonImage != null)
                {
                    confirmButtonImage.color = style.buttonBackgroundColor;
                    if (style.buttonBackgroundSprite != null)
                    {
                        confirmButtonImage.sprite = style.buttonBackgroundSprite;
                    }
                }

                if (this.confirmButtonText != null)
                {
                    this.confirmButtonText.color = style.buttonTextColor;
                    this.confirmButtonText.fontSize = style.buttonFontSize;
                    if (style.font != null)
                    {
                        this.confirmButtonText.font = style.font;
                    }
                }
            }

            if (cancelButton != null)
            {
                Image cancelButtonImage = cancelButton.GetComponent<Image>();
                if (cancelButtonImage != null)
                {
                    cancelButtonImage.color = style.buttonBackgroundColor;
                    if (style.buttonBackgroundSprite != null)
                    {
                        cancelButtonImage.sprite = style.buttonBackgroundSprite;
                    }
                }

                if (this.cancelButtonText != null)
                {
                    this.cancelButtonText.color = style.buttonTextColor;
                    this.cancelButtonText.fontSize = style.buttonFontSize;
                    if (style.font != null)
                    {
                        this.cancelButtonText.font = style.font;
                    }
                }
            }
        }

        /// <summary>
        /// 添加样式
        /// </summary>
        /// <param name="style">样式</param>
        public void AddStyle(DialogStyle style)
        {
            if (style == null || string.IsNullOrEmpty(style.name))
            {
                return;
            }

            // 检查是否已存在同名样式
            int existingIndex = dialogStyles.FindIndex(s => s.name == style.name);
            if (existingIndex >= 0)
            {
                dialogStyles[existingIndex] = style;
            }
            else
            {
                dialogStyles.Add(style);
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

            dialogStyles.RemoveAll(s => s.name == styleName);
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

            if (dialogStyles.Exists(s => s.name == styleName))
            {
                defaultStyleName = styleName;
            }
        }
    }
}
