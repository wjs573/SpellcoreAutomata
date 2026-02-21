using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public static class RuneData
{
    public static Dictionary<string, RuneModel> data = new Dictionary<string, RuneModel>();

    // 初始化方法
    public static void Initialize()
    {
        // 创建一个新的字典，键为字符串，值为RuneModel类型
        data = new Dictionary<string, RuneModel>()
        {
            {"FireBall", new RuneModel(){
                name = "FireBall",
                icon = "FireBall",
                id = "10001",
                tags = new string[] {"Fire", "Magic"},
                description = "Fire",
                skill = SkillData.data["FireBall"]}
            }   
        };
    }
}
