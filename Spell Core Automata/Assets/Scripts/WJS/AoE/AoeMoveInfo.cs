using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///aoe的移动信息
    ///</summary>
    public class AoeMoveInfo
    {
        ///<summary>
        ///此时此刻的移动方式
        ///</summary>
        public MoveType moveType;

        ///<summary>
        ///此时aoe移动的力量，在这个游戏里，y坐标依然无效，如果要做手雷一跳一跳的，请使用其他的component绑定到特效的gameobject上，而非aoe的
        ///</summary>
        public Vector3 velocity;

        ///<summary>
        ///aoe的角度变成这个值
        ///</summary>
        public float rotateToDegree;

        public AoeMoveInfo(MoveType moveType, Vector3 velocity, float rotateToDegree)
        {
            this.moveType = moveType;
            this.velocity = velocity;
            this.rotateToDegree = rotateToDegree;
        }
    }
}