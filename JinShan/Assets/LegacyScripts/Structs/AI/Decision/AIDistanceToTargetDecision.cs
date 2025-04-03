using MoreMountains.Tools;
using UnityEngine;
public class AIDistanceToTargetDecision : AIDecision
{
    public float distance;
    /// <summary>
    /// 索敌组件，包含最近的敌人、距离
    /// </summary>
    UnitGetTarget getTarget;

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
        return getTarget.distance < this.distance;
    }

}
