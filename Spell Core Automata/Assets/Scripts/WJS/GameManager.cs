using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WJS
{
    public class GameManager : MonoSingleton<GameManager>
    {
        //对象池组件
        public MMMultipleObjectPooler ObjectPooler;

        //总有一个角色是主角，也就是玩家控制的，并且镜头跟随的
        public GameObject mainCharacter;

        public Vector3 MousePositionOnXOZPlane
        {
            get
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置发出射线
                Vector3 pointOnXOZ = Vector3.zero;
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    // 如果射线与某个物体碰撞（例如地面），则hit.point就是碰撞点
                    pointOnXOZ = hit.point;
                    pointOnXOZ.y = 0; // 将Y设置为0，确保点在XOZ平面上

                    // 使用pointOnXOZ根据需要执行操作
                }
                return pointOnXOZ;
            }
        }

        [ShowInInspector]
        private bool isPaused = false;

        public void TogglePause()
        {
            isPaused = !isPaused;
            SetPause(isPaused);
        }

        public void SetPause(bool pause)
        {
            isPaused = pause;
            Time.timeScale = isPaused ? 0f : 1f; // 将时间流逝速度设为0以暂停，设为1以继续
        }


        //已登场的角色，包括主角、敌人
        [ShowInInspector]
        private List<GameObject> characters;
        public void ResetCharacters()
        {
            characters = new List<GameObject>();
        }

        /// <summary>
        /// 获取所有character
        /// </summary>
        public List<GameObject> Characters
        {
            get
            {
                if (characters == null)
                {
                    return null;
                }
                List<GameObject> _characters = new List<GameObject>();
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i] != null && characters[i].GetComponent<ChaState>())
                    {
                        _characters.Add(characters[i]);
                    }
                }

                return _characters;
            }
        }

        //所有的gameobject放置的地方
        public GameObject root;

        //特效管理器
        private Dictionary<string, GameObject> sightEffect = new Dictionary<string, GameObject>();

        private void Awake()
        {
            TimelineScripts.Initialize();
            BulletData.Initialize();
            AoEData.Initialize();
            TimelineData.Initialize();
        
            BuffData.Initialize();
            SkillData.Initialize();
            RuneData.Initialize();
            EquipmentData.Initialize();
            EnemyData.Initialize();
        }

        private void FixedUpdate()
        {
            //战斗场景的逻辑
            if (BattleManager.Instance.IsInBattle)
            {
                //管理一下视觉特效，看哪些需要清楚了
                List<string> toRemoveKey = new List<string>();
                foreach (KeyValuePair<string, GameObject> se in sightEffect)
                {
                    if (se.Value == null) toRemoveKey.Add(se.Key);
                }
                for (int i = 0; i < toRemoveKey.Count; i++) sightEffect.Remove(toRemoveKey[i]);
                toRemoveKey = null;
            }
        }

        /// <summary>
        /// 清除战斗场景
        /// </summary>
        public void ClearBattle()
        {
            //杀死所有character 包括法宝、召唤物
            for (int i = 0; i < characters.Count; i++)
            {
                if (!characters[i])
                {
                    continue;
                }
                characters[i].GetComponent<ChaState>().Kill();
            }
            //清除bullet
            BulletManager.Instance.ClearBullets();

            //清除aoe
            AoeManager.Instance.ClearAoEs();

            //清除effect
            foreach (string key in sightEffect.Keys)
            {
                RemoveSightEffect(key);
            }
        }

        //根据prefab下的资源创建东西
        private GameObject CreateFromPrefab(string prefabPath, string parentName = "", Vector3 position = new Vector3(), float rotation = 0.00f)
        {
            GameObject prefab = AssetDatabaseManager.Instance.GetPrefab(prefabPath);

            if (prefab == null)
            {
                Debug.LogError("Prefab not found at path: " + prefabPath);
                return null;
            }

            GameObject go = Instantiate<GameObject>(prefab, position, Quaternion.identity);

            if (rotation != 0)
            {
                go.transform.Rotate(new Vector3(0, rotation, 0));
            }

            if (!string.IsNullOrEmpty(parentName))
            {
                Transform parent = root.transform.Find(parentName);

                if (parent != null)
                {
                    go.transform.SetParent(parent);
                }
                else
                {
                    Debug.LogWarning("Parent object not found with name: " + parentName);
                }
            }

            return go;
        }

        ///<summary>
        ///创建一个子弹对象在场景上
        ///<param name="bulletLauncher">子弹发射器</param>
        ///</summary>
        public GameObject CreateBullet(BulletLauncher bulletLauncher)
        {
            //对象池优化后的 子弹实例化代码
            GameObject bulletObj = ObjectPooler.GetPooledGamObjectAtIndex(0);

            //为了避免出现空指针错误 一定要拿到对象
            while (bulletObj == null)
            {
                bulletObj = ObjectPooler.GetPooledGamObjectAtIndex(0);
            }

            if (bulletObj != null)
            {
                bulletObj.transform.position = bulletLauncher.firePosition;
                bulletObj.transform.rotation = Quaternion.identity;
                bulletObj.transform.SetParent(root.transform.Find("Bullet"));
                bulletObj.SetActive(true);
            }

            //初始化前移除子弹的美术预制件
            TransformerHelper.RemoveAllChildren(bulletObj);
            //处理bulletObj的数据
            bulletObj.transform.RotateAround(bulletObj.transform.position, Vector3.up,
                bulletLauncher.fireDegree +
                UnityEngine.Random.Range(-bulletLauncher.scatteringDegree, bulletLauncher.scatteringDegree));

            bulletObj.GetComponent<BulletState>().InitByBulletLauncher(
                bulletLauncher,
                GameObject.FindGameObjectsWithTag("Character") //我这个游戏里，只给你角色对象，你要跟踪子弹，那就再把子弹也抓进来就好
            );

            //枪口火焰
            if (bulletLauncher.model.flash != "")
            {
                CreateSightEffect(bulletLauncher.model.flash, bulletLauncher.firePosition, bulletLauncher.fireDegree);
            }
            return bulletObj;
        }

        ///<summary>
        /// 创建一个或多个子弹对象在场景上
        ///<param name="bulletLauncher">子弹发射器</param>
        ///</summary>
        public GameObject CreateBullets(BulletLauncher bulletLauncher)
        {
            List<GameObject> bullets = new List<GameObject>();
            List<BulletLauncher> bulletLaunchers = BulletLauncher.SplitBulletLauncher(bulletLauncher);
            for (int i = 0; i < bulletLaunchers.Count; i++)
            {
                bullets.Add(CreateBullet(bulletLaunchers[i]));
            }
            return bullets[0];
        }


        ///<summary>
        ///删除一个存在的子弹Object
        ///<param name="aoe">子弹的GameObject</param>
        ///<param name="immediately">是否当场清除，如果false，就是把时间变成0</param>
        ///</summary>
        public void RemoveBullet(GameObject bullet, bool immediately = false)
        {
            if (!bullet) return;
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            bulletState.duration = 0;
            if (immediately == true)
            {
                bulletState.model.onRemoved?.Invoke(bullet);

                //优化后代码 移除所有子物体 然后SetActive为false
                TransformerHelper.RemoveAllChildren(bullet);
                bullet.SetActive(false);
            }
        }

        ///<summary>
        ///创建一个aoe对象在场景上
        ///<param name="aoeLauncher">aoe的创建信息</param>
        ///</summary>
        public GameObject CreateAoE(AoeLauncher aoeLauncher)
        {
            GameObject aoeObj = Instantiate<GameObject>(
                ObjectPooler.GetPooledGamObjectAtIndex(2),
                aoeLauncher.position,
                Quaternion.identity,
                root.transform.Find("Aoe")
            );
            aoeObj.GetComponent<AoeState>().InitByAoeLauncher(aoeLauncher);
            return aoeObj;
        }

        ///<summary>
        ///删除一个存在的aoeObject
        ///<param name="aoe">aoe的GameObject</param>
        ///<param name="immediately">是否当场清除，如果false，就是把时间变成0</param>
        ///</summary>
        public void RemoveAoE(GameObject aoe, bool immediately = false)
        {
            if (!aoe) return;
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;
            aoeState.duration = 0;
            if (immediately == true)
            {
                if (aoeState.model.onRemoved != null)
                {
                    aoeState.model.onRemoved.Invoke(aoe);
                }
                Destroy(aoe);
            }
        }

        ///<summary>
        ///创建一个视觉特效在场景上
        ///<param name="prefab">特效的prefab文件夹，约定就在Prefabs/下，所以路径不应该加这段</param>
        ///<param name="pos">创建的位置</param>
        ///<param name="degree">角度</param>
        ///<param name="key">特效的key，如果重复则无法创建，删除的时候也有用，空字符串的话不加入管理</param>
        ///<param name="loop">是否循环，循环的得手动remove</param>
        ///</summary>
        public GameObject CreateSightEffect(string prefab, Vector3 pos, float degree, string key = "", bool loop = false, Func<float> getSkillSize = null)
        {

            if (sightEffect.ContainsKey(key) == true) return null;    //已经存在，加不成

            // 加载预制件资源
            GameObject prefabResource = AssetDatabaseManager.Instance.GetPrefab(prefab);
            GameObject effectGO = null;
            // 检查预制件是否为空
            if (prefabResource == null)
            {
                Debug.LogError("The prefab '" + prefab + "' could not be found in database. Please check the path and file name.");
            }
            else
            {
                // 实例化预制件，因为资源已确认非空
                effectGO = Instantiate<GameObject>(
                prefabResource,
                new Vector3(prefabResource.transform.position.x + pos.x, prefabResource.transform.position.y + pos.y, prefabResource.transform.position.z + pos.z),
                Quaternion.identity,
                this.gameObject.transform
            );
            }

            effectGO.transform.RotateAround(effectGO.transform.position, Vector3.up, degree);
            if (!effectGO) return null;
            SightEffect se = effectGO.GetComponent<SightEffect>();
            if (!se)
            {
                Destroy(effectGO);
                return null;
            }
            if (loop == false)
            {
                effectGO.AddComponent<UnitRemover>().duration = se.duration;
            }
            if (key != "") sightEffect.Add(key, effectGO);
            return effectGO;
        }

        ///<summary>
        ///删除一个视觉特效在场景上
        ///<param name="key">特效的key</param>
        ///</summary>
        public void RemoveSightEffect(string key)
        {
            if (sightEffect.ContainsKey(key) == false) return;
            Destroy(sightEffect[key]);
            sightEffect.Remove(key);
        }

        ///<summary>
        ///创建一个角色到场上
        ///<param name="prefab">特效的prefab文件夹，约定就在Prefabs/Character/下，所以路径不应该加这段</param>
        ///<param name="unitAnimInfo">角色的动画信息</param>
        ///<param name="side">所属阵营</param>
        ///<param name="pos">创建的位置</param>
        ///<param name="degree">角度</param>
        ///<param name="baseProp">初期的基础属性</param>
        ///<param name="tags">角色的标签，分类角色用的</param>
        ///</summary>
        public GameObject CreateCharacter(string prefab, int side, Vector3 pos, ChaProperty baseProp, float degree, string unitAnimInfo = "Default_Gunner", string[] tags = null, bool SetColliderActiveAtStart = true)
        {
            GameObject chaObj = CreateFromPrefab("CharacterObj", "Character");

            chaObj.name = string.Concat("enemay", UnityEngine.Random.Range(0, 999).ToString());
            ChaState cs = chaObj.GetComponent<ChaState>();
            if (cs)
            {
                cs.InitBaseProp(baseProp);
                cs.side = side;
                Dictionary<string, AnimInfo> aInfo = new Dictionary<string, AnimInfo>();
                if (unitAnimInfo != "" && AnimData.data.ContainsKey(unitAnimInfo))
                {
                    aInfo = AnimData.data[unitAnimInfo];
                }
                cs.SetView(CreateFromPrefab(prefab), aInfo);
                if (tags != null) cs.tags = tags;
            }

            chaObj.transform.position = pos;
            chaObj.transform.RotateAround(chaObj.transform.position, Vector3.up, degree);
            //角色列表添加此角色
            characters.Add(chaObj);

            if (SetColliderActiveAtStart == true)
            {
                chaObj.GetComponent<Rigidbody>().isKinematic = false;
                chaObj.GetComponent<CapsuleCollider>().enabled = true;
            }
            return chaObj;
        }

        public GameObject InitChaState(GameObject character, string name, int side, ChaProperty baseProp, string unitAnimInfo = "Default_Gunner", string[] tags = null)
        {
            character.name = name;
            ChaState cs = character.GetComponent<ChaState>();
            if (cs)
            {
                cs.InitBaseProp(baseProp);
                cs.side = side;
            }
            //角色列表添加此角色
            characters.Add(character);
            return character;
        }

        ///<summary>
        /// 封装创建角色函数，添加延迟和初始效果
        ///</summary>
        public void DelayedCreateCharacter(float delay, string effectPrefab, string prefab, int side, Vector3 pos, ChaProperty baseProp, float degree, string unitAnimInfo = "Default_Gunner", string[] tags = null)
        {
            StartCoroutine(CreateCharacterAfterDelay(delay, effectPrefab, prefab, side, pos, baseProp, degree, unitAnimInfo, tags));
        }

        ///<summary>
        /// 协程：延迟后创建角色
        ///</summary>
        private IEnumerator CreateCharacterAfterDelay(float delay, string effectPrefab, string prefab, int side, Vector3 pos, ChaProperty baseProp, float degree, string unitAnimInfo, string[] tags)
        {
            // 在指定位置播放效果
            GameObject effectInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + effectPrefab), pos, Quaternion.identity);
            Destroy(effectInstance, delay);  // 假设特效结束后自动销毁，或根据特效时长销毁

            // 等待指定的延迟时间
            yield return new WaitForSeconds(delay);

            // 调用创建角色函数
            CreateCharacter(prefab, side, pos, baseProp, degree, unitAnimInfo, tags);
        }
    }
}
