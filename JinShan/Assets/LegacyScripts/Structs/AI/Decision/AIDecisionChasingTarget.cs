using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class AIDecisionChasingTarget : AIDecision
{
    [ShowInInspector]
    private GameObject chasingTarget;

    [ShowInInspector]
    private float targetDistance;

    [ShowInInspector]
    private float currentDistance;

    private AIDecisionSkillNeedChasing DecisionSkillNeedChasing;

    private void Start()
    {
        DecisionSkillNeedChasing = GetComponent<AIDecisionSkillNeedChasing>();
    }

    public void StartChasing(GameObject chasingTarget, float targetDistance)
    {
        this.chasingTarget = chasingTarget;
        this.targetDistance = targetDistance;
        DecisionSkillNeedChasing.OnChasing = true;
    }

    public override bool Decide()
    {
        if (chasingTarget == null || chasingTarget.GetComponent<ChaState>().dead)
        {
            return false;
        }

        Vector3 positionA = gameObject.transform.position;
        Vector3 positionB = chasingTarget.transform.position;

        // 将Y轴坐标设置为相同的值
        positionA.y = 0;
        positionB.y = 0;

        // 计算XOZ平面上的距离
        currentDistance = Vector3.Distance(positionA, positionB);
        if (currentDistance <= targetDistance)
        {
            DecisionSkillNeedChasing.OnChasing = false;
            chasingTarget = null;
            return true;
        }
        return false;
    }
}