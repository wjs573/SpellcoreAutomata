using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    /// <summary>
    /// Buff图标资源管理器
    /// 负责管理Buff id与对应Sprite的映射关系
    /// </summary>
    public class BuffIconManager : MonoBehaviour
    {
        private static BuffIconManager instance;
        public static BuffIconManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("BuffIconManager");
                    instance = go.AddComponent<BuffIconManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        // Buff图标字典，key为buff id，value为对应的Sprite
        private Dictionary<string, Sprite> buffIconDict = new Dictionary<string, Sprite>();

        /// <summary>
        /// 加载所有Buff图标
        /// 假设图标存储在Resources/BuffIcons文件夹下，文件名与buff id相同
        /// </summary>
        private void LoadAllBuffIcons()
        {
            if (buffIconDict.Count > 0) return;

            Sprite[] icons = Resources.LoadAll<Sprite>("BuffIcons");
            foreach (var icon in icons)
            {
                buffIconDict[icon.name] = icon;
            }
        }

        /// <summary>
        /// 根据buff id获取对应的图标
        /// </summary>
        /// <param name="buffId">Buff id</param>
        /// <returns>对应的Sprite，如果没有找到则返回null</returns>
        public Sprite GetBuffIcon(string buffId)
        {
            if (buffIconDict.Count == 0)
            {
                LoadAllBuffIcons();
            }

            if (buffIconDict.ContainsKey(buffId))
            {
                return buffIconDict[buffId];
            }
            return null;
        }
    }
}
