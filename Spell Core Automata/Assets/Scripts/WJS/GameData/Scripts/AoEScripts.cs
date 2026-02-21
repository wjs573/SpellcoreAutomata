using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class AoEScripts
    {
        public static Dictionary<string, AoeOnCreate> onCreateFunc = new Dictionary<string, AoeOnCreate>(){
            {"DoDamageOnCreate", DoDamageOnCreate},
            {"CreateSightEffect", CreateSightEffect},
        };

        public static Dictionary<string, AoeOnRemoved> onRemovedFunc = new Dictionary<string, AoeOnRemoved>(){
            {"DoDamageOnRemoved", DoDamageOnRemoved},
            {"CreateAoeOnRemoved", CreateAoeOnRemoved},
            {"BarrelExplosed", BarrelExplosed},
            {"DoDamageAndAddBuffOnRemoved",DoDamageAndAddBuffOnRemoved },
        };


        public static Dictionary<string, AoeOnTick> onTickFunc = new Dictionary<string, AoeOnTick>(){
            {"BlackHole", BlackHole},
            {"DoDamageOnTick", DoDamageOnTick},
            {"AddBuffOnTick", AddBuffOnTick},
            {"GatheringEnemiesOnTick",GatheringEnemiesOnTick },
            {"DragAndDamageEnemy",DragAndDamageEnemy }
        };

        public static Dictionary<string, AoeOnCharacterEnter> onChaEnterFunc = new Dictionary<string, AoeOnCharacterEnter>(){
            {"DoDamageToEnterCha", DoDamageToEnterCha},
            {"DoDamageAndForceMoveToEnterCha", DoDamageAndForceMoveToEnterCha},
        };

        public static Dictionary<string, AoeOnCharacterLeave> onChaLeaveFunc = new Dictionary<string, AoeOnCharacterLeave>()
        {
        };

        public static Dictionary<string, AoeOnBulletEnter> onBulletEnterFunc = new Dictionary<string, AoeOnBulletEnter>(){
            {"BlockBullets", BlockBullets},
            {"SpaceMonkeyBallHit", SpaceMonkeyBallHit}
        };

        public static Dictionary<string, AoeOnBulletLeave> onBulletLeaveFunc = new Dictionary<string, AoeOnBulletLeave>()
        {
        };

        public static Dictionary<string, AoeTween> aoeTweenFunc = new Dictionary<string, AoeTween>(){
            {"AroundCaster", AroundCaster},
            {"FollowCaster",FollowCaster },
            {"SpawnInFront",SpawnInFront},
            {"SpaceMonkeyBallRolling", SpaceMonkeyBallRolling},
            {"FollowBullet", FollowBullet},
            {"ScaleAoe",ScaleAoe }
        };

        private static void DragAndDamageEnemy(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (aoeState == null)
            {
                return;
            }

            //拖拽敌人
            GatheringEnemiesOnTick(aoe);
            if (aoeState.caster == null)
            {
                return;
            }
            ChaState casterChaState = aoeState.caster.GetComponent<ChaState>();
            if (casterChaState == null)
            {
                return;
            }

            //创建伤害消息
            DamageInfo damageInfo = new DamageInfo(aoeState.caster, null, new Damage(20),
                0f, 0f, new DamageInfoTag[] { DamageInfoTag.periodDamage });
            foreach (GameObject cha in aoeState.characterInRange)
            {
                ChaState chaState = cha.GetComponent<ChaState>();
                if (chaState != null && chaState.side != casterChaState.side)
                {
                    damageInfo.defender = cha;
                    //添加伤害消息
                    SceneVariants.CreateDamage(damageInfo);
                }
            }
        }

        /// <summary>
        /// 给范围内的单位添加buff
        /// </summary>
        /// <param name="aoe"></param>
        /// <param name="cha"></param>
        /// <param name="addBuffInfo"></param>
        private static void ModifyBuffOnCharacterEnter(GameObject aoe, List<GameObject> cha, AddBuffInfo addBuffInfo, bool includeAlly, bool includeEnemy)
        {
            if (aoe == null || aoe.GetComponent<AoeState>() == null)
            {
                return;
            }
            AoeState aoeState = aoe.GetComponent<AoeState>();

            if (aoeState.caster == null || aoeState.caster.GetComponent<ChaState>() == null)
            {
                return;
            }
            int side = aoeState.caster.GetComponent<ChaState>().side;
            foreach (GameObject character in cha)
            {
                ChaState chaState = character.GetComponent<ChaState>();
                if (includeEnemy && chaState.side != side)
                {
                    addBuffInfo.target = character;
                    chaState.AddBuff(addBuffInfo);
                }
                if (includeAlly && chaState.side == side)
                {
                    addBuffInfo.target = character;
                    chaState.AddBuff(addBuffInfo);
                }
            }
        }

        /// <summary>
        /// Aoe保持在角色身前的半个身位
        /// </summary>
        /// <param name="aoe"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static AoeMoveInfo SpawnInFront(GameObject aoe, float t)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (aoeState == null || aoeState.caster == null) return null;

            ChaState chaState = aoeState.caster.GetComponent<ChaState>();

            float HitRadius = chaState == null ? 0.5f : chaState.property.hitRadius;

            Vector3 b = aoeState.caster.transform.position;

            Vector3 forward = aoeState.caster.transform.forward;

            //aoe的目标位置，即释放者的面前一个HitRadius的位置
            Vector3 aoePosition = b + forward * HitRadius;

            Vector3 targetP = new Vector3(
                aoePosition.x - aoe.transform.position.x,
                0,
                aoePosition.z - aoe.transform.position.z
            );
            return new AoeMoveInfo(MoveType.fly, targetP, 0);
        }

        /// <summary>
        /// aoe跟随子弹移动
        /// 子弹信息在tween param中
        /// </summary>
        /// <param name="aoe"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static AoeMoveInfo FollowBullet(GameObject aoe, float t)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (aoeState == null || aoeState.caster == null) return null;

            GameObject bullet = (GameObject)aoeState.param["Bullet"];

            if (!bullet)
            {
                return new AoeMoveInfo(MoveType.fly, Vector3.zero, aoeState.caster.transform.eulerAngles.y);
            }

            Vector3 b = aoeState.transform.position;

            //距离
            Vector3 targetP = new Vector3(
                bullet.transform.position.x - b.x,
                0,
                bullet.transform.position.z - b.z
            );

            //方向
            float angle = Mathf.Atan2(targetP.x, targetP.z) * Mathf.Rad2Deg;

            return new AoeMoveInfo(MoveType.fly, targetP, angle);
        }

        ///<summary>
        ///aoeTween
        ///环绕施法者旋转，参数：
        ///[0]float：距离caster的距离（单位米）
        ///[1]float：移动速度（度/秒），正负的效果是方向
        ///</summary>
        private static AoeMoveInfo AroundCaster(GameObject aoe, float t)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (aoeState == null || aoeState.caster == null) return null;
            Vector3 b = aoeState.caster.transform.position;
            float dis = aoeState.tweenParam.Length > 0 ? (float)aoeState.tweenParam[0] : 0;
            float degPlus = aoeState.tweenParam.Length > 1 ? (float)aoeState.tweenParam[1] : 0;
            float cDeg = degPlus * t;
            float dr = cDeg * Mathf.PI / 180;

            Vector3 targetP = new Vector3(
                b.x + Mathf.Sin(dr) * dis - aoe.transform.position.x,
                0,
                b.z + Mathf.Cos(dr) * dis - aoe.transform.position.z
            );
            return new AoeMoveInfo(MoveType.fly, targetP, cDeg % 360);
        }

        ///<summary>
        ///aoeTween
        ///跟随施法者一起移动，参数：
        ///[0]float：距离caster的距离（单位米）
        ///[1]float：移动速度（度/秒），正负的效果是方向
        ///</summary>
        private static AoeMoveInfo FollowCaster(GameObject aoe, float t)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (aoeState == null || aoeState.caster == null) return null;
            Vector3 b = aoeState.caster.transform.position;
            float dis = 0;
            float degPlus = 0;
            float cDeg = degPlus * t;
            float dr = cDeg * Mathf.PI / 180;

            Vector3 targetP = new Vector3(
                b.x + Mathf.Sin(dr) * dis - aoe.transform.position.x,
                0,
                b.z + Mathf.Cos(dr) * dis - aoe.transform.position.z
            );
            return new AoeMoveInfo(MoveType.fly, targetP, aoeState.caster.transform.eulerAngles.y);
        }

        ///<summary>
        /// AOE缩放Tween函数
        /// 参数：
        /// [0] float：目标半径
        /// [1] float：Tween持续时间
        ///</summary>
        private static AoeMoveInfo ScaleAoe(GameObject aoe, float t)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (aoeState == null) return null;
            float targetRadius = (float)aoeState.param["targetRadius"]; // 初始半径
            float startRadius = (float)aoeState.param["startRadius"]; // 记录初始半径
            float duration = aoeState.duration; // AOE的持续时间

            // 计算当前时间对应的目标半径
            float currentRadius = Mathf.Lerp(startRadius, targetRadius, t / duration);

            // 更新AOE的半径
            aoeState.radius = currentRadius;

            // 根据半径更新其他AOE属性（如果需要的话）
            // ...

            return FollowCaster(aoe, t); // 返回null，因为不需要移动AOE
        }

        ///<summary>
        ///onBulletEnter
        ///消灭所有进入的敌人的子弹，参数：
        ///[0]bool：是否有抵挡次数限制
        ///来自AoeState.aoeParam的参数：
        ///["times"]int：抵消多少次
        ///</summary>
        private static void BlockBullets(GameObject aoe, List<GameObject> bullets)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;
            AoeModel am = aoeState.model;
            bool countLimited = am.onBulletEnterParams.Length > 0 ? (bool)am.onBulletEnterParams[0] : false;
            int times = aoeState.param.ContainsKey("times") ? (int)aoeState.param["times"] : 1;

            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                side = ccs.side;
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                BulletState bs = bullets[i].GetComponent<BulletState>();
                int bSide = -1;
                if (bs && bs.caster)
                {
                    ChaState bcs = bs.caster.GetComponent<ChaState>();
                    if (bcs) bSide = bcs.side;
                }
                if (side != bSide)
                {
                    SceneVariants.RemoveBullet(bullets[i], false);
                    SceneVariants.CreateSightEffect("Effect/HitEffect_B", aoe.transform.position, aoe.transform.eulerAngles.y);
                }
            }

            times -= 1;
        }

        ///<summary>
        ///aoeTween
        ///小猴设计的滚球，往前滚动，受到攻击会略微转向。参数：
        ///[0]Vector3：原始的力量
        ///来自AoeState.aoeParam的参数：
        ///["forces"]List<Vector3>：被子弹施加的力
        ///</summary>
        private static AoeMoveInfo SpaceMonkeyBallRolling(GameObject aoe, float t)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return null;

            Vector3 velocity = aoeState.tweenParam.Length > 0 ? (Vector3)aoeState.tweenParam[0] : Vector3.zero;
            velocity *= Time.fixedDeltaTime; //算的是一个tick的，所以得在这里乘一下，回头再读取的地方除一下，这是因为设计者在设计这个函数时候思考环境不同所产生的必须要的“牺牲”
            List<Vector3> forces = aoeState.param.ContainsKey("forces") ? (List<Vector3>)aoeState.param["forces"] : null;
            if (forces != null)
            {
                for (int i = 0; i < forces.Count; i++)
                {
                    velocity += forces[i] * Time.fixedDeltaTime;
                }
            }

            float dis = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.z, 2));
            float rr = Mathf.Atan2(velocity.x, velocity.z);
            float rotateTo = rr * 180 / Mathf.PI;

            return new AoeMoveInfo(MoveType.fly, new Vector3(Mathf.Sin(rr) * dis, 0, Mathf.Cos(rr) * dis), rotateTo);
        }

        ///<summary>
        ///onBulletEnter
        ///小猴设计的滚球，挨打后会吃到来自子弹的力，参数：
        ///[0]float：力的大小，米/秒
        ///</summary>
        private static void SpaceMonkeyBallHit(GameObject aoe, List<GameObject> bullets)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            float baseForce = aoeState.model.onBulletEnterParams.Length > 0 ? (float)aoeState.model.onBulletEnterParams[0] : 0;
            if (baseForce == 0) return;

            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                side = ccs.side;
            }

            if (aoeState.param.ContainsKey("forces") == false)
            {
                aoeState.param["forces"] = new List<Vector3>();
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                BulletState bs = bullets[i].GetComponent<BulletState>();
                int bSide = -1;

                if (bs)
                {
                    if (bs.caster)
                    {
                        ChaState bcs = bs.caster.GetComponent<ChaState>();
                        if (bcs) bSide = bcs.side;
                    }
                    if (bSide == side)
                    {
                        Vector3 bMove = bs.velocity * baseForce;    //算了，就直接乘把，凑合凑合
                        ((List<Vector3>)aoeState.param["forces"]).Add(bMove);
                        SceneVariants.RemoveBullet(bullets[i]);
                    }
                }
            }

            float scaleTo = 1 + ((List<Vector3>)aoeState.param["forces"]).Count * 0.05f;
            aoeState.radius = 0.25f * scaleTo;
            aoeState.SetViewScale(scaleTo);
            aoeState.ModViewY(aoeState.radius);
        }

        ///<summary>
        ///onChaEnter
        ///对于范围内的人造成伤害（治疗得另写一个，这是严肃的），参数：
        ///[0]Damage：基础伤害
        ///[1]float：施法者攻击倍率
        ///[2]bool：对敌人有效
        ///[3]bool：对盟军有效
        ///[4]bool：挨打的人是否受伤动作
        ///[5]string：挨打者身上特效
        ///[6]string：挨打者特效绑点，默认"Body"
        ///</summary>
        private static void DoDamageToEnterCha(GameObject aoe, List<GameObject> characters)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            object[] p = aoeState.model.onChaEnterParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(0);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length > 2 ? (bool)p[2] : true;
            bool toAlly = p.Length > 3 ? (bool)p[3] : false;
            bool hurtAction = p.Length > 4 ? (bool)p[4] : false;
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";

            Damage damage = baseDamage * (aoeState.propWhileCreate.attack * damageTimes);

            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                ChaState cs = characters[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side)))
                {
                    Vector3 chaToAoe = characters[i].transform.position - aoe.transform.position;
                    SceneVariants.CreateDamage(
                        aoeState.caster, characters[i],
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                    );
                    if (hurtAction == true) cs.Play("Hurt");
                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }

        ///<summary>
        ///onChaEnter
        ///对于范围内的人造成伤害（治疗得另写一个，这是严肃的），参数：
        ///[0]Damage：基础伤害
        ///[1]float：施法者攻击倍率
        ///[2]bool：对敌人有效
        ///[3]bool：对盟军有效
        ///[4]bool：挨打的人是否受伤动作
        ///[5]string：挨打者身上特效
        ///[6]string：挨打者特效绑点，默认"Body"
        ///</summary>
        private static void DoDamageAndForceMoveToEnterCha(GameObject aoe, List<GameObject> characters)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            object[] p = aoeState.model.onChaEnterParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(0);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length > 2 ? (bool)p[2] : true;
            bool toAlly = p.Length > 3 ? (bool)p[3] : false;
            bool hurtAction = p.Length > 4 ? (bool)p[4] : false;
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";

            Damage damage = baseDamage * (aoeState.propWhileCreate.attack * damageTimes);

            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                ChaState cs = characters[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side)))
                {
                    Vector3 chaToAoe = characters[i].transform.position - aoe.transform.position;

                    SceneVariants.CreateDamage(
                        aoeState.caster, characters[i],
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                    );
                    if (hurtAction == true) cs.Play("Hurt");
                    if (effect != "") cs.PlaySightEffect(bp, effect);

                    //添加击退效果
                    cs.AddForceMove(new MovePreorder(3f * chaToAoe.normalized, 0.5f));
                }
            }
        }

        ///<summary>
        ///onRemoved
        ///对于范围内的人造成伤害（治疗得另写一个，这是严肃的），参数：
        ///[0]Damage：基础伤害
        ///[1]float：施法者攻击倍率
        ///[2]bool：对敌人有效
        ///[3]bool：对盟军有效
        ///[4]bool：挨打的人是否受伤动作
        ///[5]string：挨打者身上特效
        ///[6]string：挨打者特效绑点，默认"Body"
        ///</summary>
        private static void DoDamageOnRemoved(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            object[] p = aoeState.model.onRemovedParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(20);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length > 2 ? (bool)p[2] : true;
            bool toAlly = p.Length > 3 ? (bool)p[3] : false;
            bool hurtAction = p.Length > 4 ? (bool)p[4] : false;
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";
            Damage damage = baseDamage + new Damage((int)(aoeState.propWhileCreate.attack * damageTimes));
            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < aoeState.characterInRange.Count; i++)
            {
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side)))
                {
                    Vector3 chaToAoe = aoeState.characterInRange[i].transform.position - aoe.transform.position;
                    SceneVariants.CreateDamage(
                        aoeState.caster, aoeState.characterInRange[i],
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                    );
                    if (hurtAction == true) cs.Play("Hurt");
                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }

        ///<summary>
        ///onRemoved
        ///对于范围内的人造成伤害（治疗得另写一个，这是严肃的），参数：
        ///[0]Damage：基础伤害
        ///[1]float：施法者攻击倍率
        ///[2]bool：对敌人有效
        ///[3]bool：对盟军有效
        ///[4]bool：挨打的人是否受伤动作
        ///[5]string：挨打者身上特效
        ///[6]string：挨打者特效绑点，默认"Body"
        ///</summary>
        private static void DoDamageOnCreate(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            object[] p = aoeState.model.onCreateParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(20);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length > 2 ? (bool)p[2] : true;
            bool toAlly = p.Length > 3 ? (bool)p[3] : false;
            bool hurtAction = p.Length > 4 ? (bool)p[4] : false;
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";
            Damage damage = baseDamage + new Damage((int)(aoeState.propWhileCreate.attack * damageTimes));
            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < aoeState.characterInRange.Count; i++)
            {
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side)))
                {
                    Vector3 chaToAoe = aoeState.characterInRange[i].transform.position - aoe.transform.position;
                    SceneVariants.CreateDamage(
                        aoeState.caster, aoeState.characterInRange[i],
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                    );
                    if (hurtAction == true) cs.Play("Hurt");
                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }

        ///<summary>
        ///onRemoved
        ///对于范围内的人造成伤害（治疗得另写一个，这是严肃的），参数：
        ///[0]Damage：基础伤害
        ///[1]float：施法者攻击倍率
        ///[2]bool：对敌人有效
        ///[3]bool：对盟军有效
        ///[4]bool：挨打的人是否受伤动作
        ///[5]string：挨打者身上特效
        ///[6]string：挨打者特效绑点，默认"Body"
        ///[7]addbuffinfo:添加buff的信息
        ///</summary>
        private static void DoDamageAndAddBuffOnRemoved(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            object[] p = aoeState.model.onRemovedParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(20);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length > 2 ? (bool)p[2] : true;
            bool toAlly = p.Length > 3 ? (bool)p[3] : false;
            bool hurtAction = p.Length > 4 ? (bool)p[4] : false;
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";
            AddBuffInfo addBuffInfo = p.Length > 7 ? (AddBuffInfo)p[7] : new AddBuffInfo();

            Damage damage = baseDamage * (aoeState.propWhileCreate.attack * damageTimes);
            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < aoeState.characterInRange.Count; i++)
            {
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side)))
                {
                    Vector3 chaToAoe = aoeState.characterInRange[i].transform.position - aoe.transform.position;
                    SceneVariants.CreateDamage(
                        aoeState.caster, aoeState.characterInRange[i],
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                    );

                    //添加buff
                    addBuffInfo.caster = aoeState.caster;
                    addBuffInfo.target = cs.gameObject;

                    cs.AddBuff(addBuffInfo);

                    if (hurtAction == true) cs.Play("Hurt");
                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }

        /// <summary>
        /// 每隔一段时间，对范围内的敌人造成伤害
        ///[0]Damage：基础伤害
        ///[1]float：施法者攻击倍率
        ///[2]bool：对敌人有效
        ///[3]bool：对盟军有效
        ///[4]bool：挨打的人是否受伤动作
        ///[5]string：挨打者身上特效
        ///[6]string：挨打者特效绑点，默认"Body"
        /// </summary>
        /// <param name="aoe"></param>
        private static void DoDamageOnTick(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;
            object[] p = aoeState.model.onTickParams;
            Damage baseDamage = p.Length > 0 ? (Damage)p[0] : new Damage(20);
            float damageTimes = p.Length > 1 ? (float)p[1] : 0;
            bool toFoe = p.Length <= 2 || (bool)p[2];
            bool toAlly = p.Length > 3 && (bool)p[3];
            bool hurtAction = p.Length > 4 && (bool)p[4];
            string effect = p.Length > 5 ? (string)p[5] : "";
            string bp = p.Length > 6 ? (string)p[6] : "Body";

            float addDamage = aoeState.propWhileCreate.attack * damageTimes;
            Damage damage = baseDamage;

            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < aoeState.characterInRange.Count; i++)
            {
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                //如果角色存在，且未死亡，且 对敌人有效且二者为敌人 或 对友军有效且二者为友军
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side)))
                {
                    Vector3 chaToAoe = aoeState.characterInRange[i].transform.position - aoe.transform.position;
                    SceneVariants.CreateDamage(
                        aoeState.caster, aoeState.characterInRange[i],
                        damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                        0.05f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                    );
                    if (hurtAction == true) cs.Play("Hurt");

                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }

        /// <summary>
        /// 每隔一段时间，将范围内的敌人向中心聚拢
        /// </summary>
        /// <param name="aoe"></param>
        private static void GatheringEnemiesOnTick(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;
            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }
            for (int i = 0; i < aoeState.characterInRange.Count; i++)
            {
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();

                //把敌人向中心聚拢
                if (side != cs.side)
                {
                    //指向aoe中心的单位向量
                    Vector3 direction = -(aoeState.characterInRange[i].transform.position - aoe.transform.position).normalized;

                    MovePreorder movePreorder = new MovePreorder(direction * 0.8f, 0.20f);
                    cs.AddForceMove(movePreorder);
                }
            }
        }

        /// <summary>
        /// 每隔一段时间，对范围内的敌人添加buff
        ///[0]addbuffinfo：添加buff信息
        ///[1]bool：对敌人有效
        ///[2]bool：对盟军有效
        ///[3]bool：被添加buff的人是否动画
        ///[4]string：挨打者身上特效
        ///[5]string：挨打者特效绑点，默认"Body"
        /// </summary>
        /// <param name="aoe"></param>
        private static void AddBuffOnTick(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;
            object[] p = aoeState.model.onTickParams;
            AddBuffInfo buffInfo = p.Length > 0 ? new AddBuffInfo(BuffData.data[(string)p[0]], null, null, 1, 3f) : new AddBuffInfo();
            bool toFoe = p.Length > 1 ? (bool)p[1] : true;
            bool toAlly = p.Length > 2 ? (bool)p[2] : false;
            bool Action = p.Length > 3 ? (bool)p[3] : false;
            string effect = p.Length > 4 ? (string)p[4] : "";
            string bp = p.Length > 5 ? (string)p[5] : "Body";

            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                buffInfo.caster = aoeState.caster;
                if (ccs) side = ccs.side;
            }
            for (int i = 0; i < aoeState.characterInRange.Count; i++)
            {
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                //如果角色存在，且未死亡，且 对敌人有效且二者为敌人 或 对友军有效且二者为友军
                if (cs && cs.dead == false && ((toFoe == true && side != cs.side) || (toAlly == true && side == cs.side)))
                {
                    cs.AddBuff(buffInfo);
                    if (Action == true) cs.Play("Hurt");

                    if (effect != "") cs.PlaySightEffect(bp, effect);
                }
            }
        }

        ///<summary>
        ///onChaEnter
        ///鲁大师的黑洞效果
        ///</summary>
        private static void BlackHole(GameObject aoe)
        {
            AoeState ast = aoe.GetComponent<AoeState>();
            if (!ast) return;
            for (int i = 0; i < ast.characterInRange.Count; i++)
            {
                ChaState cs = ast.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false)
                {
                    Vector3 disV = aoe.transform.position - ast.characterInRange[i].transform.position;
                    float distance = Mathf.Sqrt(Mathf.Pow(disV.x, 2) + Mathf.Pow(disV.z, 2));
                    float inTime = distance / (distance + 1.00f);   //1米是0.5秒，之后越来越大，但增幅是变小的
                    cs.AddForceMove(new MovePreorder(
                        disV * inTime, 1.00f
                    ));
                }
            }
        }

        ///<summary>
        ///OnCreate
        ///在aoe的位置上放一个视觉特效
        ///[0]string：特效的prefab，Prefab/下的路径，因为是特效。必定是一次性的特效，如果要循环播放完全可以绑定在aoe上，创建时开始播放，结束时停止。
        ///</summary>
        private static void CreateSightEffect(GameObject aoe)
        {
            AoeState ast = aoe.GetComponent<AoeState>();
            if (!ast) return;
            object[] p = ast.model.onCreateParams;
            string prefab = p.Length > 0 ? (string)p[0] : "";
            SceneVariants.CreateSightEffect(
                prefab, aoe.transform.position, aoe.transform.eulerAngles.y
            );
        }

        ///<summary>
        ///onRemoved
        ///aoe移除的时候创建另外一个aoe
        ///[0]string: aoe的model的id
        ///[1]float：aoe的半径（米）
        ///[2]float：aoe持续时间（秒）
        ///[3]string：aoe的Tween函数名
        ///[4]object[]：aoe的Tween函数的参数
        ///[5]Dictionary(string, object)：aoeObj的参数
        ///</summary>
        private static void CreateAoeOnRemoved(GameObject aoe)
        {
            AoeState ast = aoe.GetComponent<AoeState>();
            if (!ast) return;
            object[] p = ast.model.onRemovedParams;
            if (p.Length <= 0) return;
            string id = (string)p[0];
            if (id == "" || AoEData.data.ContainsKey(id) == false) return;
            AoeModel model = AoEData.data[id];
            float radius = p.Length > 1 ? (float)p[1] : 0.01f;
            float duration = p.Length > 2 ? (float)p[2] : 0;
            string aoeTweenId = p.Length > 3 ? (string)p[3] : "";
            AoeTween tween = null;
            if (aoeTweenId != "" && AoEScripts.aoeTweenFunc.ContainsKey(aoeTweenId))
            {
                tween = AoEScripts.aoeTweenFunc[aoeTweenId];
            }
            object[] tp = new object[0];
            if (p.Length > 4) tp = (object[])p[4];
            Dictionary<string, object> ap = null;
            if (p.Length > 5) ap = (Dictionary<string, object>)p[5];
            AoeLauncher al = new AoeLauncher(
                model, ast.caster, aoe.transform.position, radius,
                duration, aoe.transform.eulerAngles.y, tween, tp, ap
            );
            SceneVariants.CreateAoE(al);
        }

        ///<summary>
        ///onRemoved
        ///炸药桶爆炸了
        ///</summary>
        private static void BarrelExplosed(GameObject aoe)
        {
            AoeState aoeState = aoe.GetComponent<AoeState>();
            if (!aoeState) return;

            //new Damage(0, 50), 0.15f, true, false, true, "Effect/HitEffect_A", "Body"
            Damage baseDamage = new Damage(0, 50);
            float damageTimes = 0.15f;
            string effect = "Effect/HitEffect_A";
            string bp = "Body";

            Damage damage = baseDamage * (aoeState.propWhileCreate.attack * damageTimes);

            int side = -1;
            if (aoeState.caster)
            {
                ChaState ccs = aoeState.caster.GetComponent<ChaState>();
                if (ccs) side = ccs.side;
            }

            for (int i = 0; i < aoeState.characterInRange.Count; i++)
            {
                ChaState cs = aoeState.characterInRange[i].GetComponent<ChaState>();
                if (cs && cs.dead == false && side != cs.side)
                {
                    if (cs.HasTag("Barrel") == true)
                    {
                        SceneVariants.CreateDamage(
                            (GameObject)aoeState.param["Barrel"], aoeState.characterInRange[i],
                            new Damage(0, 9999), 0f, 0f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                        );
                    }
                    else
                    {
                        Vector3 chaToAoe = aoeState.characterInRange[i].transform.position - aoe.transform.position;
                        SceneVariants.CreateDamage(
                            aoeState.caster, aoeState.characterInRange[i],
                            damage, Mathf.Atan2(chaToAoe.x, chaToAoe.z) * 180 / Mathf.PI,
                            0.05f, new DamageInfoTag[] { DamageInfoTag.directDamage }
                        );
                        cs.Play("Hurt");
                        cs.PlaySightEffect(bp, effect);
                    }
                }
            }
        }
    }
}