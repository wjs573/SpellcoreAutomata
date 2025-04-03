using MoreMountains.Tools;
using UnityEngine;

public class AIAttackAction : AIAction
{
    public string[] skill_id;
    public ChaState chaState;

    private void Start()
    {
        chaState = transform.parent.GetComponent<ChaState>();
    }

    public override void PerformAction()
    {
        int index = Random.Range(0, skill_id.Length);
        chaState.CastSkill(skill_id[index]);
    }
}