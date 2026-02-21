
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI层级枚举
    /// </summary>
    public enum UILayer
    {
        /// <summary>
        /// 背景层
        /// </summary>
        Background = 0,

        /// <summary>
        /// 普通UI层
        /// </summary>
        Normal = 100,

        /// <summary>
        /// 弹窗层
        /// </summary>
        Popup = 200,

        /// <summary>
        /// 提示层
        /// </summary>
        Tooltip = 300,

        /// <summary>
        /// 系统层
        /// </summary>
        System = 400,

        /// <summary>
        /// 加载层
        /// </summary>
        Loading = 500,

        /// <summary>
        /// 顶层
        /// </summary>
        Top = 999
    }

    /// <summary>
    /// UI层级管理器
    /// </summary>
    public class UILayerManager : MonoSingleton<UILayerManager>
    {
        [SerializeField]
        private Transform backgroundLayer;

        [SerializeField]
        private Transform normalLayer;

        [SerializeField]
        private Transform popupLayer;

        [SerializeField]
        private Transform tooltipLayer;

        [SerializeField]
        private Transform systemLayer;

        [SerializeField]
        private Transform loadingLayer;

        [SerializeField]
        private Transform topLayer;

        protected void Awake()
        {
            InitLayers();
        }

        private void InitLayers()
        {
            // 创建层级对象
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                canvas = new GameObject("Canvas");
                canvas.AddComponent<Canvas>();
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }

            // 创建各层级
            backgroundLayer = CreateLayer("BackgroundLayer", UILayer.Background, canvas.transform);
            normalLayer = CreateLayer("NormalLayer", UILayer.Normal, canvas.transform);
            popupLayer = CreateLayer("PopupLayer", UILayer.Popup, canvas.transform);
            tooltipLayer = CreateLayer("TooltipLayer", UILayer.Tooltip, canvas.transform);
            systemLayer = CreateLayer("SystemLayer", UILayer.System, canvas.transform);
            loadingLayer = CreateLayer("LoadingLayer", UILayer.Loading, canvas.transform);
            topLayer = CreateLayer("TopLayer", UILayer.Top, canvas.transform);
        }

        private Transform CreateLayer(string name, UILayer layer, Transform parent)
        {
            GameObject layerObj = new GameObject(name);
            layerObj.transform.SetParent(parent);
            RectTransform rectTransform = layerObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localScale = Vector3.one;

            CanvasGroup canvasGroup = layerObj.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;

            return layerObj.transform;
        }

        /// <summary>
        /// 获取指定层级
        /// </summary>
        public Transform GetLayer(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Background:
                    return backgroundLayer;
                case UILayer.Normal:
                    return normalLayer;
                case UILayer.Popup:
                    return popupLayer;
                case UILayer.Tooltip:
                    return tooltipLayer;
                case UILayer.System:
                    return systemLayer;
                case UILayer.Loading:
                    return loadingLayer;
                case UILayer.Top:
                    return topLayer;
                default:
                    return normalLayer;
            }
        }
    }
}
