using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public class EnemyData
{
    public static Dictionary<string, EnemyModel> data;

    public static void Initialize()
    {
        data = new Dictionary<string, EnemyModel>()
        {
            {"Skeleton", new EnemyModel()
            {
                id = "30001",
                name = "Skeleton",
                prefab = "Skeleton",
                property = new ChaProperty(50,0,100,100,0,100,20,100,20,1.5f,0.25f,0f,0.5f,0.5f),
                addBuffInfos = new AddBuffInfo[]
                {
                    new AddBuffInfo(BuffData.data["Resurrect"],null,null,1,1f,true)
                }
            }},
        };
    }
}
