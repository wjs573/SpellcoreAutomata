using UnityEngine;

namespace DesignerScripts
{
    ///<summary>
    ///这里的函数都是程序暴露给策划的脚本，这些脚本是游戏中一些“规则级”的，比如升级经验等，都是流程中一些关键的函数
    ///
    ///</summary>
    public class CommonScripts
    {
        /// <summary>
        /// 中毒伤害
        /// 输入 角色状态和中毒buffobj
        /// buff层数 *10
        /// 无法暴击
        /// 返回 中毒伤害信息
        /// </summary>
        /// <returns></returns>
        public static DamageInfo PoisoningDamage(ChaState chastate, BuffObj buffObj)
        {
            int damage = Mathf.RoundToInt(10 * buffObj.stack);

            DamageInfo damageInfo = new DamageInfo(buffObj.caster, buffObj.carrier,
                new Damage(damage), 0f, 0f, new DamageInfoTag[] { DamageInfoTag.directDamage });

            return damageInfo;
        }

        ///<summary>
        ///根据暴击等信息获得最终伤害
        ///<param name="damageInfo">伤害信息</param>
        ///<param name="asHeal">是否当做治疗</param>
        ///<return>伤害数值</return>
        ///</summary>
        public static DamageInfo DamageValue(DamageInfo damageInfo, bool asHeal = false)
        {
            if (asHeal)
            {
                damageInfo.result.damage = damageInfo.damage.ordinary_damage;
                return damageInfo;
            }

            int defence = GetDefence(damageInfo.defender);
            float baseDamage = damageInfo.damage.ordinary_damage;
            float final_damage = (damageInfo.result.isCritical ? damageInfo.attackerProperty.critic_multiplier * baseDamage : baseDamage) / (1 + defence / 100f) + damageInfo.damage.true_damage;

            damageInfo.result.damage = Mathf.CeilToInt(final_damage);

            return damageInfo;
        }

        private static int GetDefence(GameObject defender)
        {
            return defender.GetComponent<ChaState>()?.property.defence ?? 0;
        }

    }
}