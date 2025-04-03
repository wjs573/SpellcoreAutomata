using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 寻敌组件
/// 寻找最近的敌人，后续拓展其他寻敌功能
/// 主要是UnitAI组件调用
/// </summary>
public class UnitGetTarget : MonoBehaviour
{
    [ShowInInspector]
    private GameObject _closestEnemy;
    public GameObject closestEnemy
    {
        get
        {
            UpdateEnemiesList();
            return _closestEnemy;
        }
    }



    bool hasUpdated = false;

    /// <summary>
    /// 与目标的距离
    /// </summary>
    private float _distance;

    public float distance
    {
        get
        {
            UpdateEnemiesList();
            return _distance;
        }
    }

    /// <summary>
    /// 拥有者
    /// </summary>
    public ChaState ownerChaState;

    [ShowInInspector]
    // 存储敌人及其距离
    private List<KeyValuePair<GameObject, float>> enemiesDistances = new List<KeyValuePair<GameObject, float>>();

    // Start is called before the first frame update
    private void Awake()
    {
        ownerChaState = this.GetComponent<ChaState>();
        _closestEnemy = null;
        UpdateEnemiesList();
    }

    private void FixedUpdate()
    {
        hasUpdated = false;
    }

    /// <summary>
    /// 更新敌人列表及其距离
    /// </summary>
    private void UpdateEnemiesList()
    {
        GameManager GameManager = GameManager.Instance;
        if (hasUpdated)
        {
            return;
        }
        enemiesDistances.Clear();  // 清空列表

        if (GameManager == null)
        {
            return;
        }

        foreach (var target in GameManager.Characters)
        {
            if (target == null || target.GetComponent<ChaState>().side == ownerChaState.side || target.GetComponent<ChaState>().dead)
            {
                continue;
            }

            if (target.GetComponent<ChaState>().property.IsInvisible)
            {
                continue;
            }

            Vector3 point1 = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            Vector3 point2 = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            float distance = Vector3.Distance(point1, point2);

            enemiesDistances.Add(new KeyValuePair<GameObject, float>(target, distance));
        }

        // 对敌人按距离进行排序
        enemiesDistances = enemiesDistances.OrderBy(ed => ed.Value).ToList();

        // 更新最近的敌人
        if (enemiesDistances.Count > 0)
        {
            _closestEnemy = enemiesDistances[0].Key;
            _distance = enemiesDistances[0].Value;
        }
        else
        {
            _closestEnemy = null;
            _distance = float.MaxValue;
        }

        hasUpdated = true;
    }

    /// <summary>
    /// 获取指定数量的敌人，排除指定敌人
    /// </summary>
    /// <param name="number">需要返回的敌人数量</param>
    /// <param name="excludeEnemies">需要排除的敌人</param>
    /// <returns>指定数量的敌人</returns>
    public List<GameObject> GetEnemies(int number, List<GameObject> excludeEnemies = null)
    {
        UpdateEnemiesList();
        if (excludeEnemies == null)
        {
            excludeEnemies = new List<GameObject>();  // 如果为 null，创建一个空列表
        }

        // 从排序后的列表中过滤掉需要排除的敌人
        var filteredEnemies = enemiesDistances
            .Where(ed => !excludeEnemies.Contains(ed.Key))
            .Take(number)
            .Select(ed => ed.Key)
            .ToList();
        // 如果过滤后的敌人列表为空，添加一个null值
        if (filteredEnemies.Count == 0)
        {
            filteredEnemies.Add(null);
        }

        return filteredEnemies;
    }

}
