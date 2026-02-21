using Sirenix.OdinInspector;
using UnityEngine;
using WJS;

///<summary>
///顾名思义，就是为了凑效果的，并不是真的ai结构，只是让敌人看起来运动了
///</summary>
public class SimpleAI : MonoBehaviour
{
    private float moveDegree;
    [ShowInInspector]
    Vector3 mInfo;
    //private TimelineModel fire = new TimelineModel();

    private ChaState chaState;

    private void Start()
    {
        chaState = this.gameObject.GetComponent<ChaState>();
        moveDegree = this.transform.rotation.eulerAngles.y;
    }

    private void FixedUpdate()
    {
        if (!chaState || chaState.dead == true) return;
        mInfo = Vector3.zero;
        Vector3 faceVec = SceneVariants.mainChacter.transform.position - this.transform.position;
        float distance = Mathf.Pow(SceneVariants.mainChacter.transform.position.x - this.transform.position.x, 2)
        + Mathf.Pow(SceneVariants.mainChacter.transform.position.z - this.transform.position.z, 2);
        if (distance < 0.8f)
        {
            return;
        }
        float rotateTo = Mathf.Atan2(faceVec.x, faceVec.z) * 180.00f / Mathf.PI;
        chaState.OrderRotateTo(rotateTo);

        moveDegree = transform.rotation.eulerAngles.y;
        float rRadius = moveDegree * Mathf.PI / 180;
        float mSpd = chaState.moveSpeed;

        mInfo = new Vector3(
             Mathf.Sin(rRadius) * mSpd,
             0,
             Mathf.Cos(rRadius) * mSpd
         );
        chaState.OrderMove(mInfo);
    }
}