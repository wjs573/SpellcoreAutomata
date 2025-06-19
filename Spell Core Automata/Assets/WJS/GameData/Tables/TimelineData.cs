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
                // 如果 id 不存在，可以根据需要返回一个默认值或者抛出异常
                // 以下是返回 null 的示例
                return data["base"];
            }
        }
        public static Dictionary<string, TimelineModel> data = new Dictionary<string, TimelineModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, TimelineModel>();
            //空
            data.Add("base", new TimelineModel("base", new TimelineNode[] { }, 0.00f, TimelineGoTo.Null));
        }
    }
}

