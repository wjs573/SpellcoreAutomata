using System.Collections.Generic;
using JinShan;
using UnityEngine;

///<summary>
///负责处理游戏中所有的DamageInfo
///</summary>
public class DamageManager : MonoSingleton<DamageManager>
{
    private List<DamageInfo> damageInfos = new List<DamageInfo>();

    private void FixedUpdate()
    {
        lastPlayerHitTime += Time.fixedDeltaTime;
        int i = 0;
        while (i < damageInfos.Count)
        {
            DealWithDamage(damageInfos[i]);
            damageInfos.RemoveAt(0);
        }
    }

    ///<summary>
    ///处理DamageInfo的流程，也就是整个游戏的伤害流程
    ///<param name="dInfo">要处理的damageInfo</param>
    ///<retrun>处理完之后返回出一个damageInfo，依照这个，给对应角色扣血处理</return>
    ///</summary>
    private void DealWithDamage(DamageInfo dInfo)
    {
        //如果目标已经挂了，就直接return了
        if (!dInfo.defender) return;
        //如果目标身上没有角色状态组件，也直接return
        ChaState defenderChaState = dInfo.defender.GetComponent<ChaState>();
        if (!defenderChaState) return;

        ChaState attackerChaState = null;
        if (defenderChaState.dead == true)
            return;

        //计算闪避与暴击    
        float dodge_rate = (dInfo.defender != null && dInfo.defender.GetComponent<ChaState>() != null) ?
            dInfo.defender.GetComponent<ChaState>().property.dodge_rate : 0.05f;

        //治疗必定命中 
        bool isHeal = dInfo.isHeal();
        if (isHeal) dodge_rate = 0f;

        bool isHit = Random.Range(0.00f, 1.00f) <= (1 - dodge_rate);

        dInfo.criticalRate = dInfo.attackerProperty.critic_rate;
        bool isCrit = Random.Range(0.00f, 1.00f) <= dInfo.criticalRate;
        dInfo.result.isCritical = isCrit;
        dInfo.result.isHit = isHit;
        dInfo.result.isHeal = isHeal;
        //先走一遍所有攻击者的onHit
        if (dInfo.attacker)
        {
            attackerChaState = dInfo.attacker.GetComponent<ChaState>();
            for (int i = 0; i < attackerChaState.buffs.Count; i++)
            {
                if (attackerChaState.buffs[i].model.onHit != null)
                {
                    dInfo = attackerChaState.buffs[i].model.onHit.Invoke<DamageInfo>(attackerChaState.buffs[i], dInfo, dInfo.defender);
                }
            }
        }
        //然后走一遍挨打者的beHurt
        for (int i = 0; i < defenderChaState.buffs.Count; i++)
        {
            if (defenderChaState.buffs[i].model.onBeHurt != null)
            {
                defenderChaState.buffs[i].model.onBeHurt.Invoke(defenderChaState.buffs[i], dInfo, dInfo.attacker);
            }
        }
        if (defenderChaState.CanBeKilledByDamageInfo(dInfo) == true)
        {
            //如果角色可能被杀死，就会走OnKill和OnBeKilled，这个游戏里面没有免死金牌之类的技能，所以只要判断一次就好
            if (attackerChaState != null)
            {
                for (int i = 0; i < attackerChaState.buffs.Count; i++)
                {
                    //现在有复活类buff 所以需要多次判定
                    if (defenderChaState.CanBeKilledByDamageInfo(dInfo) != true) continue;
                    if (attackerChaState.buffs[i].model.onKill != null)
                    {
                        attackerChaState.buffs[i].model.onKill.Invoke(attackerChaState.buffs[i], dInfo, dInfo.defender);
                    }
                }
            }
            for (int i = 0; i < defenderChaState.buffs.Count; i++)
            {
                //现在有复活类buff 所以需要多次判定
                if (defenderChaState.CanBeKilledByDamageInfo(dInfo) != true) continue;
                if (defenderChaState.buffs[i].model.onBeKilled != null)
                {
                    defenderChaState.buffs[i].model.onBeKilled.Invoke(defenderChaState.buffs[i], dInfo, dInfo.attacker);
                }
            }
        }

        int dVal = dInfo.DamageValue(isHeal).result.damage;

        if (isHeal == true || defenderChaState.immuneTime <= 0)
        {
            if (dInfo.requireDoHurt() == true && defenderChaState.CanBeKilledByDamageInfo(dInfo) == false)
            {
                UnitAnim ua = defenderChaState.GetComponent<UnitAnim>();
                if (ua) ua.Play("Hurt");
            }
            defenderChaState.ModResource(new ChaResource(
                -dVal
            ));

            //按游戏设计的规则跳数字，如果要有暴击，也可以丢在策划脚本函数（lua可以返回多参数）也可以随便怎么滴
            PopTextManager.Instance.PopUpDamageNumberOnCharacter(dInfo);

            UpdateUICombatHud(dInfo);
        }

        //伤害流程走完，添加buff
        for (int i = 0; i < dInfo.addBuffs.Count; i++)
        {
            GameObject toCha = dInfo.addBuffs[i].target;
            ChaState toChaState = toCha.Equals(dInfo.attacker) ? attackerChaState : defenderChaState;

            if (toChaState != null && toChaState.dead == false)
            {
                toChaState.AddBuff(dInfo.addBuffs[i]);
            }
        }

        //打击感优化
        //TODO：受到伤害方向的击退效果
        //获得僵直
        if (defenderChaState.gameObject != GameManager.Instance.mainActor && !isHeal && dInfo.attacker != dInfo.defender)
        {
            AddBuffInfo StunLockBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["StunLock"], dInfo.attacker, dInfo.defender, 1, 0.50f, true);
            defenderChaState.AddBuff(StunLockBuffInfo);
            dInfo.defender.GetComponent<UnitFeedback>().PlayDamageFeedbacks();
        }

    }


    public GameObject TargetBeingAttackedByPlayer;
    public float lastPlayerHitTime;
    private void UpdateUICombatHud(DamageInfo dInfo)
    {
        if (dInfo.attacker == GameManager.Instance.mainActor && dInfo.defender.GetComponent<ChaState>().side != dInfo.attacker.GetComponent<ChaState>().side)
        {
            lastPlayerHitTime = 0f;
            TargetBeingAttackedByPlayer = dInfo.defender;
            UIManager.Instance.GetWindow<UICombatHUDWindow>().SetEliteHpBar(TargetBeingAttackedByPlayer);
        }
    }

    ///<summary>
    ///添加一个damageInfo
    ///<param name="attacker">攻击者，可以为null</param>
    ///<param name="target">挨打对象</param>
    ///<param name="damage">基础伤害值</param>
    ///<param name="damageDegree">伤害的角度</param>
    ///<param name="criticalRate">暴击率，0-1</param>
    ///<param name="tags">伤害信息类型</param>
    ///</summary>
    public void DoDamage(GameObject attacker, GameObject target, Damage damage, float damageDegree, float criticalRate, DamageInfoTag[] tags)
    {
        //原则上来说 角色死亡
        //它释放的技能、子弹都应该清除
        //目前我懒得这样做 就在
        //这个处理伤害的最后一步判定 如果角色不存在 就无效化本次伤害
        if (attacker == null)
        {
            return;
        }
        //如果攻击者的state不为空 调用
        if (attacker.GetComponent<ChaState>().hasParent)
        {
            this.damageInfos.Add(new DamageInfo(
            attacker.GetComponent<ChaState>().State.gameObject, target, damage, damageDegree, criticalRate, tags
        ));
            return;
        }
        this.damageInfos.Add(new DamageInfo(
            attacker, target, damage, damageDegree, criticalRate, tags
        ));
    }

    ///<summary>
    ///添加一个damageInfo
    ///</summary>
    public void DoDamage(DamageInfo damageInfo)
    {
        if (damageInfo.attacker != null && damageInfo.attacker.GetComponent<ChaState>() != null)
        {
            this.damageInfos.Add(damageInfo);
        }
    }

}