using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///角色的数值属性部分，比如最大hp、攻击力等等都在这里
    ///这个建一个结构是因为并非只有角色有这些属性，包括装备、buff、aoe、damageInfo等都会用上
    ///</summary>
    public struct ChaProperty
    {
        ///<summary>
        ///最大生命，基本都得有，哪怕角色只有1，装备可以是0
        ///</summary>
        public int hp;
        /// <summary>
        /// 最大灵力,角色使用法术、催动法宝都需要消耗灵力。
        /// </summary>
        public int mp;
        /// <summary>
        /// 每秒生命回复值
        /// </summary>
        public int hp_recover;
        /// <summary>
        /// 每秒灵力回复值
        /// </summary>
        public int mp_recover;
        ///<summary>
        ///攻击力
        ///</summary>
        public int attack;
        /// <summary>
        /// 防御力
        /// </summary>
        public int defence;
        /// <summary>
        /// 暴击率
        /// </summary>
        public float critic_rate;
        /// <summary>
        /// 暴击伤害倍率
        /// </summary>
        public float critic_multiplier;
        /// <summary>
        /// 闪避率
        /// </summary>
        public float dodge_rate;

        /// <summary>
        /// 技能冷却速率,默认为0，100点时减少一半冷却时间，200点时减少三分之二冷却时间，以此类推
        /// </summary>
        public int cd_speed;
        ///<summary>
        ///移动速度，他不是米/秒作为单位的，而是一个可以培养的数值。
        ///具体转化为米/秒，是需要一个规则的，所以是策划脚本 int SpeedToMoveSpeed(int speed)来返回
        ///</summary>
        public int moveSpeed;
        ///<summary>
        ///行动速度，和移动速度不同，他是增加角色行动速度，也就是变化timeline和动画播放的scale的，比如wow里面开嗜血就是加行动速度
        ///具体多少也不是一个0.2f（我这个游戏中规则设定的最快为正常速度的20%，你的游戏你自己定）到5.0f（我这个游戏设定了最慢是正常速度20%），和移动速度一样需要脚本接口返回策划公式
        ///</summary>
        public int actionSpeed;
        ///<summary>
        ///体型圆形半径，用于移动碰撞的，单位：米
        ///这个属性因人而异，但是其实在玩法中几乎不可能经营它，只有buff可能会改变一下，所以直接用游戏中用的数据就行了，不需要转化了
        ///</summary>
        public float bodyRadius;
        ///<summary>
        ///挨打圆形半径，同体型圆形，只是用途不同，用在判断子弹是否命中的时候
        ///</summary>
        public float hitRadius;
        ///<summary>
        ///角色移动类型
        ///</summary>
        public MoveType moveType;
        /// <summary>
        /// 角色可见性 是否处于隐身状态
        /// </summary>
        public bool IsInvisible;

        public ChaProperty(ChaProperty other)
        {
            this.cd_speed = other.cd_speed;
            this.defence = other.defence;
            this.hp_recover = other.hp_recover;
            this.mp_recover = other.mp_recover;
            this.mp = other.mp;
            this.critic_multiplier = other.critic_multiplier;
            this.critic_rate = other.critic_rate;
            this.dodge_rate = other.dodge_rate;
            this.moveSpeed = other.moveSpeed;
            this.hp = other.hp;
            this.attack = other.attack;
            this.actionSpeed = other.actionSpeed;
            this.bodyRadius = other.bodyRadius;
            this.hitRadius = other.hitRadius;
            this.moveType = other.moveType;
            this.IsInvisible = other.IsInvisible;
        }

        public ChaProperty(
            int moveSpeed = 100, int cd_speed = 0, int actionSpeed = 100,
            int hp = 0, int hp_recover = 0, int mp = 0, int mp_recover = 0,
            int attack = 0, int defence = 0,
            float critic_multiplier = 1.5f, float critic_rate = 0.25f, float dodge_rate = 0.05f, float bodyRadius = 0.25f,
            float hitRadius = 0.25f, MoveType moveType = MoveType.ground, bool IsInvisible = false
        )
        {
            this.cd_speed = cd_speed;
            this.defence = defence;
            this.hp_recover = hp_recover;
            this.mp_recover = mp_recover;
            this.mp = mp;
            this.critic_multiplier = critic_multiplier;
            this.critic_rate = critic_rate;
            this.dodge_rate = dodge_rate;

            this.moveSpeed = moveSpeed;
            this.hp = hp;
            this.attack = attack;
            this.actionSpeed = actionSpeed;
            this.bodyRadius = bodyRadius;
            this.hitRadius = hitRadius;
            this.moveType = moveType;
            this.IsInvisible = IsInvisible;
        }

        public static ChaProperty zero = new ChaProperty(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public void SetToDefault()
        {
            this.cd_speed = 0;
            this.defence = 50;
            this.hp_recover = 5;
            this.mp_recover = 5;
            this.mp = 100;
            this.critic_multiplier = 1.5f;
            this.critic_rate = 0.25f;
            this.dodge_rate = 0f;

            this.moveSpeed = 100;
            this.hp = 100;
            this.attack = 25;
            this.actionSpeed = 100;
            this.bodyRadius = 0.75f;
            this.hitRadius = 0.75f;
            this.moveType = MoveType.ground;
            this.IsInvisible = false;
        }

        ///<summary>
        ///将所有值清0
        ///<param name="moveType">移动类型设置为</param>
        ///</summary>
        public void Zero(MoveType moveType = MoveType.ground)
        {
            this.cd_speed = 0;
            this.moveSpeed = 0;
            this.actionSpeed = 0;
            this.defence = 0;
            this.hp_recover = 0;
            this.mp_recover = 0;
            this.mp = 0;
            this.critic_multiplier = 0f;
            this.critic_rate = 0f;
            this.dodge_rate = 0f;
            this.hp = 0;
            this.attack = 0;
            this.bodyRadius = 0;
            this.hitRadius = 0;
            this.moveType = moveType;
            this.IsInvisible = false;
        }

        //定义加法和乘法的用法，其实这个应该走脚本函数返回，抛给脚本函数多个ChaProperty，由脚本函数运作他们的运算关系，并返回结果
        public static ChaProperty operator +(ChaProperty a, ChaProperty b)
        {
            return new ChaProperty(
                a.moveSpeed + b.moveSpeed,
                a.cd_speed + b.cd_speed,
                a.actionSpeed + b.actionSpeed,
                a.hp + b.hp,
                a.hp_recover + b.hp_recover,
                a.mp + b.mp,
                a.mp_recover + b.mp_recover,
                a.attack + b.attack,
                a.defence + b.defence,
                a.critic_multiplier + b.critic_multiplier,
                Mathf.Min(a.critic_rate + b.critic_rate, 1f),
                Mathf.Min(a.dodge_rate + b.dodge_rate, 1f),
                a.bodyRadius + b.bodyRadius,
                a.hitRadius + b.hitRadius,
                a.moveType == MoveType.fly || b.moveType == MoveType.fly ? MoveType.fly : MoveType.ground,
                a.IsInvisible || b.IsInvisible
            );
        }

        public static ChaProperty operator *(ChaProperty a, ChaProperty b)
        {
            return new ChaProperty(
                Mathf.RoundToInt(a.moveSpeed * (1.0000f + Mathf.Max(b.moveSpeed / 100f, -0.9999f))),
                Mathf.RoundToInt(a.cd_speed * (1.0000f + Mathf.Max(b.cd_speed / 100f, -0.9999f))),
                Mathf.RoundToInt(a.actionSpeed * (1.0000f + Mathf.Max(b.actionSpeed / 100f, -0.9999f))),
                Mathf.RoundToInt(a.hp * (1.0000f + Mathf.Max(b.hp / 100f, -0.9999f))),
                Mathf.RoundToInt(a.hp_recover * (1.0000f + Mathf.Max(b.hp_recover / 100f, -0.9999f))),
                Mathf.RoundToInt(a.mp * (1.0000f + Mathf.Max(b.mp / 100f, -0.9999f))),
                Mathf.RoundToInt(a.mp_recover * (1.0000f + Mathf.Max(b.mp_recover / 100f, -0.9999f))),
                Mathf.RoundToInt(a.attack * (1.0000f + Mathf.Max(b.attack / 100f, -0.9999f))),
                Mathf.RoundToInt(a.defence * (1.0000f + Mathf.Max(b.defence / 100f, -0.9999f))),
                a.critic_multiplier * (1.0000f + Mathf.Max(b.critic_multiplier, -0.9999f)),
                a.critic_rate * (1.0000f + Mathf.Max(b.critic_rate, -0.9999f)),
                a.dodge_rate * (1.0000f + Mathf.Max(b.dodge_rate, -0.9999f)),
                a.bodyRadius * (1.0000f + Mathf.Max(b.bodyRadius, -0.9999f)),
                a.hitRadius * (1.0000f + Mathf.Max(b.hitRadius, -0.9999f)),
                a.moveType == MoveType.fly || b.moveType == MoveType.fly ? MoveType.fly : MoveType.ground,
                a.IsInvisible || b.IsInvisible
            );
        }

        public static ChaProperty operator *(ChaProperty a, float b)
        {
            return new ChaProperty(
                Mathf.RoundToInt(a.moveSpeed * b),
                Mathf.RoundToInt(a.cd_speed * b),
                Mathf.RoundToInt(a.actionSpeed * b),

                Mathf.RoundToInt(a.hp * b),
                Mathf.RoundToInt(a.hp_recover * b),
                Mathf.RoundToInt(a.mp * b),
                Mathf.RoundToInt(a.mp_recover * b),

                Mathf.RoundToInt(a.attack * b),
                Mathf.RoundToInt(a.defence * b),

                a.critic_multiplier * b,
                a.critic_rate * b,
                a.dodge_rate * b,
                a.bodyRadius * b,
                a.hitRadius * b,
                a.moveType,
                a.IsInvisible
            );
        }

        public string GetDescription()
        {
            StringBuilder description = new StringBuilder();

            // 最大生命
            if (hp != 0)
            {
                description.AppendLine("最大生命:    " + FormatAttribute(hp));
            }

            // 最大灵力
            if (mp != 0)
            {
                description.AppendLine("最大灵力:    " + FormatAttribute(mp));
            }

            // 每秒生命回复
            if (hp_recover != 0)
            {
                description.AppendLine("生命回复:    " + FormatAttribute(hp_recover));
            }

            // 每秒灵力回复
            if (mp_recover != 0)
            {
                description.AppendLine("灵力回复:    " + FormatAttribute(mp_recover));
            }

            // 暴击率
            if (critic_rate != 0)
            {
                description.AppendLine("暴击率    :    " + FormatAttribute(critic_rate * 100) + "%");
            }

            // 暴击伤害倍率
            if (critic_multiplier != 0)
            {
                description.AppendLine("暴击倍率:    " + FormatAttribute(critic_multiplier));
            }

            // 闪避率
            if (dodge_rate != 0)
            {
                description.AppendLine("闪避率    :    " + FormatAttribute(dodge_rate * 100) + "%");
            }

            // 攻击力
            if (attack != 0)
            {
                description.AppendLine("攻击力    :    " + FormatAttribute(attack));
            }

            // 防御力
            if (defence != 0)
            {
                description.AppendLine("防御力    :    " + FormatAttribute(defence));
            }

            // 移动速度
            if (moveSpeed != 0)
            {
                description.AppendLine("移动速度:    " + FormatAttribute(moveSpeed));
            }

            // 行动速度
            if (actionSpeed != 0)
            {
                description.AppendLine("施法速度:    " + FormatAttribute(actionSpeed));
            }

            // 冷却速度
            if (cd_speed != 0)
            {
                description.AppendLine("冷却速度:    " + FormatAttribute(cd_speed));
            }

            // 返回描述字符串
            return description.ToString();
        }

        // 辅助函数，用于格式化属性值
        private string FormatAttribute(int value)
        {

            if (value > 0)
            {
                return $"<color=green>+{value}</color>";
            }
            else if (value < 0)
            {
                return $"<color=red>-{value}</color>";
            }
            else
            {
                return "0";
            }
        }

        private string FormatAttribute(float value)
        {

            if (value > 0)
            {
                return $"<color=green>+{value}%</color>";
            }
            else if (value < 0)
            {
                return $"<color=red>-{value}%</color>";
            }
            else
            {
                return "0";
            }
        }
    }
}
