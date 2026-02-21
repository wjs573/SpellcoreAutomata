using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///道具的模板属性，这些属性都是策划填表来的，在初始化一个道具Obj的时候会派上用场
    ///但是道具被初始化之后，很多属性在运行中也会发生变化，所以我们不能用道具模板的地址，由此直接作为struct更合算
    ///因为这个demo的ui并不打算精心制作，所以包括icon这样的属性也就省略了
    ///</summary>
    public struct ItemModel
    {
        ///<summary>
        ///道具id
        ///</summary>
        public string id;

        ///<summary>
        ///道具的icon
        ///</summary>
        public string icon;

        ///<summary>
        ///道具名称
        ///</summary>
        public string name;

        ///<summary>
        ///道具Tag
        ///</summary>
        public string[] tags;

        ///<summary>
        ///最大堆叠数，不是所有的游戏道具堆叠的规则都一样的
        ///我这个demo里面，可能存在药水之类的，他们的model几乎不会在运行中被改变（游戏规则如此，而非正常逻辑），所以才能堆叠
        ///我们在ui上看到一个道具图标带一个数字，未必她就真的是带有“堆叠数”这个属性的，很可能是统计了有多少个id一样的道具，按照显示规则显示成这样了罢了
        ///</summary>
        public int maxStack;

        ///<summary>
        ///对于道具而言，这是最核心的部分，就是使用效果，使用效果被抽象为一个timeline
        ///</summary>
        public TimelineModel useEffect;



        public ItemModel(
            string id, string icon, string name, string[] tags,
            TimelineModel useEffect,
            int maxStack = 1
        )
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
            this.tags = tags;
            this.maxStack = maxStack;
            this.useEffect = useEffect;
        }
    }
}
