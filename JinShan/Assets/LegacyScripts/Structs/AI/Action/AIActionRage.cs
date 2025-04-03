using MoreMountains.Tools;
using UnityEngine;

public class AIActionRage : AIAction
{
    public bool IsRageNow = false;
    public override void PerformAction()
    {
        if (IsRageNow)
        {
            return;
        }
        IsRageNow = true;
      
        transform.parent.GetChild(0).transform.localScale =
            new Vector3(transform.parent.GetChild(0).transform.localScale.x * 1.25f, transform.parent.GetChild(0).transform.localScale.y * 1.25f, transform.parent.GetChild(0).transform.localScale.z * 1.25f);
        Material[] redEyeMaterials = new Material[] { Resources.Load<Material>("Materials/RedEye") };

        transform.parent.GetChild(0).GetChild(0).Find("LeftEyeBall").GetComponent<MeshRenderer>().materials = redEyeMaterials;
        transform.parent.GetChild(0).GetChild(0).Find("RightEyeBall").GetComponent<MeshRenderer>().materials = redEyeMaterials;

     
        TimelineObj timelineObj = new TimelineObj(DesignerTables.Timeline.data["skill_SummoningFiveIceToad"], transform.parent.gameObject, null);
        SceneVariants.ForceCreateTimeline(timelineObj);

        
        AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["Rage"], transform.parent.gameObject, transform.parent.gameObject, 1, 9999f, true, true);
    }
}
