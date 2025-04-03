using MoreMountains.Tools;
using UnityEngine;

public class AIAimAction : AIAction
{
    private ChaState chaState;

    private GameObject target;

    private void Start()
    {
        chaState = transform.parent.GetComponent<ChaState>();
    }

    public override void PerformAction()
    {
        if (!chaState || chaState.dead == true) return;

        target = transform.parent.GetComponent<UnitGetTarget>().closestEnemy;

        if (!chaState.controlState.canRotate)
        {
            return;
        }

        Vector3 faceVec = (target != null) ?
            target.transform.position - this.transform.position :
            new Vector3(Random.value, 0, Random.value);

        float rotateTo = Mathf.Atan2(faceVec.x, faceVec.z) * 180.00f / Mathf.PI;
        chaState.OrderRotateTo(rotateTo);
    }
}