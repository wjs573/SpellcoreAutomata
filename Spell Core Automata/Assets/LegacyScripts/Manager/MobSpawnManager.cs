using System.Collections;
using System.Collections.Generic;
using JinShan;
using Sirenix.OdinInspector;
using UnityEngine;

public class BattleSpawnData
{
    public Dictionary<string, MobSpawnInfo> data = new Dictionary<string, MobSpawnInfo>();
    public float spawnPeriod = 0.5f;

    public BattleSpawnData(Dictionary<string, MobSpawnInfo> data, float spawnPeriod = 0.25f)
    {
        this.data = data;
        this.spawnPeriod = spawnPeriod;
    }

    public void Reset()
    {
        foreach (MobSpawnInfo mobSpawnInfo in data.Values)
        {
            mobSpawnInfo.ResetCount();
        }
    }
}
///<summary>
///管理怪物生成的管理器
///</summary>
public class MobSpawnManager : MonoSingleton<MobSpawnManager>
{
    /// <summary>
    /// 当前波次的怪物生成数据
    /// </summary>
    public BattleSpawnData battleSpawnData;

    private int currentMaxMobCount = 10; // 初始允许的最大怪物数量
    private int finalMaxMobCount = 50; // 最终允许的最大怪物数量
    private float mobCountIncrementInterval = 15f; // 每次增加最大怪物数量的间隔时间
    private float nextMobCountIncrementTime = 0f; // 下一次增加最大怪物数量的时间点

    [ShowInInspector]
    private bool isSpawning = false; // 标志用于跟踪是否正在生成怪物

    public float spawnInterval = 0.5f; // 怪物生成的时间间隔

    public float elapsedTime = 0f; // 计时器，用于跟踪生成时间
    private int idCounter; // 生成怪物的唯一ID计数器

    private void Start()
    {
        if (battleSpawnData != null)
        {
            StartCoroutine(HandleMobSpawning());
        }

        idCounter = 0;
    }

    /// <summary>
    /// 开始生成怪物
    /// </summary>
    /// <param name="battleSpawnData"></param>
    public void BeginSpawning(BattleSpawnData battleSpawnData)
    {
        if (isSpawning)
        {
            return;
        }

        StopAllSpawning();

        elapsedTime = 0f;
        spawnInterval = battleSpawnData.spawnPeriod + Random.Range(0, 2f);

        this.battleSpawnData = battleSpawnData;
        battleSpawnData.Reset();

        isSpawning = true;
        StartCoroutine(HandleMobSpawning());
    }

    /// <summary>
    /// 停止生成怪物
    /// </summary>
    public void StopAllSpawning()
    {
        StopAllCoroutines();
        ResetSpawnParametersForNextWave();
    }

    /// <summary>
    /// 处理怪物生成的主协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleMobSpawning()
    {
        isSpawning = true;

        while (ShouldSpawnMobs())
        {
            foreach (MobSpawnInfo mobSpawnInfo in battleSpawnData.data.Values)
            {
                TrySpawnMob(mobSpawnInfo);
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    /// <summary>
    /// 在指定延迟后生成一个怪物并播放特效
    /// </summary>
    /// <param name="mobSpawnInfo">怪物生成信息</param>
    /// <param name="effectPrefab">特效预制件名称</param>
    /// <param name="delay">延迟时间（秒）</param>
    private IEnumerator SpawnMobWithEffect(MobSpawnInfo mobSpawnInfo, string effectPrefab, float delay, Vector3 spawnLocation = new Vector3())
    {
        spawnLocation = spawnLocation == null ? MobSpawnLocation.data[mobSpawnInfo.mobSpawnLocation](mobSpawnInfo.mobSpawnLocationParam) : spawnLocation;
        GameObject effectInstance = SceneVariants.CreateSightEffect(effectPrefab, spawnLocation, 0f, "", true);

        yield return new WaitForSeconds(delay);

        Destroy(effectInstance);
        CreateMob(mobSpawnInfo, spawnLocation);
    }

    /// <summary>
    /// 启动生成怪物并播放特效的协程
    /// </summary>
    /// <param name="mobSpawnInfo">怪物生成信息</param>
    /// <param name="effectPrefab">特效预制件名称</param>
    /// <param name="delay">延迟时间（秒）</param>
    public void InitiateMobSpawnWithEffect(MobSpawnInfo mobSpawnInfo, string effectPrefab, float delay)
    {
        StartCoroutine(SpawnMobWithEffect(mobSpawnInfo, effectPrefab, delay));
    }

    /// <summary>
    /// 判断是否应该生成怪物
    /// </summary>
    /// <returns></returns>
    private bool ShouldSpawnMobs()
    {
        return GameManager.Instance.IsInBattle;
    }

    /// <summary>
    /// 尝试生成怪物，如果符合条件
    /// </summary>
    /// <param name="mobSpawnInfo">怪物生成信息</param>
    private void TrySpawnMob(MobSpawnInfo mobSpawnInfo)
    {
        int totalCurrentMobs = GetTotalActiveMobs();

        if (mobSpawnInfo.spawndCount >= mobSpawnInfo.maxAllowSpawnCount ||
            mobSpawnInfo.currentCount >= mobSpawnInfo.maxAllowCurrentCount)
        {
            return;
        }

        if (MobSpawnCondition.data[mobSpawnInfo.MobSpawnCondition](mobSpawnInfo.spawnConditionParam))
        {
            int spawnCount = Mathf.Min(
                mobSpawnInfo.spawnAmount,
                mobSpawnInfo.maxAllowSpawnCount - mobSpawnInfo.spawndCount,
                mobSpawnInfo.maxAllowCurrentCount - mobSpawnInfo.currentCount,
                currentMaxMobCount - totalCurrentMobs);

            for (int i = 0; i < spawnCount; i++)
            {
                StartCoroutine(SpawnMobWithEffect(mobSpawnInfo, "Effect/Circle/RedCircle", 1f));
            }
        }
    }

    /// <summary>
    /// 创建并初始化一个怪物
    /// </summary>
    /// <param name="mobSpawnInfo">怪物生成信息</param>
    /// <param name="location">生成位置</param>
    private void CreateMob(MobSpawnInfo mobSpawnInfo, Vector3 location)
    {
        EnemyData enemyData = mobSpawnInfo.GetEnemyData();
        if (enemyData == null)
        {
            return;
        }

        GameObject enemy = SceneVariants.CreateCharacter(
            enemyData.View, mobSpawnInfo.side,
            location,
            enemyData.ChaProperty, Random.Range(0.00f, 359.99f), enemyData.data.Name, new string[] { "Mob" });

        // 播放出生动画
        enemy.GetComponent<UnitAnim>().BufferAnimation = "Spawn";

        Object prefab = Resources.Load("Prefabs/AI/" + enemyData.FSM);
        if (prefab != null)
        {
            Instantiate(prefab, enemy.transform);
        }
        else
        {
            Debug.Log("预制件不存在: " + "Prefabs/AI/" + enemyData.FSM);
        }

        ChaState chaState = enemy.GetComponent<ChaState>();

        for (int i = 0; i < enemyData.skills.Count; i++)
        {
            chaState.LearnSkill(DesignerTables.Skill.data[enemyData.skills[i]]);
        }

        chaState.GetComponent<UnitRotate>().rotateSpeed = 360f;

        if (enemyData.addBuffInfos != null)
        {
            for (int i = 0; i < enemyData.addBuffInfos.Count; i++)
            {
                AddBuffInfo addBuffInfo = enemyData.addBuffInfos[i].AddBuffInfo;
                addBuffInfo.caster = enemy;
                addBuffInfo.target = enemy;
                chaState.AddBuff(addBuffInfo);
            }
        }

        idCounter += 1;
        enemy.name = string.Concat(enemyData.data.Name, idCounter);

        mobSpawnInfo.gameObjectsOfMobs.Add(enemy);
        mobSpawnInfo.spawndCount += 1;
        mobSpawnInfo.currentCount += 1;
    }

    bool hasCreateCircleMobs = false;
    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;
        IncrementMaxMobCountOverTime();
        if ((elapsedTime > 15f) && hasCreateCircleMobs == false)
        {
            hasCreateCircleMobs = true;
            CreateMobsCircleAroundPlayer(GameManager.Instance.mainActor);
        }
    }

    /// <summary>
    /// 重置参数为下一波次做准备
    /// </summary>
    public void ResetSpawnParametersForNextWave()
    {
        currentMaxMobCount = 10;
        nextMobCountIncrementTime = Time.time + mobCountIncrementInterval;
    }

    /// <summary>
    /// 获取当前场景中所有活动怪物的数量
    /// </summary>
    /// <returns>当前活动怪物的数量</returns>
    private int GetTotalActiveMobs()
    {
        int total = 0;
        foreach (var mobInfo in battleSpawnData.data.Values)
        {
            total += mobInfo.currentCount;
        }
        return total;
    }

    /// <summary>
    /// 随时间递增允许的最大怪物数量
    /// </summary>
    private void IncrementMaxMobCountOverTime()
    {
        if (Time.time >= nextMobCountIncrementTime && currentMaxMobCount < finalMaxMobCount)
        {
            currentMaxMobCount += 5; // 每次递增5个怪物
            if (currentMaxMobCount > finalMaxMobCount)
            {
                currentMaxMobCount = finalMaxMobCount;
            }
            nextMobCountIncrementTime = Time.time + mobCountIncrementInterval;
        }
    }


    private void CreateMobsCircleAroundPlayer(GameObject mainCharacter)
    {
        if (mainCharacter == null)
        {
            return;
        }
        Vector3 playerPosition = mainCharacter.transform.position;
        float radius = 10f;
        int count = 20;
        Vector3[] positionArray = GetCircularPositionsAroundPlayer(playerPosition, radius, count);
        foreach (Vector3 pos in positionArray)
        {
            if (battleSpawnData.data.ContainsKey("Skeleton"))
            {
                StartCoroutine(SpawnMobWithEffect(battleSpawnData.data["Skeleton"], "Effect/Circle/RedCircle", 1f, pos));
            }
        }
    }

    private void InitializeComponents(GameObject enemy, CharacterSpawnInfo enemyData)
{
    Object prefab = Resources.Load("Prefabs/AI/" + enemyData.FSM);
    if (prefab != null)
    {
        Instantiate(prefab, enemy.transform);
    }
    else
    {
        Debug.Log("预制件不存在: " + "Prefabs/AI/" + enemyData.FSM);
    }

    ChaState chaState = enemy.GetComponent<ChaState>();
    enemyData.GetSkillModels();
    for (int i = 0; i < enemyData.skills.Count; i++)
    {
        chaState.LearnSkill(enemyData.skills[i]);
    }

    chaState.GetComponent<UnitRotate>().rotateSpeed = 360f;

    if (enemyData.addBuffInfos != null)
    {
        for (int i = 0; i < enemyData.addBuffInfos.Count; i++)
        {
            AddBuffInfo addBuffInfo = enemyData.addBuffInfos[i];
            addBuffInfo.caster = enemy;
            addBuffInfo.target = enemy;
            chaState.AddBuff(addBuffInfo);
        }
    }
}


    private Vector3[] GetCircularPositionsAroundPlayer(Vector3 playerPosition, float radius, int count)
    {
        Vector3[] positions = new Vector3[count];
        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;
            float radian = angle * Mathf.Deg2Rad;
            float x = playerPosition.x + radius * Mathf.Cos(radian);
            float z = playerPosition.z + radius * Mathf.Sin(radian);
            positions[i] = new Vector3(x, playerPosition.y, z);
        }
        return positions;
    }

}
