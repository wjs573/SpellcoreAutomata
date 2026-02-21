
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    /// <summary>
    /// UI预制体加载器
    /// 用于加载和实例化UI预制体
    /// </summary>
    public class UIPrefabLoader : MonoSingleton<UIPrefabLoader>
    {
        [System.Serializable]
        public class UIPrefabInfo
        {
            public string name;
            public GameObject prefab;
            public UILayer layer;
        }

        [SerializeField]
        private List<UIPrefabInfo> uiPrefabs = new List<UIPrefabInfo>();

        /// <summary>
        /// 已加载的UI实例
        /// </summary>
        private Dictionary<string, GameObject> loadedUIs = new Dictionary<string, GameObject>();

        /// <summary>
        /// 添加UI预制体信息
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <param name="prefab">预制体</param>
        /// <param name="layer">UI层级</param>
        public void AddUIPrefab(string name, GameObject prefab, UILayer layer = UILayer.Normal)
        {
            if (string.IsNullOrEmpty(name) || prefab == null)
            {
                Debug.LogError("UIPrefabLoader: 无效的UI预制体信息");
                return;
            }

            // 检查是否已存在
            for (int i = 0; i < uiPrefabs.Count; i++)
            {
                if (uiPrefabs[i].name == name)
                {
                    uiPrefabs[i] = new UIPrefabInfo { name = name, prefab = prefab, layer = layer };
                    return;
                }
            }

            // 添加新的预制体信息
            uiPrefabs.Add(new UIPrefabInfo { name = name, prefab = prefab, layer = layer });
        }

        /// <summary>
        /// 加载UI
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <returns>UI实例</returns>
        public GameObject LoadUI(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("UIPrefabLoader: UI名称为空");
                return null;
            }

            // 检查是否已加载
            if (loadedUIs.ContainsKey(name))
            {
                return loadedUIs[name];
            }

            // 查找预制体信息
            UIPrefabInfo prefabInfo = null;
            for (int i = 0; i < uiPrefabs.Count; i++)
            {
                if (uiPrefabs[i].name == name)
                {
                    prefabInfo = uiPrefabs[i];
                    break;
                }
            }

            if (prefabInfo == null)
            {
                Debug.LogError($"UIPrefabLoader: 未找到名为 {name} 的UI预制体");
                return null;
            }

            // 获取目标层级
            Transform parent = UILayerManager.Instance.GetLayer(prefabInfo.layer);
            if (parent == null)
            {
                Debug.LogError($"UIPrefabLoader: 未找到层级 {prefabInfo.layer}");
                return null;
            }

            // 实例化UI
            GameObject uiInstance = Instantiate(prefabInfo.prefab, parent);
            uiInstance.name = name;

            // 添加到已加载字典
            loadedUIs.Add(name, uiInstance);

            return uiInstance;
        }

        /// <summary>
        /// 卸载UI
        /// </summary>
        /// <param name="name">UI名称</param>
        public void UnloadUI(string name)
        {
            if (string.IsNullOrEmpty(name) || !loadedUIs.ContainsKey(name))
            {
                return;
            }

            Destroy(loadedUIs[name]);
            loadedUIs.Remove(name);
        }

        /// <summary>
        /// 卸载所有UI
        /// </summary>
        public void UnloadAllUI()
        {
            foreach (var pair in loadedUIs)
            {
                if (pair.Value != null)
                {
                    Destroy(pair.Value);
                }
            }

            loadedUIs.Clear();
        }

        /// <summary>
        /// 获取UI实例
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <returns>UI实例</returns>
        public GameObject GetUI(string name)
        {
            if (string.IsNullOrEmpty(name) || !loadedUIs.ContainsKey(name))
            {
                return null;
            }

            return loadedUIs[name];
        }

        /// <summary>
        /// 检查UI是否已加载
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <returns>是否已加载</returns>
        public bool IsUILoaded(string name)
        {
            return !string.IsNullOrEmpty(name) && loadedUIs.ContainsKey(name);
        }
    }
}
