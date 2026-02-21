using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///游戏中伤害值的struct，这游戏的伤害类型包括子弹伤害（治疗）、爆破伤害（治疗）、精神伤害（治疗）3种，这两种的概念更像是类似物理伤害、金木水火土属性伤害等等这种元素伤害的概念
    ///但是游戏的逻辑可能会依赖于这个伤害做一些文章，比如“受到子弹伤害减少90%”之类的
    ///</summary>
    public struct Damage
    {
        public int bulletDamage;
        public int aoeDamage;

        public Damage(int bulletDamage, int AoeDamage = 0)
        {
            this.bulletDamage = bulletDamage;
            this.aoeDamage = AoeDamage;
        }

        ///<summary>
        ///统计规则，在这个游戏里伤害和治疗不能共存在一个结果里，作为抵消用
        ///<param name="asHeal">是否当做治疗来统计</name>
        ///</summary>
        public int Overall(bool asHeal = false)
        {
            return (asHeal == false) ?
                (Mathf.Max(0, bulletDamage) + Mathf.Max(0, bulletDamage)) :
                (Mathf.Min(0, aoeDamage) + Mathf.Min(0, aoeDamage));
        }

        public static Damage operator +(Damage a, Damage b)
        {
            return new Damage(
                a.bulletDamage + b.bulletDamage,
                a.aoeDamage + b.aoeDamage
            );
        }

        public static Damage operator *(Damage a, float b)
        {
            return new Damage(
                Mathf.RoundToInt(a.bulletDamage * b),
                Mathf.RoundToInt(a.aoeDamage * b)
            );
        }

        public static Damage operator *(Damage a, Damage b)
        {
            return new Damage(
                Mathf.RoundToInt(a.bulletDamage * b.bulletDamage),
                Mathf.RoundToInt(a.aoeDamage * b.aoeDamage)
            );
        }
    }
}

