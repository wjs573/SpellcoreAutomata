using UnityEngine;
using System;

namespace WJS
{
    /// <summary>
    /// 专门负责监听快捷键并控制UI开关的控制器
    /// </summary>
    public class UIInputController : MonoBehaviour
    {
        [System.Serializable]
        public struct KeyWindowBinding
        {
            public KeyCode keyCode;
            public string windowName; // 对应 UIManager 中的窗口 ID 或名称
        }

        [Header("快捷键绑定设置")]
        [SerializeField] private KeyWindowBinding[] bindings;

        private void Update()
        {
            // 遍历所有绑定的按键
            foreach (var binding in bindings)
            {
                if (Input.GetKeyDown(binding.keyCode))
                {
                    HandleWindowToggle(binding.windowName);
                }
            }

            // 全局关闭逻辑
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTopWindow();
            }
        }

        private void HandleWindowToggle(string name)
        {
            if (UIWindowStack.Instance != null)
            {
                UIWindowStack.Instance.ToggleWindow(name);
                
                // 作为美术，我建议在这里触发一个全局的 UI 点击音效
                // AudioSource.PlayClipAtPoint(uiOpenSound, Camera.main.transform.position);
            }
        }

        private void CloseTopWindow()
        {
            // ESC 优先关闭当前打开的最上层窗口
            UIWindowStack.Instance?.PopWindow();
        }
    }
}