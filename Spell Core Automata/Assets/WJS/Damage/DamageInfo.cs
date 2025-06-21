using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class DamageInfo
    {
        ///<summary>
        ///造成伤害的攻击者，当然可以是null的
        ///</summary>
        public GameObject attacker;

        /// <summary>
        /// 攻击者的chastate clone
        /// </summary>
        public ChaProperty attackerProperty;

        ///<summary>
        ///造成攻击伤害的受击者，这个必须有
        ///</summary>
        public GameObject defender;

        ///<summary>
        ///这次伤害的类型Tag，这个会被用于buff相关的逻辑，是一个极其重要的信息
        ///这里是策划根据游戏设计来定义的，比如游戏中可能存在"frozen" "fire"之类的伤害类型，还会存在"directDamage" "period" "reflect"之类的类型伤害
        ///根据这些伤害类型，逻辑处理可能会有所不同，典型的比如"reflect"，来自反伤的，那本身一个buff的作用就是受到伤害的时候反弹伤害，如果双方都有这个buff
        ///并且这个buff没有判断damageInfo.tags里面有reflect，则可能造成“短路”，最终有一下有一方就秒了。
        ///</summary>
        public DamageInfoTag[] tags;

        ///<summary>
        ///伤害值，其实伤害值是多元的，通常游戏都会有多个属性伤害，所以会用一个struct，否则就会是一个int
        ///尽管起名叫Damage，但实际上治疗也是这个，只是负数叫做治疗量，这个起名看似不严谨，对于游戏（这个特殊的业务）而言却又是严谨的
        ///</summary>
        public Damage damage;

        ///<summary>
        ///是否暴击，这是游戏设计了有暴击的可能性存在。
        ///这里记录一个总暴击率，随着buff的不断改写，最后这个暴击率会得到一个0-1的值，代表0%-100%。
        ///最终处理的时候，会根据这个值来进行抉择，可以理解为，当这个值超过1的时候，buff就可以认为这次攻击暴击了。
        ///</summary>
        public float criticalRate;

        ///<summary>
        ///是否命中，是否命中与是否暴击并不直接相关，都是单独的算法
        ///作为一个射击游戏，子弹命中敌人是一种技巧，所以在这里设计命中了还会miss是愚蠢的
        ///因此这里的hitRate始终是2，就是必定命中的，之所以把这个属性放着，也是为了说明问题，而不是这个属性真的对这个游戏有用。
        ///要不要这个属性还是取决于游戏设计，比如当前游戏，本不该有这个属性。
        ///</summary>
        public float hitRate = 2.00f;

        ///<summary>
        ///伤害的角度，作为伤害打向角色的入射角度，比如子弹，就是它当前的飞行角度
        ///</summary>
        public float degree;

        /// <summary>
        /// 本次伤害的结果 在策划脚本中进行复制
        /// </summary>
        public DamageResult result;

        ///<summary>
        ///伤害过后，给角色添加的buff
        ///</summary>
        public List<AddBuffInfo> addBuffs = new List<AddBuffInfo>();

        public DamageInfo(GameObject attacker, GameObject defender, Damage damage, float damageDegree, float baseCriticalRate, DamageInfoTag[] tags)
        {
            this.attacker = attacker;
            this.attackerProperty = attacker != null ? new ChaProperty(attacker.GetComponent<ChaState>().property) : ChaProperty.zero;
            this.defender = defender;
            this.damage = damage;
            this.criticalRate = baseCriticalRate;
            this.degree = damageDegree;
            this.tags = new DamageInfoTag[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                this.tags[i] = tags[i];
            }
        }

        ///<summary>
        ///从策划脚本获得最终的伤害值
        ///</summary>
        public DamageInfo DamageValue(bool asHeal)
        {
            return CommonScripts.DamageValue(this, asHeal);
        }

        ///<summary>
        ///根据tag判断，这是否是一次治疗，那些tag算是治疗，当然是策划定义了才算数的
        ///</summary>
        public bool isHeal()
        {
            for (int i = 0; i < this.tags.Length; i++)
            {
                if (tags[i] == DamageInfoTag.directHeal || tags[i] == DamageInfoTag.periodHeal)
                {
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        ///根据tag决定是否要播放受伤动作，当然你还可以是根据类型决定不同的受伤动作，但是我这个demo就没这么复杂了
        ///</summary>
        public bool requireDoHurt()
        {
            for (int i = 0; i < this.tags.Length; i++)
            {
                if (tags[i] == DamageInfoTag.directDamage)
                {
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        ///将添加buff信息添加到伤害信息中来
        ///buffOnHit\buffBeHurt\buffOnKill\buffBeKilled等伤害流程张的buff添加通常走这里
        ///<param name="info">要添加的buff的信息</param>
        ///</summary>
        public void AddBuffToCha(AddBuffInfo buffInfo)
        {
            this.addBuffs.Add(buffInfo);
        }
    }
    public struct DamageResult
    {
        public int damage;
        public bool isCritical;
        public bool isHit;
        public bool isHeal;
    }
}

