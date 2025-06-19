using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WJS
{
    ///<summary>
    ///在一个gameObject下添加这个，让这个gameObject成为一个“绑点”，这样就可以在这东西里面管理一些挂载的gameObject
    ///最常见的用途是角色身上某个点播放视觉特效什么的。
    ///</summary>
    ///<summary>
    ///在一个gameObject下添加这个，让这个gameObject成为一个“绑点”
    ///</summary>
    public class UnitBindPoint : MonoBehaviour
    {
        ///<summary>
        ///绑点的名称
        ///</summary>
        public string key;

        ///<summary>
        ///偏移坐标
        ///</summary>
        public Vector3 offset;

        ///<summary>
        ///已经挂着的gameobject信息
        ///</summary>
        private Dictionary<string, BindGameObjectInfo> bindGameObject = new Dictionary<string, BindGameObjectInfo>();

        ///<summary>
        ///是否为临时创建的绑点
        ///</summary>
        public bool isTemporary = false;

        private void FixedUpdate()
        {
            List<string> toRemove = new List<string>();
            foreach (KeyValuePair<string, BindGameObjectInfo> goInfo in bindGameObject)
            {
                if (goInfo.Value.gameObject == null)
                {
                    toRemove.Add(goInfo.Key);
                    continue;
                }
                if (!goInfo.Value.forever)
                {
                    goInfo.Value.duration -= Time.fixedDeltaTime;
                    if (goInfo.Value.duration <= 0)
                    {
                        Destroy(goInfo.Value.gameObject);
                        toRemove.Add(goInfo.Key);
                    }
                }
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                bindGameObject.Remove(toRemove[i]);
            }

            // 如果没有挂载的对象，并且是临时绑点，则销毁自己
            if (bindGameObject.Count == 0 && isTemporary)
            {
                Destroy(this.gameObject);
            }
        }

        ///<summary>
        ///添加一个gameObject绑定
        ///</summary>
        public void AddBindGameObject(string goPath, string key, bool loop)
        {
            if (!string.IsNullOrEmpty(key) && bindGameObject.ContainsKey(key)) return;

            GameObject prefab = Resources.Load<GameObject>(goPath);
            if (prefab == null) return;

            GameObject effectGO = Instantiate(prefab, Vector3.zero, Quaternion.identity, this.gameObject.transform);
            effectGO.transform.localPosition = this.offset;
            effectGO.transform.localRotation = Quaternion.identity;

            if (effectGO == null) return;
            SightEffect se = effectGO.GetComponent<SightEffect>();
            if (se == null)
            {
                Destroy(effectGO);
                return;
            }

            float duration = se.duration * (loop ? -1 : 1);
            BindGameObjectInfo bindGameObjectInfo = new BindGameObjectInfo(effectGO, duration);

            if (!string.IsNullOrEmpty(key))
            {
                this.bindGameObject.Add(key, bindGameObjectInfo);
            }
            else
            {
                string randomKey = $"{Time.frameCount}_{Random.Range(1, 10000)}";
                this.bindGameObject.Add(randomKey, bindGameObjectInfo);
            }
        }

        ///<summary>
        ///移除一个gameObject的绑定
        ///</summary>
        public void RemoveBindGameObject(string key)
        {
            if (!bindGameObject.ContainsKey(key)) return;
            if (bindGameObject[key].gameObject != null)
            {
                Destroy(bindGameObject[key].gameObject);
            }
            bindGameObject.Remove(key);
        }
    }


    ///<summary>
    ///被挂载的gameobject的记录
    ///</summary>
    public class BindGameObjectInfo
    {
        ///<summary>
        ///gameObject的地址
        ///</summary>
        public GameObject gameObject;

        ///<summary>
        ///还有多少时间之后被销毁，单位：秒
        ///</summary>
        public float duration;

        ///<summary>
        ///有些是不能被销毁的，得外部控制销毁，所以永久存在
        ///</summary>
        public bool forever;

        ///<summary>
        ///<param name="gameObject">要挂载的gameObject</param>
        ///<param name="duration">挂的时间，时间到了销毁，[Magic]如果<=0则代表永久</param>
        ///</summary>
        public BindGameObjectInfo(GameObject gameObject, float duration)
        {
            this.gameObject = gameObject;
            this.duration = Mathf.Abs(duration);
            this.forever = duration <= 0;
        }
    }
}
