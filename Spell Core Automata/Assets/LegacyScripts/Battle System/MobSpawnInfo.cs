using System.Collections.Generic;
using UnityEngine;
using JinShan;
/// <summary>
/// 怪物生成信息
/// </summary>
public class MobSpawnInfo
{
    /// <summary>
    /// 怪物属性
    /// </summary>
    public string EnemyData;

    public EnemyData GetEnemyData()
    {
        ItemObject[] EnemyDatas = GameManager.Instance.database.enemyDataItemObjects;
        foreach (ItemObject item in EnemyDatas)
        {
            EnemyData enemyData = (EnemyData)item;
            if (enemyData.name == EnemyData)
            {
                return enemyData;
            }
        }
        return null;
    }

    public int side;

    /// <summary>
    /// 生成条件
    /// </summary>
    public string MobSpawnCondition;

    /// <summary>
    /// 生成条件的参数
    /// </summary>
    public object[] spawnConditionParam;

    /// <summary>
    /// 生成数量
    /// </summary>
    public int spawnAmount;

    /// <summary>
    /// 现存的Mobs
    /// </summary>
    public List<GameObject> gameObjectsOfMobs;

    /// <summary>
    /// 生成位置
    /// </summary>
    public string mobSpawnLocation;

    /// <summary>
    /// 生成位置的参数
    /// </summary>
    public object[] mobSpawnLocationParam;

    /// <summary>
    /// 已生成数量
    /// </summary>
    public int spawndCount;

    /// <summary>
    /// 现存数量
    /// </summary>
    public int currentCount;

    /// <summary>
    /// 已死亡数量
    /// </summary>
    public int deathCount;

    /// <summary>
    /// 最多允许现存数量
    /// </summary>
    public int maxAllowCurrentCount;

    /// <summary>
    /// 最多允许生成数量
    /// </summary>
    public int maxAllowSpawnCount;

    public MobSpawnInfo(string EnemyData, int spawnAmount, string mobSpawnCondition, object[] spawnConditionParam, string mobSpawnLocation, object[] mobSpawnLocationParam, int maxAllowCurrentCount, int maxAllowSpawnCount, int side = 2)
    {
        this.EnemyData = EnemyData;
        MobSpawnCondition = mobSpawnCondition;
        this.spawnConditionParam = spawnConditionParam;
        this.spawnAmount = spawnAmount;
        this.mobSpawnLocation = mobSpawnLocation;
        this.mobSpawnLocationParam = mobSpawnLocationParam;
        spawndCount = 0;
        currentCount = 0;
        deathCount = 0;
        gameObjectsOfMobs = new List<GameObject>();
        this.maxAllowCurrentCount = maxAllowCurrentCount;
        this.maxAllowSpawnCount = maxAllowSpawnCount;
        this.side = side;
    }

    /// <summary>
    /// 清零重置
    /// </summary>
    public void ResetCount()
    {
        spawndCount = 0;
        currentCount = 0;
        deathCount = 0;
    }
}
