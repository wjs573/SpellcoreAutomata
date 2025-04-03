using JinShan;
using UnityEngine;

///<summary>
///负责子弹的一切，包括移动、生命周期等
///还负责子弹和角色等的碰撞，需要加入子弹与子弹碰撞也在这里。值得注意的是：子弹是主体
///</summary>
public class BulletManager : MonoSingleton<BulletManager>
{
    private void FixedUpdate()
    {
        if (GameManager.Instance.IsInBattle)
        {
            UpdateBullets();
        }
    }

    public void ClearBullets()
    {
        //获取所有子弹
        GameObject[] bullet = GameObject.FindGameObjectsWithTag("Bullet");

        if (bullet.Length <= 0) return;

        for (int i = 0; i < bullet.Length; i++)
        {
            BulletState bs = bullet[i].GetComponent<BulletState>();
            if (!bs || bs.hp <= 0) continue;
            bs.duration = 0f;
            RemoveBullet(bullet[i]);
        }
    }

    /// <summary>
    /// 子弹运行逻辑
    /// 包括子弹移动、子弹碰撞、子弹移除
    /// </summary>
    private void UpdateBullets()
    {
        GameObject[] bullet = GameObject.FindGameObjectsWithTag("Bullet");
        if (bullet.Length <= 0) return;
        GameObject[] character = GameObject.FindGameObjectsWithTag("Character");
        if (bullet.Length <= 0 || character.Length <= 0) return;

        float timePassed = Time.fixedDeltaTime;

        for (int i = 0; i < bullet.Length; i++)
        {
            BulletState bs = bullet[i].GetComponent<BulletState>();
            if (!bs || bs.hp <= 0) continue;

            //如果是刚创建的，那么就要处理刚创建的事情
            if (bs.timeElapsed <= 0 && bs.model.onCreate != null)
            {
                bs.model.onCreate.Invoke(bullet[i]);
            }

            //处理子弹命中纪录信息
            int hIndex = 0;
            while (hIndex < bs.hitRecords.Count)
            {
                bs.hitRecords[hIndex].timeToCanHit -= timePassed;
                if (bs.hitRecords[hIndex].timeToCanHit <= 0 || bs.hitRecords[hIndex].target == null)
                {
                    //理论上应该支持可以鞭尸，所以即使target dead了也得留着……
                    bs.hitRecords.RemoveAt(hIndex);
                }
                else
                {
                    hIndex += 1;
                }
            }

            //处理子弹的移动信息
            bs.SetMoveForce(
                bs.tween == null ? Vector3.forward : bs.tween(bs.timeElapsed, bullet[i], bs.followingTarget)
            );

            //处理子弹的碰撞信息，如果子弹可以碰撞，才会执行碰撞逻辑
            if (bs.canHitAfterCreated > 0)
            {
                bs.canHitAfterCreated -= timePassed;
            }
            else
            {
                float bRadius = bs.model.radius;
                int bSide = -1;
                if (bs.caster)
                {
                    ChaState bcs = bs.caster.GetComponent<ChaState>();
                    if (bcs)
                    {
                        bSide = bcs.side;
                    }
                }

                for (int j = 0; j < character.Length; j++)
                {
                    if (bs.CanHit(character[j]) == false) continue;

                    ChaState cs = character[j].GetComponent<ChaState>();
                    if (!cs || cs.dead == true || cs.immuneTime > 0) continue;

                    if (
                        (bs.model.hitAlly == false && bSide == cs.side) ||
                        (bs.model.hitFoe == false && bSide != cs.side)
                    ) continue;

                    float cRadius = cs.property.hitRadius;
                    Vector3 bulletPosition = bullet[i].transform.position;
                    Vector3 characterPosition = character[j].transform.position;
                    // 设置 y 坐标为相同值（例如 0）
                    bulletPosition.y = 0;
                    characterPosition.y = 0;

                    // 计算 xz 平面的距离
                    float dis = Vector3.Distance(bulletPosition, characterPosition);

                    if (dis <= (cRadius + bRadius))
                    {
                        //命中了
                        bs.hp -= 1;

                        bs.model.onHit?.Invoke(bullet[i], character[j]);

                        if (bs.hp > 0)
                        {
                            bs.AddHitRecord(character[j]);
                        }
                        else
                        {
                            //对象池优化前 是直接销毁
                            //Destroy(bullet[i]);

                            //优化后代码 移除所有子物体 然后SetActive为false
                            RemoveBullet(bullet[i]);
                            continue;
                        }
                    }
                }
            }

            ///生命周期的结算
            bs.duration -= timePassed;
            bs.timeElapsed += timePassed;
            if (bs.duration <= 0 || bs.HitObstacle() == true)
            {
                bs.model.onRemoved?.Invoke(bullet[i]);
                //对象池优化前 是直接销毁
                //Destroy(bullet[i]);

                //优化后代码 移除所有子物体 然后SetActive为false
                TransformerHelper.RemoveAllChildren(bullet[i]);
                bullet[i].SetActive(false);
                continue;
            }
        }
    }

    /// <summary>
    /// 移除子弹，实际上是把子弹还给对象池
    /// </summary>
    private void RemoveBullet(GameObject bullet)
    {
        TransformerHelper.RemoveAllChildren(bullet);
        bullet.SetActive(false);
    }
}