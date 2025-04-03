using MoreMountains.Tools;

public class AIDoNothingAction : AIAction
{
    private ChaState chaState;

    private void Start()
    {
        chaState = GetComponent<ChaState>();
    }

    public override void PerformAction()
    {
    }
}