using System.Collections.Generic;
using JinShan;
using UnityEngine;
using MoreMountains.Tools;
using Random = UnityEngine.Random;

namespace DesignerScripts
{
    public class Timeline
    {
        /// <summary>
        /// 辅助函数，尝试从paramsDict获取参数，如果找不到则使用默认参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="timeline"></param>
        /// <param name="paramsName">参数名称</param>
        /// <param name="index">参数数组序号</param>
        /// <param name="args">参数数组</param>
        /// <param name="defaultValue">默认参数</param>
        /// <returns></returns>
        private static T GetValueFromParams<T>(int index, object[] args, T defaultValue)
        {
            return args.Length > index ? (T)args[index] : defaultValue;
        }

        public static void Initialize()
        {
            functions = new Dictionary<string, TimelineEvent>()
            {
                {"CasterPlayAnim", CasterPlayAnim},
                {"CasterForceMove", CasterForceMove},
                {"CasterBlinkMove", CasterBlinkMove},
                {"CasterForceMoveByInput",CasterForceMoveByInput },
                {"SetCasterControlState", SetCasterControlState},
                {"PlaySightEffectOnCaster", PlaySightEffectOnCaster},
                {"PlayFeedbacksOnCaster", PlayFeedbacksOnCaster},
                {"PlayFeedbacksByManagerOnCaster", PlayFeedbacksByManagerOnCaster},
                {"PlayFeedbacksByManager", PlayFeedbacksByManager},
                {"StopSightEffectOnCaster", StopSightEffectOnCaster},
                {"FireBullet", FireBullet},
                {"FireRandomBullet",FireRandomBullet },
                {"SpinFireBullet", SpinFireBullet},
                {"CasterImmune", CasterImmune},
                {"CreateAoE", CreateAoE},
                {"AICreateAoE", AICreateAoE},
                {"AICreateAoEs",AICreateAoEs },
                {"CreateAoEAndSetDurationRadiusByParams", CreateAoEAndSetDurationRadiusByParams},
                {"CreateAoEAndSetDurationByParams",CreateAoEAndSetDurationByParams},
                {"CreateAoEInEnemy", CreateAoEInEnemy},
                {"AddBuffToCaster", AddBuffToCaster},
                {"SummonCharacter", SummonCharacter},
                {"SummonCharacterByEnemyData", SummonCharacterByEnemyData},
                {"SummonAICharacter", SummonAICharacter},
                {"SwordControl",SwordControl},
                {"CasterForceJump",CasterForceJump},
                {"AddShield",AddShield },
                {"ResetViewContainer", ResetViewContainer},
                {"AddAoEIndicator", AddAoEIndicator},
                {"PopTextOnCaster" ,PopTextOnCaster},
                {"FireBulletAtRandomAngles",FireBulletAtRandomAngles },
                {"CreateTurret",CreateTurret },
                {"CreateLaser", CreateLaser},
                {"LaserAddDuration",LaserAddDuration },
                {"PopChargingTime",PopChargingTime },
                {"CloneCharacter",CloneCharacter },
                {"ShowIndicator", ShowIndicator},
                {"ShowChargeDashIndicator", ShowChargeDashIndicator},
                {"PlaySound", PlaySound},
                {"BaseAttack",BaseAttack }
            };
        }

        private static void BaseAttack(TimelineObj timeline, object[] args)
        {
            string selectorId = GetValueFromParams<string>(0, args, "GetCurrentTarget");
            if (timeline.caster == null)
            {
                return;
            }
            ChessPiece attackerChessPiece = timeline.caster.GetComponentInChildren<ChessPiece>();
            List<ChessPiece> targetChessPieces = DataTargetSelector.data[selectorId](attackerChessPiece);
            ChaState attackerState = attackerChessPiece.GetComponent<ChaState>();
            Damage damage = new Damage(attackerState.property.attack);
            foreach (ChessPiece defenderChessPiece in targetChessPieces)
            {
                //如果目标位于攻击距离之外，无法造成伤害
                if (HexGrid.Instance.Distance(attackerChessPiece.CurrentCell, defenderChessPiece.CurrentCell) > attackerChessPiece.attackRange)
                {
                    continue;
                }
                DamageInfo damageInfo = new DamageInfo(attackerChessPiece.gameObject, defenderChessPiece.gameObject,
                    damage, 0f, attackerState.property.critic_rate, new DamageInfoTag[] { DamageInfoTag.directDamage });
                SceneVariants.CreateDamage(damageInfo);
            }
        }

        private static void PlaySound(TimelineObj timeline, object[] args)
        {
            string clipName = (string)args[0];
            MMSoundManager.MMSoundManagerTracks type = (MMSoundManager.MMSoundManagerTracks)args[1];
            GameSoundManager.Instance.PlaySoundOneTimes(clipName, type);
        }

        private static void ShowChargeDashIndicator(TimelineObj timeline, object[] args)
        {
            string bindpoint = GetValueFromParams<string>(0, args, "Body");
            float duration = GetValueFromParams<float>(1, args, 0.00f);
            float width = GetValueFromParams<float>(2, args, 0.00f);
            float maxLength = GetValueFromParams<float>(3, args, 0.00f);
            if (timeline.caster)
            {
                UnitBindPoint ubp = timeline.caster.GetComponent<UnitBindManager>().GetBindPointByKey(bindpoint, timeline.caster);
                if (timeline.caster.GetComponent<UnitChargeDashWarningIndicator>() == null)
                {
                    timeline.caster.AddComponent<UnitChargeDashWarningIndicator>();
                }
                timeline.caster.GetComponent<UnitChargeDashWarningIndicator>().
                    ShowChargeWarning(ubp.transform.position,
                    ubp.transform.position + ubp.transform.forward * maxLength,
                    duration, width, maxLength);
            }
        }

        private static void ShowIndicator(TimelineObj timeline, object[] args)
        {
            float duration = GetValueFromParams<float>(0, args, 0.00f);
            if (timeline.caster)
            {
                if (timeline.caster.GetComponent<UnitTrajectoryIndicator>() == null)
                {
                    timeline.caster.AddComponent<UnitTrajectoryIndicator>();
                }
                timeline.caster.GetComponent<UnitTrajectoryIndicator>().ShowRayForDuration(duration);
            }
        }

        private static void CloneCharacter(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                ChaState chaState1 = timeline.caster.GetComponent<ChaState>();

                GameObject cloneCharacter = SceneVariants.CreateCharacter(
                    "FireMage", chaState1.side, timeline.caster.transform.position, new ChaProperty(
                    100, 0, 100,
                    250, 10, 200, 20, 100,
                    200, 100, 15,
                    1.5f, 0.25f, 0.05f, 0.25f, 0.4f), chaState1.faceDegree, "FireMage");

                ChaState chaState = cloneCharacter.GetComponent<ChaState>();
                AddBuffInfo ThunderStrikeOnDeadBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["ThunderStrikeOnDead"], timeline.caster, cloneCharacter, 1, 10f, true, true);
                AddBuffInfo ScheduledDeadBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["ScheduledDead"], timeline.caster, cloneCharacter, 1, 3f, true, false);
                chaState.AddBuff(ThunderStrikeOnDeadBuffInfo);
                chaState.AddBuff(ScheduledDeadBuffInfo);
            }
        }

        private static void CasterBlinkMove(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                ChaState cs = timeline.caster.GetComponent<ChaState>();
                float dis = GetValueFromParams<float>(0, args, 0.00f);
                if (cs)
                {
                    // 获取当前游戏物体的位置
                    Vector3 currentPosition = cs.gameObject.transform.position;

                    // 计算新的位置，只在Z轴上位移
                    Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z + dis);

                    // 将游戏物体的位置设置为新位置
                    cs.gameObject.transform.position = SceneVariants.map.FixTargetPosition(currentPosition, cs.property.bodyRadius, newPosition,
                        cs.property.moveType, false, cs.gameObject).suggestPos;
                }
            }
        }

        private static void CreateAoEAndSetDurationRadiusByParams(TimelineObj timeline, object[] args)
        {
            float chargingPercent = Mathf.Clamp(timeline.realTimeElapsed / 3f, 0f, 1f);
            float aoeDuration = 0.2f + chargingPercent * 2f;
            float targetRadius = 5f + chargingPercent * 24f;
            AoeLauncher aoeLauncher = new AoeLauncher(DesignerTables.AoE.data["FireWave"],
                        timeline.caster, Vector3.zero, 3.00f, aoeDuration, 0f,
                        DesignerScripts.AoE.aoeTweenFunc["ScaleAoe"], null,
                        new Dictionary<string, object>() { { "targetRadius", targetRadius }, { "startRadius", 3f } });
            SceneVariants.CreateAoE(aoeLauncher);
        }

        private static void PopChargingTime(TimelineObj timeline, object[] args)
        {
            float chargingTime = timeline.realTimeElapsed;
            string popText = "";
            if (chargingTime <= 3f)
            {
                popText = string.Format("蓄力中：{0:P0}!", chargingTime / 3f);
            }
            else
            {
                popText = string.Concat("蓄力达到最大值!");
            }
            PopTextManager.Instance.PopUpStringOnCharacter(timeline.caster, string.Concat("蓄力中", popText, "!"));
        }

        private static void CreateLaser(TimelineObj timeline, object[] args)
        {
            object[] p = args;
            LaserLauncher laserLauncher = p.Length >= 0 ? (LaserLauncher)p[0] : null;
            if (laserLauncher != null)
            {
                UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;
                string bindPointKey = p.Length > 1 ? (string)p[1] : null;
                UnitBindPoint laserFirePosition = ubm.GetBindPointByKey(bindPointKey);
                laserLauncher.caster = timeline.caster;
                laserLauncher.firePositionTransform = laserFirePosition.transform;

                List<GameObject> Lasers = new List<GameObject>();
                if (timeline.values.ContainsKey("Laser"))
                {
                    Lasers = (List<GameObject>)timeline.values["Laser"];
                    Lasers.Add(SceneVariants.CreateLaser(laserLauncher));
                    timeline.values["Laser"] = Lasers;
                }
                GameObject laser = SceneVariants.CreateLaser(laserLauncher);
                Lasers.Add(laser);
                laser.GetComponent<LaserState>().launchingLaserTimelineObj = timeline;
                timeline.values.Add("Laser", Lasers);
            }
        }

        private static void LaserAddDuration(TimelineObj timeline, object[] args)
        {
            List<GameObject> Lasers = (List<GameObject>)timeline.values["Laser"];
            foreach (GameObject laser in Lasers)
            {
                if (laser != null && laser.GetComponent<LaserState>() != null)
                {
                    //laser.GetComponent<LaserState>().duration = 0.00f;
                }
            }
        }

        /// <summary>
        /// 在Caster身上pop一个text
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void PopTextOnCaster(TimelineObj timeline, object[] args)
        {
            ChaState chaState = timeline.caster.GetComponent<ChaState>();

            if (chaState == null)
            {
                return;
            }

            string text = (string)args[0];

            PopTextManager.Instance.PopUpStringOnCharacter(chaState.gameObject, text);
        }

        /// <summary>
        /// 添加aoe指示器
        /// 参数1 指示器对应的路径 默认"Effect/Circle/RedCircle"
        /// 参数2 指示器的大小
        /// 参数3 指示器的持续时间
        /// 参数4 指示器的位置
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void AddAoEIndicator(TimelineObj timeline, object[] args)
        {
            string indicator = GetValueFromParams(0, args, default(string));
            float size = GetValueFromParams(1, args, 0f);
            float time = GetValueFromParams(2, args, 0f);
            Vector3 pos = GetValueFromParams(3, args, default(Vector3));

            GameObject effect = SceneVariants.CreateSightEffect(indicator, pos, 0f);
            effect.GetComponent<SightEffect>().duration = time;
            ParticleSystemUtils.SetParticleParameter(effect, "MarkerCircle", "ShockWave", "startSize", size);
            ParticleSystemUtils.SetParticleParameter(effect, "MarkerCircle", "ShockWave", "startLifetime", time);
        }

        /// <summary>
        /// 重置美术物体的位置
        /// 模型动画会移动位置，导致美术模型和实际游戏物体错位
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void ResetViewContainer(TimelineObj timeline, object[] args)
        {
            if (timeline.caster == null)
            {
                return;
            }
            Transform parentTransform = timeline.caster.transform;
            if (parentTransform != null)
            {
                Transform viewContainer = parentTransform.Find("ViewContainer");
                if (viewContainer != null)
                {
                    if (viewContainer.childCount > 0)
                    {
                        Transform firstChild = viewContainer.GetChild(0);
                        firstChild.transform.localPosition = Vector3.zero;
                        firstChild.transform.localRotation = Quaternion.identity;
                        firstChild.transform.localScale = Vector3.one;
                    }
                }
            }
        }

        /// <summary>
        /// 给施法者添加护盾
        /// 参数0 int 要添加的护盾值
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void AddShield(TimelineObj timeline, object[] args)
        {
            int shield = GetValueFromParams<int>(0, args, 0);
            if (timeline.caster == null) return;
            ChaState chaState = timeline.caster.GetComponent<ChaState>();
            if (chaState)
            {
                chaState.ModResource(new ChaResource(0, 0, shield));
            }
        }

        public static Dictionary<string, TimelineEvent> functions = new Dictionary<string, TimelineEvent>();

        /// <summary>
        /// 读取gameobj/viewcontianer/feedbacks
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void PlayFeedbacksByManagerOnCaster(TimelineObj timeline, object[] args)
        {
            Transform feedbacks = TransformerHelper.FindChildByName(timeline.caster.transform, "Feedbacks");
            if (!feedbacks)
            {
                return;
            }
            feedbacks.GetComponent<FeedbacksManager>().play((string)args[0]);
        }

        /// <summary>
        /// 读取gameobj/viewcontianer/feedbacks
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void PlayFeedbacksByManager(TimelineObj timeline, object[] args)
        {
            string feedbackKey = GetValueFromParams<string>(0, args, "");
            FeedbacksManager.Instance.play(feedbackKey);
        }

        /// <summary>
        /// 青春版 默认播放落地反馈效果
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void PlayFeedbacksOnCaster(TimelineObj timeline, object[] args)
        {
            string feedbackId = GetValueFromParams<string>(0, args, "JumpLand");
            GameObject caster = timeline.caster;

            if (caster)
            {
                caster.GetComponent<ChaState>().PlayFeedbacks(feedbackId);
            }
        }

        ///<summary>
        ///在Caster的某个绑点(Muzzle/Head/Body)上发射一个子弹出来
        ///<param name="args">总共3个参数：
        ///[0]BulletLauncher：子弹发射信息，其中caster和position是需要获得后该写的，degree则需要加上角色的转向
        ///[1]string：角色身上绑点位置，默认Muzzle
        ///</param>
        ///</summary>
        private static void FireBullet(TimelineObj timeline, object[] args)
        {
            GameObject actor = null;
            if (timeline.caster)
            {
                actor = timeline.caster;
                if (timeline.timelineType == TimelineType.ComboSkill)
                {
                    actor = timeline.caster;
                }

                UnitBindManager ubm = actor.GetComponent<UnitBindManager>();
                if (!ubm) return;

                BulletLauncher bLauncher = GetValueFromParams<BulletLauncher>(0, args, null);
                if (bLauncher == null) return;

                string bindPointKey = GetValueFromParams<string>(1, args, "Muzzle");
                UnitBindPoint ubp = ubm.GetBindPointByKey(bindPointKey, actor);
                if (!ubp) return;

                bLauncher.caster = timeline.caster;
                bLauncher.fireDegree = actor.transform.rotation.eulerAngles.y;
                bLauncher.firePosition = ubp.transform.position;
                SceneVariants.CreateBullet(bLauncher);
            }
        }

        private static void FireRandomBullet(TimelineObj timeline, object[] args)
        {
            GameObject actor = null;
            if (timeline.caster)
            {
                string[] Bullets = new string[] {
                    "Green Dart",
                    "Thunder Missile",
                    "Flame Missile",
                    "Yellow Flying Sword",
                    "Crimson Dart",
                    "Blue Green Arrow",
                    "Purple Firework",
                    "Dagger",
                    "Ice Blue Missile",
                    "High-Speed Blue Missile",
                    "Yellow Missile with Purple Tail",
                    "Green Blob with Wood Attribute and Poison",
                    "Fast Small Red Missile",
                    "Ice Blue Arrow that Shatters on Impact",
                    "Peach-colored Missile that Explodes on Impact",
                    "Flame Shot",
                    "Lightning Orb",
                    "Fire Orb",
                    "Red Orb",
                    "Purple Arrow",
                    "High-Speed Rocket",
                    "Star Strike",
                    "Gold Nugget Strike",
                    "Green Cannon",
                    "Red Cannon",
                    "Lightning Missile",
                    "Enchanting Red Heart"
                };

                actor = timeline.caster;
                UnitBindManager ubm = actor.GetComponent<UnitBindManager>();
                if (!ubm) return;

                BulletLauncher bLauncher = GetValueFromParams<BulletLauncher>(0, args, null);
                if (bLauncher == null) return;

                bLauncher.model = DesignerTables.Bullet.data[Bullets[Random.Range(0, 27)]];
                string bindPointKey = GetValueFromParams<string>(1, args, "Muzzle");
                UnitBindPoint ubp = ubm.GetBindPointByKey(bindPointKey, actor);
                if (!ubp) return;

                bLauncher.caster = timeline.caster;
                bLauncher.fireDegree = actor.transform.rotation.eulerAngles.y;
                bLauncher.firePosition = ubp.transform.position;

                SceneVariants.CreateBullet(bLauncher);
            }
        }

        ///<summary>
        ///高速旋转（feedback）。从0-360度划分12个角度，每个0.2秒发射一枚绿色子弹。
        ///在Caster的某个绑点(Muzzle/Head/Body)上发射一个子弹出来
        ///<param name="args">总共3个参数：
        ///[0]BulletLauncher：子弹发射信息，其中caster和position是需要获得后该写的，degree则需要加上角色的转向
        ///[1]string：角色身上绑点位置，默认Muzzle
        ///</param>
        ///</summary>
        private static void SpinFireBullet(TimelineObj timeline, object[] args)
        {
            if (args.Length <= 0) return;

            GameObject actor = null;

            if (timeline.caster)
            {
                actor = timeline.caster;
                UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;

                BulletLauncher bLauncher = (BulletLauncher)args[0];
                UnitBindPoint ubp = ubm.GetBindPointByKey(args.Length > 1 ? (string)args[1] : "Muzzle", actor);
                if (!ubp) return;

                bLauncher.caster = timeline.caster;
                bLauncher.firePosition = ubp.transform.position;

                //读取技能创建时间 发射子弹 例如 1.2秒时 是330度发射子弹

                for (int i = 0; i < 3; i++)
                {
                    bLauncher.fireDegree = 30f * (timeline.timeElapsed - 0.10f) / 0.10f + Random.Range(-60f, 60f);
                    SceneVariants.CreateBullet(bLauncher);
                }
            }
        }

        ///<summary>
        // 随机发射子弹。从0-360度划分12个角度，每个0.2秒发射一枚绿色子弹。
        ///在Caster的某个绑点(Muzzle/Head/Body)上发射一个子弹出来
        ///<param name="args">总共3个参数：
        ///[0]BulletLauncher：子弹发射信息，其中caster和position是需要获得后该写的，degree则需要加上角色的转向
        ///[1]string：角色身上绑点位置，默认Muzzle
        ///</param>
        ///</summary>
        private static void FireBulletAtRandomAngles(TimelineObj timeline, object[] args)
        {
            if (args.Length <= 0) return;

            GameObject actor = null;

            if (timeline.caster)
            {
                actor = timeline.caster;
                UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;

                BulletLauncher bLauncher = (BulletLauncher)args[0];
                UnitBindPoint ubp = ubm.GetBindPointByKey(args.Length > 1 ? (string)args[1] : "Muzzle", actor);
                if (!ubp) return;

                bLauncher.caster = timeline.caster;
                bLauncher.firePosition = ubp.transform.position;

                for (int i = 0; i < 3; i++)
                {
                    bLauncher.fireDegree = actor.transform.rotation.eulerAngles.y + Random.Range(120f, 240f);
                    SceneVariants.CreateBullet(bLauncher);
                }
            }
        }

        ///<summary>
        ///读取角色身上的buff
        ///如果处于收剑状态：
        ///在Caster的某个绑点(Muzzle/Head/Body)上发射一个飞剑子弹出来
        ///如果处于出剑状态：
        ///通过buff中的信息，找到飞剑子弹进行移除，同时修改buff状态为收剑
        ///<param name="args">总共3个参数：
        ///[0]BulletLauncher：子弹发射信息，其中caster和position是需要获得后该写的，degree则需要加上角色的转向
        ///[1]string：角色身上绑点位置，默认Muzzle
        ///</param>
        ///</summary>
        private static void SwordControl(TimelineObj timeline, object[] args)
        {
            if (args.Length <= 0) return;
            GameObject actor = null;
            if (timeline.caster)
            {
                //通过参数3中的buff id 获得这把飞剑对应的buffobj
                ChaState chaState = timeline.caster.GetComponent<ChaState>();
                string buffId = args.Length >= 3 ? (string)args[2] : null;

                List<BuffObj> buffs = chaState.GetBuffById(buffId);
                if (buffs == null)
                {
                    Debug.Log("读取buff失败");
                    return;
                }

                actor = timeline.caster;
                UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;

                BulletLauncher bLauncher = (BulletLauncher)args[0];

                UnitBindPoint ubp = ubm.GetBindPointByKey(args.Length > 1 ? (string)args[1] : "Muzzle", actor);

                if (!ubp) return;

                //如果buffs的buffparam为空 说明还未出剑
                //则添加buff参数 记录为未出剑
                if (buffs[0].buffParam.Count == 0)
                {
                    buffs[0].buffParam.Add("IsOut", false);
                }

                //如果飞剑状态为出剑 则移除飞剑子弹
                if ((bool)buffs[0].buffParam["IsOut"])
                {
                    //收剑逻辑
                    //移除飞剑子弹
                    SceneVariants.RemoveBullet(buffs[0].buffParam["bullet"] as GameObject, true);
                    //重新显示飞剑武器

                    //设置收剑状态
                    buffs[0].buffParam["IsOut"] = false;
                }
                //否则为收剑状态 创建并发射飞剑子弹 并隐藏当前法宝
                else
                {
                    //创建并发射飞剑子弹
                    bLauncher.caster = timeline.caster;
                    bLauncher.fireDegree = actor.transform.rotation.eulerAngles.y;
                    bLauncher.firePosition = ubp.transform.position;

                    buffs[0].buffParam["bullet"] =
                    SceneVariants.CreateBullet(bLauncher);

                    //隐藏当前的飞剑武器
                    //目前buff id就等于这把剑的id

                    //设置收剑状态
                    buffs[0].buffParam["IsOut"] = true;
                }
            }
        }

        ///<summary>
        ///在caster=timeline.caster的面前位置aoe
        ///<param name="args">总共3个参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///[1]bool：true=面前，false=角色坐标
        ///</param>
        ///</summary>
        private static void CreateAoE(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;

                AoeLauncher aLauncher = GetValueFromParams<AoeLauncher>(0, args, null)?.Clone();
                if (aLauncher == null) return;

                bool inFront = GetValueFromParams<bool>(1, args, true);

                aLauncher.caster = timeline.caster;
                aLauncher.degree += timeline.caster.transform.rotation.eulerAngles.y;

                float rr = aLauncher.degree * Mathf.PI / 180;
                Vector3 pos = aLauncher.position;

                float dis = Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.z, 2));
                if (inFront)
                {
                    dis += timeline.caster.GetComponent<ChaState>().property.bodyRadius + aLauncher.radius;
                }

                aLauncher.position.x = dis * Mathf.Sin(rr) + timeline.caster.transform.position.x;
                aLauncher.position.z = dis * Mathf.Cos(rr) + timeline.caster.transform.position.z;

                aLauncher.tweenParam = new object[]
                {
            new Vector3(
                dis * Mathf.Sin(rr),
                0,
                dis * Mathf.Cos(rr)
            )
                };
                SceneVariants.CreateAoE(aLauncher);
            }
        }

        public static Vector3[] CalculateVertices(Vector3 center, float radius)
        {
            // 创建一个保存顶点的数组
            Vector3[] vertices = new Vector3[3];
            float angleIncrement = 120f; // 每个角度增量为120度

            for (int i = 0; i < 3; i++)
            {
                float angle = i * angleIncrement * Mathf.Deg2Rad;
                float x = center.x + radius * Mathf.Cos(angle);
                float z = center.z + radius * Mathf.Sin(angle);
                vertices[i] = new Vector3(x, center.y, z);
            }

            return vertices;
        }

        ///<summary>
        ///在caster=timeline.caster的面前位置aoe
        ///<param name="args">总共3个参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///[1]bool：true=面前，false=角色坐标
        ///[2]bool: true=添加随机偏移 true = 不添加随机偏移
        ///[3]float: 随机偏移的最大值
        ///</param>
        ///</summary>
        private static void AICreateAoEs(TimelineObj timeline, object[] args)
        {
            if (!timeline.caster)
                return;

            UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
            if (!ubm)
                return;

            AoeLauncher aLauncher = GetValueFromParams<AoeLauncher>(0, args, null)?.Clone();
            if (aLauncher == null)
                return;

            bool inFront = GetValueFromParams<bool>(1, args, true);

            aLauncher.caster = timeline.caster;
            aLauncher.degree += timeline.caster.transform.rotation.eulerAngles.y;

            float rr = aLauncher.degree * Mathf.Deg2Rad;
            Vector3 pos = aLauncher.position;

            float dis = Mathf.Sqrt(pos.x * pos.x + pos.z * pos.z);
            if (inFront)
            {
                ChaState casterState = timeline.caster.GetComponent<ChaState>();
                if (casterState != null)
                    dis += casterState.property.bodyRadius + aLauncher.radius;
            }

            bool addRandomOffset = GetValueFromParams<bool>(2, args, true);
            float randomOffset = GetValueFromParams<float>(3, args, 0f);

            aLauncher.position.x = dis * Mathf.Sin(rr) + timeline.caster.transform.position.x;
            aLauncher.position.z = dis * Mathf.Cos(rr) + timeline.caster.transform.position.z;

            Vector3 center = timeline.caster == SceneVariants.MainActor()
                ? GameManager.Instance.MousePositionOnXOZPlane
                : Vector3.zero;

            if (addRandomOffset)
            {
                UnitGetTarget unitGetTarget = aLauncher.caster.GetComponent<UnitGetTarget>();
                if (unitGetTarget != null && unitGetTarget.closestEnemy != null)
                {
                    Vector3 offsetPosition = unitGetTarget.closestEnemy.transform.position;
                    offsetPosition.x += Random.Range(0, randomOffset);
                    offsetPosition.z += Random.Range(0, randomOffset);
                    aLauncher.position = offsetPosition;
                    center = offsetPosition;
                }
            }

            aLauncher.tweenParam = new object[]
            {
        new Vector3(
            dis * Mathf.Sin(rr),
            0,
            dis * Mathf.Cos(rr)
        )
            };
            Vector3[] aoePos = CalculateVertices(center, 4.63f);
            foreach (Vector3 childPos in aoePos)
            {
                aLauncher.position = childPos;
                SceneVariants.CreateAoE(aLauncher);
            }
        }

        ///<summary>
        ///在caster=timeline.caster的面前位置aoe
        ///<param name="args">总共3个参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///[1]bool：true=面前，false=角色坐标
        ///[2]bool: true=添加随机偏移 true = 不添加随机偏移
        ///[3]float: 随机偏移的最大值
        ///</param>
        ///</summary>
        private static void AICreateAoE(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
                if (!ubm) return;

                AoeLauncher aLauncher = GetValueFromParams<AoeLauncher>(0, args, null)?.Clone();
                if (aLauncher == null) return;

                bool inFront = GetValueFromParams<bool>(1, args, true);

                aLauncher.caster = timeline.caster;
                aLauncher.degree += timeline.caster.transform.rotation.eulerAngles.y;

                float rr = aLauncher.degree * Mathf.PI / 180;
                Vector3 pos = aLauncher.position;

                float dis = Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.z, 2));
                if (inFront)
                {
                    dis += timeline.caster.GetComponent<ChaState>().property.bodyRadius + aLauncher.radius;
                }
                bool AddRandomOffset = GetValueFromParams<bool>(2, args, true);
                float RandomOffset = GetValueFromParams<float>(3, args, 0f);
                aLauncher.position.x = dis * Mathf.Sin(rr) + timeline.caster.transform.position.x;
                aLauncher.position.z = dis * Mathf.Cos(rr) + timeline.caster.transform.position.z;

                if (timeline.caster == SceneVariants.MainActor())
                {
                    aLauncher.position = GameManager.Instance.MousePositionOnXOZPlane;
                }
                else
                {
                    if (AddRandomOffset)
                    {
                        UnitGetTarget unitGetTarget = aLauncher.caster.GetComponent<UnitGetTarget>();
                        if (unitGetTarget && unitGetTarget.closestEnemy)
                        {
                            Vector3 offsetPosition = unitGetTarget.closestEnemy.transform.position;
                            offsetPosition.x += Random.Range(0, RandomOffset);
                            offsetPosition.z += Random.Range(0, RandomOffset);
                            aLauncher.position = offsetPosition;
                        }
                    }
                }

                aLauncher.tweenParam = new object[]
                {
            new Vector3(
                dis * Mathf.Sin(rr),
                0,
                dis * Mathf.Cos(rr)
            )
                };
                SceneVariants.CreateAoE(aLauncher);
            }
        }

        ///<summary>
        ///在caster=timeline.caster的面前位置aoe
        ///并且根据参数修改aoe的duration
        ///<param name="args">总共4个参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///[1]bool：true=面前，false=角色坐标
        ///[2]string：duration在字典中的键
        ///[3]float：默认duration
        ///</param>
        ///</summary>
        private static void CreateAoEAndSetDurationByParams(TimelineObj timeline, object[] args)
        {
            AoeLauncher aoeLauncher = (AoeLauncher)args[0];
            string durationKey = (string)args[2];
            float duration = (float)args[3];
            if (timeline.values.ContainsKey(durationKey))
            {
                duration = (float)timeline.values[durationKey];
            }
            aoeLauncher.duration = duration;
            args[0] = aoeLauncher;
            CreateAoE(timeline, args);
        }

        ///<summary>
        ///在caster=timeline.caster的面前位置aoe
        ///<param name="args">总共3个参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///[1]bool：true=面前，false=角色坐标
        ///</param>
        ///</summary>
        private static void CreateAoEInEnemy(TimelineObj timeline, object[] args)
        {
            if (args.Length <= 0) return;

            if (timeline.caster)
            {
                UnitBindManager ubm = timeline.caster.GetComponent<UnitBindManager>();
                if (timeline.timelineType == TimelineType.Weapon)
                {
                    ubm = timeline.weapon.GetComponent<UnitBindManager>();
                }
                if (!ubm) return;
                AoeLauncher aLauncher = GetValueFromParams<AoeLauncher>(0, args, null).Clone();
                GameObject target = timeline.caster.GetComponent<UnitGetTarget>().closestEnemy;
                if (target != null && target.GetComponent<ChaState>() != null)
                {
                    aLauncher.position = target.transform.position;
                    aLauncher.caster = timeline.caster;
                    SceneVariants.CreateAoE(aLauncher);
                }
            }
        }

        ///<summary>
        ///timelien的焦点角色播放某个动作，是否是跳转到那个动作一直播放还是会回到站立，这取决于animator里面做的，我也无能为力
        ///<param name="args">总共3个参数：
        ///[0]string：是要播放的动画
        ///[1]bool：是否要取得动画的方向，如果不要就直接用预设的值了
        ///[2]bool：是否启用当前正在进行的面向和移动角度，如果false或者缺省了，就代表启用timeline中储存的（开始时的）
        ///</param>
        ///</summary>
        private static void CasterPlayAnim(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                string animName = GetValueFromParams<string>(0, args, "");
                if (string.IsNullOrEmpty(animName)) return;

                bool getTail = GetValueFromParams<bool>(1, args, false);
                bool useCurrentDeg = GetValueFromParams<bool>(2, args, false);
                ChaState cs = timeline.caster.GetComponent<ChaState>();
                if (cs)
                {
                    float faceDeg = useCurrentDeg ? (cs != null ? cs.faceDegree : 0f) : (timeline.GetValue("faceDegree") != null ? (float)timeline.GetValue("faceDegree") : 0f);
                    float moveDeg = useCurrentDeg ? cs.moveDegree : (timeline.GetValue("moveDegree") != null ? (float)timeline.GetValue("moveDegree") : 0f);
                    if (getTail) animName += Utils.GetTailStringByDegree(faceDeg, moveDeg);
                    cs.Play(animName);
                }
            }
        }

        ///<summary>
        ///timeline的焦点角色强制进行移动
        ///<param name="args">总共4个参数：
        ///[0]float：想要强行移动的距离，单位：米。
        ///[1]float：在多久内完成这个移动，单位：秒。这是匀速直线移动的。
        ///[2]float：基于角色移动方向或者面向（取决于[2]），获得一个基础的移动角度偏移量。
        ///[3]bool：是否要基于角色移动方向，如果不是，就是基于角色的面朝方向。
        ///[4]bool：如果启用面向，是否启用正在进行的，而非timeline创建时的，缺省或者false代表启用timeline创建时产生的
        ///</param>
        ///</summary>
        private static void CasterForceMove(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                ChaState cs = timeline.caster.GetComponent<ChaState>();
                float dis = GetValueFromParams<float>(0, args, 0.00f);
                float inSec = GetValueFromParams<float>(1, args, 0.00f) / timeline.timeScale;
                float degOffset = GetValueFromParams<float>(2, args, 0.00f);
                bool basedOnMoveDir = GetValueFromParams<bool>(3, args, true);
                bool useCurrentDeg = GetValueFromParams<bool>(4, args, false);

                //如果是玩家操控角色
                //应该优先考虑让强制位移的终点为玩家鼠标位置
                Vector3 mousePosition = GameManager.Instance.MousePositionOnXOZPlane;
                float hopeDis = Vector3.Distance(timeline.caster.transform.position, mousePosition);
                float finalDis = Mathf.Clamp(hopeDis, 0, dis);
                // 计算移动时间的新值
                float distanceRatio = finalDis / dis;
                float adjustedTime = inSec * distanceRatio; // 根据距离比例调整时间

                //在timelineobj中记录时间
                if (timeline.values.ContainsKey("CasterForceMoveTime") == false)
                {
                    timeline.values.Add("CasterForceMoveTime", adjustedTime);
                }
                else
                {
                    timeline.values["CasterForceMoveTime"] = adjustedTime;
                }

                if (cs)
                {
                    object moveDegreeValue = timeline.GetValue("moveDegree");
                    object faceDegreeValue = timeline.GetValue("faceDegree");

                    float moveDegree = moveDegreeValue != null ? (float)moveDegreeValue : 0.0f;
                    float faceDegree = faceDegreeValue != null ? (float)faceDegreeValue : 0.0f;

                    float mr = (
                        (
                            basedOnMoveDir == true ?
                                (useCurrentDeg == true ? cs.moveDegree : moveDegree) :
                                (useCurrentDeg == true ? cs.faceDegree : faceDegree)
                        ) + degOffset
                    ) * Mathf.PI / 180.00f;

                    Vector3 mdir = new Vector3(
                        Mathf.Sin(mr) * finalDis,
                        0,
                        Mathf.Cos(mr) * finalDis
                    );
                    cs.AddForceMove(new MovePreorder(mdir, adjustedTime));
                }
            }
        }

        ///<summary>
        ///timeline的焦点角色强制进行移动
        ///<param name="args">总共4个参数：
        ///[0]float：想要强行移动的距离，单位：米。
        ///[1]float：在多久内完成这个移动，单位：秒。这是匀速直线移动的。
        ///[2]float：基于角色移动方向或者面向（取决于[2]），获得一个基础的移动角度偏移量。
        ///[3]bool：是否要基于角色移动方向，如果不是，就是基于角色的面朝方向。
        ///[4]bool：如果启用面向，是否启用正在进行的，而非timeline创建时的，缺省或者false代表启用timeline创建时产生的
        ///</param>
        ///</summary>
        private static void CasterForceMoveByInput(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                ChaState cs = timeline.caster.GetComponent<ChaState>();
                float dis = GetValueFromParams<float>(0, args, 0.00f);
                float inSec = GetValueFromParams<float>(1, args, 0.00f) / timeline.timeScale;
                float degOffset = GetValueFromParams<float>(2, args, 0.00f);
                bool basedOnMoveDir = GetValueFromParams<bool>(3, args, true);
                bool useCurrentDeg = GetValueFromParams<bool>(4, args, false);

                if (cs)
                {
                    float mr = (
                        (
                            basedOnMoveDir == true ?
                                (useCurrentDeg == true ? cs.moveDegree : (float)timeline.GetValue("moveDegree")) :
                                (useCurrentDeg == true ? cs.faceDegree : (float)timeline.GetValue("faceDegree"))
                        ) + degOffset
                    ) * Mathf.PI / 180.00f;

                    Vector3 mdir = new Vector3(
                        Mathf.Sin(mr) * dis,
                        0,
                        Mathf.Cos(mr) * dis
                    );

                    if (Input.GetKey(KeyCode.W))
                    {
                        mdir = new Vector3(0, 0, dis);
                    }

                    if (Input.GetKey(KeyCode.D))
                    {
                        mdir = new Vector3(dis, 0, 0);
                    }

                    if (Input.GetKey(KeyCode.S))
                    {
                        mdir = new Vector3(0, 0, -dis);
                    }

                    if (Input.GetKey(KeyCode.A))
                    {
                        mdir = new Vector3(-dis, 0, 0);
                    }

                    if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
                    {
                        mdir = new Vector3(-dis * 0.71f, 0, -dis * 0.71f);
                    }

                    if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
                    {
                        mdir = new Vector3(dis * 0.71f, 0, -dis * 0.71f);
                    }

                    if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
                    {
                        mdir = new Vector3(dis * 0.71f, 0, dis * 0.71f);
                    }

                    if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
                    {
                        mdir = new Vector3(-dis * 0.71f, 0, dis * 0.71f);
                    }
                    cs.AddForceMove(new MovePreorder(mdir, inSec));
                }
            }
        }

        ///<summary>
        ///timeline的焦点角色强制进行跳跃
        ///<param name="args">总共4个参数：
        ///[0]float：想要强行移动的距离，单位：米。
        ///[1]float：在多久内完成这次跳跃，单位：秒。这是匀速直线移动的。
        ///[2]float：基于角色移动方向或者面向（取决于[2]），获得一个基础的移动角度偏移量。
        ///[3]bool：是否要基于角色移动方向，如果不是，就是基于角色的面朝方向。
        ///[4]bool：如果启用面向，是否启用正在进行的，而非timeline创建时的，缺省或者false代表启用timeline创建时产生的
        ///</param>
        ///</summary>
        private static void CasterForceJump(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                ChaState cs = timeline.caster.GetComponent<ChaState>();
                float dis = GetValueFromParams<float>(0, args, 0.00f);
                float inSec = GetValueFromParams<float>(1, args, 0.00f) / timeline.timeScale;
                float degOffset = GetValueFromParams<float>(2, args, 0.00f);
                bool basedOnMoveDir = GetValueFromParams<bool>(3, args, true);
                bool useCurrentDeg = GetValueFromParams<bool>(4, args, false);

                //加上一点随机值
                dis *= 1f + Random.Range(0f, 1f);

                if (cs)
                {
                    float mr = (
                        (
                            basedOnMoveDir == true ?
                                (useCurrentDeg == true ? cs.moveDegree : (float)timeline.GetValue("moveDegree")) :
                                (useCurrentDeg == true ? cs.faceDegree : (float)timeline.GetValue("faceDegree"))
                        ) + degOffset
                    ) * Mathf.PI / 180.00f;

                    Vector3 mdir = new Vector3(
                        Mathf.Sin(mr) * dis,
                        0,
                        Mathf.Cos(mr) * dis
                    );
                    cs.AddForceMove(new MovePreorder(mdir, inSec));
                    cs.gameObject.GetComponent<JumpingY>().JumpStart(inSec, dis / 2);
                }
            }
        }

        ///<summary>
        ///设置timeline的焦点角色的ChaControlState
        ///<param name="args">总共3个参数：
        ///[0]bool：可否移动，如果得不到参数，就保持原值。
        ///[1]bool：可否转身，如果得不到参数，就保持原值。
        ///[2]bool：可否释放技能，如果得不到参数，就保持原值。
        ///</param>
        ///</summary>
        private static void SetCasterControlState(TimelineObj timeline, object[] args)
        {
            if (timeline.caster)
            {
                ChaState cs = timeline.caster.GetComponent<ChaState>();
                if (cs)
                {
                    if (args.Length >= 1) cs.timelineControlState.canMove = (bool)args[0];
                    if (args.Length >= 2) cs.timelineControlState.canRotate = (bool)args[1];
                    if (args.Length >= 3) cs.timelineControlState.canUseSkill = (bool)args[2];
                }
            }
        }

        ///<summary>
        ///在timeline焦点角色身上播放一个视觉特效
        ///<param name="args">总共4个参数：
        ///[0]string：要播放特效的绑点
        ///[1]string：特效的文件名，位于Prafabs/下
        ///[2]string：特效的key，用于删除的
        ///[3]bool：是否循环播放特效（循环就要手动删除）
        ///</param>
        ///</summary>
        private static void PlaySightEffectOnCaster(TimelineObj timeline, object[] args)
        {
            if (!timeline.caster) return;

            ChaState cs = timeline.caster.GetComponent<ChaState>();
            if (cs)
            {
                string bindPointKey = GetValueFromParams<string>(0, args, "Body");
                string effectName = GetValueFromParams<string>(1, args, "");
                string effectKey = GetValueFromParams<string>(2, args, Random.value.ToString());
                bool loop = GetValueFromParams<bool>(3, args, false);

                cs.PlaySightEffect(bindPointKey, effectName, effectKey, loop);
            }
        }

        ///<summary>
        ///在timeline焦点角色身上关闭一个视觉特效
        ///<param name="args">总共2个参数：
        ///[0]string：要关闭的特效所处绑点
        ///[1]string：特效的key，创建时产生的
        ///</param>
        ///</summary>
        private static void StopSightEffectOnCaster(TimelineObj timeline, object[] args)
        {
            if (!timeline.caster) return;

            ChaState cs = timeline.caster.GetComponent<ChaState>();
            if (cs)
            {
                string bindPointKey = GetValueFromParams<string>(0, args, "Body");
                string effectKey = GetValueFromParams<string>(1, args, "");
                if (string.IsNullOrEmpty(effectKey)) return;

                cs.StopSightEffect(bindPointKey, effectKey);
            }
        }

        ///<summary>
        ///设置timeline的caster身上的无敌时间
        ///<param name="args">总共1个参数：
        ///[0]float：无敌的时间，单位：秒
        ///</param>
        ///</summary>
        private static void CasterImmune(TimelineObj timeline, object[] args)
        {
            if (!timeline.caster) return;

            ChaState cs = timeline.caster.GetComponent<ChaState>();
            if (cs)
            {
                float immT = GetValueFromParams<float>(0, args, 0f);
                cs.SetImmuneTime(immT);
            }
        }

        ///<summary>
        ///给timeline的caster添加一个buff
        ///[0]AddBuffInfo：如何添加一个buff，其中caster和carrier都会是timeline.caster本身
        ///</summary>
        private static void AddBuffToCaster(TimelineObj timeline, object[] args)
        {
            if (!timeline.caster) return;

            AddBuffInfo abi = GetValueFromParams<AddBuffInfo>(0, args, default(AddBuffInfo));
            abi.caster = timeline.caster;
            abi.target = timeline.caster;
            ChaState cs = timeline.caster.GetComponent<ChaState>();
            if (cs)
            {
                cs.AddBuff(abi);
            }
        }

        private static void SummonCharacter(TimelineObj timeline, object[] args)
        {
            if (!timeline.caster) return;

            string prefab = GetValueFromParams<string>(0, args, "");
            ChaProperty cp = GetValueFromParams<ChaProperty>(1, args, new ChaProperty(100, 0, 0, 100, 0));
            float degree = GetValueFromParams<float>(2, args, 0f);
            string uaInfo = GetValueFromParams<string>(3, args, "");
            string[] tags = GetValueFromParams<string[]>(4, args, null);
            AddBuffInfo[] addBuffs = GetValueFromParams<AddBuffInfo[]>(5, args, new AddBuffInfo[0]);

            Vector3 pos = timeline.caster.transform.position;
            int side = timeline.caster.GetComponent<ChaState>().side;

            GameObject sumGuy = SceneVariants.CreateCharacter(prefab, side, pos, cp, degree, uaInfo, tags);
            ChaState sgs = sumGuy.GetComponent<ChaState>();
            for (int i = 0; i < addBuffs.Length; i++)
            {
                addBuffs[i].caster = timeline.caster;
                addBuffs[i].target = sumGuy;
                sgs.AddBuff(addBuffs[i]);
            }
        }

        private static void CreateTurret(TimelineObj timeline, object[] args)
        {
            string prefab = GetValueFromParams<string>(0, args, "");
            ChaProperty cp = GetValueFromParams<ChaProperty>(1, args, new ChaProperty(100, 0, 0, 100, 0));
            float degree = GetValueFromParams<float>(2, args, 0f);
            string uaInfo = GetValueFromParams<string>(3, args, "");
            string[] tags = GetValueFromParams<string[]>(4, args, null);
            AddBuffInfo[] addBuffs = GetValueFromParams<AddBuffInfo[]>(5, args, new AddBuffInfo[0]);

            Vector3 pos = SceneVariants.map.GetRandomPosForCharacter(new RectInt(0, 0, SceneVariants.map.MapWidth(), SceneVariants.map.MapHeight()));
            int side = timeline.caster.GetComponent<ChaState>().side;

            GameObject sumGuy = SceneVariants.CreateCharacter(prefab, side, pos, cp, degree, uaInfo, tags);
            ChaState sumGuycChaState = sumGuy.GetComponent<ChaState>();
            sumGuycChaState.LearnSkill(DesignerTables.Skill.data["LaunchingHighSpeedShell"]);

            ChaState chaState = timeline.caster.GetComponent<ChaState>();
            List<BuffObj> buffObjs = chaState.GetBuffById("TurretControl");
            if (buffObjs.Count <= 0)
            {
                AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["TurretControl"], timeline.caster, timeline.caster, 1, 10f, false, true);
                chaState.AddBuff(addBuffInfo);
                List<GameObject> turrets = new List<GameObject>() { sumGuy };
                chaState.GetBuffById("TurretControl")[0].buffParam.Add("turrets", turrets);
            }
            else
            {
                BuffObj buffObj = chaState.GetBuffById("TurretControl")[0];
                if (buffObj.buffParam.ContainsKey("turrets"))
                {
                    List<GameObject> turrets = (List<GameObject>)buffObj.buffParam["turrets"];
                    turrets.Add(sumGuy);
                    buffObj.buffParam["turrets"] = turrets;
                }
            }
        }

        ///<summary>
        ///创建一个buff给角色，并且给他添加一系列buff
        ///[0]string： prefab,
        ///[1]ChaProperty: baseProp,
        ///[2]float: degree,
        ///[3]string: unitAnimInfo = "Default_Gunner",
        ///[4]string[]: tags = null
        ///[5]AddBuffInfo[]: 开始时候要添加的buff
        ///[6]string[]: skills 开始时候要学习的技能
        ///[7]string: AI 状态机id
        ///</summary>
        private static void SummonAICharacter(TimelineObj timeline, object[] args)
        {
            if (!timeline.caster) return;

            string prefab = GetValueFromParams<string>(0, args, "");
            ChaProperty cp = GetValueFromParams<ChaProperty>(1, args, new ChaProperty(100, 0, 0, 100, 0));
            float degree = GetValueFromParams<float>(2, args, 0f);
            string uaInfo = GetValueFromParams<string>(3, args, "");
            string[] tags = GetValueFromParams<string[]>(4, args, null);
            AddBuffInfo[] addBuffs = GetValueFromParams<AddBuffInfo[]>(5, args, new AddBuffInfo[0]);
            string[] skills = GetValueFromParams<string[]>(6, args, null);
            string AIBrain = GetValueFromParams<string>(7, args, "");

            Vector3 pos = timeline.caster.transform.position;

            GameObject sumGuy = SceneVariants.CreateCharacter(prefab, timeline.caster.GetComponent<ChaState>().side, pos, cp, degree, uaInfo, tags);
            ChaState sgs = sumGuy.GetComponent<ChaState>();

            for (int i = 0; i < addBuffs.Length; i++)
            {
                addBuffs[i].caster = timeline.caster;
                addBuffs[i].target = sumGuy;

                if (addBuffs[i].buffModel.id == "SummonedEntity" && addBuffs[i].buffParam.ContainsKey("Summoner"))
                {
                    addBuffs[i].buffParam["Summoner"] = addBuffs[i].caster;
                }

                sgs.AddBuff(addBuffs[i]);
            }

            // Adding AI State Machine
            UnityEngine.Object.Instantiate(Resources.Load("Prefabs/AI/" + AIBrain), sumGuy.transform);

            // Learning skills
            for (int i = 0; i < skills.Length; i++)
            {
                sgs.LearnSkill(DesignerTables.Skill.data[skills[i]]);
            }
        }

        /// <summary>
        /// 围绕主角生成召唤物
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args"></param>
        private static void SummonCharacterByEnemyData(TimelineObj timeline, object[] args)
        {
            CharacterSpawnInfo spawnInfo = GetValueFromParams(0, args, new CharacterSpawnInfo("骷髅", 1));
            int count = spawnInfo.count;
            float radius = 3f;
            int side = timeline.caster.GetComponent<ChaState>().side;
            Vector3 playerPosition = timeline.caster.transform.position;
            Vector3[] positions = new Vector3[count];
            float angleStep = 360f / count;
            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep;
                float radian = angle * Mathf.Deg2Rad;
                float x = playerPosition.x + radius * Mathf.Cos(radian);
                float z = playerPosition.z + radius * Mathf.Sin(radian);
                positions[i] = new Vector3(x, playerPosition.y, z);
            }
            foreach (Vector3 pos in positions)
            {
                GameObject enemy = SceneVariants.CreateCharacter(
                spawnInfo.View, side,
                pos,
                spawnInfo.ChaProperty, Random.Range(0.00f, 359.99f), spawnInfo.Name, new string[] { });
            }
        }
    }
}