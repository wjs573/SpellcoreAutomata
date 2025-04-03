using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class AIDecisionSkillNeedChasing : AIDecision
{
    public bool OnChasing = false;

    public override bool Decide()
    {
        return OnChasing;
    }
}