using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignerScripts
{
    ///<summary>
    ///子弹的效果
    ///</summary>
    public class Bullet
    {
        public static Dictionary<string, BulletOnCreate> onCreateFunc = new Dictionary<string, BulletOnCreate>(){
            {"RecordBullet", RecordBullet},
            {"SetBombBouncing", SetBombBouncing},
            {"CreateAoeFollowBullet", CreateAoeFollowBullet},
            {"SetUnitRotateSpeed",SetUnitRotateSpeed }
        };

        public static Dictionary<string, BulletOnHit> onHitFunc = new Dictionary<string, BulletOnHit>(){
            {"CommonBulletHit", CommonBulletHit},
            {"BleedingDamageOnHit", BleedingDamageOnHit},
            {"BladeMirageHit", BladeMirageHit},
            {"CreateAoEOnHit", CreateAoEOnHit},
            {"CloakBoomerangHit", CloakBoomerangHit},
            {"FlyingSwordHit",FlyingSwordHit},
            {"TheBoneChillingSpiritualFireBulletHit",TheBoneChillingSpiritualFireBulletHit },
            {"AddPoisonOnHit",AddPoisonOnHit },
            {"AddIgniteBuffOnHit",AddIgniteBuffOnHit},
            {"AddDeathRelayBuffOnHit",AddDeathRelayBuffOnHit },
            {"BulletDeflectionOnHit",BulletDeflectionOnHit }
        };

        public static Dictionary<string, BulletOnRemoved> onRemovedFunc = new Dictionary<string, BulletOnRemoved>(){
            {"CommonBulletRemoved", CommonBulletRemoved},
            {"CreateAoEOnRemoved", CreateAoEOnRemoved},
            {"CreateAoeOnRemoved",CreateAoeOnRemoved }
        };

        public static Dictionary<string, BulletTween> bulletTween = new Dictionary<string, BulletTween>(){
            {"SpeedUpFollowingTarget",SpeedUpFollowingTarget },
            {"FollowingTarget", FollowingTarget},
            {"CloakBoomerangTween", CloakBoomerangTween},
            {"SlowlyFaster", SlowlyFaster},
            {"BoomBallRolling", BoomBallRolling},
            {"FollowingTargetSword",FollowingTargetSword },
            {"SineWaveTween",SineWaveTween },
            {"SineWaveTweenHalfTDelay",SineWaveTweenHalfTDelay },
            {"RevolutionAroundCaster", RevolutionAroundCaster},
            {"FollowingMouse", FollowingMouse},
            {"Wandering", Wandering}
        };

        public static Dictionary<string, BulletTargettingFunction> targettingFunc = new Dictionary<string, BulletTargettingFunction>(){
            {"GetNearestEnemy", GetNearestEnemy},
            {"BulletCasterSelf", BulletCasterSelf}
        };

        private static Vector3 FollowingMouse(float t, GameObject bullet, GameObject target)
        {
            // 获取鼠标在屏幕上的位置
            Vector3 mouseScreenPosition = Input.mousePosition;

            // 将鼠标的屏幕坐标转换为世界坐标，并设置 y 坐标为子弹的 y 坐标
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.transform.position.y - bullet.transform.position.y));

            // 保证鼠标位置在xoz平面上，设置 y 坐标与子弹的 y 坐标一致
            mouseWorldPosition.y = bullet.transform.position.y;

            // 计算子弹到鼠标位置的方向
            Vector3 tarDir = mouseWorldPosition - bullet.transform.position;

            // 计算子弹当前朝向与目标方向之间的夹角
            float flyingRad = (Mathf.Atan2(tarDir.x, tarDir.z) * 180 / Mathf.PI - bullet.transform.eulerAngles.y) * Mathf.PI / 180;

            // 计算新的朝向向量
            Vector3 res = bullet.transform.forward;
            res.x = Mathf.Sin(flyingRad);
            res.z = Mathf.Cos(flyingRad);

            return res;
        }

        public static Vector3 GetTweenResultVector3ByDirection(Vector3 tarDir, GameObject bullet)
        {
            // 计算子弹当前朝向与目标方向之间的夹角
            float flyingRad = (Mathf.Atan2(tarDir.x, tarDir.z) * 180 / Mathf.PI - bullet.transform.eulerAngles.y) * Mathf.PI / 180;

            // 计算新的朝向向量
            Vector3 res = bullet.transform.forward;
            res.x = Mathf.Sin(flyingRad);
            res.z = Mathf.Cos(flyingRad);

            return res;
        }


        private static void BulletDeflectionOnHit(GameObject bullet, GameObject target)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            // 获取子弹发射者的 ChaState 组件
            ChaState casterChaState = bulletState.caster.GetComponent<ChaState>();

            // 初始化最小距离和最近的角色
            float minDistance = float.MaxValue;
            GameObject closestCharacter = null;

            // 遍历 GameManager 中的角色
            foreach (GameObject character in GameManager.Instance.Characters)
            {
                ChaState characterChaState = character.GetComponent<ChaState>();

                // 检查是否为不同的阵营
                if (characterChaState.side != casterChaState.side)
                {
                    // 排除目标
                    if (character != target)
                    {
                        // 检查是否已经命中过该目标
                        bool alreadyHit = false;
                        foreach (BulletHitRecord hitRecord in bulletState.hitRecords)
                        {
                            if (hitRecord.target == character)
                            {
                                alreadyHit = true;
                                break;
                            }
                        }

                        if (!alreadyHit)
                        {
                            // 计算角色与子弹之间的距离
                            float distance = Vector3.Distance(character.transform.position, bullet.transform.position);

                            // 如果距离比当前最小距离小，则更新最小距离和最近的角色
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestCharacter = character;
                            }
                        }
                    }
                }
            }

            // 检查是否找到了最近的角色
            if (closestCharacter != null)
            {
                // 计算子弹指向最近角色的向量
                Vector3 direction = closestCharacter.transform.position - bullet.transform.position;

                // 计算角度（弧度）
                float angleRad = Mathf.Atan2(direction.x, direction.z);

                // 转换为角度
                float angleDeg = angleRad * Mathf.Rad2Deg;

                // 在 xoz 平面上的角度
                float angleXOZ = angleDeg;

                // 在这里，你可以使用角度 angleXOZ 做你想要的处理
                bullet.GetComponent<UnitRotate>().SetRotation(angleXOZ);
            }
        }

        private static void SetUnitRotateSpeed(GameObject bullet)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (bulletState != null)
            {
                bulletState.GetComponent<UnitRotate>().rotateSpeed = (float)bulletState.model.onCreateParam[0];
            }
        }

        /// <summary>
        /// 发射飞剑子弹，命中敌人时创建n道剑影随机对n名敌人造成伤害
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="target"></param>
        private static void BladeMirageHit(GameObject bullet, GameObject target)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            CommonBulletHit(bullet, target);

            int SplitBulletCount = (int)bulletState.param["SplitBulletCount"];
            List<GameObject> newTargets = GetRandomEnemy(bulletState.caster.GetComponent<ChaState>().side, SplitBulletCount, target);

            for (int i = 0; i < SplitBulletCount; i++)
            {
                CreateBladeMirage(bulletState, newTargets[i]);
            }
        }

        private static void CreateBladeMirage(BulletState parentBulletState, GameObject target)
        {
            int parentRemainingSplittingTimes = (int)parentBulletState.param["RemainingSplittingTimes"];
            int parentSplitBulletCount = (int)parentBulletState.param["SplitBulletCount"];
            if (parentRemainingSplittingTimes <= 0)
            {
                return;
            }

            float angleInDegrees = UnityEngine.Random.Range(0f, 360f);

            if (target != null)
            {
                Vector3 fireDirection = target.transform.position - parentBulletState.transform.position;
                Vector3 directionXZ = new Vector3(fireDirection.x, 0f, fireDirection.z);

                // 计算该向量在 xoz 平面上的角度（以度为单位）
                angleInDegrees = Vector3.Angle(Vector3.forward, directionXZ);
            }
            Vector3 firePosition = new Vector3(parentBulletState.transform.position.x, 1, parentBulletState.transform.position.z);
            BulletLauncher bulletLauncher = new BulletLauncher(DesignerTables.Bullet.data["BladeMirage"],
                parentBulletState.caster, firePosition, angleInDegrees, 10f, 3f, 0.2f,
                null, null, true, new Dictionary<string, object> { { "RemainingSplittingTimes", parentRemainingSplittingTimes - 1 }, { "SplitBulletCount", parentSplitBulletCount } });
            SceneVariants.CreateBullet(bulletLauncher);
        }

        /// <summary>
        /// 随机获取指定数量的敌对单位
        /// </summary>
        /// <param name="allySide">我方立场</param>
        /// <param name="count">敌对单位数量</param>
        /// <param name="exceptionalEnemy">返回值不应该包含的敌对单位</param>
        /// <returns></returns>
        private static List<GameObject> GetRandomEnemy(int allySide, int count, GameObject exceptionalEnemy)
        {
            List<GameObject> result = new List<GameObject>();
            List<GameObject> characters = new List<GameObject>(GameManager.Instance.Characters); // 创建临时列表

            // 过滤掉exceptionalEnemy
            characters.RemoveAll(chara => chara == exceptionalEnemy || chara.GetComponent<ChaState>().side == allySide);

            // 如果敌对单位数量不足指定数量，返回null填充的列表
            if (characters.Count <= count)
            {
                // 将所有剩余敌对单位添加到结果列表
                result.AddRange(characters);

                // 用null填充直到达到指定数量
                while (result.Count < count)
                {
                    result.Add(null);
                }
            }
            else
            {
                // 使用 Fisher-Yates 随机洗牌算法打乱临时列表顺序
                for (int i = 0; i < characters.Count - 1; i++)
                {
                    int j = UnityEngine.Random.Range(i, characters.Count);
                    GameObject temp = characters[i];
                    characters[i] = characters[j];
                    characters[j] = temp;
                }

                // 选择前count个敌对单位作为结果
                result.AddRange(characters.GetRange(0, count));
            }

            return result;
        }

        /// <summary>
        /// 子弹围绕施法者旋转
        /// </summary>
        /// <param name="t"></param>
        /// <param name="bullet"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static Vector3 RevolutionAroundCaster(float t, GameObject bullet, GameObject target)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState || bulletState.caster == null)
            {
                return bullet.transform.forward;
            }
            GameObject caster = bulletState.caster;

            // 获取施法者和子弹的位置
            Vector3 casterPosition = caster.transform.position;
            casterPosition.y = 0;
            Vector3 bulletPosition = bullet.transform.position;
            bulletPosition.y = 0;
            // 计算子弹到施法者的方向向量
            Vector3 directionToCaster = (casterPosition - bulletPosition);

            return GetTweenResultVector3ByDirection(Vector3.Cross(directionToCaster, Vector3.up).normalized, bullet);
        }

        /// <summary>
        /// 创建一个aoe，并让这个aoe跟随子弹
        /// 参数0 aoelauncher
        /// </summary>
        /// <param name="bullet"></param>
        private static void CreateAoeFollowBullet(GameObject bullet)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState)
            {
                return;
            }

            AoeLauncher aoeLauncher = (AoeLauncher)bulletState.model.onCreateParam[0];
            //aoe的创建者就是 bullet的创建者
            aoeLauncher.caster = bulletState.caster;

            //这个aoe的tween就是跟随bullet
            aoeLauncher.tween = DesignerScripts.AoE.aoeTweenFunc["FollowBullet"];

            //把bullet放进这个aoe的param中
            aoeLauncher.param = new Dictionary<string, object>() { { "Bullet", bullet } };
            SceneVariants.CreateAoE(aoeLauncher);
        }

        /// <summary>
        /// 在子弹移除时，在子弹位置处创建一个aoe
        /// 参数0 aoelauncher
        /// </summary>
        /// <param name="bullet"></param>
        private static void CreateAoeOnRemoved(GameObject bullet)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState)
            {
                return;
            }

            AoeLauncher aoeLauncher = (AoeLauncher)bulletState.model.onRemovedParams[0];

            //aoe的创建者就是 bullet的创建者
            aoeLauncher.caster = bulletState.caster;

            //这个aoe的位置就是bullet的位置
            aoeLauncher.position = new Vector3(bullet.transform.position.x, 0, bullet.transform.position.z);

            SceneVariants.CreateAoE(aoeLauncher);
        }

        ///<summary>
        ///onHit
        ///普通子弹命中效果，参数：
        ///[0]伤害倍数
        ///[1]基础暴击率
        ///[2]命中视觉特效
        ///[3]播放特效位于目标的绑点，默认Body
        ///</summary>
        private static void CommonBulletHit(GameObject bullet, GameObject target)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            bulletState.lastHitTime = bulletState.timeElapsed;
            ParamDictionary onHitParam = bulletState.model.onHitParams;
            float damageTimes = onHitParam.Get<float>("攻击力加成", 1.0f);
            float critRate = onHitParam.Get<float>("基础暴击率", 0.25f);
            string sightEffect = onHitParam.Get<string>("命中视觉特效", "");
            string bpName = onHitParam.Get<string>("特效绑定点", "Body");

            if (!string.IsNullOrEmpty(sightEffect))
            {
                UnitBindManager ubm = target.GetComponent<UnitBindManager>();
                if (ubm)
                {
                    Vector3 hitPosition = bullet.transform.position;
                    ubm.CreateTemporaryBindEffect("Prefabs/" + sightEffect, hitPosition, "HitEffect");
                }
            }


            SceneVariants.CreateDamage(
                bulletState.caster,
                target,
                new Damage(Mathf.CeilToInt(damageTimes * bulletState.propWhileCast.attack)),
                bullet.transform.eulerAngles.y,
                critRate,
                new DamageInfoTag[] { DamageInfoTag.directDamage, }
            );
        }

        ///BleedingDamageOnHit
        private static void BleedingDamageOnHit(GameObject bullet, GameObject target)
        {
            CommonBulletHit(bullet, target);
            BulletState bulletState = bullet.GetComponent<BulletState>();
            ParamDictionary onHitParam = bulletState.model.onHitParams;
            int bleedingDamage = onHitParam.Get<int>("流血伤害", 10);

            ChaState targetState = target.GetComponent<ChaState>();
            BuffObj buff = targetState.GetBuffById("Bleeding").Count > 0 ? targetState.GetBuffById("Bleeding")[0] : null;
            if (buff == null)
            {
                AddBuffInfo bleedingBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["Bleeding"],
                bulletState.caster, target, 1, 3f, true, false,
                new Dictionary<string, object>() { { "RemainBleedingDamage", bleedingDamage } });
                targetState.AddBuff(bleedingBuffInfo);
            }
            else
            {
                buff.duration = 3f;
                if (buff.buffParam.ContainsKey("RemainBleedingDamage"))
                {
                    int damage = (int)buff.buffParam["RemainBleedingDamage"];
                    buff.buffParam["RemainBleedingDamage"] = damage + bleedingDamage;
                }
                else
                {
                    buff.buffParam["RemainBleedingDamage"] = bleedingDamage;
                }

            }

        }


        ///<summary>
        ///onHit
        ///骨冷灵火子弹命中效果，参数：
        ///[0]伤害倍数
        ///[1]基础暴击率
        ///[2]命中视觉特效
        ///[3]播放特效位于目标的绑点，默认Body
        ///</summary>
        private static void TheBoneChillingSpiritualFireBulletHit(GameObject bullet, GameObject target)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            bulletState.lastHitTime = bulletState.timeElapsed;
            ParamDictionary onHitParam = bulletState.model.onHitParams;
            float damageTimes = onHitParam.Get<float>("攻击力加成", 1.0f);
            float critRate = onHitParam.Get<float>("基础暴击率", 0.25f);
            string sightEffect = onHitParam.Get<string>("命中视觉特效", "");
            string bpName = onHitParam.Get<string>("特效绑定点", "Body");
            if (sightEffect != "")
            {
                UnitBindManager ubm = target.GetComponent<UnitBindManager>();
                if (ubm)
                {
                    ubm.AddBindGameObject(bpName, "Prefabs/" + sightEffect, "", false);
                }
            }

            //有百分之50概率 造成寒冷
            if (Toolbox.RandomResult(0.02f))
            {
                AddBuffInfo info = new AddBuffInfo(DesignerTables.Buff.data["Cold"],
                    bulletState.caster, target, 1, 3f, true, false);
                target.GetComponent<ChaState>().AddBuff(info);
            }
            else
            {
                if (Toolbox.RandomResult(0.02f))
                {
                    AddBuffInfo info = new AddBuffInfo(DesignerTables.Buff.data["Burn"],
                    bulletState.caster, target, 1, 10f);
                    target.GetComponent<ChaState>().AddBuff(info);
                }
            }

            SceneVariants.CreateDamage(
                bulletState.caster,
                target,
                new Damage(Mathf.CeilToInt(damageTimes * bulletState.propWhileCast.attack)),
                bullet.transform.eulerAngles.y,
                critRate,
                new DamageInfoTag[] { DamageInfoTag.directDamage, }
            );
        }

        ///<summary>
        ///onRemoved
        ///普通子结束，参数：
        ///[0]命中视觉特效
        ///</summary>
        private static void CommonBulletRemoved(GameObject bullet)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            object[] onRemovedParams = bulletState.model.onRemovedParams;
            string sightEffect = onRemovedParams.Length > 0 ? (string)onRemovedParams[0] : "";
            if (sightEffect != "")
            {
                SceneVariants.CreateSightEffect(
                    sightEffect,
                    bullet.transform.position,
                    bullet.transform.rotation.eulerAngles.y
                );
            }
        }

        ///<summary>
        ///targetting
        ///选择最近的敌人作为目标
        ///</summary>
        private static GameObject GetNearestEnemy(GameObject bullet, GameObject[] targets)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            int side = -1;
            if (bs.caster)
            {
                ChaState ccs = bs.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            GameObject bestTarget = null;
            float bestDis = float.MaxValue;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null)
                {
                    continue;
                }
                ChaState tcs = targets[i].GetComponent<ChaState>();
                if (!tcs || tcs.side == side || tcs.dead == true) continue;
                float dis2 = (
                    Mathf.Pow(bullet.transform.position.x - targets[i].transform.position.x, 2) +
                    Mathf.Pow(bullet.transform.position.z - targets[i].transform.position.z, 2)
                );
                if (bestDis > dis2 || bestTarget == null)
                {
                    bestTarget = targets[i];
                    bestDis = dis2;
                }
            }

            return bestTarget;
        }

        ///<summary>
        ///tween
        ///跟踪目标
        ///</summary>
        private static Vector3 FollowingTarget(float t, GameObject bullet, GameObject target)
        {
            Vector3 res = Vector3.forward;
            if (target != null)
            {
                Vector3 tarDir = target.transform.position - bullet.transform.position;
                float flyingRad = (Mathf.Atan2(tarDir.x, tarDir.z) * 180 / Mathf.PI - bullet.transform.eulerAngles.y) * Mathf.PI / 180;

                res.x = Mathf.Sin(flyingRad);
                res.z = Mathf.Cos(flyingRad);
            }
            return res;
        }

        ///<summary>
        ///tween
        ///跟踪目标 加速版
        ///</summary>
        private static Vector3 SpeedUpFollowingTarget(float t, GameObject bullet, GameObject target)
        {
            bullet.GetComponent<BulletState>().speed = 10f + t * 2f;

            Vector3 res = Vector3.forward;
            if (target != null && t >= 0.5f)
            {
                Vector3 tarDir = target.transform.position - bullet.transform.position;
                float flyingRad = (Mathf.Atan2(tarDir.x, tarDir.z) * 180 / Mathf.PI - bullet.transform.eulerAngles.y) * Mathf.PI / 180;

                res.x = Mathf.Sin(flyingRad);
                res.y = 1f;
                res.z = Mathf.Cos(flyingRad);
            }

            return res;
        }

        ///<summary>
        ///tween
        ///跟踪目标 飞剑版
        ///若飞剑上次攻击时间 与 子弹存在时间 之差大于xs，即距离上次攻击已经过去了x秒钟，才进入追踪状态
        ///</summary>
        private static Vector3 FollowingTargetSword(float t, GameObject bullet, GameObject target)
        {
            Vector3 res = Vector3.forward;

            BulletState bs = bullet.GetComponent<BulletState>();
            //若飞剑上次攻击时间 与 子弹存在时间 之差小于x s
            //继续往前飞
            if (!bs || bs.timeElapsed - bs.lastHitTime < 0.55f)
            {
                return res;
            }

            //否则寻找最近的敌人攻击
            //最近的敌人
            List<GameObject> characters = new List<GameObject>(GameManager.Instance.Characters);
            bs.followingTarget = GetNearestEnemy(bullet, characters.ToArray());

            if (target != null && !target.GetComponent<ChaState>().dead)
            {
                Vector3 tarDir = target.transform.position - bullet.transform.position;
                float flyingRad = (Mathf.Atan2(tarDir.x, tarDir.z) * 180 / Mathf.PI - bullet.transform.eulerAngles.y) * Mathf.PI / 180;

                res.x = Mathf.Sin(flyingRad);
                res.z = Mathf.Cos(flyingRad);
            }
            return res;
        }

        ///<summary>
        ///targetting
        ///选择子弹的施法者作为跟踪的目标s
        ///</summary>
        private static GameObject BulletCasterSelf(GameObject bullet, GameObject[] targets)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return null;
            return bulletState.caster;
        }

        ///<summary>
        ///Tween
        ///氪漏氪回力标的轨迹，向前丢出去以后，会开始飞回到丢出去的人手里，bulletObj.param
        ///["backTime"]多少秒以后回头，在这个时间内移动速度呈sin函数
        ///</summary>
        private static Vector3 CloakBoomerangTween(float t, GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            if (!bs) return Vector3.forward;
            float backTime = bs.param.ContainsKey("backTime") ? (float)bs.param["backTime"] : 1.0f; //默认1秒
            if (t < backTime)
            {
                //飞出去的过程
                float rad = t / backTime * Mathf.PI;
                return Vector3.forward * (Mathf.Sin(rad) + 0.100f);
            }
            else
            {
                if (target == null) return Vector3.back;
                float rad = Mathf.Min((t - backTime) / backTime * Mathf.PI, 0.5f);
                float dis = Mathf.Sin(rad) + 0.100f;
                Vector3 tarDir = target.transform.position - bullet.transform.position;
                float toRad = Mathf.Atan2(tarDir.x, tarDir.z) - bs.fireDegree * Mathf.PI / 180;
                return new Vector3(
                    Mathf.Sin(toRad) * dis,
                    0,
                    Mathf.Cos(toRad) * dis
                );
            }
        }

        ///<summary>
        ///onHit
        ///氪漏氪回力标命中效果，除了普通效果，就是命中自己的时候移除子弹，参数：
        ///[0]伤害倍数
        ///[1]基础暴击率
        ///[2]命中视觉特效
        ///[3]播放特效位于目标的绑点，默认Body
        ///</summary>
        private static void CloakBoomerangHit(GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            if (!bs) return;

            ChaState ccs = bs.caster.GetComponent<ChaState>();
            ChaState tcs = target.GetComponent<ChaState>();
            if (ccs != null && tcs != null && ccs.side != tcs.side)
            {
                CommonBulletHit(bullet, target);
            }
            else
            {
                float backTime = bs.param.ContainsKey("backTime") ? (float)bs.param["backTime"] : 1.0f; //默认1秒
                if (bs.timeElapsed > backTime && target.Equals(bs.caster))
                {
                    SceneVariants.RemoveBullet(bullet);
                    if (ccs) ccs.PlaySightEffect("Body", "Effect/Heart");
                }
            }
        }

        /// <summary>
        /// 飞剑命中效果，造成普通效果（造成伤害），
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="target"></param>
        private static void FlyingSwordHit(GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            if (!bs) return;

            ChaState ccs = bs.caster.GetComponent<ChaState>();
            ChaState tcs = target.GetComponent<ChaState>();
            if (ccs != null && tcs != null && ccs.side != tcs.side)
            {
                CommonBulletHit(bullet, target);
                CreateAoEOnTarget(bullet, target, 4);
            }
        }

        ///<summary>
        ///Tween
        ///逐渐加速的子弹，bulletObj参数：
        ///["turningPoint"]float：在第几秒达到预设的速度（100%），并且逐渐减缓增速。
        ///</summary>
        private static Vector3 SlowlyFaster(float t, GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            if (!bs) return Vector3.forward;
            float tp = 5.0f; //默认5秒后达到100%速度
            if (bs.param.ContainsKey("turningPoint")) tp = (float)bs.param["turningPoint"];
            if (tp < 1.0f) tp = 1.0f;
            return Vector3.forward * (2 * t / (t + tp));
        }
        public static Vector3 SineWaveTween(float t, GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            bs.useFireDegreeForever = true;
            Vector3 startPoint = bs.firePosition;
            float fireDegree = bs.fireDegree;
            float speed = bs.speed; // 假设 BulletState 包含 speed 属性

            // 计算当前时间点的位置
            float distance = t * speed;
            float z = distance;
            float x = 10f * Mathf.Sin(distance); // 正弦函数轨迹

            // 将相对于初始方向的轨迹转换为世界坐标
            Quaternion rotation = Quaternion.Euler(0, fireDegree, 0);
            Vector3 offset = new Vector3(x, 0, z); // 在 XZ 平面上移动
            Vector3 worldPosition = startPoint + rotation * offset;

            // 计算速度向量
            Vector3 velocity = worldPosition - bs.transform.position;

            return GetTweenResultVector3ByDirection(velocity, bullet);
        }

        public static Vector3 SineWaveTweenHalfTDelay(float t, GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            bs.useFireDegreeForever = true;
            Vector3 startPoint = bs.firePosition;
            float fireDegree = bs.fireDegree;
            float speed = bs.speed; // 假设 BulletState 包含 speed 属性

            // 计算当前时间点的位置
            float distance = t * speed;
            float z = distance;
            float x = -10f * Mathf.Sin(distance); // 正弦函数轨迹

            // 将相对于初始方向的轨迹转换为世界坐标
            Quaternion rotation = Quaternion.Euler(0, fireDegree, 0);
            Vector3 offset = new Vector3(x, 0, z); // 在 XZ 平面上移动
            Vector3 worldPosition = startPoint + rotation * offset;

            // 计算速度向量
            Vector3 velocity = worldPosition - bs.transform.position;

            return GetTweenResultVector3ByDirection(velocity, bullet);
        }

        public static Vector3 Wandering(float t, GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            float fireDegree = bs.fireDegree;
            float speed = bs.speed; // 假设 BulletState 包含 speed 属性
            Vector3 velocity = Vector3.forward;
            if (t < 0.5f)
            {
                float newFireDegree = fireDegree;
                // 角度转弧度
                float newFireRadian = newFireDegree * Mathf.Deg2Rad;
                // 计算新的方向向量
                velocity = new Vector3(Mathf.Sin(newFireRadian), 0, Mathf.Cos(newFireRadian)) * speed;
            }
            else
            {
                if (t % 1f <= 0.50f)
                {
                    return bullet.transform.forward;
                }
                bs.speed = 3f - 2f * (1f - Mathf.Pow(bs.timeElapsed / bs.duration, 2));
                // 添加随机偏差到发射角度
                float randomDeviation = UnityEngine.Random.Range(-45f, 45f);
                float newFireDegree = fireDegree + randomDeviation;
                // 角度转弧度
                float newFireRadian = newFireDegree * Mathf.Deg2Rad;
                // 计算新的方向向量
                velocity = new Vector3(Mathf.Sin(newFireRadian), 0, Mathf.Cos(newFireRadian)) * speed;
            }

            return GetTweenResultVector3ByDirection(velocity, bullet);
        }


        ///<summary>
        ///onCreate
        ///记录一下这个子弹，作为最后发射的子弹
        ///</summary>
        private static void RecordBullet(GameObject bullet)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            if (!bs || bs.caster == null) return;
            ChaState cs = bs.caster.GetComponent<ChaState>();
            if (!cs) return;
            List<BuffObj> bos = cs.GetBuffById("TeleportBulletPassive", new List<GameObject>() { bs.caster });
            if (bos.Count <= 0)
            {
                cs.AddBuff(new AddBuffInfo(
                    DesignerTables.Buff.data["TeleportBulletPassive"], bs.caster, bs.caster, 1, 10, true, true, new Dictionary<string, object>() { { "firedBullet", bullet } }
                ));
            }
            else
            {
                bos[0].buffParam["firedBullet"] = bullet;
            }
        }

        ///<summary>
        ///onCreate
        ///手雷丢出去，要设置一下动画那个
        ///</summary>
        private static void SetBombBouncing(GameObject bullet)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            BouncingBallY bb = bullet.GetComponentInChildren<BouncingBallY>();
            if (!bs || !bb) return;
            float totalTime = bs.duration;
            if (totalTime <= 0)
            {
                Debug.Log("Boom Explosed immeditly");
                return;
            }
            float[] dTime = new float[]{
                totalTime * 3.000f / 6.000f,
                totalTime * 5.000f / 6.000f,
                totalTime - 0.001f
            };
            float highest = 2.2f;
            bb.ResetTo(highest, dTime);
        }

        ///<summary>
        ///Tween
        ///手雷的轨迹，在这里要做的是修改bullet的移动模式，一般不推荐这么干
        ///</summary>
        private static Vector3 BoomBallRolling(float t, GameObject bullet, GameObject target)
        {
            BulletState bs = bullet.GetComponent<BulletState>();
            BouncingBallY bb = bullet.GetComponentInChildren<BouncingBallY>();
            if (!bs || !bb) return Vector3.forward;
            MoveType toType = MoveType.fly;
            if (bb.hitGroundAt.Length <= 0 || t > bb.hitGroundAt[bb.hitGroundAt.Length - 1])
            {
                toType = MoveType.ground;
            }
            else
            {
                float tt = Time.fixedDeltaTime;
                for (int i = 0; i < bb.hitGroundAt.Length; i++)
                {
                    if (bb.hitGroundAt[i] - tt <= t && t <= bb.hitGroundAt[i] + tt)
                    {
                        toType = MoveType.ground;
                        break;
                    }
                }
            }

            bs.SetMoveType(toType);
            return Vector3.forward;
        }

        ///<summary>
        ///onRemoved
        ///在子弹位置创建一个aoe，所以aoe的始作俑者肯定是caster了，位置也是子弹位置，填写什么都无效，角度也是子弹角度，参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///[1]AoeLauncher：如果bullet移除时后duration>0或者是obstacled，则会创建这个，如果有这个的话
        ///</summary>
        private static void CreateAoEOnRemoved(GameObject bullet)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            object[] onRemovedParams = bulletState.model.onRemovedParams;
            if (onRemovedParams.Length <= 0) return;
            AoeLauncher al = (AoeLauncher)onRemovedParams[0];
            if (onRemovedParams.Length > 1 && (bulletState.duration > 0 || bulletState.HitObstacle() == true))
            {
                al = (AoeLauncher)onRemovedParams[1];
            }
            al.caster = bulletState.caster;
            al.position = bullet.transform.position;
            al.degree = bullet.transform.eulerAngles.y;
            Debug.Log("to create aoe effect " + al.model.prefab);
            SceneVariants.CreateAoE(al);
        }



        ///<summary>
        ///onHit
        ///在子弹位置创建一个aoe，所以aoe的始作俑者肯定是caster了，位置也是子弹位置，填写什么都无效，角度也是子弹角度，参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///</summary>
        private static void CreateAoEOnHit(GameObject bullet, GameObject target)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            ParamDictionary onHitParams = bulletState.model.onHitParams;
            AoeLauncher al = onHitParams.Get<AoeLauncher>("Aoe发射信息");
            if (al == null) return;
            al.caster = bulletState.caster;
            al.position = bullet.transform.position;
            al.degree = bullet.transform.eulerAngles.y;
            SceneVariants.CreateAoE(al);
        }

        private static void AddPoisonOnHit(GameObject bullet, GameObject target)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            ChaState chaState = target.GetComponent<ChaState>();
            if (!chaState) return;
            AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["Poisoning"], null, null, 1, 3f);
            addBuffInfo.caster = bulletState.caster;
            addBuffInfo.target = target;
            chaState.AddBuff(addBuffInfo);
        }

        private static void AddIgniteBuffOnHit(GameObject bullet, GameObject target)
        {
            CommonBulletHit(bullet, target);
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            ChaState chaState = target.GetComponent<ChaState>();
            if (!chaState) return;
            AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["Ignite"], null, null, 1, 5f, true);
            addBuffInfo.caster = bulletState.caster;
            addBuffInfo.target = target;
            chaState.AddBuff(addBuffInfo);
        }

        //DeathRelayDebuff
        private static void AddDeathRelayBuffOnHit(GameObject bullet, GameObject target)
        {
            CommonBulletHit(bullet, target);
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            ChaState chaState = target.GetComponent<ChaState>();
            if (!chaState) return;
            AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["DeathRelayDebuff"], null, null, 1, 999f, true);
            addBuffInfo.caster = bulletState.caster;
            addBuffInfo.target = target;
            chaState.AddBuff(addBuffInfo);
        }
        ///<summary>
        ///在子弹位置创建一个aoe，所以aoe的始作俑者肯定是caster了，位置也是子弹位置，填写什么都无效，角度也是子弹角度，参数：
        ///index:参数的位置
        ///</summary>
        private static void CreateAoEOnTarget(GameObject bullet, GameObject target, int index)
        {
            BulletState bulletState = bullet.GetComponent<BulletState>();
            if (!bulletState) return;
            ParamDictionary onHitParams = bulletState.model.onHitParams;
            if (onHitParams.Count <= index) return;
            AoeLauncher al = onHitParams.Get<AoeLauncher>("Aoe发射信息");
            if (al == null) return;
            al.caster = bulletState.caster;
            al.position = target.transform.position;
            al.degree = bullet.transform.eulerAngles.y;
            SceneVariants.CreateAoE(al);
        }
    }
}