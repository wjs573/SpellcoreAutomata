using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SkillObjModifiers
/// 修改技能obj 获得强化版技能
/// </summary>
public static class SkillObjModifiers
{
    public static Dictionary<string, SkillObjModifierEvent> data = new Dictionary<string, SkillObjModifierEvent>()
    {
        {"CdModifier", CdModifier},
        {"ConditionModifier",ConditionModifier},
        {"CostModifier",CostModifier },
        {"TimelineNodeModifier", TimelineNodeModifier},
        {"AddTimelineNode",AddTimelineNode }
    };

    /// <summary>
    /// 添加一个timelinenode
    /// 不需要刻意调整顺序 因为执行是严格依据timelineNode的时间设置的
    /// 0 TimelineNode timelineNode即将添加的node
    /// </summary>
    /// <param name="skillObj"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    private static SkillObj AddTimelineNode(SkillObj skillObj, object[] param)
    {
        TimelineNode timelineNode = (TimelineNode)param[0];
        TimelineNode[] nodes = skillObj.model.effect.nodes;

        // 扩展数组大小
        Array.Resize(ref nodes, nodes.Length + 1);

        // 将 timelineNode 添加到数组中
        nodes[nodes.Length - 1] = timelineNode;

        // 更新 skillObj.model.effect.nodes 引用
        skillObj.model.effect.nodes = nodes;

        return skillObj;
    }

    /// <summary>
    /// 输入node 和 更新后的node，把timeline置换成更新后的状态
    /// 0 TimelineNode OriginTimelineNode
    /// 1 TimelineNode NewtimelineNode
    /// </summary>
    /// <param name="skillObj"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    private static SkillObj TimelineNodeModifier(SkillObj skillObj, object[] param)
    {
        TimelineNode OriginTimelineNode = (TimelineNode)param[0];
        TimelineNode NewtimelineNode = (TimelineNode)param[1];
        TimelineNode[] nodes = skillObj.model.effect.nodes;
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].TimelineEventName == OriginTimelineNode.TimelineEventName && nodes[i].eveParams == OriginTimelineNode.eveParams)
            {
                nodes[i] = NewtimelineNode;
                break;
            }
        }
        // 更新 skillObj.model.effect.nodes 引用
        skillObj.model.effect.nodes = nodes;

        return skillObj;
    }

    private static SkillObj CostModifier(SkillObj skillObj, object[] param)
    {
        ChaResource cost = (ChaResource)param[0];
        skillObj.model.cost = cost;
        return skillObj;
    }

    private static SkillObj ConditionModifier(SkillObj skillObj, object[] param)
    {
        ChaResource condition = (ChaResource)param[0];
        skillObj.model.condition = condition;
        return skillObj;
    }

    /// <summary>
    /// CD修改器
    /// 0 bool 是否为加法，否则是乘法
    /// 1 float 修改值
    /// </summary>
    /// <param name="skillObj"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    private static SkillObj CdModifier(SkillObj skillObj, object[] param)
    {
        bool IsAdd = (bool)param[0];
        float modifierNumber = (float)param[1];

        float cooldown = skillObj.model.cooldown;  // 当前冷却时间

        float minCooldown = 0.1f;  // 最小冷却时间

        float result = 0.1f;

        if (IsAdd)
        {
            result = cooldown + modifierNumber;
        }
        else
        {
            result = cooldown * modifierNumber;
        }

        //确保结果大于0.1f
        result = Mathf.Clamp(result, minCooldown, float.MaxValue);
        skillObj.model.cooldown = result;

        return skillObj;
    }
}

public delegate SkillObj SkillObjModifierEvent(SkillObj skillObj, object[] param);