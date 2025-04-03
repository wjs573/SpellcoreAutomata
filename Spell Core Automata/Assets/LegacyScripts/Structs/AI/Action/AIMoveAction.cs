using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class AIMoveAction : AIAction
{
    private ChaState chaState;

    private GameObject target;

    private float moveDegree;

    Vector3 mInfo = new Vector3();

    [ShowInInspector]
    public List<SkillAIOption> moveSkillOptions = new List<SkillAIOption>(); // 假设您已经定义了技能列表

    private void Start()
    {
        chaState = transform.parent.GetComponent<ChaState>();
        moveDegree = transform.parent.rotation.eulerAngles.y;
        moveSkillOptions.Add(new SkillAIOption("ThreeThousandThunderMovements", 5f, 1, 0));
    }

    private void FixedUpdate()
    {
        chaState.OrderMove(mInfo);
    }

    public override void PerformAction()
    {

        if (!chaState || chaState.dead == true) return;

        target = transform.parent.GetComponent<UnitGetTarget>().closestEnemy;

        Vector3 faceVec = (target != null) ?
            target.transform.position - this.transform.position :
            transform.forward;

        float rotateTo = Mathf.Atan2(faceVec.x, faceVec.z) * 180.00f / Mathf.PI;

        chaState.OrderRotateTo(rotateTo);

        SkillAIOption selectedSkill = SelectSkill(target); // 选择技能
        if (selectedSkill.skillId != null)
        {
            chaState.CastSkill(selectedSkill.skillId); // 使用技能
            return;
        }

        moveDegree = this.transform.rotation.eulerAngles.y;
        float rRadius = moveDegree * Mathf.PI / 180;
        float mSpd = chaState.moveSpeed;

        if (GetComponent<AIDistanceToTargetDecision>().Decide() == true)
        {
            mInfo = new Vector3(0, 0, 0);
            return;
        }
        mInfo = new Vector3(
            Mathf.Sin(rRadius) * mSpd,
            0,
            Mathf.Cos(rRadius) * mSpd
        );
    }

    private SkillAIOption SelectSkill(GameObject target)
    {
        if (target == null)
        {
            return new SkillAIOption();
        }
        SkillAIOption selectedSkillOption = new SkillAIOption(); // 初始化为默认值
        float minPriority = float.MaxValue;
        float distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);

        foreach (SkillAIOption skillOption in moveSkillOptions)
        {
            SkillObj skill = chaState.GetSkillById(skillOption.skillId);
            if (skill == null) continue; // 检查是否找到技能

            // 检查技能是否处于冷却中
            if (skill.cooldown <= 0 && distanceToTarget <= skillOption.range)
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
}