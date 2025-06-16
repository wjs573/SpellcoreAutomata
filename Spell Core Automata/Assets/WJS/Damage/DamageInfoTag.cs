using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///伤害类型的Tag元素，因为DamageInfo的逻辑需要的严谨性远高于其他的元素，所以伤害类型应该是枚举数组的
    ///这个伤害类型不应该是类似 火伤害、水伤害、毒伤害之类的，如果是这种元素伤害，那么应该是在damage做文章，即damange不是int而是一个struct或者array或者dictionary，然后DamageValue函数里面去改最终值算法
    ///这里的伤害类型，指的还是比如直接伤害、反弹伤害、dot伤害等等，一些在逻辑处理流程会有不同待遇的东西，比如dot伤害可能不会触发一些效果等，当然这最终还是取决于策划设计的规则。
    ///</summary>
    public enum DamageInfoTag
    {
        directDamage = 0,   //直接伤害
        periodDamage = 1,   //间歇性伤害
        reflectDamage = 2,  //反噬伤害
        directHeal = 10,    //直接治疗
        periodHeal = 11,    //间歇性治疗  
    }

}
