using UnityEngine;

namespace JinShan
{
    public static class TransformerHelper
    {
        /// <summary>
        /// 通过字符串找到子物体
        /// </summary>
        /// <param name="currentTf">父物体</param>
        /// <param name="name">字符串</param>
        /// <returns></returns>
        public static Transform FindChildByName(this Transform currentTF, string name)
        {
            Transform childTF = currentTF.Find(name);
            if (childTF != null) return childTF;
            for (int i = 0; i < currentTF.childCount; i++)
            {
                childTF = FindChildByName(currentTF.GetChild(i), name);
                if (childTF != null) return childTF;
            }
            return null;
        }

        /// <summary>
        /// 移除所有子物体
        /// </summary>
        /// <param name="parent"></param>
        public static void RemoveAllChildren(GameObject parent)
        {
            if (parent == null) // 添加空引用检查
            {
                Debug.LogError("Parent GameObject is null in RemoveAllChildren.");
                return;
            }
            Transform transform;

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                transform = parent.transform.GetChild(i);

                //如果是美术载体 就移除它的子物体
                if (transform.name == "ViewContainer")
                {
                    TransformerHelper.RemoveAllChildren(transform.gameObject);
                    continue;
                }
                GameObject.Destroy(transform.gameObject);
            }
        }
    }
}