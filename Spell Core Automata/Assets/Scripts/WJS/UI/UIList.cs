
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI列表项
    /// </summary>
    public class UIListItem : MonoBehaviour
    {
        /// <summary>
        /// 列表项索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 列表项数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 列表项选中事件
        /// </summary>
        public event Action<UIListItem> OnSelected;

        /// <summary>
        /// 列表项取消选中事件
        /// </summary>
        public event Action<UIListItem> OnDeselected;

        /// <summary>
        /// 列表项点击事件
        /// </summary>
        public event Action<UIListItem> OnClicked;

        private bool isSelected = false;

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

                    if (isSelected)
                    {
                        OnSelected?.Invoke(this);
                    }
                    else
                    {
                        OnDeselected?.Invoke(this);
                    }

                    UpdateSelectionVisual();
                }
            }
        }

        /// <summary>
        /// 更新选中视觉效果
        /// </summary>
        protected virtual void UpdateSelectionVisual()
        {
            // 子类可以重写此方法以自定义选中视觉效果
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
        /// 点击列表项
        /// </summary>
        public void Click()
        {
            OnClicked?.Invoke(this);
        }
    }

    /// <summary>
    /// UI列表
    /// </summary>
    public class UIList : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private GameObject itemPrefab;

        [SerializeField]
        private float itemHeight = 50f;

        [SerializeField]
        private float itemSpacing = 5f;

        [SerializeField]
        private bool horizontal = false;

        [SerializeField]
        private bool canSelectMultiple = false;

        private List<UIListItem> items = new List<UIListItem>();
        private List<UIListItem> selectedItems = new List<UIListItem>();
        private List<object> dataList = new List<object>();

        /// <summary>
        /// 列表项选中事件
        /// </summary>
        public event Action<UIListItem> OnItemSelected;

        /// <summary>
        /// 列表项取消选中事件
        /// </summary>
        public event Action<UIListItem> OnItemDeselected;

        /// <summary>
        /// 列表项点击事件
        /// </summary>
        public event Action<UIListItem> OnItemClicked;

        /// <summary>
        /// 初始化列表
        /// </summary>
        public void Initialize()
        {
            if (scrollRect == null)
            {
                scrollRect = GetComponent<ScrollRect>();
            }

            if (content == null && scrollRect != null)
            {
                content = scrollRect.content;
            }

            ClearItems();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataList">数据列表</param>
        public void SetData<T>(IList<T> dataList) where T : class
        {
            ClearItems();

            if (dataList == null)
            {
                return;
            }

            foreach (T data in dataList)
            {
                AddItem(data);
            }
        }

        /// <summary>
        /// 添加项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns>列表项</returns>
        public UIListItem AddItem<T>(T data) where T : class
        {
            if (itemPrefab == null)
            {
                Debug.LogError("UIList: 未设置列表项预制体");
                return null;
            }

            // 创建列表项
            GameObject itemObj = Instantiate(itemPrefab, content);
            UIListItem item = itemObj.GetComponent<UIListItem>();

            if (item == null)
            {
                item = itemObj.AddComponent<UIListItem>();
            }

            // 设置列表项数据
            item.Index = items.Count;
            item.SetData(data);

            // 注册列表项事件
            item.OnSelected += OnItemSelectedHandler;
            item.OnDeselected += OnItemDeselectedHandler;
            item.OnClicked += OnItemClickedHandler;

            // 添加到列表
            items.Add(item);
            dataList.Add(data);

            // 更新布局
            UpdateLayout();

            return item;
        }

        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveItem(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                return;
            }

            // 取消选中
            if (items[index].IsSelected)
            {
                items[index].IsSelected = false;
            }

            // 移除事件
            items[index].OnSelected -= OnItemSelectedHandler;
            items[index].OnDeselected -= OnItemDeselectedHandler;
            items[index].OnClicked -= OnItemClickedHandler;

            // 销毁对象
            Destroy(items[index].gameObject);

            // 从列表中移除
            items.RemoveAt(index);
            dataList.RemoveAt(index);

            // 更新索引
            for (int i = index; i < items.Count; i++)
            {
                items[i].Index = i;
            }

            // 更新布局
            UpdateLayout();
        }

        /// <summary>
        /// 清除所有项
        /// </summary>
        public void ClearItems()
        {
            // 取消选中所有项
            DeselectAll();

            // 移除事件
            foreach (UIListItem item in items)
            {
                item.OnSelected -= OnItemSelectedHandler;
                item.OnDeselected -= OnItemDeselectedHandler;
                item.OnClicked -= OnItemClickedHandler;

                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }

            // 清空列表
            items.Clear();
            dataList.Clear();
        }

        /// <summary>
        /// 选中项
        /// </summary>
        /// <param name="index">索引</param>
        public void SelectItem(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                return;
            }

            if (!canSelectMultiple)
            {
                DeselectAll();
            }

            items[index].IsSelected = true;
        }

        /// <summary>
        /// 取消选中项
        /// </summary>
        /// <param name="index">索引</param>
        public void DeselectItem(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                return;
            }

            items[index].IsSelected = false;
        }

        /// <summary>
        /// 取消选中所有项
        /// </summary>
        public void DeselectAll()
        {
            foreach (UIListItem item in selectedItems)
            {
                if (item != null)
                {
                    item.IsSelected = false;
                }
            }

            selectedItems.Clear();
        }

        /// <summary>
        /// 获取选中的项
        /// </summary>
        /// <returns>选中的项列表</returns>
        public List<UIListItem> GetSelectedItems()
        {
            return new List<UIListItem>(selectedItems);
        }

        /// <summary>
        /// 获取选中的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>选中的数据列表</returns>
        public List<T> GetSelectedData<T>() where T : class
        {
            List<T> result = new List<T>();

            foreach (UIListItem item in selectedItems)
            {
                if (item != null && item.Data is T data)
                {
                    result.Add(data);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>项</returns>
        public UIListItem GetItem(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                return null;
            }

            return items[index];
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>数据</returns>
        public object GetData(int index)
        {
            if (index < 0 || index >= dataList.Count)
            {
                return null;
            }

            return dataList[index];
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="index">索引</param>
        /// <returns>数据</returns>
        public T GetData<T>(int index) where T : class
        {
            if (index < 0 || index >= dataList.Count)
            {
                return null;
            }

            return dataList[index] as T;
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        private void UpdateLayout()
        {
            if (content == null)
            {
                return;
            }

            float totalSize = 0;

            for (int i = 0; i < items.Count; i++)
            {
                RectTransform itemRect = items[i].GetComponent<RectTransform>();

                if (horizontal)
                {
                    itemRect.anchorMin = new Vector2(0, 0);
                    itemRect.anchorMax = new Vector2(0, 1);
                    itemRect.sizeDelta = new Vector2(itemHeight, 0);
                    itemRect.anchoredPosition = new Vector2(totalSize, 0);
                }
                else
                {
                    itemRect.anchorMin = new Vector2(0, 1);
                    itemRect.anchorMax = new Vector2(1, 1);
                    itemRect.sizeDelta = new Vector2(0, itemHeight);
                    itemRect.anchoredPosition = new Vector2(0, -totalSize);
                }

                totalSize += itemHeight + itemSpacing;
            }

            // 更新内容大小
            if (horizontal)
            {
                content.sizeDelta = new Vector2(totalSize - itemSpacing, 0);
            }
            else
            {
                content.sizeDelta = new Vector2(0, totalSize - itemSpacing);
            }
        }

        /// <summary>
        /// 选中项事件处理器
        /// </summary>
        private void OnItemSelectedHandler(UIListItem item)
        {
            if (!selectedItems.Contains(item))
            {
                selectedItems.Add(item);
            }

            OnItemSelected?.Invoke(item);
        }

        /// <summary>
        /// 取消选中项事件处理器
        /// </summary>
        private void OnItemDeselectedHandler(UIListItem item)
        {
            selectedItems.Remove(item);
            OnItemDeselected?.Invoke(item);
        }

        /// <summary>
        /// 点击项事件处理器
        /// </summary>
        private void OnItemClickedHandler(UIListItem item)
        {
            if (!canSelectMultiple)
            {
                DeselectAll();
                item.IsSelected = true;
            }
            else
            {
                item.IsSelected = !item.IsSelected;
            }

            OnItemClicked?.Invoke(item);
        }
    }
}
