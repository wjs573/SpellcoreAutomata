using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnLocation : MonoBehaviour
{
    public delegate Vector3 MobSpawnLocationDelegate(object[] parameters);

    public static Dictionary<string, MobSpawnLocationDelegate> data = new Dictionary<string, MobSpawnLocationDelegate>()
        {
            {"RandomPosition",RandomPosition },
            {"CentralLocation",CentralLocation}
        };

    /// <summary>
    /// 随机返回一个坐标
    /// </summary>
    /// <returns></returns>
    private static Vector3 RandomPosition(object[] parameters)
    {
        return SceneVariants.map.GetRandomPosForCharacter(new RectInt(0, 0, SceneVariants.map.MapWidth(), SceneVariants.map.MapHeight()));
    }

    /// <summary>
    /// 返回地图中心坐标
    /// </summary>
    /// <returns></returns>
    private static Vector3 CentralLocation(object[] parameters)
    {
        return SceneVariants.map.GetCentralPosForCharacter(new RectInt(0, 0, SceneVariants.map.MapWidth(), SceneVariants.map.MapHeight()));
    }
}