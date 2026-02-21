
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI菜单项
    /// </summary>
    public class UIMenuItem : MonoBehaviour
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
        private Color highlightedColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        [SerializeField]
        private Color selectedColor = new Color(0.6f, 0.8f, 1f, 1f);

        [SerializeField]
        private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private bool isInteractable = true;
        private bool isSelected = false;

        /// <summary>
        /// 菜单项索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 菜单项数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 菜单项点击事件
        /// </summary>
        public event Action<UIMenuItem> OnClicked;

        /// <summary>
        /// 菜单项选中事件
        /// </summary>
        public event Action<UIMenuItem> OnSelected;

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
        /// 模拟点击菜单项
        /// </summary>
        public void Click()
        {
            OnClick();
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

        /// <summary>
        /// 高亮
        /// </summary>
        public void Highlight()
        {
            if (backgroundImage != null && isInteractable && !isSelected)
            {
                backgroundImage.color = highlightedColor;
            }
        }

        /// <summary>
        /// 取消高亮
        /// </summary>
        public void Unhighlight()
        {
            UpdateVisualState();
        }
    }

    /// <summary>
    /// UI菜单
    /// </summary>
    public class UIMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject menuItemPrefab;

        [SerializeField]
        private Transform menuItemsContainer;

        [SerializeField]
        private bool vertical = true;

        [SerializeField]
        private float spacing = 5f;

        [SerializeField]
        private bool allowMultipleSelection = false;

        private List<UIMenuItem> menuItems = new List<UIMenuItem>();
        private List<UIMenuItem> selectedMenuItems = new List<UIMenuItem>();
        private int currentIndex = -1;

        /// <summary>
        /// 菜单项点击事件
        /// </summary>
        public event Action<UIMenuItem> OnMenuItemClicked;

        /// <summary>
        /// 菜单项选中事件
        /// </summary>
        public event Action<UIMenuItem> OnMenuItemSelected;

        /// <summary>
        /// 菜单项取消选中事件
        /// </summary>
        public event Action<UIMenuItem> OnMenuItemDeselected;

        /// <summary>
        /// 当前索引
        /// </summary>
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                if (value != currentIndex)
                {
                    currentIndex = Mathf.Clamp(value, -1, menuItems.Count - 1);

                    if (currentIndex >= 0)
                    {
                        SelectMenuItem(currentIndex);
                    }
                }
            }
        }

        private void Awake()
        {
            // 如果没有指定菜单项容器，使用自身
            if (menuItemsContainer == null)
            {
                menuItemsContainer = transform;
            }
        }

        /// <summary>
        /// 添加菜单项
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="icon">图标</param>
        /// <param name="data">数据</param>
        /// <returns>菜单项</returns>
        public UIMenuItem AddMenuItem(string text, Sprite icon = null, object data = null)
        {
            if (menuItemPrefab == null)
            {
                Debug.LogError("UIMenu: 未设置菜单项预制体");
                return null;
            }

            // 创建菜单项
            GameObject menuItemObj = Instantiate(menuItemPrefab, menuItemsContainer);
            UIMenuItem menuItem = menuItemObj.GetComponent<UIMenuItem>();

            if (menuItem == null)
            {
                menuItem = menuItemObj.AddComponent<UIMenuItem>();
            }

            // 设置菜单项属性
            menuItem.Index = menuItems.Count;
            menuItem.Text = text;
            menuItem.Icon = icon;
            menuItem.SetData(data);

            // 注册菜单项事件
            menuItem.OnClicked += OnMenuItemClickedHandler;
            menuItem.OnSelected += OnMenuItemSelectedHandler;

            // 添加到列表
            menuItems.Add(menuItem);

            // 更新布局
            UpdateLayout();

            return menuItem;
        }

        /// <summary>
        /// 移除菜单项
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveMenuItem(int index)
        {
            if (index < 0 || index >= menuItems.Count)
            {
                return;
            }

            // 取消选中
            if (menuItems[index].IsSelected)
            {
                DeselectMenuItem(index);
            }

            // 移除事件
            menuItems[index].OnClicked -= OnMenuItemClickedHandler;
            menuItems[index].OnSelected -= OnMenuItemSelectedHandler;

            // 销毁对象
            Destroy(menuItems[index].gameObject);

            // 从列表中移除
            menuItems.RemoveAt(index);

            // 更新索引
            for (int i = index; i < menuItems.Count; i++)
            {
                menuItems[i].Index = i;
            }

            // 更新当前索引
            if (currentIndex >= menuItems.Count)
            {
                currentIndex = menuItems.Count - 1;
            }

            // 更新布局
            UpdateLayout();
        }

        /// <summary>
        /// 清除所有菜单项
        /// </summary>
        public void ClearMenuItems()
        {
            // 取消选中所有菜单项
            DeselectAllMenuItems();

            // 移除事件
            foreach (UIMenuItem menuItem in menuItems)
            {
                menuItem.OnClicked -= OnMenuItemClickedHandler;
                menuItem.OnSelected -= OnMenuItemSelectedHandler;

                if (menuItem != null && menuItem.gameObject != null)
                {
                    Destroy(menuItem.gameObject);
                }
            }

            // 清空列表
            menuItems.Clear();
            currentIndex = -1;
        }

        /// <summary>
        /// 选中菜单项
        /// </summary>
        /// <param name="index">索引</param>
        public void SelectMenuItem(int index)
        {
            if (index < 0 || index >= menuItems.Count)
            {
                return;
            }

            if (!allowMultipleSelection)
            {
                DeselectAllMenuItems();
            }

            menuItems[index].IsSelected = true;
            currentIndex = index;
        }

        /// <summary>
        /// 取消选中菜单项
        /// </summary>
        /// <param name="index">索引</param>
        public void DeselectMenuItem(int index)
        {
            if (index < 0 || index >= menuItems.Count)
            {
                return;
            }

            menuItems[index].IsSelected = false;
        }

        /// <summary>
        /// 取消选中所有菜单项
        /// </summary>
        public void DeselectAllMenuItems()
        {
            foreach (UIMenuItem menuItem in selectedMenuItems)
            {
                if (menuItem != null)
                {
                    menuItem.IsSelected = false;
                }
            }

            selectedMenuItems.Clear();
            currentIndex = -1;
        }

        /// <summary>
        /// 获取选中的菜单项
        /// </summary>
        /// <returns>选中的菜单项列表</returns>
        public List<UIMenuItem> GetSelectedMenuItems()
        {
            return new List<UIMenuItem>(selectedMenuItems);
        }

        /// <summary>
        /// 获取选中的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>选中的数据列表</returns>
        public List<T> GetSelectedData<T>() where T : class
        {
            List<T> result = new List<T>();

            foreach (UIMenuItem menuItem in selectedMenuItems)
            {
                if (menuItem != null && menuItem.Data is T data)
                {
                    result.Add(data);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取菜单项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>菜单项</returns>
        public UIMenuItem GetMenuItem(int index)
        {
            if (index < 0 || index >= menuItems.Count)
            {
                return null;
            }

            return menuItems[index];
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        private void UpdateLayout()
        {
            if (menuItemsContainer == null)
            {
                return;
            }

            float position = 0;

            for (int i = 0; i < menuItems.Count; i++)
            {
                RectTransform itemRect = menuItems[i].GetComponent<RectTransform>();

                if (itemRect == null)
                {
                    continue;
                }

                if (vertical)
                {
                    itemRect.anchorMin = new Vector2(0, 1);
                    itemRect.anchorMax = new Vector2(1, 1);
                    itemRect.anchoredPosition = new Vector2(0, -position);
                }
                else
                {
                    itemRect.anchorMin = new Vector2(0, 0);
                    itemRect.anchorMax = new Vector2(0, 1);
                    itemRect.anchoredPosition = new Vector2(position, 0);
                }

                position += GetMenuItemSize(menuItems[i]) + spacing;
            }
        }

        /// <summary>
        /// 获取菜单项大小
        /// </summary>
        private float GetMenuItemSize(UIMenuItem menuItem)
        {
            if (menuItem == null)
            {
                return 0;
            }

            RectTransform itemRect = menuItem.GetComponent<RectTransform>();
            if (itemRect == null)
            {
                return 0;
            }

            return vertical ? itemRect.rect.height : itemRect.rect.width;
        }

        /// <summary>
        /// 处理菜单项点击事件
        /// </summary>
        private void OnMenuItemClickedHandler(UIMenuItem menuItem)
        {
            // 如果不允许多选，点击菜单项时选中它
            if (!allowMultipleSelection)
            {
                SelectMenuItem(menuItem.Index);
            }
            else
            {
                // 如果允许多选，点击菜单项时切换选中状态
                if (menuItem.IsSelected)
                {
                    DeselectMenuItem(menuItem.Index);
                }
                else
                {
                    SelectMenuItem(menuItem.Index);
                }
            }

            // 触发事件
            OnMenuItemClicked?.Invoke(menuItem);
        }

        /// <summary>
        /// 处理菜单项选中事件
        /// </summary>
        private void OnMenuItemSelectedHandler(UIMenuItem menuItem)
        {
            if (menuItem.IsSelected)
            {
                if (!selectedMenuItems.Contains(menuItem))
                {
                    selectedMenuItems.Add(menuItem);
                }

                OnMenuItemSelected?.Invoke(menuItem);
            }
            else
            {
                if (selectedMenuItems.Contains(menuItem))
                {
                    selectedMenuItems.Remove(menuItem);
                }

                OnMenuItemDeselected?.Invoke(menuItem);
            }
        }

        /// <summary>
        /// 导航到下一个菜单项
        /// </summary>
        public void NavigateNext()
        {
            if (menuItems.Count == 0)
            {
                return;
            }

            CurrentIndex = (currentIndex + 1) % menuItems.Count;
        }

        /// <summary>
        /// 导航到上一个菜单项
        /// </summary>
        public void NavigatePrevious()
        {
            if (menuItems.Count == 0)
            {
                return;
            }

            CurrentIndex = (currentIndex - 1 + menuItems.Count) % menuItems.Count;
        }

        /// <summary>
        /// 确认当前选中的菜单项
        /// </summary>
        public void ConfirmSelection()
        {
            if (currentIndex >= 0 && currentIndex < menuItems.Count)
            {
                menuItems[currentIndex].Click();
            }
        }

    }
}
