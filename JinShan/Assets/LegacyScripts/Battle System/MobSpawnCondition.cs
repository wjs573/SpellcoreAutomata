using System.Collections.Generic;
using UnityEngine;

public class MobSpawnCondition : MonoBehaviour
{
    public delegate bool MobSpawnConditionDelegate(object[] parameters);

    public static Dictionary<string, MobSpawnConditionDelegate> data = new Dictionary<string, MobSpawnConditionDelegate>()
        {
            {"NoCondition",NoCondition },
            {"SpawnWithProbability",SpawnWithProbability },
            {"SpawnAfterSeconds",SpawnAfterSeconds },
            {"SpawnWithKilledMonster",SpawnWithKilledMonster }
        };

    /// <summary>
    /// 击杀指定id的怪物的数量达到n个时生成
    /// 参数0 id
    /// 参数1 数量
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static bool SpawnWithKilledMonster(object[] parameters)
    {
        string id = (string)parameters[0];
        int count = (int)parameters[1];

        foreach (MobSpawnInfo info in MobSpawnManager.Instance.battleSpawnData.data.Values)
        {
            if (info.GetEnemyData().data.Name == id && info.deathCount >= count)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 战斗开始n秒后生成
    /// 参数0 时间
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static bool SpawnAfterSeconds(object[] parameters)
    {
        float t = (float)parameters[0];
        return MobSpawnManager.Instance.elapsedTime >= t;
    }

    /// <summary>
    /// 按照概率生成
    /// 参数0 概率
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static bool SpawnWithProbability(object[] parameters)
    {
        object[] p = parameters;
        float provability = p.Length > 0 ? (float)p[0] : 0f;
        return Random.Range(0f, 1f) <= provability;
    }

    /// <summary>
    /// 无条件返回true
    /// 即 直接生成
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static bool NoCondition(object[] parameters)
    {
        return true;
    }
}