using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class AIActionSkillAttack : AIAction
{
    private ChaState chaState;
    private UnitGetTarget unitGetTarget;

    [ShowInInspector]
    public List<SkillAIOption> skillAIOptions = new List<SkillAIOption>(); // 假设您已经定义了技能列表

    private void Start()
    {
        chaState = transform.parent.GetComponent<ChaState>();
        unitGetTarget = transform.parent.GetComponent<UnitGetTarget>();
        skillAIOptions.Add(new SkillAIOption("LaunchingFlameLotusOfWrath", 5f, 2, 0));
        skillAIOptions.Add(new SkillAIOption("FireWaveSlash", 40f, 1, 3f));
        skillAIOptions.Add(new SkillAIOption("FireBall", 5f, 3, 0));
    }

    public override void PerformAction()
    {
        if (!chaState || chaState.dead == true || chaState.charging) return;
        GameObject target = unitGetTarget.closestEnemy;
        if (target == null)
        {
            return;
        }
        SkillAIOption selectedSkill = SelectSkill(target); // 选择技能
        if (selectedSkill.skillId != null)
        {
            if (IsInRange(target, selectedSkill))
            {
                if (selectedSkill.chargingTime != 0f)
                {
                    chaState.charging = true;
                    StartCoroutine(EndChargingAfterDelay(selectedSkill.chargingTime));
                }
                UseSkill(selectedSkill); // 使用技能
            }
            else
            {
                StartChase(target, selectedSkill); // 开始追踪
            }
        }
    }

    private IEnumerator EndChargingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        chaState.charging = false;
    }

    private SkillAIOption SelectSkill(GameObject target)
    {
        SkillAIOption selectedSkillOption = new SkillAIOption(); // 初始化为默认值
        float minPriority = float.MaxValue;
        float distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);

        foreach (SkillAIOption skillOption in skillAIOptions)
        {
            SkillObj skill = chaState.GetSkillById(skillOption.skillId);
            if (skill == null) continue; // 检查是否找到技能

            // 检查技能是否处于冷却中
            if (skill.cooldown <= 0.02)
            {
                // 如果技能的优先级更高，更新选择的技能
                if (skillOption.priority < minPriority)
                {
                    minPriority = skillOption.priority;
                    selectedSkillOption = skillOption;
                }
            }
        }

        // 检查是否有有效的选择
        if (minPriority != float.MaxValue)
        {
            return selectedSkillOption;
        }

        // 如果没有可用技能，返回默认技能选项
        return new SkillAIOption();
    }

    private bool IsInRange(GameObject target, SkillAIOption skill)
    {
        // 检查目标是否在技能的攻击范围内
        Vector3 positionA = gameObject.transform.position;
        Vector3 positionB = target.transform.position;

        // 将Y轴坐标设置为相同的值
        positionA.y = 0;
        positionB.y = 0;

        // 计算XOZ平面上的距离
        float currentDistance = Vector3.Distance(positionA, positionB);
        return currentDistance <= skill.range;
    }

    private void UseSkill(SkillAIOption skill)
    {
        // 执行使用技能的逻辑
        chaState.CastSkill(skill.skillId);
    }

    private void StartChase(GameObject target, SkillAIOption skill)
    {
        GetComponent<AIDecisionChasingTarget>().StartChasing(target, skill.range);
    }
}