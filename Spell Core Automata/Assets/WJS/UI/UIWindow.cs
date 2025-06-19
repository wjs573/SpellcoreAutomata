using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WJS
{
    public class UIWindow : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private Dictionary<string, UIEventListener> uiEventDIC;
        [Header("UIWindow")]
        public bool visibleState;
        public GameObject UIExitButton;

        private void Awake()
        {
            if (this.GetComponent<CanvasGroup>() == null)
            {
                gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup = GetComponent<CanvasGroup>();

            uiEventDIC = new Dictionary<string, UIEventListener>();

            if (UIExitButton != null)
            {
                if (UIExitButton.GetComponent<UIEventListener>() == null)
                {
                    UIExitButton.AddComponent<UIEventListener>();
                }
                UIExitButton.GetComponent<UIEventListener>().PointerClick += OnExitButtonClick;
            }
        }

        private void OnExitButtonClick(PointerEventData eventData)
        {
            SetVisible();
        }

        public void SetVisible(float delay = 0)
        {
            StartCoroutine(SetVisibleDelay(!visibleState, delay));
            visibleState = !visibleState;
        }

        public void SetVisible(bool state, float delay = 0)
        {
            //Debug.Log(gameObject);
            StartCoroutine(SetVisibleDelay(state, delay));
            visibleState = false;
        }

        private IEnumerator SetVisibleDelay(bool state, float delay)
        {
            yield return new WaitForSeconds(delay);
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = state ? 1 : 0;
            //根据面板状态 设置面板是否影响光线检测
            canvasGroup.blocksRaycasts = state;
        }

        public void ApplyVisibleState()
        {
            canvasGroup.alpha = visibleState ? 1 : 0;
            canvasGroup.blocksRaycasts = visibleState;
        }

        //根据名称获取ui监听器
        public UIEventListener GetUIEventListener(string name)
        {
            if (!uiEventDIC.ContainsKey(name))
            {
                Transform tf = transform.FindChildByName(name);
                UIEventListener uiEvent = UIEventListener.GetListener(tf);
                uiEventDIC.Add(name, uiEvent);
            };
            return uiEventDIC[name];
        }
    }
}
