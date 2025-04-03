using UnityEngine;

public class UnitAutoAim : MonoBehaviour
{
    /// <summary>
    /// 游戏管理器 之后可以用单例模式优化
    /// </summary>
    private GameManager GameManager;

    /// <summary>
    /// 最接近的敌人
    /// </summary>
    public GameObject closestEnemy;

    /// <summary>
    /// 拥有者
    /// </summary>
    public ChaState ownerChaState;

    /// <summary>
    /// 这个法宝主动技能对应的id
    /// </summary>
    public string skill_id;

    // Start is called before the first frame update
    private void Start()
    {
        //待优化部分
        GameManager = GameManager.Instance;

        //法宝获得技能id后 应该“让”自己的拥有者学会这个技能 好让后期自己可以释放这个技能
        GetComponent<ChaState>().LearnSkill(DesignerTables.Skill.data[skill_id]);
    }

    private void Update()
    {
        UpdateClosestEnemy();
        AimClosestEnemy();
    }

    private void FixedUpdate()
    {
        Shoot();
    }

    /// <summary>
    /// 指向最近的敌人
    /// </summary>
    private void AimClosestEnemy()
    {
        if (closestEnemy == null)
        {
            return;
        }
        float _x = (closestEnemy.transform.position.x - gameObject.transform.position.x);
        float _z = (closestEnemy.transform.position.z - gameObject.transform.position.z);
        float degree = (_z > 0) ? Mathf.Atan(_x / _z) * 180.00f / Mathf.PI : Mathf.Atan(_x / _z) * 180.00f / Mathf.PI - 180f;

        gameObject.GetComponent<ChaState>().OrderRotateTo(degree);
    }

    /// <summary>
    /// 更新最近的敌人
    /// </summary>
    private void UpdateClosestEnemy()
    {
        float min_distance = float.MaxValue;
        GameObject new_closestEnemy = null;
        for (int i = 0; i < GameManager.Characters.Count; i++)
        {
            GameObject target = GameManager.Characters[i];
            if (target.GetComponent<ChaState>().side == 1)
            {
                continue;
            }

            float distance = Mathf.Pow(target.transform.position.x - gameObject.transform.position.x, 2) +
                Mathf.Pow(target.transform.position.z - gameObject.transform.position.z, 2);
            if (distance < min_distance)
            {
                min_distance = distance;
                new_closestEnemy = target;
            }
        }

        if (new_closestEnemy != null && new_closestEnemy != closestEnemy)
        {
            closestEnemy = new_closestEnemy;
        }
    }

    /// <summary>
    /// 射击
    /// </summary>
    public void Shoot()
    {
        if (ownerChaState == null)
        {
            return;
        }
        //法宝释放技能
        gameObject.GetComponent<ChaState>().CastSkill(skill_id);
    }
}