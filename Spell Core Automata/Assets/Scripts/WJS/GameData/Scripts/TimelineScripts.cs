using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class TimelineScripts : MonoBehaviour
    {
        // Start is called before the first frame update
        public static Dictionary<string, TimelineEvent> functions = new Dictionary<string, TimelineEvent>();

        public static void Initialize()
        {
            functions = new Dictionary<string, TimelineEvent>()
            {
                {"CasterPlayAnim", CasterPlayAnim},
                {"CasterForceMove", CasterForceMove},
                {"SetCasterControlState", SetCasterControlState},
                {"PlaySightEffectOnCaster", PlaySightEffectOnCaster},
                {"StopSightEffectOnCaster", StopSightEffectOnCaster},
                {"FireBullet", FireBullet},
                {"CasterImmune", CasterImmune},
                {"CreateAoE", CreateAoE},
                {"AddBuffToCaster", AddBuffToCaster},
            };
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
            if (timeline.caster)
            {
                GameObject actor = timeline.caster;
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
                    if (getTail) animName += CommonScripts.GetTailStringByDegree(faceDeg, moveDeg);
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
        }///<summary>
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
                string effectKey = GetValueFromParams<string>(2, args, UnityEngine.Random.value.ToString());
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
    }

}
