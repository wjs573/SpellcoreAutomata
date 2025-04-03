using MoreMountains.Tools;

/// <summary>
/// 寻找最近的目标
/// 如果目标存在（side与自己不同，chastate存在，未死亡）
/// </summary>
public class AITargetDetectionDecision : AIDecision
{
    /// <summary>
    /// 索敌组件，包含最近的敌人、距离
    /// </summary>
    private UnitGetTarget getTarget;

    public override void Initialization()
    {
        base.Initialization();

        //初始化getTarget的引用
        //如果没有 就主动添加
        getTarget = transform.parent.GetComponent<UnitGetTarget>();
        if (getTarget == null)
        {
            getTarget = transform.parent.gameObject.AddComponent<UnitGetTarget>();
        }
    }

    public override bool Decide()
    {
        if (getTarget.closestEnemy == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}