using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///游戏中的道具，其实道具实只有玩家有的，而不是某个角色有的，所以道具和角色本质上没有什么直接关系
    ///包括装备也是如此，我们通常以为装备是挂在角色身上的，但实际上只有玩这个游戏的主玩家（最多一起热座的几个）才有装备的概念
    ///而通常因为我们看到了一个角色的外貌变化、数值变化，也猜得到他肯定穿了装备（其他玩家角色），我们就认为装备和角色是直接相关的
    ///这都是幻术，实际上我们只是看到了一个“属性被hack”了的角色而已。
    ///</summary>
    public struct ItemObj
    {
        ///<summary>
        ///道具的model，当然model并不是运行中不可变化的
        ///比如你强化了某个装备，致使起名字变化了，比如铁锹变成钢锹
        ///他不一定是非得直接删除一个itemObj再添加一个的，因为其他属性可能还需要保留
        ///就比如作为当前耐久度来使用的count
        ///</summary>
        public ItemModel model;

        ///<summary>
        ///持有的个数，因为有堆叠规则，所以才有这个
        ///</summary>
        public int count;

        ///<summary>
        ///冷却时间，单位：秒，顾名思义>0的时候道具就没法使用
        ///我们并没有在道具model看到冷却时间，那么这个数字怎么来的？
        ///其1，是游戏规则所致，比如wow里面使用道具都有1.5秒gcd，这时候使用一个道具会导致所有道具都进入1.5秒的cooldown
        ///其2，是道具的使用效果所致，比如使用了某个道具，他的效果就是导致角色身上含有某tag的道具进入5秒冷却
        ///这些规则都可以没有，那么cooldown就不需要了吗？还是留着吧，他是规则，只是我们需不需要用这个规则而已
        ///</summary>
        public float cooldown;

        public ItemObj(ItemModel model, int count = 0, float cooldown = 0)
        {
            this.model = model;
            this.cooldown = cooldown;
            this.count = count;
        }
    }

}
