using MoreMountains.Tools;

public class AIActionTryUseSkill : AIAction
{
    public string skill_id;
    public ChaState chaState;
    private void Start()
    {
        chaState = transform.parent.GetComponent<ChaState>();
    }
    public override void PerformAction()
    {
        chaState.CastSkill(skill_id);
    }
}