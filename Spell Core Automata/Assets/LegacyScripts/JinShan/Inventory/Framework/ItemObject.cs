using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JinShan
{
    //物品品级
    public enum ItemRank
    {
        默认,
        凡品,
        稀有,
        罕见,
        绝世
    }

    //物品类型
    public enum ItemType
    {
        Default,
        法宝,
        功法,
        技能,
        神通,
        技能强化,
        触发器,
        怪兽
    }

    //物品属性
    public enum Attributes
    {
        生命值,
        生命回复值,
        法力值,
        法力回复值,
        护盾值,
        攻击力,
        防御力,
        神魂强度,
        暴击率,
        暴击伤害倍率,
        闪避率,
        金灵根,
        木灵根,
        水灵根,
        火灵根,
        土灵根,
        行动速率,
        移动速率,
        冷却速率
    }

    //物品 包括 精灵 是否堆叠的属性 描述 物品属性
    [CreateAssetMenu(fileName = "New Item Object", menuName = "Inventory System/Items/Item")]
    public class ItemObject : SerializedScriptableObject
    {
        [HorizontalGroup("ItemData", 100)]
        [HideLabel, PreviewField(Height = 100)]
        public Sprite uiDisplay;

        [HideLabel, VerticalGroup("ItemData/BaseData")]
        [LabelWidth(50)]
        public ItemType type;

        [VerticalGroup("ItemData/BaseData")]
        [LabelWidth(100)]
        public bool stackable;

        [VerticalGroup("ItemData/BaseData")]
        [LabelWidth(100)]
        public int DrawCount = 0;

        [HideLabel, VerticalGroup("ItemData/BaseData"), TextArea(3, 5)]
        public string description;

        public Item data = new Item();

        public Item CreateItem()
        {
            Item newItem = new Item(this);
            return newItem;
        }
    }

    //物品属性 包括名字 id 影响属性
    [System.Serializable]
    public class Item
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 母版物体
        /// </summary>
        [HideInInspector]
        public ItemObject itemObject;
        /// <summary>
        /// 物品在数据库中的序号
        /// </summary>
        public int Id = -1;

        /// <summary>
        /// 物品品级
        /// </summary>
        public ItemRank Rank = ItemRank.默认;

        /// <summary>
        /// 物品价值
        /// </summary>
        public int Value = 0;

        /// <summary>
        /// 物品属性 包括加成在角色身上的属性
        /// </summary>
        public ItemBuff[] buffs;

        // 获取技能模型的方法
        public SkillModel GetSkillModel()
        {
            SkillScriptableObject skillSO = itemObject as SkillScriptableObject;
            return skillSO != null ? skillSO.Model : new SkillModel();
        }

        public SkillObj skillObj;


        public ChaProperty property
        { get { return GetPropertyFromAttribute(buffs); } }

        public Item()
        {
            Name = "";
            Id = -1;
            itemObject = null;
        }

        public Item(ItemObject item)
        {
            Name = item.name;
            itemObject = item;
            Id = item.data.Id;
            Rank = item.data.Rank;
            buffs = new ItemBuff[item.data.buffs.Length];
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max)
                {
                    attribute = item.data.buffs[i].attribute
                };
            }
        }

        /// <summary>
        /// Item将物体的buff属性转化成property属性 便于后续运算
        /// </summary>
        /// <param name="buffs"></param>
        /// <returns></returns>
        public ChaProperty GetPropertyFromAttribute(ItemBuff[] buffs)
        {
            ///<summary>
            ///最大生命，基本都得有，哪怕角色只有1，装备可以是0
            ///</summary>
            int hp = 0;

            /// <summary>
            /// 最大灵力,角色使用法术、催动法宝都需要消耗灵力。
            /// </summary>
            int mp = 0;

            /// <summary>
            /// 每秒生命回复值
            /// </summary>
            int hp_recover = 0;

            /// <summary>
            /// 每秒灵力回复值
            /// </summary>
            int mp_recover = 0;

            /// <summary>
            /// 暴击率
            /// </summary>
            float critic_rate = 0;

            /// <summary>
            /// 暴击伤害倍率
            /// </summary>
            float critic_multiplier = 0;

            /// <summary>
            /// 闪避率
            /// </summary>
            float dodge_rate = 0;

            /// <summary>
            /// 护盾值，受到伤害时优先扣除护盾值，护盾归零后才会扣除生命值。
            /// </summary>
            int shield = 0;

            /// <summary>
            /// 防御力
            /// </summary>
            int defence = 0;

            /// <summary>
            /// 神魂强度，装备法宝会消耗神魂强度
            /// </summary>
            int mind = 0;

            /// <summary>
            /// 金灵根，玩家造成金属性伤害时会得到提升
            /// </summary>
            int mental_talent = 0;

            /// <summary>
            /// 木灵根
            /// </summary>
            int wood_talent = 0;

            /// <summary>
            /// 水灵根
            /// </summary>
            int water_talent = 0;

            /// <summary>
            /// 火灵根
            /// </summary>
            int fire_talent = 0;

            /// <summary>
            /// 土灵根
            /// </summary>
            int earth_talent = 0;

            /// <summary>
            /// 技能冷却速率,默认为0，100点时减少一半冷却时间，200点时减少三分之二冷却时间，以此类推
            /// </summary>
            int cd_speed = 0;

            ///<summary>
            ///攻击力
            ///</summary>
            int attack = 0;

            ///<summary>
            ///移动速度，他不是米/秒作为单位的，而是一个可以培养的数值。
            ///具体转化为米/秒，是需要一个规则的，所以是策划脚本 int SpeedToMoveSpeed(int speed)来返回
            ///</summary>
            int moveSpeed = 0;

            ///<summary>
            ///行动速度，和移动速度不同，他是增加角色行动速度，也就是变化timeline和动画播放的scale的，比如wow里面开嗜血就是加行动速度
            ///具体多少也不是一个0.2f（我这个游戏中规则设定的最快为正常速度的20%，你的游戏你自己定）到5.0f（我这个游戏设定了最慢是正常速度20%），和移动速度一样需要脚本接口返回策划公式
            ///</summary>
            int actionSpeed = 0;

            for (int i = 0; i < buffs.Length; i++)
            {
                switch (buffs[i].attribute)
                {
                    case Attributes.生命值:
                        hp = buffs[i].value;
                        break;

                    case Attributes.生命回复值:
                        hp_recover = buffs[i].value;
                        break;

                    case Attributes.法力值:
                        mp = buffs[i].value;
                        break;

                    case Attributes.法力回复值:
                        mp_recover = buffs[i].value;
                        break;

                    case Attributes.护盾值:
                        shield = buffs[i].value;
                        break;

                    case Attributes.攻击力:
                        attack = buffs[i].value;
                        break;

                    case Attributes.防御力:
                        defence = buffs[i].value;
                        break;

                    case Attributes.神魂强度:
                        mind = buffs[i].value;
                        break;

                    case Attributes.暴击率:
                        critic_rate = buffs[i].value / 100;
                        break;

                    case Attributes.暴击伤害倍率:
                        critic_multiplier = buffs[i].value / 100;
                        break;

                    case Attributes.闪避率:
                        dodge_rate = buffs[i].value / 100;
                        break;

                    case Attributes.行动速率:
                        actionSpeed = buffs[i].value;
                        break;

                    case Attributes.金灵根:
                        mental_talent = buffs[i].value;
                        break;

                    case Attributes.木灵根:
                        wood_talent = buffs[i].value;
                        break;

                    case Attributes.水灵根:
                        water_talent = buffs[i].value;
                        break;

                    case Attributes.火灵根:
                        fire_talent = buffs[i].value;
                        break;

                    case Attributes.土灵根:
                        earth_talent = buffs[i].value;
                        break;

                    case Attributes.冷却速率:
                        cd_speed = buffs[i].value;
                        break;

                    case Attributes.移动速率:
                        moveSpeed = buffs[i].value;
                        break;

                    default:
                        break;
                }
            }
            return new ChaProperty(moveSpeed, cd_speed, actionSpeed,
                hp, hp_recover, mp, mp_recover, shield,
                attack, defence, mind,
                critic_multiplier, critic_rate, dodge_rate,
                0.25f, 0.25f, MoveType.ground);
        }

        public Item Clone()
        {
            Item cloneItem = new Item();

            cloneItem.Name = this.Name;

            cloneItem.itemObject = this.itemObject;
            cloneItem.Id = this.itemObject.data.Id;
            cloneItem.Rank = this.itemObject.data.Rank;
            cloneItem.buffs = new ItemBuff[this.itemObject.data.buffs.Length];
            for (int i = 0; i < cloneItem.buffs.Length; i++)
            {
                cloneItem.buffs[i] = new ItemBuff(this.itemObject.data.buffs[i].min, this.itemObject.data.buffs[i].max)
                {
                    attribute = this.itemObject.data.buffs[i].attribute
                };
            }
            return cloneItem;
        }
    }

    [System.Serializable]
    public class ItemBuff : Imodifiers
    {
        [InlineProperty]
        [HideLabel]
        public Attributes attribute;

        [HideInInspector]
        public int value; // 不在编辑器中显示，只在运行时使用

        [HorizontalGroup("Range"), LabelWidth(50)]
        public int min;

        [HorizontalGroup("Range"), LabelWidth(50)]
        public int max;

        public ItemBuff(int _min, int _max)
        {
            min = _min;
            max = _max;
            GenerateValue();
        }

        public void AddValue(ref int baseValue)
        {
            baseValue += value;
        }

        public void GenerateValue()
        {
            value = UnityEngine.Random.Range(min, max);
        }
    }

}