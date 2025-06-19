using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///技能是角色拥有的东西，因为角色有技能，玩家或者ai才能操作角色释放技能
    ///</summary>
    public class SkillObj
    {
        ///<summary>
        ///技能的模板，创建于skillModel，但运行中还是会允许改变
        ///</summary>
        public SkillModel model;

        ///<summary>
        ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
        ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
        ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
        ///</summary>
        public float cooldown;

        public SkillObj(SkillModel model)
        {
            this.model = model;
            this.cooldown = 0;
        }
    }

}
