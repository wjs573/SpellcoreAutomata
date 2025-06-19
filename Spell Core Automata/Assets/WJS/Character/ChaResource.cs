using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///角色的资源类属性，比如hp，mp等都属于这个
    ///</summary>
    public class ChaResource
    {
        ///<summary>
        ///生命值
        ///</summary>
        public int hp;

        /// <summary>
        /// 法力值
        /// </summary>
        public int mp;

        public ChaResource(int hp, int mp = 0)
        {
            this.hp = hp;
            this.mp = mp;
        }

        ///<summary>
        ///是否足够
        ///</summary>
        public bool Enough(ChaResource requirement)
        {
            return (
                this.hp >= requirement.hp &&
                this.mp >= requirement.mp 
            );
        }

        public static ChaResource operator +(ChaResource a, ChaResource b)
        {
            return new ChaResource(
                a.hp + b.hp,
                a.mp + b.mp
            );
        }

        public static ChaResource operator *(ChaResource a, float b)
        {
            return new ChaResource(
                Mathf.FloorToInt(a.hp * b),
                Mathf.FloorToInt(a.mp * b)
            );
        }

        public static ChaResource operator *(float a, ChaResource b)
        {
            return new ChaResource(
                Mathf.FloorToInt(b.hp * a),
                Mathf.FloorToInt(b.mp * a)
            );
        }

        /// <summary>
        /// 用于计算技能伤害的乘法重载
        /// ChaResource(100)*ChaResource(1) = ChaResource(100)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ChaResource operator *(ChaResource a, ChaResource b)
        {
            return new ChaResource(
                Mathf.RoundToInt(a.hp * (1.0000f + Mathf.Max(b.hp, -0.9999f))),
                Mathf.RoundToInt(a.mp * (1.0000f + Mathf.Max(b.mp, -0.9999f)))
            );
        }

        public static ChaResource operator -(ChaResource a, ChaResource b)
        {
            return a + b * (-1);
        }

        public static ChaResource Null = new ChaResource(0);
    }
}
