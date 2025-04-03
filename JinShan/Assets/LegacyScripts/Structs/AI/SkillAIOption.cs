using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ShowInInspector]
public struct SkillAIOption
{
    public string skillId;
    public float range;
    public int priority;
    public float chargingTime;

    public SkillAIOption(string skillId, float range, int priority, float chargingTime)
    {
        this.skillId = skillId;
        this.range = range;
        this.priority = priority;
        this.chargingTime = chargingTime;
    }
}
