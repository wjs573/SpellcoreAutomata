using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能伤害工具类：这是一个class，用于描述技能伤害。
/// 例如造成一次100+角色攻击力*0.1的火属性伤害。
/// </summary>
public class SkillDamage
{
    /// <summary>
    /// 技能伤害类型，例如火属性伤害
    /// </summary>
    private DamageType damageType;

    /// <summary>
    /// 基础伤害
    /// </summary>
    private float baseDamage;

    /// <summary>
    /// 角色属性加成倍率
    /// </summary>
    private ChaProperty AddChaPropertyDamage;

    /// <summary>
    /// 角色资源加成倍率
    /// </summary>
    private ChaResource AddChaResourceDamage;

    public float GetDamageNumber(ChaState chaState)
    {
        ChaProperty AddedChaPropertyDamage = AddChaPropertyDamage * chaState.property;
        ChaResource AddedChaResourceDamage = AddChaResourceDamage * chaState.resource;
        return baseDamage + AddedChaPropertyDamage.TotalValue() + AddedChaResourceDamage.TotalValue();
    }

    public Damage GetDamage(ChaState chaState)
    {
        float skillDamageNumber = GetDamageNumber(chaState);
        Damage damage = new Damage(0);
        switch (damageType)
        {
            case DamageType.无属性伤害:
                damage = new Damage((int)skillDamageNumber);
                break;
            case DamageType.真实伤害:
                damage = new Damage(0, (int)skillDamageNumber);
                break;

            default:
                damage = new Damage(0); // 默认情况下，所有伤害值为0。
                break;
        }

        return damage;
    }
}