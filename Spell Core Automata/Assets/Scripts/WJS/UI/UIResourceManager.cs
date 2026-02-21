
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    /// <summary>
    /// UI资源管理器
    /// 用于管理UI资源的加载和释放
    /// </summary>
    public class UIResourceManager : MonoSingleton<UIResourceManager>
    {
        [System.Serializable]
        public class UIResourceInfo
        {
            public string name;
            public string path;
            public Type type;
            public UnityEngine.Object resource;
            public int referenceCount;
        }

        [SerializeField]
        private List<UIResourceInfo> uiResources = new List<UIResourceInfo>();

        private Dictionary<string, UIResourceInfo> resourceDict = new Dictionary<string, UIResourceInfo>();


        protected void Awake()
        {
            InitResourceManager();
        }

        private void InitResourceManager()
        {
            // 初始化资源字典
            foreach (UIResourceInfo resourceInfo in uiResources)
            {
                if (!string.IsNullOrEmpty(resourceInfo.name) && !resourceDict.ContainsKey(resourceInfo.name))
                {
                    resourceDict.Add(resourceInfo.name, resourceInfo);
                }
            }
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        public void AddResource(string name, string path, Type type)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path) || type == null)
            {
                Debug.LogError("UIResourceManager: 无效的资源信息");
                return;
            }

            // 检查是否已存在
            if (resourceDict.ContainsKey(name))
            {
                Debug.LogWarning($"UIResourceManager: 资源 {name} 已存在");
                return;
            }

            // 创建资源信息
            UIResourceInfo resourceInfo = new UIResourceInfo
            {
                name = name,
                path = path,
                type = type,
                resource = null,
                referenceCount = 0
            };

            // 添加到列表和字典
            uiResources.Add(resourceInfo);
            resourceDict.Add(name, resourceInfo);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <returns>资源</returns>
        public T LoadResource<T>(string name) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(name) || !resourceDict.ContainsKey(name))
            {
                Debug.LogError($"UIResourceManager: 未找到资源 {name}");
                return null;
            }

            UIResourceInfo resourceInfo = resourceDict[name];

            // 如果资源未加载，则加载
            if (resourceInfo.resource == null)
            {
                resourceInfo.resource = Resources.Load(resourceInfo.path, resourceInfo.type);

                if (resourceInfo.resource == null)
                {
                    Debug.LogError($"UIResourceManager: 无法加载资源 {name} 从路径 {resourceInfo.path}");
                    return null;
                }
            }

            // 增加引用计数
            resourceInfo.referenceCount++;

            return resourceInfo.resource as T;
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="name">资源名称</param>
        public void UnloadResource(string name)
        {
            if (string.IsNullOrEmpty(name) || !resourceDict.ContainsKey(name))
            {
                return;
            }

            UIResourceInfo resourceInfo = resourceDict[name];

            // 减少引用计数
            if (resourceInfo.referenceCount > 0)
            {
                resourceInfo.referenceCount--;
            }

            // 如果引用计数为0，卸载资源
            if (resourceInfo.referenceCount == 0 && resourceInfo.resource != null)
            {
                Resources.UnloadAsset(resourceInfo.resource);
                resourceInfo.resource = null;
            }
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="names">资源名称列表</param>
        public void PreloadResources(List<string> names)
        {
            if (names == null || names.Count == 0)
            {
                return;
            }

            foreach (string name in names)
            {
                if (!string.IsNullOrEmpty(name) && resourceDict.ContainsKey(name))
                {
                    UIResourceInfo resourceInfo = resourceDict[name];

                    // 如果资源未加载，则加载
                    if (resourceInfo.resource == null)
                    {
                        resourceInfo.resource = Resources.Load(resourceInfo.path, resourceInfo.type);
                    }
                }
            }
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <returns>资源</returns>
        public T GetResource<T>(string name) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(name) || !resourceDict.ContainsKey(name))
            {
                return null;
            }

            UIResourceInfo resourceInfo = resourceDict[name];
            return resourceInfo.resource as T;
        }

        /// <summary>
        /// 检查资源是否已加载
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns>是否已加载</returns>
        public bool IsResourceLoaded(string name)
        {
            if (string.IsNullOrEmpty(name) || !resourceDict.ContainsKey(name))
            {
                return false;
            }

            return resourceDict[name].resource != null;
        }

        /// <summary>
        /// 获取资源引用计数
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns>引用计数</returns>
        public int GetResourceReferenceCount(string name)
        {
            if (string.IsNullOrEmpty(name) || !resourceDict.ContainsKey(name))
            {
                return 0;
            }

            return resourceDict[name].referenceCount;
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        public void UnloadAllResources()
        {
            foreach (UIResourceInfo resourceInfo in uiResources)
            {
                if (resourceInfo.resource != null)
                {
                    Resources.UnloadAsset(resourceInfo.resource);
                    resourceInfo.resource = null;
                }

                resourceInfo.referenceCount = 0;
            }
        }

        /// <summary>
        /// 清除未使用的资源
        /// </summary>
        public void ClearUnusedResources()
        {
            foreach (UIResourceInfo resourceInfo in uiResources)
            {
                if (resourceInfo.referenceCount == 0 && resourceInfo.resource != null)
                {
                    Resources.UnloadAsset(resourceInfo.resource);
                    resourceInfo.resource = null;
                }
            }
        }
    }
}
