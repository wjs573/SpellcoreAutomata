using MoreMountains.Tools;

/// <summary>
/// °ëÑª·µ»Øtrue
/// </summary>
public class AIDecisionHalfHp : AIDecision
{
    ChaState chaState;
    public override void Initialization()
    {
        base.Initialization();
        chaState = transform.parent.gameObject.GetComponent<ChaState>();
    }
    public override bool Decide()
    {
        return chaState.resource.hp <= chaState.property.hp / 2;
    }
}
