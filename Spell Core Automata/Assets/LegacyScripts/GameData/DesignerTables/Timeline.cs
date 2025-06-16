using System.Collections.Generic;
using DesignerScripts;
using MoreMountains.Tools;
using UnityEngine;

namespace DesignerTables
{
    public class Timeline
    {
        public static Dictionary<string, TimelineModel> data = new Dictionary<string, TimelineModel>();
        // 添加一个方法来获取完全拷贝的 TimelineModel
        public static TimelineModel GetTimelineCopy(string id)
        {
            if (data.ContainsKey(id))
            {
                // 使用 Clone 方法创建完全拷贝的副本
                return data[id].Clone();
            }
            else
            {
                // 如果 id 不存在，可以根据需要返回一个默认值或者抛出异常
                // 以下是返回 null 的示例
                return data["base"];
            }
        }
        public static void Initialize()
        {
            data = new Dictionary<string, TimelineModel>();
            //空
            data.Add("base", new TimelineModel("base", new TimelineNode[] { }, 0.00f, TimelineGoTo.Null));
            data.Add("FireBall", new TimelineModel("FireBall", new TimelineNode[] 
            {
                new TimelineNode(0.00f,"CasterPlayAnim",new object[] {"CastSpell"}),
                new TimelineNode(0.20f,"FireBullet",new object[] {
                    new BulletLauncher(DesignerTables.Bullet.data["BlueFireBall"],null,Vector3.zero,0f,10f,3f,0.1f)})
             }, 2.00f, TimelineGoTo.Null));
        }
    }
}