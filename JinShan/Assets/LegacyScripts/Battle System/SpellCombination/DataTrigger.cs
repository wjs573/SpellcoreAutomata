using System;
using System.Collections;
using System.Collections.Generic;


namespace DesignerTables
{
    public class DataTrigger
    {
        public static Dictionary<string, TriggerEffect> data = new Dictionary<string, TriggerEffect>
    {
        { "OnHit",TriggerOnHit}
    };

        private static SkillModel TriggerOnHit(SkillModel triggerSkill, SkillModel beTriggeredSkill, string eventTriggerCondition, object[] eventTriggerConditionParams)
        {
            SkillModel modifiedSkillModel = triggerSkill.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                DataEnhancedEffect.ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.model.onHit.AddTimelineModel(beTriggeredSkill.effect, DataEventTriggerCondition.data[eventTriggerCondition], eventTriggerConditionParams);
                    return value;
                });
                DataEnhancedEffect.ModifyParameterOfType<LaserLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (LaserLauncher value) =>
                {
                    value.model.onHit.AddTimelineModel(beTriggeredSkill.effect, DataEventTriggerCondition.data[eventTriggerCondition], eventTriggerConditionParams);
                    return value;
                });
            }
            return modifiedSkillModel;
        }
    }

    /// <summary>
    /// 触发器处理逻辑
    /// </summary>
    /// <param name="triggerSkill">携带触发器的技能</param>
    /// <param name="beTriggeredSkill">被触发的技能</param>
    /// <param name="eventTriggerCondition">触发条件</param>
    /// <param name="eventTriggerConditionParams">触发条件参数</param>
    /// <returns></returns>
    public delegate SkillModel TriggerEffect(SkillModel triggerSkill, SkillModel beTriggeredSkill, string eventTriggerCondition, object[] eventTriggerConditionParams);
}