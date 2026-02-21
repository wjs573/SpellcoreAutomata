
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI标签页
    /// </summary>
    public class UITab : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private Image icon;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField]
        private Color selectedColor = new Color(0.6f, 0.8f, 1f, 1f);

        [SerializeField]
        private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private bool isInteractable = true;
        private bool isSelected = false;

        /// <summary>
        /// 标签页索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 标签页数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 标签页点击事件
        /// </summary>
        public event Action<UITab> OnClicked;

        /// <summary>
        /// 标签页选中事件
        /// </summary>
        public event Action<UITab> OnSelected;

        /// <summary>
        /// 标签页取消选中事件
        /// </summary>
        public event Action<UITab> OnDeselected;

        /// <summary>
        /// 是否可交互
        /// </summary>
        public bool IsInteractable
        {
            get { return isInteractable; }
            set
            {
                if (isInteractable != value)
                {
                    isInteractable = value;
                    UpdateVisualState();
                }
            }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    UpdateVisualState();

                    if (isSelected)
                    {
                        OnSelected?.Invoke(this);
                    }
                    else
                    {
                        OnDeselected?.Invoke(this);
                    }
                }
            }
        }

        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get { return text != null ? text.text : ""; }
            set
            {
                if (text != null)
                {
                    text.text = value;
                }
            }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public Sprite Icon
        {
            get { return icon != null ? icon.sprite : null; }
            set
            {
                if (icon != null)
                {
                    icon.sprite = value;
                    icon.gameObject.SetActive(value != null);
                }
            }
        }

        private void Awake()
        {
            // 获取组件
            if (text == null)
            {
                text = GetComponentInChildren<Text>();
            }

            if (icon == null)
            {
                Transform iconTransform = transform.Find("Icon");
                if (iconTransform != null)
                {
                    icon = iconTransform.GetComponent<Image>();
                }
            }

            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }

            // 添加按钮组件
            Button button = GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }

            // 注册按钮点击事件
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (isInteractable)
            {
                OnClicked?.Invoke(this);
            }
        }

        /// <summary>
        /// 更新视觉状态
        /// </summary>
        private void UpdateVisualState()
        {
            if (backgroundImage == null)
            {
                return;
            }

            if (!isInteractable)
            {
                backgroundImage.color = disabledColor;
            }
            else if (isSelected)
            {
                backgroundImage.color = selectedColor;
            }
            else
            {
                backgroundImage.color = normalColor;
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data">数据</param>
        public virtual void SetData(object data)
        {
            Data = data;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>数据</returns>
        public T GetData<T>() where T : class
        {
            return Data as T;
        }
    }

    /// <summary>
    /// UI标签页视图
    /// </summary>
    public class UITabView : MonoBehaviour
    {
        [SerializeField]
        private GameObject tabPrefab;

        [SerializeField]
        private Transform tabsContainer;

        [SerializeField]
        private Transform pagesContainer;

        [SerializeField]
        private bool horizontal = true;

        [SerializeField]
        private float spacing = 5f;

        private List<UITab> tabs = new List<UITab>();
        private List<GameObject> pages = new List<GameObject>();
        private int currentTabIndex = -1;

        /// <summary>
        /// 标签页切换事件
        /// </summary>
        public event Action<int, int> OnTabChanged; // 参数: 旧索引, 新索引

        /// <summary>
        /// 当前标签页索引
        /// </summary>
        public int CurrentTabIndex
        {
            get { return currentTabIndex; }
            set
            {
                if (value != currentTabIndex && value >= -1 && value < tabs.Count)
                {
                    int oldIndex = currentTabIndex;
                    currentTabIndex = value;

                    // 更新标签页和页面显示
                    UpdateTabsAndPages();

                    // 触发事件
                    OnTabChanged?.Invoke(oldIndex, currentTabIndex);
                }
            }
        }

        private void Awake()
        {
            // 如果没有指定标签页容器，使用自身
            if (tabsContainer == null)
            {
                tabsContainer = transform.Find("TabsContainer");
                if (tabsContainer == null)
                {
                    tabsContainer = transform;
                }
            }

            // 如果没有指定页面容器，查找或创建
            if (pagesContainer == null)
            {
                pagesContainer = transform.Find("PagesContainer");
                if (pagesContainer == null)
                {
                    GameObject pagesContainerObj = new GameObject("PagesContainer");
                    pagesContainerObj.transform.SetParent(transform);
                    RectTransform pagesContainerRect = pagesContainerObj.AddComponent<RectTransform>();
                    pagesContainerRect.anchorMin = Vector2.zero;
                    pagesContainerRect.anchorMax = Vector2.one;
                    pagesContainerRect.sizeDelta = Vector2.zero;
                    pagesContainer = pagesContainerObj.transform;
                }
            }
        }

        /// <summary>
        /// 添加标签页
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="page">页面</param>
        /// <param name="icon">图标</param>
        /// <param name="data">数据</param>
        /// <returns>标签页</returns>
        public UITab AddTab(string text, GameObject page, Sprite icon = null, object data = null)
        {
            if (tabPrefab == null)
            {
                Debug.LogError("UITabView: 未设置标签页预制体");
                return null;
            }

            // 创建标签页
            GameObject tabObj = Instantiate(tabPrefab, tabsContainer);
            UITab tab = tabObj.GetComponent<UITab>();

            if (tab == null)
            {
                tab = tabObj.AddComponent<UITab>();
            }

            // 设置标签页属性
            tab.Index = tabs.Count;
            tab.Text = text;
            tab.Icon = icon;
            tab.SetData(data);

            // 注册标签页事件
            tab.OnClicked += OnTabClickedHandler;

            // 添加到列表
            tabs.Add(tab);

            // 添加页面
            if (page != null)
            {
                page.transform.SetParent(pagesContainer);
                page.SetActive(false);
                pages.Add(page);
            }
            else
            {
                pages.Add(null);
            }

            // 更新布局
            UpdateLayout();

            // 如果是第一个标签页，默认选中
            if (tabs.Count == 1)
            {
                CurrentTabIndex = 0;
            }

            return tab;
        }

        /// <summary>
        /// 移除标签页
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveTab(int index)
        {
            if (index < 0 || index >= tabs.Count)
            {
                return;
            }

            // 移除事件
            tabs[index].OnClicked -= OnTabClickedHandler;

            // 销毁标签页和页面
            Destroy(tabs[index].gameObject);

            if (pages[index] != null)
            {
                Destroy(pages[index]);
            }

            // 从列表中移除
            tabs.RemoveAt(index);
            pages.RemoveAt(index);

            // 更新索引
            for (int i = index; i < tabs.Count; i++)
            {
                tabs[i].Index = i;
            }

            // 更新当前索引
            if (currentTabIndex >= tabs.Count)
            {
                CurrentTabIndex = tabs.Count - 1;
            }
            else if (currentTabIndex == index)
            {
                CurrentTabIndex = -1;
            }

            // 更新布局
            UpdateLayout();
        }

        /// <summary>
        /// 清除所有标签页
        /// </summary>
        public void ClearTabs()
        {
            // 移除事件
            foreach (UITab tab in tabs)
            {
                tab.OnClicked -= OnTabClickedHandler;

                if (tab != null && tab.gameObject != null)
                {
                    Destroy(tab.gameObject);
                }
            }

            // 销毁页面
            foreach (GameObject page in pages)
            {
                if (page != null)
                {
                    Destroy(page);
                }
            }

            // 清空列表
            tabs.Clear();
            pages.Clear();
            currentTabIndex = -1;
        }

        /// <summary>
        /// 更新标签页和页面显示
        /// </summary>
        private void UpdateTabsAndPages()
        {
            // 更新标签页选中状态
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].IsSelected = (i == currentTabIndex);
            }

            // 更新页面显示
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i] != null)
                {
                    pages[i].SetActive(i == currentTabIndex);
                }
            }
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        private void UpdateLayout()
        {
            if (tabsContainer == null)
            {
                return;
            }

            float totalSize = 0;

            for (int i = 0; i < tabs.Count; i++)
            {
                RectTransform tabRect = tabs[i].GetComponent<RectTransform>();

                if (horizontal)
                {
                    tabRect.anchorMin = new Vector2(0, 0);
                    tabRect.anchorMax = new Vector2(0, 1);
                    tabRect.sizeDelta = new Vector2(100, 0); // 默认宽度为100
                    tabRect.anchoredPosition = new Vector2(totalSize, 0);
                }
                else
                {
                    tabRect.anchorMin = new Vector2(0, 1);
                    tabRect.anchorMax = new Vector2(1, 1);
                    tabRect.sizeDelta = new Vector2(0, 30); // 默认高度为30
                    tabRect.anchoredPosition = new Vector2(0, -totalSize);
                }

                totalSize += (horizontal ? 100 : 30) + spacing;
            }

            // 更新容器大小
            RectTransform containerRect = tabsContainer.GetComponent<RectTransform>();
            if (horizontal)
            {
                containerRect.sizeDelta = new Vector2(totalSize, 0);
            }
            else
            {
                containerRect.sizeDelta = new Vector2(0, totalSize);
            }
        }

        /// <summary>
        /// 标签页点击事件处理
        /// </summary>
        private void OnTabClickedHandler(UITab tab)
        {
            CurrentTabIndex = tab.Index;
        }

        /// <summary>
        /// 获取标签页
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>标签页</returns>
        public UITab GetTab(int index)
        {
            if (index < 0 || index >= tabs.Count)
            {
                return null;
            }

            return tabs[index];
        }

        /// <summary>
        /// 获取页面
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>页面</returns>
        public GameObject GetPage(int index)
        {
            if (index < 0 || index >= pages.Count)
            {
                return null;
            }

            return pages[index];
        }
    }
}
