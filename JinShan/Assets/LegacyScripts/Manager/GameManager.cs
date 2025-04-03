using System;
using System.Collections;
using System.Collections.Generic;
using DesignerScripts;
using JinShan;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public ItemDatabaseObject database;
    public CinemachineFollow cinemachineFollow;
    //对象池组件
    public MMMultipleObjectPooler ObjectPooler;

    //总有一个角色是主角，也就是玩家控制的，并且镜头跟随的
    public GameObject mainActor
    {
        get
        {
            return mainCharacter;
        }
    }

    private GameObject mainCharacter;

    public Vector3 MousePositionOnXOZPlane
    {
        get
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置发出射线
            RaycastHit hit;
            Vector3 pointOnXOZ = Vector3.zero;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
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

        // 在进入暂停状态或离开暂停状态时执行额外的逻辑
        if (isPaused)
        {
            // 执行进入暂停状态时的逻辑，比如显示暂停菜单
            GameSoundManager.Instance.StopSound("BGM");
        }
        else
        {
            // 执行离开暂停状态时的逻辑，比如关闭暂停菜单
            GameSoundManager.Instance.PlaySound("BGM");
        }
    }

    /// <summary>
    /// 是否处于战斗场景
    /// </summary>
    public bool IsInBattle = false;

    private bool HasInit = false;

    /// <summary>
    /// id生成器
    /// </summary>
    public RandomInt generatedIds;

    //已登场的角色，包括主角、敌人、法宝
    private List<GameObject> characters;

    /// <summary>
    /// 获取除去法宝的所有character
    /// </summary>
    public List<GameObject> Characters
    {
        get
        {
            if (characters == null)
            {
                return null;
            }
            //去除掉法宝
            List<GameObject> _characters = new List<GameObject>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && !characters[i].GetComponent<ChaState>().hasParent)
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
        //初始化id生成器
        generatedIds = RandomInt.LoadData();

        //这是生成游戏物体的父物体
        root = GameObject.Find("GameObjectLayer");

        //初始化策划填表
        //DesignerTables.Buff.Initialize();
        DesignerTables.AoE.Initialize();
        DesignerTables.Bullet.Initialize();
        DataLaserModel.Init();
        DesignerScripts.Timeline.Initialize();
        DesignerTables.Timeline.Initialize();
        DesignerTables.Skill.Initialize();
        DesignerTables.DataMartialArt.Initialize();
        DesignerTables.BattleSpawn.Initialize();
    }

    private void FixedUpdate()
    {
        //战斗场景的逻辑
        if (IsInBattle)
        {
            if (mainActor == null || mainActor.GetComponent<ChaState>().dead)
            {
                //UIManager.Instance.GetWindow<UIGameoverWindow>().GameOver();
            }
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
    /// 初始化战斗场景
    /// </summary>
    public void IniBattle()
    {
        if (IsInBattle)
        {
            return;
        }
        IsInBattle = true;

        //开启BGM
        GameSoundManager.Instance.PlaySound("BGM", true);

        //创建地图
        SceneVariants.RandomMap(36, 36, 1);

        //初始化人物表
        characters = new List<GameObject>();

        //创建主角
        Vector3 playerPos = SceneVariants.map.GetRandomPosForCharacter(new RectInt(0, 0, SceneVariants.map.MapWidth(), SceneVariants.map.MapHeight()));
        mainCharacter = this.CreateCharacter(
            "FemaleMage", 1, new Vector3(), new ChaProperty(
                100, 0, 100,
                5000, 10, 200, 10, 100,
                200, 100, 15,
                1.5f, 0.25f, 0.05f, 0.25f, 0.4f), 0, "女法师"
        );  //这里必须是new Vector3()因为相机跟随的设置问题
        mainCharacter.AddComponent<PlayerController>().mainCamera = Camera.main;
        mainCharacter.name = "MainCharacter";

        //给主角添加buff
        AddBuffInfo buffInfo = new AddBuffInfo(DesignerTables.Buff.data["BaseRecover"], mainActor, mainActor, 1, 999);
        mainCharacter.GetComponent<ChaState>().AddBuff(buffInfo);

        //给主角添加一个寻敌组件
        //目前是准备给法宝调用
        //法宝获得最近的敌人
        mainCharacter.AddComponent<UnitGetTarget>();

        //镜头跟随主角
        GameObject.Find("MainCamera").GetComponent<CinemachineFollow>().SetFollowCharacter(mainCharacter);

        //再设置主角位置
        mainCharacter.transform.position = playerPos;
        ChaState mcs = mainCharacter.GetComponent<ChaState>();

        //添加法宝
        MainCharacter.Instance.FaBao_Equippment_Inventory.Clear();
        MainCharacter.Instance.Main_inventory.Clear();
        for (int i = 0; i < database.ItemObjects.Length; i++)
        {
            if (database.ItemObjects[i].type == ItemType.法宝)
            {
                Item fabao = new Item(database.ItemObjects[i]);
                MainCharacter.Instance.Main_inventory.AddItem(fabao, 1);
            }
        }

        //设置背包
        mcs.FaBao_Equippment_Inventory = MainCharacter.Instance.FaBao_Equippment_Inventory;
        //给主角添加拾取掉落物组件
        mcs.gameObject.AddComponent<UnitItemCollector>();
        //设置掉落组件的目标仓库为玩家的背包
        mcs.gameObject.GetComponent<UnitItemCollector>().targetInventory = MainCharacter.Instance.Main_inventory;

        if (HasInit == false)
        {
            HasInit = true;
        }

        //学习技能
        MainCharacter.Instance.Equipped_Skill_Inventory.Clear();

        MainCharacter.Instance.Equipped_ComboSpell_Inventory.Clear();

        MainCharacter.Instance.Skill_Inventory.Clear();

        for (int i = 0; i < database.ItemObjects.Length; i++)
        {
            Item skill = new Item(database.ItemObjects[i]);
            if (skill.itemObject.type == ItemType.技能 || skill.itemObject.type == ItemType.技能强化 ||
                skill.itemObject.type == ItemType.触发器)
            {
                mcs.LearnSkill(skill.GetSkillModel());
                MainCharacter.Instance.Main_inventory.AddItem(skill, 1);
            }
        }
        //添加技能组合组件
        mcs.gameObject.AddComponent<SpellCombinationManagerContainer>();
        MainCharacter.Instance.bar.UpdateBar(0f);

        UIManager.Instance.GetWindow<UIEquipmentWindow>().SetMainInventory(MainCharacter.Instance.Main_inventory, MainCharacter.Instance.FaBao_Equippment_Inventory, mcs.gameObject.GetComponent<SpellCombinationManagerContainer>());
    }

    public void StartTrainMode()
    {
        IniBattle();
        UIManager.Instance.GetWindow<UICombatHUDWindow>().SetVisible(true);
        UIManager.Instance.GetWindow<UICombatHUDWindow>().SetMainCharacter(mainActor);
        //初始化刷怪管理器
        BattleSpawnData battleSpawnData = DesignerTables.BattleSpawn.data["Train"];
        MobSpawnManager.Instance.BeginSpawning(battleSpawnData);
    }

    public void StartBattleMode()
    {
        IniBattle();
        UIManager.Instance.GetWindow<UICombatHUDWindow>().SetVisible(true);
        UIManager.Instance.GetWindow<UICombatHUDWindow>().SetMainCharacter(mainActor);
        //初始化刷怪管理器
        BattleSpawnData battleSpawnData = DesignerTables.BattleSpawn.data["Level2"];
        MobSpawnManager.Instance.BeginSpawning(battleSpawnData);
    }

    public void StartChessPieceBattleMode()
    {
        IsInBattle = true;
        //创建地图
        SceneVariants.RandomMap(36, 36, 1);
        MainCharacter.Instance.Skill_Inventory.Clear();
        for (int i = 0; i < database.ItemObjects.Length; i++)
        {
            Item skill = new Item(database.ItemObjects[i]);
            if (skill.itemObject.type == ItemType.技能 || skill.itemObject.type == ItemType.技能强化 ||
                skill.itemObject.type == ItemType.触发器)
            {
                MainCharacter.Instance.Skill_Inventory.AddItem(skill, 1);
            }
        }
        //初始化人物表
        characters = new List<GameObject>();

    }

    /// <summary>
    /// 清除战斗场景
    /// </summary>
    public void ClearBattle()
    {
        IsInBattle = false;
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

        //清除laser
        LaserManager.Instance.ClearLasers();
    }

    //根据prefab下的资源创建东西
    private GameObject CreateFromPrefab(string prefabPath, string parentName = "", Vector3 position = new Vector3(), float rotation = 0.00f)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabPath);

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


    //根据global.map制作地图的prefabs
    private void CreateMapGameObjects()
    {
        GameObject[] mt = GameObject.FindGameObjectsWithTag("MapTile");
        for (var i = 0; i < mt.Length; i++)
        {
            Destroy(mt[i]);
        }
        mt = null;

        for (var i = 0; i < SceneVariants.map.MapWidth(); i++)
        {
            for (var j = 0; j < SceneVariants.map.MapHeight(); j++)
            {
                CreateFromPrefab(SceneVariants.map.grid[i, j].prefabPath, "Map", new Vector3(i, 0, j));
            }
        }
    }

    /// <summary>
    /// 创建一个激光对象在场景上
    /// </summary>
    /// <param name="laserLauncher"></param>
    /// <returns></returns>
    public GameObject CreateLaser(LaserLauncher laserLauncher)
    {
        if (!IsInBattle)
        {
            return null;
        }

        GameObject laserObj = Instantiate<GameObject>(
             Resources.Load<GameObject>("Prefabs/Laser/LaserObj"),
             laserLauncher.firePositionTransform.position,
             Quaternion.identity,
             root.transform.Find("Laser")
         );
        laserObj.GetComponent<LaserState>().InitByLaserLauncher(laserLauncher);
        return laserObj;
    }

    ///<summary>
    ///创建一个子弹对象在场景上
    ///<param name="bulletLauncher">子弹发射器</param>
    ///</summary>
    public GameObject CreateBullet(BulletLauncher bulletLauncher)
    {
        //如果是处在非战斗场景 不能创建战斗场景的对象：bullet、character、effect、timeline
        if (!IsInBattle)
        {
            return null;
        }

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

        float spreadAngle = Mathf.Abs(bulletLauncher.spreadAngle);
        //处理bulletObj的数据
        bulletObj.transform.RotateAround(bulletObj.transform.position, Vector3.up,
            bulletLauncher.fireDegree +
            UnityEngine.Random.Range(-bulletLauncher.scatteringDegree, bulletLauncher.scatteringDegree));

        bulletObj.GetComponent<BulletState>().InitByBulletLauncher(
            bulletLauncher,
            GameObject.FindGameObjectsWithTag("Character") //我这个游戏里，只给你角色对象，你要跟踪子弹，那就再把子弹也抓进来就好
        );

        //枪口火焰
        //if (bulletLauncher.model.flash != "")
        //{
        //    CreateSightEffect(bulletLauncher.model.flash, bulletLauncher.firePosition, bulletLauncher.fireDegree);
        //}
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
            if (bulletState.model.onRemoved != null)
            {
                bulletState.model.onRemoved.Invoke(bullet);
            }

            //对象池优化前 是直接销毁
            //Destroy(bullet);

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
        //如果是处在非战斗场景 不能创建战斗场景的对象：bullet、character、effect、timelin、aoe
        if (!IsInBattle)
        {
            return null;
        }

        //创建一个bulletObj，这是个“空”的子弹，其实也就是没有视觉效果，其他都有了
        GameObject aoeObj = Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/Effect/AoeObj"),
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
    public GameObject CreateSightEffect(string prefab, Vector3 pos, float degree, string key = "", bool loop = false,Func<float> getSkillSize = null)
    {
        //如果是处在非战斗场景 不能创建战斗场景的对象：bullet、character、effect、timelin、aoe
        if (!IsInBattle)
        {
            return null;
        }

        if (sightEffect.ContainsKey(key) == true) return null;    //已经存在，加不成

        // 加载预制件资源
        GameObject prefabResource = Resources.Load<GameObject>("Prefabs/" + prefab);
        GameObject effectGO = null;
        // 检查预制件是否为空
        if (prefabResource == null)
        {
            Debug.LogError("The prefab '" + prefab + "' could not be found in 'Resources/Prefabs/'. Please check the path and file name.");
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
        //如果是处在非战斗场景 不能创建战斗场景的对象：bullet、character、effect、timelin、aoe
        if (!IsInBattle)
        {
            return null;
        }

        GameObject chaObj = CreateFromPrefab("Character/CharacterObj", "Character");

        chaObj.name = string.Concat("enemay", UnityEngine.Random.Range(0, 999).ToString());
        ChaState cs = chaObj.GetComponent<ChaState>();
        if (cs)
        {
            cs.InitBaseProp(baseProp);
            cs.side = side;
            Dictionary<string, AnimInfo> aInfo = new Dictionary<string, AnimInfo>();
            if (unitAnimInfo != "" && DesignerTables.UnitAnimInfo.data.ContainsKey(unitAnimInfo))
            {
                aInfo = DesignerTables.UnitAnimInfo.data[unitAnimInfo];
            }
            cs.SetView(CreateFromPrefab("Character/" + prefab), aInfo);
            if (tags != null) cs.tags = tags;
        }

        chaObj.transform.position = pos;
        chaObj.transform.RotateAround(chaObj.transform.position, Vector3.up, degree);
        chaObj.GetComponent<UnitFeedback>().Init();
        //角色列表添加此角色
        characters.Add(chaObj);

        if (SetColliderActiveAtStart == true)
        {
            chaObj.GetComponent<Rigidbody>().isKinematic = false;
            chaObj.GetComponent<CapsuleCollider>().enabled = true;
        }
        return chaObj;
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