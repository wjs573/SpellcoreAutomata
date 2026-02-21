using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///装备的模板属性，策划填表数据
    ///我们通常因为在一个背包内看到了道具和装备，就认为他们是一样的东西
    ///但实际上他们是存在于2个不同的容器中的不同数据，只是这两个容器的“上限之和”就是我们肉眼看到的“背包容量”
    ///</summary>
    public struct EquipmentModel
    {
        ///<summary>
        ///装备id
        ///</summary>
        public string id;

        ///<summary>
        ///装备的icon
        ///</summary>
        public string icon;

        ///<summary>
        ///装备名称
        ///</summary>
        public string name;

        ///<summary>
        ///装备Tag
        ///</summary>
        public string[] tags;

        ///<summary>
        ///装备的部位
        ///</summary>
        public EquipmentType type;

        ///<summary>
        ///对于装备而言，装上以后可以获得的属性
        ///而对于使用类的道具，则不该依赖于这个属性做事，因为给人增加临时属性的是使用效果timeline里面创建的buff
        ///</summary>
        public ChaProperty equipmentProperty;

        ///<summary>
        ///如果是装备，则在装备之后会有buff，移除之后去掉buff，但是使用效果的buff不应该在这里
        ///</summary>
        public AddBuffInfo[] buffs;

        public WandData wandData;

        public EquipmentModel(
            string id, string icon, string name, string[] tags,
            ChaProperty equipment,
            AddBuffInfo[] buffs,
            EquipmentType slot = EquipmentType.weapon,
            WandData wandData = new WandData()
        )
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
            this.tags = tags;
            this.type = slot;
            this.equipmentProperty = equipment;
            this.buffs = buffs;
            this.wandData = wandData;
        }
    }
}
