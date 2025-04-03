using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

/// <summary>
/// use a skill one times
/// </summary>
public class AIActionOneTimesSkill : AIAction
{
    public string skill_id;
    public ChaState chaState;
    public bool IsUsed;
    private void Start()
    {
        IsUsed = false;
        chaState = transform.parent.GetComponent<ChaState>();
    }
    public override void PerformAction()
    {
        if (IsUsed)
        {
            return;
        }

        chaState.CastSkill(skill_id);
        IsUsed = true;
    }
}
