using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WJS
{
    public class UIWindow : MonoBehaviour
    {
        protected CanvasGroup canvasGroup; // 改为 protected 方便子类访问
        private Dictionary<string, UIEventListener> uiEventDIC;

        [Header("UIWindow Settings")]
        public bool visibleState;

        // 核心：引入过渡组件
        protected UITransition transition;

        protected void Awake()
        {
            // 确保有 CanvasGroup
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // 获取过渡组件（可以挂在同一个物体上）
            transition = GetComponent<UITransition>();

            uiEventDIC = new Dictionary<string, UIEventListener>();
        }

        // UIWindow.cs 核心调整
        public virtual void SetVisible(bool state, float delay = 0)
        {
            if (visibleState == state) return;
            visibleState = state;

            StopAllCoroutines(); // 必须停止旧的显隐协程
            StartCoroutine(HandleVisibilityWithTransition(state, delay));
        }

        private IEnumerator HandleVisibilityWithTransition(bool state, float delay)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);

            if (state) // 打开窗口
            {
                // 先把交互打开，防止动画过程中点不动
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;

                if (transition != null)
                { 
                    // 此时 transition 会接管 Alpha 和 Position 的修改
                    transition.PlayEnterTransition();
                }
                else
                {
                    // 兜底逻辑：如果没有动画组件，执行硬切换
                    canvasGroup.alpha = 1;
                }
            }
            else // 关闭窗口
            {
                // 立即关闭交互，防止退出动画时玩家误触窗口按钮
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;

                if (transition != null)
                {
                    // 播放退出动画，并利用回调在彻底隐藏
                    transition.PlayExitTransition();
                }
                else
                {
                    canvasGroup.alpha = 0;
                }
            }
        }

        // 保留原有的 Apply 方法，用于 UIManager 初始化时一键重置
        public void ApplyVisibleState()
        {
            StopAllCoroutines();
            canvasGroup.alpha = visibleState ? 1 : 0;
            canvasGroup.blocksRaycasts = visibleState;
            if (transition != null) transition.ResetToOriginalState();
        }

        public UIEventListener GetUIEventListener(string name)
        {
            if (!uiEventDIC.ContainsKey(name))
            {
                Transform tf = transform.FindChildByName(name);
                if (tf == null) return null;
                UIEventListener uiEvent = UIEventListener.GetListener(tf);
                uiEventDIC.Add(name, uiEvent);
            }
            return uiEventDIC[name];
        }
    }
}