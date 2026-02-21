using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class TimelineData
    {
        public static TimelineModel GetTimelineCopy(string id)
        {
            if (data.ContainsKey(id))
            {
                // 使用 Clone 方法创建完全拷贝的副本
                return data[id].Clone();
            }
            else
            {
                return data["base"];
            }
        }
        public static Dictionary<string, TimelineModel> data = new Dictionary<string, TimelineModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, TimelineModel>
            {
                //空
                { "base", new TimelineModel("base", new TimelineNode[] { }, 0.00f, TimelineGoTo.Null) },
                {
                    "FireBall",
                    new TimelineModel("FireBall", new TimelineNode[]{
                        new TimelineNode(0.00f, "CasterPlayAnim",new object[]{"Fire", false}),
                        new TimelineNode(0.00f, "FireBullet",new object[] {
                        new BulletLauncher(
                        BulletData.data["fireball"], null, Vector3.zero, 0, 16.0f, 5.0f,0f,null,null,false,null,2,0,10f
                    ), "Muzzle"
                    })
                        }, 0.10f, TimelineGoTo.Null)
                }
            };
        }
    }
}

