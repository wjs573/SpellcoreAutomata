using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///目标地点的信息
    ///</summary>
    public struct MapTargetPosInfo
    {
        ///<summary>
        ///是否会碰到阻碍
        ///</summary>
        public bool obstacle;

        ///<summary>
        ///建议移动到的位置
        ///</summary>
        public Vector3 suggestPos;

        public MapTargetPosInfo(bool obstacle, Vector3 suggestPos)
        {
            this.obstacle = obstacle;
            this.suggestPos = suggestPos;
        }
    }
}
