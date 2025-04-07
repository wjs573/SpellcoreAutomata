using System.Collections.Generic;
using JinShan;
using UnityEngine;

///<summary>
///负责aoe的移动、生命周期等
///还负责aoe和角色、子弹的碰撞，需要加aoe碰撞也在这里。值得注意的是：aoe是主体
///aoe捕捉范围与子弹碰撞不同的是，他不判断角色的体型（hitRadius或者bodyRadius），当然如果需要也可以加上，只是这个demo里不需要
///</summary>
public class AoeManager : MonoSingleton<AoeManager>
{
    [Header("Debug Settings")]
    public bool showIndicators = false; // 控制是否显示指示器
    public GameObject aoeIndicatorPrefab; // 指示器预制件

    private Dictionary<GameObject, GameObject> aoeIndicators = new Dictionary<GameObject, GameObject>();

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsInBattle)
        {
            UpdateAoEs();
        }
    }

    /// <summary>
    /// 移除所有aoe
    /// </summary>
    public void ClearAoEs()
    {
        GameObject[] aoe = GameObject.FindGameObjectsWithTag("AoE");
        for (int i = 0; i < aoe.Length; i++)
        {
            Destroy(aoe[i]);
            DestroyIndicator(aoe[i]); // 销毁指示器
        }
    }

    private void DestroyIndicator(GameObject aoe)
    {
        if (aoeIndicators.TryGetValue(aoe, out GameObject indicator))
        {
            Destroy(indicator);
            aoeIndicators.Remove(aoe);
        }
    }

    private void CreateIndicator(GameObject aoe, float radius)
    {
        if (!showIndicators || aoeIndicatorPrefab == null) return; // 不显示指示器
        GameObject indicator = Instantiate(aoeIndicatorPrefab, aoe.transform.position, Quaternion.identity);
        indicator.transform.localScale = new Vector3(radius * 2, 1, radius * 2); // 根据AOE半径调整指示器大小
        aoeIndicators[aoe] = indicator;
    }

    private void UpdateIndicator(GameObject aoe, float radius)
    {
        if (!showIndicators)
        {
            DestroyIndicator(aoe); // 如果不显示指示器，直接销毁现有的
            return;
        }
        if (aoeIndicators.TryGetValue(aoe, out GameObject indicator))
        {
            indicator.transform.position = new Vector3(aoe.transform.position.x, 0, aoe.transform.position.z); // 更新位置
            indicator.transform.localScale = new Vector3(radius * 2, 1, radius * 2); // 更新大小
        }
    }

    /// <summary>
    /// aoe运行逻辑
    /// aoe的移动、碰撞检测
    /// </summary>
    private void UpdateAoEs()
    {
        GameObject[] aoe = GameObject.FindGameObjectsWithTag("AoE");
        if (aoe.Length <= 0) return;
        GameObject[] cha = GameObject.FindGameObjectsWithTag("Character");
        GameObject[] bullet = GameObject.FindGameObjectsWithTag("Bullet");

        float timePassed = Time.fixedDeltaTime;

        for (int i = 0; i < aoe.Length; i++)
        {
            AoeState aoeState = aoe[i].GetComponent<AoeState>();
            if (!aoeState) continue;

            // 创建或更新指示器
            if (!aoeIndicators.ContainsKey(aoe[i]))
            {
                CreateIndicator(aoe[i], aoeState.radius);
            }
            else
            {
                UpdateIndicator(aoe[i], aoeState.radius);
            }

            //首先是aoe的移动
            if (aoeState.duration > 0 && aoeState.tween != null)
            {
                AoeMoveInfo aoeMoveInfo = aoeState.tween(aoe[i], aoeState.tweenRunnedTime);
                aoeState.tweenRunnedTime += timePassed;
                aoeState.SetMoveAndRotate(aoeMoveInfo);
            }

            if (aoeState.justCreated == true)
            {
                //刚创建的，走onCreate
                aoeState.justCreated = false;
                //捕获所有角色
                for (int m = 0; m < cha.Length; m++)
                {
                    if (
                        cha[m] &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z,
                            cha[m].transform.position.x, cha[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    )
                    {
                        aoeState.characterInRange.Add(cha[m]);
                    }
                }
                //捕获所有的子弹
                for (int m = 0; m < bullet.Length; m++)
                {
                    if (
                        bullet[m] &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z,
                            bullet[m].transform.position.x, bullet[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    )
                    {
                        aoeState.bulletInRange.Add(bullet[m]);
                    }
                }
                //执行OnCreate
                if (aoeState.model.onCreate != null)
                {
                    aoeState.model.onCreate.Invoke(aoe[i]);
                }
            }
            else
            {
                //已经创建完成的
                //先抓角色离开事件
                List<GameObject> leaveCha = new List<GameObject>();
                List<GameObject> toRemove = new List<GameObject>();
                for (int m = 0; m < aoeState.characterInRange.Count; m++)
                {
                    if (aoeState.characterInRange[m] != null)
                    {
                        if (Utils.InRange(
                                aoe[i].transform.position.x, aoe[i].transform.position.z,
                                aoeState.characterInRange[m].gameObject.transform.position.x, aoeState.characterInRange[m].gameObject.transform.position.z,
                                aoeState.radius
                            ) == false
                        )
                        {
                            leaveCha.Add(aoeState.characterInRange[m]);
                            toRemove.Add(aoeState.characterInRange[m]);
                        }
                    }
                    else
                    {
                        toRemove.Add(aoeState.characterInRange[m]);
                    }
                }
                for (int m = 0; m < toRemove.Count; m++)
                {
                    aoeState.characterInRange.Remove(toRemove[m]);
                }
                if (aoeState.model.onChaLeave != null)
                {
                    aoeState.model.onChaLeave.Invoke(aoe[i], leaveCha);
                }

                //再看进入的角色
                List<GameObject> enterCha = new List<GameObject>();
                for (int m = 0; m < cha.Length; m++)
                {
                    if (
                        cha[m] &&
                        aoeState.characterInRange.IndexOf(cha[m]) < 0 &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z,
                            cha[m].transform.position.x, cha[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    )
                    {
                        enterCha.Add(cha[m]);
                    }
                }
                if (aoeState.model.onChaEnter != null)
                {
                    aoeState.model.onChaEnter.Invoke(aoe[i], enterCha);
                }
                for (int m = 0; m < enterCha.Count; m++)
                {
                    if (enterCha[m] != null && enterCha[m].GetComponent<ChaState>() && enterCha[m].GetComponent<ChaState>().dead == false)
                    {
                        aoeState.characterInRange.Add(enterCha[m]);
                    }
                }

                //子弹离开
                List<GameObject> leaveBullet = new List<GameObject>();
                toRemove = new List<GameObject>();
                for (int m = 0; m < aoeState.bulletInRange.Count; m++)
                {
                    if (aoeState.bulletInRange[m])
                    {
                        if (Utils.InRange(
                                aoe[i].transform.position.x, aoe[i].transform.position.z,
                                aoeState.bulletInRange[m].gameObject.transform.position.x, aoeState.bulletInRange[m].gameObject.transform.position.z,
                                aoeState.radius
                            ) == false
                        )
                        {
                            leaveBullet.Add(aoeState.bulletInRange[m]);
                            toRemove.Add(aoeState.bulletInRange[m]);
                        }
                    }
                    else
                    {
                        toRemove.Add(aoeState.bulletInRange[m]);
                    }
                }
                for (int m = 0; m < toRemove.Count; m++)
                {
                    aoeState.bulletInRange.Remove(toRemove[m]);
                }
                if (aoeState.model.onBulletLeave != null)
                {
                    aoeState.model.onBulletLeave.Invoke(aoe[i], leaveBullet);
                }
                toRemove = null;

                //子弹进入
                List<GameObject> enterBullet = new List<GameObject>();
                for (int m = 0; m < bullet.Length; m++)
                {
                    if (
                        bullet[m] &&
                        aoeState.bulletInRange.IndexOf(bullet[m]) < 0 &&
                        Utils.InRange(
                            aoe[i].transform.position.x, aoe[i].transform.position.z,
                            bullet[m].transform.position.x, bullet[m].transform.position.z,
                            aoeState.radius
                        ) == true
                    )
                    {
                        enterBullet.Add(bullet[m]);
                    }
                }
                if (aoeState.model.onBulletEnter != null)
                {
                    aoeState.model.onBulletEnter.Invoke(aoe[i], enterBullet);
                }
                for (int m = 0; m < enterBullet.Count; m++)
                {
                    if (enterBullet[m] != null)
                    {
                        aoeState.bulletInRange.Add(enterBullet[m]);
                    }
                }
            }
            //然后是aoe的duration
            if (aoeState.isDieWithParent && aoeState.parent == null)
            {
                aoeState.duration = 0f;
            }
            aoeState.duration -= timePassed;
            aoeState.timeElapsed += timePassed;
            if (aoeState.duration <= 0 || aoeState.HitObstacle() == true)
            {
                if (aoeState.model.onRemoved != null)
                {
                    aoeState.model.onRemoved.Invoke(aoe[i]);
                }
                DestroyIndicator(aoe[i]); // 销毁指示器
                //如果该aoe的美术特效没有播放完成
                //则将该aoe的美术特效移到aoemanager下，并播放完成后再销毁
                SightEffect sightEffect = aoe[i].GetComponentInChildren<SightEffect>();
                if (sightEffect != null && sightEffect.duration - aoeState.timeElapsed > 0)
                {
                    sightEffect.transform.SetParent(transform);
                    sightEffect.gameObject.AddComponent<UnitRemover>();
                    sightEffect.GetComponentInChildren<UnitRemover>().duration = sightEffect.duration - aoeState.timeElapsed;
                }
                Destroy(aoe[i]);
                continue;
            }
            else
            {
                //最后是onTick，remove
                if (
                    aoeState.model.tickTime > 0 && aoeState.model.onTick != null &&
                    Mathf.RoundToInt(aoeState.duration * 1000) % Mathf.RoundToInt(aoeState.model.tickTime * 1000) <= 19
                )
                {
                    aoeState.model.onTick.Invoke(aoe[i]);
                }
            }
        }
    }
}