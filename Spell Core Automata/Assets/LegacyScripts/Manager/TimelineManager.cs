using System;
using System.Collections.Generic;
using JinShan;
using Sirenix.OdinInspector;
using UnityEngine;

///<summary>
///管理游戏中所有的timeline
///</summary>
public class TimelineManager : MonoSingleton<TimelineManager>
{
    [ShowInInspector]
    private List<TimelineObj> timelines = new List<TimelineObj>();

    /// <summary>
    /// 缓冲区
    /// 如果这个timeline是一定要执行的 但当前无法添加
    /// 就把它放进缓冲区
    /// 缓冲区的timeline会在每次fixedupdate时尝试添加
    /// </summary>
    private List<TimelineObj> bufferTimelines = new List<TimelineObj>();

    private void FixedUpdate()
    {
        if (bufferTimelines.Count != 0)
        {
            for (int i = 0; i < bufferTimelines.Count; i++)
            {
                //如果缓冲区的timeline可以添加
                //就从缓冲区移除 添加至timelines
                if (bufferTimelines[i].timelineType == TimelineType.Character)
                {
                    if (!CasterHasTimeline(bufferTimelines[i].caster))
                    {
                        timelines.Add(bufferTimelines[i]);
                        bufferTimelines.RemoveAt(i);
                    }
                }
                else
                {
                    if (!WeaponHasTimeline(bufferTimelines[i].weapon))
                    {
                        timelines.Add(bufferTimelines[i]);
                        bufferTimelines.RemoveAt(i);
                    }
                }
            }
        }

        if (this.timelines.Count <= 0) return;

        int idx = 0;
        while (idx < this.timelines.Count)
        {
            float wasTimeElapsed = timelines[idx].timeElapsed;
            timelines[idx].timeElapsed += Time.fixedDeltaTime * timelines[idx].timeScale;
            timelines[idx].realTimeElapsed += Time.fixedDeltaTime * timelines[idx].timeScale;
            //循环节点的处理
            if (timelines[idx].model.IsContainsLoop)
            {
                List<TimelineNode> loopNodes = new List<TimelineNode>();

                // 遍历所有节点，查找循环节点
                for (int i = 0; i < timelines[idx].model.nodes.Length; i++)
                {
                    // 判断是否为循环节点
                    if (timelines[idx].model.nodes[i].loopTimes > 1)
                    {
                        int loopTimes = timelines[idx].model.nodes[i].loopTimes;
                        float loopInterval = timelines[idx].model.nodes[i].loopIntervalTime;

                        // 计算添加循环节点的次数
                        float remainingTime = timelines[idx].model.duration - timelines[idx].model.nodes[i].timeElapsed;
                        loopTimes = Mathf.Clamp((int)(remainingTime / loopInterval) + 1, 1, loopTimes);

                        // 添加循环节点到列表中
                        for (int j = 0; j < loopTimes - 1; j++)
                        {
                            TimelineNode newNode = new TimelineNode(
                                timelines[idx].model.nodes[i].timeElapsed + (j + 1) * loopInterval,
                                timelines[idx].model.nodes[i].TimelineEventName,
                                1,
                                0f,
                                timelines[idx].model.nodes[i].eveParams);
                            loopNodes.Add(newNode);
                        }
                    }
                }

                // 如果存在循环节点，则将其一次性添加到节点数组末尾
                if (loopNodes.Count > 0)
                {
                    int originalLength = timelines[idx].model.nodes.Length;
                    Array.Resize(ref timelines[idx].model.nodes, originalLength + loopNodes.Count);
                    loopNodes.CopyTo(timelines[idx].model.nodes, originalLength);

                    //并且把这个timeline的IsContainsLoop设置为false 只处理一次循环逻辑 避免死循环
                    timelines[idx].model.IsContainsLoop = false;
                }
            }

            //判断有没有返回点
            if (
                timelines[idx].model.chargeGoBack.atDuration < timelines[idx].timeElapsed &&
                timelines[idx].model.chargeGoBack.atDuration >= wasTimeElapsed
            )
            {

                if (timelines[idx].caster)
                {

                    ChaState cs = timelines[idx].caster.GetComponent<ChaState>();
                    if (timelines[idx].timelineType == TimelineType.Weapon)
                    {
                        cs = timelines[idx].caster.GetComponent<WeaponState>().ownerState;
                    }
                    if (cs.charging == true)
                    {
                        timelines[idx].timeElapsed = timelines[idx].model.chargeGoBack.gotoDuration;
                        continue;
                    }
                }
            }
            //执行时间点内的事情
            for (int i = 0; i < timelines[idx].model.nodes.Length; i++)
            {
                if (
                    timelines[idx].model.nodes[i].timeElapsed < timelines[idx].timeElapsed &&
                    timelines[idx].model.nodes[i].timeElapsed >= wasTimeElapsed
                )
                {
                    timelines[idx].model.nodes[i].doEvent(
                        timelines[idx],
                        timelines[idx].model.nodes[i].eveParams
                    );
                }
            }

            //判断timeline是否终结
            if (timelines[idx].model.duration <= timelines[idx].timeElapsed)
            {
                timelines.RemoveAt(idx);
            }
            else
            {
                idx++;
            }
        }
    }

    public bool IsTimelineObjOver(GameObject caster, string timelineModelId)
    {
        for (int i = 0; i < timelines.Count; i++)
        {
            if (timelines[i].caster == caster && timelines[i].model.id == timelineModelId)
            {
                return false;
            }
        }
        return true;
    }

    ///<summary>
    ///添加一个timeline
    ///<param name="timelineModel">要添加的timeline的model</param>
    ///<param name="caster">timeline的负责人</param>
    ///<param name="source">添加的源数据，比如技能就是skillObj</param>
    ///</summary>
    public void AddTimeline(TimelineModel timelineModel, GameObject caster, object[] source)
    {
        if (CasterHasTimeline(caster) == true) return;
        this.timelines.Add(new TimelineObj(timelineModel, caster, source));
    }

    ///<summary>
    ///添加一个timeline
    ///<param name="timelineModel">要添加的timeline</param>
    ///</summary>
    public void AddTimeline(TimelineObj timeline)
    {
        bool shouldAddTimeline = false;

        if (timeline.timelineType == TimelineType.Character)
        {
            if (timeline.caster != null && !CasterHasTimeline(timeline.caster))
            {
                shouldAddTimeline = true;
            }
        }
        else if (timeline.timelineType == TimelineType.Weapon)
        {
            if (timeline.weapon != null && !WeaponHasTimeline(timeline.weapon))
            {
                shouldAddTimeline = true;
            }
        }
        else if (timeline.timelineType == TimelineType.ComboSkill)
        {
            shouldAddTimeline = true;
        }

        if (shouldAddTimeline)
        {
            this.timelines.Add(timeline);
        }

    }

    public bool CasterHasTimeline(GameObject caster)
    {
        for (int i = 0; i < timelines.Count; i++)
        {
            if (timelines[i].timelineType == TimelineType.Character && timelines[i].caster == caster) return true;
        }
        return false;
    }

    public bool WeaponHasTimeline(GameObject weapon)
    {
        for (var i = 0; i < timelines.Count; i++)
        {
            if (timelines[i].timelineType == TimelineType.Weapon && timelines[i].weapon == weapon) return true;
        }
        return false;
    }

    /// <summary>
    /// 强行添加timeline
    /// </summary>
    /// <param name="timeline"></param>
    public void ForceAddTimeline(TimelineObj timeline)
    {
        if (timeline.caster == null)
        {
            return; // 如果caster为null，不添加时间线
        }

        if (timeline.timelineType == TimelineType.Character)
        {
            if (CasterHasTimeline(timeline.caster))
            {
                // 如果Character已经有时间线，则将其添加到 bufferTimelines 中
                bufferTimelines.Add(timeline);
            }
            else
            {
                this.timelines.Add(timeline);
            }
        }
        else if (timeline.timelineType == TimelineType.Weapon)
        {
            if (WeaponHasTimeline(timeline.weapon))
            {
                // 如果武器已经有时间线，则将其添加到 bufferTimelines 中
                bufferTimelines.Add(timeline);
            }
            else
            {
                this.timelines.Add(timeline);
            }
        }
    }
}

public enum TimelineType
{
    Character,
    Weapon,
    ComboSkill
}