using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public Dictionary<string, UIWindow> uiWindowDIC;

        /// <summary>
        /// 临时显示在屏幕上的按钮
        /// 当玩家打开一个新的临时按钮
        /// 之前打开的应该隐藏
        /// </summary>
        public List<UIEventListener> TempUIEventListeners;

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
                //Debug.Log(uiWindowsArr[i].GetComponent<CanvasGroup>());

                CanvasGroup canvasGroup = uiWindowsArr[i].GetComponent<CanvasGroup>();

                uiWindowsArr[i].SetVisible(false);
                AddWindow(uiWindowsArr[i]);
            }
        }

        //关闭所有窗口
        public void CloseAllWindow()
        {
            foreach (string uiWindow in uiWindowDIC.Keys)
            {
                uiWindowDIC[uiWindow].SetVisible(false);
            }
        }

        internal UIWindow GetWindow(string v)
        {
            throw new NotImplementedException();
        }

        //输入id 打开窗口 并关闭其他窗口
        public void OpenWindow<T>() where T : UIWindow
        {
            CloseAllWindow();
            GetWindow<T>().SetVisible(true);
        }

        //添加窗口  动态创建
        public void AddWindow(UIWindow window)
        {
            uiWindowDIC.Add(window.GetType().Name, window);
        }
        
        //移除TempUIEventListeners的所有元素，然后添加
        public void UpdateTempUIEventlisteners(UIEventListener eventListener)
        {
            foreach (UIEventListener tempUIEventListener in TempUIEventListeners){
                //TempUIEventListeners.
            }
            
        }
    }
}
