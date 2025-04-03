using MoreMountains.Tools;

public class AIActionJump : AIAction
{
    public override void PerformAction()
    {
        //TimelineObj timelineObj = new TimelineObj(DesingerTables.Timeline.data["Move_Jump"], transform.parent.gameObject,null);
        //SceneVariants.CreateTimeline(timelineObj);
        transform.parent.gameObject.GetComponent<ChaState>().CastSkill("Jump");
    }

}
