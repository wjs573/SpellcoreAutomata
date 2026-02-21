using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///游戏中的装备，其实道具实只有玩家有的，而不是某个角色有的，所以道具和角色本质上没有什么直接关系
    ///我们通常以为装备是挂在角色身上的，但实际上只有玩这个游戏的主玩家（最多一起热座的几个）才有装备的概念
    ///而通常因为我们看到了一个角色的外貌变化、数值变化，也猜得到他肯定穿了装备（其他玩家角色），我们就认为装备和角色是直接相关的
    ///这都是幻术，实际上我们只是看到了一个“属性被hack”了的角色而已。
    ///</summary>
    public class EquipmentObj
    {
        ///<summary>
        ///Model是什么
        ///</summary>
        public EquipmentModel model;

        //由于没有其他的内容，所以只有一个属性，但是为了扩展性和结构本身，obj还是需要的，万一要强化升星打孔镶钻了呢？

        public EquipmentObj(EquipmentModel model)
        {
            this.model = model;
        }
    }
}