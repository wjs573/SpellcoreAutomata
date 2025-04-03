using UnityEngine;

/// <summary>
/// AI组件
/// 装备此组件后，角色会自动从UnitGetTarget组件中获取目标
/// 当距离目标过远的时候，会靠近目标
/// 当距离目标距离合适时，会释放冲撞技能
/// </summary>
public class UnitAI : MonoBehaviour
{
    /// <summary>
    /// AI模式
    /// </summary>
    public AIMode AIMode;

    //距离下次释放技能的时间
    public float toNextSkill = 1.10f;

    //距离下次移动的时间
    //private float toNextRotate = 2.0f;
    //当前移动角度
    private float moveDegree;

    //当前攻击目标
    public GameObject target;

    //当前攻击目标的距离
    public float distance;

    private ChaState chaState;

    // Start is called before the first frame update
    private void Start()
    {
        chaState = this.gameObject.GetComponent<ChaState>();
        moveDegree = this.transform.rotation.eulerAngles.y;

        float random = Random.Range(0f, 1f);
        if (random > 0.66f)
        {
            AIMode = AIMode.冲撞型;
            chaState.LearnSkill(DesignerTables.Skill.data["collide"]);
        }
        else
        {
            if (random > 0.33f)
            {
                AIMode = AIMode.远程型;
                chaState.LearnSkill(DesignerTables.Skill.data["fire"]);
            }
            else
            {
                AIMode = AIMode.近战型;
                chaState.LearnSkill(DesignerTables.Skill.data["slash"]);
            }
        }
    }

    private void FixedUpdate()
    {
        //如果角色死亡 则返回
        if (!chaState || chaState.dead == true) return;

        float timePassed = Time.fixedDeltaTime;
        target = this.GetComponent<UnitGetTarget>().closestEnemy;
        distance = this.GetComponent<UnitGetTarget>().distance;

        //旧代码 是直接默认寻找玩家角色作为目标
        //Vector3 faceVec = (SceneVariants.MainActor().transform.position - this.transform.position);
        //新代码 默认为零向量 如果存在目标 则指向目标
        Vector3 faceVec = (target != null) ?
            target.transform.position - this.transform.position :
            new Vector3(Random.value, 0, Random.value);

        //朝向目标的角度
        float rotateTo = Mathf.Atan2(faceVec.x, faceVec.z) * 180.00f / Mathf.PI;
        //toNextRotate -= timePassed;

        //操作角色朝向目标
        chaState.OrderRotateTo(rotateTo);

        moveDegree = this.transform.rotation.eulerAngles.y;
        float rRadius = moveDegree * Mathf.PI / 180;
        float mSpd = chaState.moveSpeed;

        //创建移动信息
        Vector3 mInfo = new Vector3(
            Mathf.Sin(rRadius) * mSpd,
            0,
            Mathf.Cos(rRadius) * mSpd
        );

        toNextSkill -= timePassed;

        switch (AIMode)
        {
            case AIMode.冲撞型://冲撞技能的判定逻辑 与目标距离小于3 且 技能冷却ok 则释放技能
                if (distance <= 3f && toNextSkill <= 0)
                {
                    chaState.CastSkill("collide");
                    //SceneVariants.CreateTimeline(skill, this.gameObject, null);
                    toNextSkill = Random.Range(1.20f, 1.50f);
                }
                else
                {
                    if (distance >= 3f)
                    {
                        //操控角色向前移动
                        chaState.OrderMove(mInfo);
                    }
                }
                break;

            case AIMode.远程型://开火技能的判定逻辑 与目标距离小于6 技能冷却ok 则开火
                if (distance < 12f && toNextSkill <= 0)
                {
                    chaState.CastSkill("fire");
                    //SceneVariants.CreateTimeline(fire, this.gameObject, null);
                    toNextSkill = Random.Range(1.20f, 1.50f);
                }
                //与敌人距离大于6 靠近敌人
                if (distance >= 12f)
                {
                    //操控角色向前移动
                    chaState.OrderMove(mInfo);
                }
                //与敌人距离小于3 远离敌人
                if (distance <= 2f)
                {
                    //操控角色向前移动
                    chaState.OrderMove(-mInfo);
                }
                break;

            case AIMode.近战型://开火技能的判定逻辑 与目标距离小于6 技能冷却ok 则开火
                if (distance <= 1f && toNextSkill <= 0)
                {
                    chaState.CastSkill("slash");
                    //SceneVariants.CreateTimeline(slash, this.gameObject, null);
                    toNextSkill = Random.Range(1.20f, 1.50f);
                }
                //与敌人距离大于1 靠近敌人
                if (distance >= 1f)
                {
                    //操控角色向前移动
                    chaState.OrderMove(mInfo);
                }
                break;
        }
    }
}

public enum AIMode
{
    冲撞型, 远程型, 近战型
}