using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// 随机id管理器
/// 为了给每个生成的物体添加一个唯一的id
/// </summary>
[Serializable]
public class RandomInt
{
    /// <summary>
    /// 已经生成的ID
    /// </summary>
    static HashSet<int> generatedIds = new HashSet<int>();

    /// <summary>
    /// 获取一个新的id
    /// </summary>
    /// <returns></returns>
    public int GetId()
    {
        int newId = UnityEngine.Random.Range(0, 999999);
        while (generatedIds.Contains(newId))
        {
            newId = UnityEngine.Random.Range(0, 999999);
        }
        return newId;
    }

    /// <summary>
    /// 移除物品时，也要归还这个id
    /// 即从已经保存的id里移除这个id
    /// </summary>
    /// <param name="id"></param>
    public void RemoveId(int id)
    {
        if (generatedIds.Contains(id))
        {
            generatedIds.Remove(id);
        }
    }


    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="data"></param>
    public static void SaveData(RandomInt data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/generatedIds.dat");
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <returns></returns>
    public static RandomInt LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/generatedIds.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/generatedIds.dat", FileMode.Open);
            RandomInt data = (RandomInt)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            return new RandomInt();
        }
    }
}
