using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WJS
{
    public class UIManager : MonoSingleton<UIManager>
    {

        [ShowInInspector]
        public Dictionary<string, UIWindow> uiWindowDIC;

        [ContextMenu("Update UI Visibility")]
        public void UpdateUIVisibility()
        {
            foreach (UIWindow window in uiWindowDIC.Values)
            {
                window.ApplyVisibleState();
            }
        }

        //获取窗口
        public T GetWindow<T>() where T : class
        {
            string key = typeof(T).Name;

            if (uiWindowDIC.ContainsKey(key))
            {
                return uiWindowDIC[key] as T;
            }
            return null;
        }

        //初始化 隐藏并记录所有窗口
        public override void Init()
        {
            base.Init();
            uiWindowDIC = new Dictionary<string, UIWindow>();
            UIWindow[] uiWindowsArr = FindObjectsOfType<UIWindow>();

            for (int i = 0; i < uiWindowsArr.Length; i++)
            {
                CanvasGroup canvasGroup = uiWindowsArr[i].GetComponent<CanvasGroup>();
                uiWindowsArr[i].SetVisible(false);
                AddWindow(uiWindowsArr[i]);
            }
        }

        //添加窗口  动态创建
        public void AddWindow(UIWindow window)
        {
            uiWindowDIC.Add(window.GetType().Name, window);
        }

        // 1. 修复字符串版本的获取方法
        public UIWindow GetWindow(string windowName)
        {
            if (uiWindowDIC.TryGetValue(windowName, out UIWindow window))
            {
                return window;
            }
            Debug.LogWarning($"UIManager: 找不到名为 {windowName} 的窗口！请检查字典中是否存在该键。");
            return null;
        }

        // 2. 优化 OpenWindow 以配合 Stack 使用 (可选)
        public void OpenWindowByStack(string windowName)
        {
            // 不再直接 CloseAll，而是交给 UIWindowStack 处理
            UIWindowStack.Instance.PushWindow(windowName);
            GetWindow(windowName).SetVisible(true);
        }

    }
}
