using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///单个动画信息，主要是在animator中的name，以及多久以后回到可以被改写的程度
    ///</summary>
    public struct SingleAnimInfo
    {
        ///<summary>
        ///animator中的名称
        ///</summary>
        public string animName;

        ///<summary>
        ///在多久之后权重清0，单位秒
        ///</summary>
        public float duration;

        public SingleAnimInfo(string animName, float duration = 0)
        {
            this.animName = animName;
            this.duration = duration;
        }

        public static SingleAnimInfo Null = new SingleAnimInfo("", 0);

    }
}
