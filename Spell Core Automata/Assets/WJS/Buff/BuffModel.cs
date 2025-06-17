using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public struct BuffModel
    {
        ///<summary>
        ///buff的id
        ///</summary>
        public string id;

        ///<summary>
        ///buff的名称
        ///</summary>
        public string name;

        /// <summary>
        /// 位于Resource/Prefabs/下的路径
        /// 例如 /Effect/Buff/Stunned
        /// </summary>
        public string prefab;

        /// <summary>
        /// 特效所处的位置
        /// 例如 Muzzle/Head/Body/Foot
        /// </summary>
        public string bindPointKey;

        ///<summary>
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int priority;

        ///<summary>
        ///buff堆叠的规则中需要的层数，在这个游戏里只要id和caster相同的buffObj就可以堆叠
        ///激战2里就不同，尽管图标显示堆叠，其实只是统计了有多少个相同id的buffObj作为层数显示了
        ///</summary>
        public int maxStack;

        ///<summary>
        ///buff的tag
        ///</summary>
        public string[] tags;

        ///<summary>
        ///buff的工作周期，单位：秒。
        ///每多少秒执行工作一次，如果<=0则代表不会周期性工作，只要>0，则最小值为Time.FixedDeltaTime。
        ///</summary>
        public float tickTime;

        ///<summary>
        ///buff会给角色添加的属性，这些属性根据这个游戏设计只有2种，plus和times，所以这个数组实际上只有2维
        ///</summary>
        public ChaProperty[] propMod;

        ///<summary>
        ///buff对于角色的ChaControlState的影响
        ///</summary>
        public ChaControlState stateMod;

        ///<summary>
        ///buff在被添加、改变层数时候触发的事件
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="modifyStack">会传递本次改变的层数</param>
        ///</summary>
        public EventManager<BuffOnOccur> onOccur;
        public object[] onOccurParams;

        ///<summary>
        ///buff在每个工作周期会执行的函数，如果这个函数为空，或者tickTime<=0，都不会发生周期性工作
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///</summary>
        public EventManager<BuffOnTick> onTick;
        public object[] onTickParams;

        ///<summary>
        ///在这个buffObj被移除之前要做的事情，如果运行之后buffObj又不足以被删除了就会被保留
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///</summary>
        public EventManager<BuffOnRemoved> onRemoved;
        public object[] onRemovedParams;

        ///<summary>
        ///在释放技能的时候运行的buff，执行这个buff获得最终技能要产生的Timeline
        ///<param name="buff">会传递给脚本的buffObj</param>
        ///<param name="skill">即将释放的技能skillObj</param>
        ///<param name="timeline">释放出来的技能，也就是一个timeline，这里的本质就是让你通过buff还能对timeline进行hack以达到修改技能效果的目的</return>
        ///</summary>
        public EventManager<BuffOnCast> onCast;
        public object[] onCastParams;

        ///<summary>
        ///在伤害流程中，持有这个buff的人作为攻击者会发生的事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="target">挨打的角色对象</param>
        ///</summary>
        public EventManager<BuffOnHit> onHit;
        public object[] onHitParams;

        ///<summary>
        ///在伤害流程中，持有这个buff的人作为挨打者会发生的事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="attacker">打我的角色，当然可以是空的</param>
        ///</summary>
        public EventManager<BuffOnBeHurt> onBeHurt;
        public object[] onBeHurtParams;

        ///<summary>
        ///在伤害流程中，如果击杀目标，则会触发的啥事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="target">挨打的角色对象</param>
        ///</summary>
        public EventManager<BuffOnKill> onKill;
        public object[] onKillParams;

        ///<summary>
        ///在伤害流程中，持有这个buff的人被杀死了，会触发的事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="attacker">发起攻击造成击杀的角色对象</param>
        ///</summary>
        public EventManager<BuffOnBeKilled> onBeKilled;
        public object[] onBeKilledParams;

        public BuffModel(
            string id, string name, string prefab, string bindPointKey, string[] tags, int priority, int maxStack, float tickTime,
            string onOccur, object[] occurParam,
            string onRemoved, object[] removedParam,
            string onTick, object[] tickParam,
            string onCast, object[] castParam,
            string onHit, object[] hitParam,
            string beHurt, object[] hurtParam,
            string onKill, object[] killParam,
            string beKilled, object[] beKilledParam,
            ChaControlState stateMod, ChaProperty[] propMod = null
        )
        {
            this.id = id;
            this.name = name;
            this.prefab = prefab;
            this.bindPointKey = bindPointKey;
            this.tags = tags;
            this.priority = priority;
            this.maxStack = maxStack;
            this.stateMod = stateMod;
            this.tickTime = tickTime;

            this.propMod = new ChaProperty[2]{
            ChaProperty.zero,
            ChaProperty.zero
        };
            if (propMod != null)
            {
                for (int i = 0; i < Mathf.Min(2, propMod.Length); i++)
                {
                    this.propMod[i] = propMod[i];
                }
            }

            this.onOccur = (onOccur == "") ? null : new EventManager<BuffOnOccur>(BuffData.onOccurFunc[onOccur]);
            this.onOccurParams = occurParam;
            this.onRemoved = (onRemoved == "") ? null : new EventManager<BuffOnRemoved>(BuffData.onRemovedFunc[onRemoved]);
            this.onRemovedParams = removedParam;
            this.onTick = (onTick == "") ? null : new EventManager<BuffOnTick>(BuffData.onTickFunc[onTick]);
            this.onTickParams = tickParam;
            this.onCast = (onCast == "") ? null : new EventManager<BuffOnCast>(BuffData.onCastFunc[onCast]);
            this.onCastParams = castParam;
            this.onHit = (onHit == "") ? null : new EventManager<BuffOnHit>(BuffData.onHitFunc[onHit]);
            this.onHitParams = hitParam;
            this.onBeHurt = (beHurt == "") ? null : new EventManager<BuffOnBeHurt>(BuffData.beHurtFunc[beHurt]);
            this.onBeHurtParams = hurtParam;
            this.onKill = (onKill == "") ? null : new EventManager<BuffOnKill>(BuffData.onKillFunc[onKill]);
            this.onKillParams = killParam;
            this.onBeKilled = (beKilled == "") ? null : new EventManager<BuffOnBeKilled>(BuffData.beKilledFunc[beKilled]);
            this.onBeKilledParams = beKilledParam;
        }
    }

    public delegate void BuffOnOccur(BuffObj buff, int modifyStack);
    public delegate void BuffOnRemoved(BuffObj buff);
    public delegate void BuffOnTick(BuffObj buff);
    public delegate void BuffOnHit(BuffObj buff, ref DamageInfo damageInfo, GameObject target);
    public delegate void BuffOnBeHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker);
    public delegate void BuffOnKill(BuffObj buff, DamageInfo damageInfo, GameObject target);
    public delegate void BuffOnBeKilled(BuffObj buff, DamageInfo damageInfo, GameObject attacker);
    public delegate TimelineObj BuffOnCast(BuffObj buff, SkillObj skill, TimelineObj timeline);

}
