
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WJS
{
    /// <summary>
    /// UI事件系统
    /// 用于管理UI事件和事件分发
    /// </summary>
    public class UIEventSystem : MonoSingleton<UIEventSystem>
    {
        /// <summary>
        /// UI事件类型
        /// </summary>
        public enum UIEventType
        {
            /// <summary>
            /// 点击事件
            /// </summary>
            Click,

            /// <summary>
            /// 按下事件
            /// </summary>
            PointerDown,

            /// <summary>
            /// 抬起事件
            /// </summary>
            PointerUp,

            /// <summary>
            /// 进入事件
            /// </summary>
            PointerEnter,

            /// <summary>
            /// 退出事件
            /// </summary>
            PointerExit,

            /// <summary>
            /// 拖拽开始事件
            /// </summary>
            DragBegin,

            /// <summary>
            /// 拖拽中事件
            /// </summary>
            Drag,

            /// <summary>
            /// 拖拽结束事件
            /// </summary>
            DragEnd,

            /// <summary>
            /// 滚动事件
            /// </summary>
            Scroll,

            /// <summary>
            /// 选择事件
            /// </summary>
            Select,

            /// <summary>
            /// 取消选择事件
            /// </summary>
            Deselect,

            /// <summary>
            /// 提交事件
            /// </summary>
            Submit,

            /// <summary>
            /// 取消事件
            /// </summary>
            Cancel
        }

        /// <summary>
        /// UI事件数据
        /// </summary>
        public class UIEventData
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            public UIEventType EventType;

            /// <summary>
            /// 事件源
            /// </summary>
            public GameObject Source;

            /// <summary>
            /// 事件数据
            /// </summary>
            public BaseEventData EventData;

            /// <summary>
            /// 自定义数据
            /// </summary>
            public object CustomData;
        }

        /// <summary>
        /// UI事件委托
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public delegate void UIEventHandler(UIEventData eventData);

        /// <summary>
        /// 事件字典
        /// </summary>
        private Dictionary<UIEventType, List<UIEventHandler>> eventHandlers = new Dictionary<UIEventType, List<UIEventHandler>>();

        /// <summary>
        /// 命名空间事件字典
        /// </summary>
        private Dictionary<string, Dictionary<UIEventType, List<UIEventHandler>>> namespaceEventHandlers = new Dictionary<string, Dictionary<UIEventType, List<UIEventHandler>>>();

        protected void Awake()
        {
            InitEventSystem();
        }

        private void InitEventSystem()
        {
            // 初始化事件字典
            foreach (UIEventType eventType in Enum.GetValues(typeof(UIEventType)))
            {
                eventHandlers[eventType] = new List<UIEventHandler>();
            }
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理器</param>
        public void RegisterEventHandler(UIEventType eventType, UIEventHandler handler)
        {
            if (handler == null)
            {
                return;
            }

            if (!eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType] = new List<UIEventHandler>();
            }

            if (!eventHandlers[eventType].Contains(handler))
            {
                eventHandlers[eventType].Add(handler);
            }
        }

        /// <summary>
        /// 注销事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理器</param>
        public void UnregisterEventHandler(UIEventType eventType, UIEventHandler handler)
        {
            if (handler == null || !eventHandlers.ContainsKey(eventType))
            {
                return;
            }

            eventHandlers[eventType].Remove(handler);
        }

        /// <summary>
        /// 注册命名空间事件处理器
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理器</param>
        public void RegisterNamespaceEventHandler(string @namespace, UIEventType eventType, UIEventHandler handler)
        {
            if (string.IsNullOrEmpty(@namespace) || handler == null)
            {
                return;
            }

            if (!namespaceEventHandlers.ContainsKey(@namespace))
            {
                namespaceEventHandlers[@namespace] = new Dictionary<UIEventType, List<UIEventHandler>>();
            }

            if (!namespaceEventHandlers[@namespace].ContainsKey(eventType))
            {
                namespaceEventHandlers[@namespace][eventType] = new List<UIEventHandler>();
            }

            if (!namespaceEventHandlers[@namespace][eventType].Contains(handler))
            {
                namespaceEventHandlers[@namespace][eventType].Add(handler);
            }
        }

        /// <summary>
        /// 注销命名空间事件处理器
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理器</param>
        public void UnregisterNamespaceEventHandler(string @namespace, UIEventType eventType, UIEventHandler handler)
        {
            if (string.IsNullOrEmpty(@namespace) || handler == null)
            {
                return;
            }

            if (!namespaceEventHandlers.ContainsKey(@namespace))
            {
                return;
            }

            if (!namespaceEventHandlers[@namespace].ContainsKey(eventType))
            {
                return;
            }

            namespaceEventHandlers[@namespace][eventType].Remove(handler);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="source">事件源</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="customData">自定义数据</param>
        public void TriggerEvent(UIEventType eventType, GameObject source, BaseEventData eventData = null, object customData = null)
        {
            if (!eventHandlers.ContainsKey(eventType))
            {
                return;
            }

            UIEventData uiEventData = new UIEventData
            {
                EventType = eventType,
                Source = source,
                EventData = eventData,
                CustomData = customData
            };

            // 触发全局事件
            foreach (UIEventHandler handler in eventHandlers[eventType])
            {
                handler?.Invoke(uiEventData);
            }

            // 触发命名空间事件
            foreach (var namespacePair in namespaceEventHandlers)
            {
                if (namespacePair.Value.ContainsKey(eventType))
                {
                    foreach (UIEventHandler handler in namespacePair.Value[eventType])
                    {
                        handler?.Invoke(uiEventData);
                    }
                }
            }
        }

        /// <summary>
        /// 触发命名空间事件
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="source">事件源</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="customData">自定义数据</param>
        public void TriggerNamespaceEvent(string @namespace, UIEventType eventType, GameObject source, BaseEventData eventData = null, object customData = null)
        {
            if (string.IsNullOrEmpty(@namespace) || !namespaceEventHandlers.ContainsKey(@namespace))
            {
                return;
            }

            if (!namespaceEventHandlers[@namespace].ContainsKey(eventType))
            {
                return;
            }

            UIEventData uiEventData = new UIEventData
            {
                EventType = eventType,
                Source = source,
                EventData = eventData,
                CustomData = customData
            };

            foreach (UIEventHandler handler in namespaceEventHandlers[@namespace][eventType])
            {
                handler?.Invoke(uiEventData);
            }
        }

        /// <summary>
        /// 清除所有事件处理器
        /// </summary>
        public void ClearAllEventHandlers()
        {
            foreach (var eventType in eventHandlers.Keys)
            {
                eventHandlers[eventType].Clear();
            }

            foreach (var namespacePair in namespaceEventHandlers)
            {
                foreach (var eventType in namespacePair.Value.Keys)
                {
                    namespacePair.Value[eventType].Clear();
                }
            }
        }

        /// <summary>
        /// 清除命名空间事件处理器
        /// </summary>
        /// <param name="namespace">命名空间</param>
        public void ClearNamespaceEventHandlers(string @namespace)
        {
            if (string.IsNullOrEmpty(@namespace) || !namespaceEventHandlers.ContainsKey(@namespace))
            {
                return;
            }

            foreach (var eventType in namespaceEventHandlers[@namespace].Keys)
            {
                namespaceEventHandlers[@namespace][eventType].Clear();
            }
        }
    }
}
