using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class TimelineScripts : MonoBehaviour
    {
        // Start is called before the first frame update
        public static Dictionary<string, TimelineEvent> functions = new Dictionary<string, TimelineEvent>();

        public static void Initialize()
        {
            functions = new Dictionary<string, TimelineEvent>()
            {

            };
        }
        /// <summary>
        /// 辅助函数，尝试从paramsDict获取参数，如果找不到则使用默认参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="timeline"></param>
        /// <param name="paramsName">参数名称</param>
        /// <param name="index">参数数组序号</param>
        /// <param name="args">参数数组</param>
        /// <param name="defaultValue">默认参数</param>
        /// <returns></returns>
        private static T GetValueFromParams<T>(int index, object[] args, T defaultValue)
        {
            return args.Length > index ? (T)args[index] : defaultValue;
        }
    }

}
