using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///Timeline每一个节点上要发生的事情
    ///</summary>
    public struct TimelineNode : IComparable<TimelineNode>
    {
        ///<summary>
        ///Timeline运行多久之后发生，单位：秒
        ///</summary>
        public float timeElapsed;

        ///<summary>
        ///要执行的脚本函数
        ///</summary>
        public TimelineEvent doEvent;

        /// <summary>
        /// 要执行的脚本函数的名称
        /// </summary>
        public string TimelineEventName;

        public float loopIntervalTime;
        public int loopTimes;

        ///<summary>
        ///要执行的函数的参数
        ///</summary>
        public object[] eveParams;

        public TimelineNode(float time, string doEve, params object[] eveArg)
        {
            this.timeElapsed = time;
            this.TimelineEventName = doEve;
            // 检查是否存在指定的 key
            if (TimelineScripts.functions.ContainsKey(doEve))
            {
                this.doEvent = TimelineScripts.functions[doEve];
            }
            else
            {
                // 如果找不到 key，记录日志
                this.doEvent = null;
                Debug.Log("Key not found in functions: " + doEve);
            }
            this.eveParams = eveArg?.ToArray(); // 创建一个新的数组
            this.loopTimes = 1;
            this.loopIntervalTime = 0f;
        }

        public TimelineNode(float time, string doEve, int loopTimes = 1, float loopIntervalTime = 0f, params object[] eveArg)
        {
            this.timeElapsed = time;
            this.TimelineEventName = doEve;
            // 检查是否存在指定的 key
            if (TimelineScripts.functions.ContainsKey(doEve))
            {
                this.doEvent = TimelineScripts.functions[doEve];
            }
            else
            {
                // 如果找不到 key，记录日志
                this.doEvent = null;
                Debug.Log("Key not found in functions: " + doEve);
            }
            this.eveParams = eveArg?.ToArray(); // 创建一个新的数组
            this.loopTimes = loopTimes;
            this.loopIntervalTime = loopIntervalTime;
        }


        public int CompareTo(TimelineNode other)
        {
            return timeElapsed.CompareTo(other.timeElapsed);
        }
    }
}