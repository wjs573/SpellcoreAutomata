using MoreMountains.Tools;

public class AIActionBomb : AIAction
{
    private bool IsOn = false;

    public override void PerformAction()
    {
        if (IsOn)
        {
            return;
        }
        AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["ExplosionBuff"], transform.parent.gameObject,
             transform.parent.gameObject, 1, 1.25f, true, false);
        transform.parent.gameObject.GetComponent<ChaState>().AddBuff(addBuffInfo);
        IsOn = true;
    }
}