
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI加载界面系统
    /// 用于显示游戏加载进度和加载动画
    /// </summary>
    public class UILoadingScreen : MonoSingleton<UILoadingScreen>
    {
        [SerializeField]
        private GameObject loadingScreenPrefab;

        [SerializeField]
        private Slider progressBar;

        [SerializeField]
        private Text progressText;

        [SerializeField]
        private Text loadingTipText;

        [SerializeField]
        private string[] loadingTips;

        private GameObject loadingScreenInstance;
        private Coroutine loadingCoroutine;
        private bool isLoading = false;

        protected void Awake()
        {
            InitLoadingScreen();
        }

        private void InitLoadingScreen()
        {
            if (loadingScreenPrefab == null)
            {
                // 创建默认加载界面预制体
                loadingScreenPrefab = CreateDefaultLoadingScreenPrefab();
            }

            // 创建加载界面实例
            loadingScreenInstance = Instantiate(loadingScreenPrefab, UILayerManager.Instance.GetLayer(UILayer.Loading));
            loadingScreenInstance.name = "LoadingScreen";
            loadingScreenInstance.SetActive(false);

            // 获取组件
            progressBar = loadingScreenInstance.GetComponentInChildren<Slider>();
            progressText = loadingScreenInstance.GetComponentInChildren<Text>();

            // 查找提示文本
            Transform tipTransform = loadingScreenInstance.transform.Find("TipText");
            if (tipTransform != null)
            {
                loadingTipText = tipTransform.GetComponent<Text>();
            }
        }

        /// <summary>
        /// 创建默认加载界面预制体
        /// </summary>
        private GameObject CreateDefaultLoadingScreenPrefab()
        {
            GameObject loadingScreenObj = new GameObject("DefaultLoadingScreen");

            // 添加背景
            Image background = loadingScreenObj.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.8f);

            // 添加进度条
            GameObject progressBarObj = new GameObject("ProgressBar");
            progressBarObj.transform.SetParent(loadingScreenObj.transform);

            RectTransform progressBarRect = progressBarObj.AddComponent<RectTransform>();
            progressBarRect.anchorMin = new Vector2(0.3f, 0.45f);
            progressBarRect.anchorMax = new Vector2(0.7f, 0.5f);
            progressBarRect.sizeDelta = Vector2.zero;

            // 添加进度条背景
            GameObject progressBarBackgroundObj = new GameObject("Background");
            progressBarBackgroundObj.transform.SetParent(progressBarObj.transform);

            Image progressBarBackground = progressBarBackgroundObj.AddComponent<Image>();
            progressBarBackground.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            RectTransform progressBarBackgroundRect = progressBarBackgroundObj.GetComponent<RectTransform>();
            progressBarBackgroundRect.anchorMin = Vector2.zero;
            progressBarBackgroundRect.anchorMax = Vector2.one;
            progressBarBackgroundRect.sizeDelta = Vector2.zero;

            // 添加进度条填充
            GameObject progressBarFillObj = new GameObject("Fill");
            progressBarFillObj.transform.SetParent(progressBarObj.transform);

            Image progressBarFill = progressBarFillObj.AddComponent<Image>();
            progressBarFill.color = new Color(0.5f, 0.8f, 1f, 1f);

            RectTransform progressBarFillRect = progressBarFillObj.GetComponent<RectTransform>();
            progressBarFillRect.anchorMin = Vector2.zero;
            progressBarFillRect.anchorMax = new Vector2(0, 1);
            progressBarFillRect.sizeDelta = Vector2.zero;

            // 添加进度条滑块组件
            Slider slider = progressBarObj.AddComponent<Slider>();
            slider.fillRect = progressBarFillRect;
            slider.targetGraphic = progressBarFill;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 0;

            // 添加进度文本
            GameObject progressTextObj = new GameObject("ProgressText");
            progressTextObj.transform.SetParent(loadingScreenObj.transform);

            RectTransform progressTextRect = progressTextObj.AddComponent<RectTransform>();
            progressTextRect.anchorMin = new Vector2(0.4f, 0.5f);
            progressTextRect.anchorMax = new Vector2(0.6f, 0.55f);
            progressTextRect.sizeDelta = Vector2.zero;

            Text progressText = progressTextObj.AddComponent<Text>();
            progressText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            progressText.fontSize = 18;
            progressText.color = Color.white;
            progressText.alignment = TextAnchor.MiddleCenter;
            progressText.text = "0%";

            // 添加提示文本
            GameObject tipTextObj = new GameObject("TipText");
            tipTextObj.transform.SetParent(loadingScreenObj.transform);

            RectTransform tipTextRect = tipTextObj.AddComponent<RectTransform>();
            tipTextRect.anchorMin = new Vector2(0.2f, 0.6f);
            tipTextRect.anchorMax = new Vector2(0.8f, 0.65f);
            tipTextRect.sizeDelta = Vector2.zero;

            Text tipText = tipTextObj.AddComponent<Text>();
            tipText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            tipText.fontSize = 14;
            tipText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            tipText.alignment = TextAnchor.MiddleCenter;
            tipText.text = "Loading...";

            return loadingScreenObj;
        }

        /// <summary>
        /// 显示加载界面
        /// </summary>
        /// <param name="loadAction">加载操作</param>
        /// <param name="onComplete">加载完成回调</param>
        public void ShowLoadingScreen(Func<IEnumerator> loadAction, Action onComplete = null)
        {
            if (isLoading)
            {
                Debug.LogWarning("UILoadingScreen: 已有加载任务在进行中");
                return;
            }

            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
            }

            loadingCoroutine = StartCoroutine(LoadCoroutine(loadAction, onComplete));
        }

        /// <summary>
        /// 加载协程
        /// </summary>
        private IEnumerator LoadCoroutine(Func<IEnumerator> loadAction, Action onComplete)
        {
            isLoading = true;

            // 显示加载界面
            loadingScreenInstance.SetActive(true);

            // 显示随机提示
            if (loadingTipText != null && loadingTips != null && loadingTips.Length > 0)
            {
                loadingTipText.text = loadingTips[UnityEngine.Random.Range(0, loadingTips.Length)];
            }

            // 重置进度
            UpdateProgress(0);

            // 执行加载操作
            IEnumerator loadEnumerator = loadAction();
            while (loadEnumerator.MoveNext())
            {
                yield return loadEnumerator.Current;
            }

            // 完成加载
            UpdateProgress(1);

            // 等待一小段时间，让用户看到100%的进度
            yield return new WaitForSecondsRealtime(0.5f);

            // 隐藏加载界面
            loadingScreenInstance.SetActive(false);

            isLoading = false;

            // 调用完成回调
            onComplete?.Invoke();
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="progress">进度值（0-1）</param>
        public void UpdateProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = Mathf.RoundToInt(progress * 100) + "%";
            }
        }

        /// <summary>
        /// 设置加载提示
        /// </summary>
        /// <param name="tips">提示文本数组</param>
        public void SetLoadingTips(string[] tips)
        {
            loadingTips = tips;
        }

        /// <summary>
        /// 添加加载提示
        /// </summary>
        /// <param name="tip">提示文本</param>
        public void AddLoadingTip(string tip)
        {
            if (loadingTips == null)
            {
                loadingTips = new string[0];
            }

            Array.Resize(ref loadingTips, loadingTips.Length + 1);
            loadingTips[loadingTips.Length - 1] = tip;
        }

        /// <summary>
        /// 隐藏加载界面
        /// </summary>
        public void HideLoadingScreen()
        {
            if (loadingScreenInstance != null)
            {
                loadingScreenInstance.SetActive(false);
            }

            isLoading = false;

            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
        }
    }
}
