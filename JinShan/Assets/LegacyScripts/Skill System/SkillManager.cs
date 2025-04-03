using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;

public class SkillManager : MonoSingleton<SkillManager>
{
    //获取技能对象
    public SkillObj GetUpgradeSkillObjById(SkillObj skillObj)
    {
        //遍历已解锁的技能升级
        foreach (string unlockedSkillUpgrade in skillObj.unlockedSkillUpgrades)
        {

            //如果DataSkillObjModifier中存在unlockedSkillUpgrade，则从DataSkillObjModifier中获取SkillObjModifier
            if (DataSkillObjModifier.data.TryGetValue(unlockedSkillUpgrade, out SkillObjModifier skillObjModifier))
            {

                //如果SkillObjModifiers中存在skillObjModifier.Modifier，则从SkillObjModifiers中获取SkillObjModifierEvent
                if (SkillObjModifiers.data.TryGetValue(skillObjModifier.Modifier, out SkillObjModifierEvent modifierFunction))
                {
                    //调用modifierFunction函数，并传入skillObj和skillObjModifier.Params
                    skillObj = modifierFunction(skillObj, skillObjModifier.Params);
                }
            }
        }

        //返回技能对象
        return skillObj;
    }
}