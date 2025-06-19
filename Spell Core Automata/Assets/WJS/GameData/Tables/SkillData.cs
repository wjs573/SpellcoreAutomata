using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class SkillData
    {
        public static Dictionary<string, SkillModel> data = new Dictionary<string, SkillModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, SkillModel>() 
            {
                
                //火球
                {
                    "FireBall",new SkillModel("FireBall",new ChaResource(0,30),new ChaResource(0,30),
                    "FireBall",null,1.00f,1,null,null,false)
                }
            };
        }
    }
}

