using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///Timeline的一个跳转点信息
    ///</summary>
    public struct TimelineGoTo
    {
        ///<summary>
        ///自身处于时间点
        ///</summary>
        public float atDuration;

        ///<summary>
        ///跳转到时间点
        ///</summary>
        public float gotoDuration;

        public TimelineGoTo(float atDuration, float gotoDuration)
        {
            this.atDuration = atDuration;
            this.gotoDuration = gotoDuration;
        }

        public static TimelineGoTo Null = new TimelineGoTo(float.MaxValue, float.MaxValue);
    }
}
