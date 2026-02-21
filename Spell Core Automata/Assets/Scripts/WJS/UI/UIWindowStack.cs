
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WJS
{
    /// <summary>
    /// UI窗口堆栈管理器
    /// 用于管理UI窗口的打开和关闭顺序
    /// </summary>
    public class UIWindowStack : MonoSingleton<UIWindowStack>
    {
        /// <summary>
        /// 窗口堆栈
        /// </summary>
        [ShowInInspector]
        private Stack<UIWindow> windowStack = new Stack<UIWindow>();

        /// <summary>
        /// 当前窗口
        /// </summary>
        public UIWindow CurrentWindow
        {
            get
            {
                return windowStack.Count > 0 ? windowStack.Peek() : null;
            }
        }

        /// <summary>
        /// 窗口数量
        /// </summary>
        public int Count
        {
            get { return windowStack.Count; }
        }

        /// <summary>
        /// 推入窗口到堆栈
        /// </summary>
        /// <param name="window">要推入的窗口</param>
        public void PushWindow(string windowName)
        {
            UIWindow window = UIManager.Instance.GetWindow(windowName);
            if (window == null)
            {
                Debug.LogError("UIWindowStack: 尝试推入空窗口");
                return;
            }

            // 如果窗口已经在堆栈中，先移除
            if (windowStack.Contains(window))
            {
                RemoveWindow(window);
            }

            // 推入新窗口
            windowStack.Push(window);

            // 显示窗口
            window.SetVisible(true);

            // 更新窗口层级
            UpdateWindowLayers();
        }

        /// <summary>
        /// 弹出堆栈顶部的窗口
        /// </summary>
        public void PopWindow()
        {
            if (windowStack.Count > 0)
            {
                UIWindow window = windowStack.Pop();
                window.SetVisible(false);

                // 更新窗口层级
                UpdateWindowLayers();
            }
        }

        /// <summary>
        /// 移除指定窗口
        /// </summary>
        /// <param name="window">要移除的窗口</param>
        public void RemoveWindow(string windowName)
        {
            UIWindow window = UIManager.Instance.GetWindow(windowName);
            if (window == null || !windowStack.Contains(window))
            {
                return;
            }

            // 创建临时堆栈
            Stack<UIWindow> tempStack = new Stack<UIWindow>();

            // 弹出所有窗口直到找到目标窗口
            while (windowStack.Count > 0)
            {
                UIWindow topWindow = windowStack.Pop();
                if (topWindow == window)
                {
                    break;
                }
                tempStack.Push(topWindow);
                topWindow.SetVisible(false);
            }

            // 将临时堆栈中的窗口推回原堆栈
            while (tempStack.Count > 0)
            {
                windowStack.Push(tempStack.Pop());
            }

            // 更新窗口层级
            UpdateWindowLayers();
        }

        public void RemoveWindow(UIWindow window)
        {
            if (window == null || !windowStack.Contains(window))
            {
                return;
            }

            // 创建临时堆栈
            Stack<UIWindow> tempStack = new Stack<UIWindow>();

            // 弹出所有窗口直到找到目标窗口
            while (windowStack.Count > 0)
            {
                UIWindow topWindow = windowStack.Pop();
                if (topWindow == window)
                {
                    break;
                }
                tempStack.Push(topWindow);
            }

            // 将临时堆栈中的窗口推回原堆栈
            while (tempStack.Count > 0)
            {
                windowStack.Push(tempStack.Pop());
            }

            // 更新窗口层级
            UpdateWindowLayers();
        }

        /// <summary>
        /// 清空窗口堆栈
        /// </summary>
        public void Clear()
        {
            foreach (UIWindow window in windowStack)
            {
                window.SetVisible(false);
            }

            windowStack.Clear();
        }

        /// <summary>
        /// 更新窗口层级
        /// </summary>
        private void UpdateWindowLayers()
        {
            int index = 0;
            foreach (UIWindow window in windowStack)
            {
                CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    // 根据在堆栈中的位置设置排序顺序
                    canvasGroup.interactable = (index == windowStack.Count - 1);
                }
                index++;
            }
        }

        /// <summary>
        /// 获取所有窗口
        /// </summary>
        public UIWindow[] GetAllWindows()
        {
            return windowStack.ToArray();
        }

        /// <summary>
        /// 切换窗口状态：如果在栈顶则关闭，否则打开并推入栈顶
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public void ToggleWindow(string windowName)
        {
            UIWindow targetWindow = UIManager.Instance.GetWindow(windowName);
            if (targetWindow == null) return;

            // 情况 A：目标窗口就在栈顶 -> 关闭它（Pop）
            if (CurrentWindow == targetWindow)
            {
                PopWindow();
            }
            // 情况 B：目标窗口不在栈顶 -> 打开它（Push）
            else
            {
                // 推入堆栈处理层级逻辑
                PushWindow(windowName);
            }
        }
    }
}
