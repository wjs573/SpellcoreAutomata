using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///策划预先填表制作的，就是这个东西，同样她也是被clone到obj当中去的
    ///</summary>
    public struct TimelineModel
    {
        public string id;

        ///<summary>
        ///Timeline运行多久之后发生，单位：秒
        ///</summary>
        public TimelineNode[] nodes;

        ///<summary>
        ///Timeline一共多长时间（到时间了就丢掉了），单位秒
        ///</summary>
        public float duration;

        ///<summary>
        ///如果有caster，并且caster处于蓄力状态，则可能会经历跳转点
        ///</summary>
        public TimelineGoTo chargeGoBack;

        /// <summary>
        /// 是否存在循环的TimelineNode
        /// 默认不存在
        /// </summary>
        public bool IsContainsLoop;

        public TimelineModel(string id, TimelineNode[] nodes, float duration, TimelineGoTo chargeGoBack, bool IsContainsLoop = false)
        {
            this.id = id;
            this.nodes = nodes;
            this.duration = duration;
            this.chargeGoBack = chargeGoBack;
            this.IsContainsLoop = IsContainsLoop;
        }

        public void ResetEventManager()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                CommonScripts.ModifyParameterOfType<BulletLauncher>(nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.model.ResetEvent();
                    return value;
                });
                CommonScripts.ModifyParameterOfType<AoeLauncher>(nodes[i].eveParams, (AoeLauncher value) =>
                {
                    value.model.ResetEvent();
                    return value;
                });
                CommonScripts.ModifyParameterOfType<LaserLauncher>(nodes[i].eveParams, (LaserLauncher value) =>
                {
                    value.model.ResetEvent();
                    return value;
                });
            }
        }

        public TimelineModel Clone()
        {
            // 创建一个新的 TimelineModel 对象，复制所有属性和字段
            // 并复制节点数组中的每个节点
            TimelineNode[] clonedNodes = new TimelineNode[this.nodes.Length];
            for (int i = 0; i < this.nodes.Length; i++)
            {
                List<object> clonedEveParams = new List<object>();

                if (this.nodes[i].eveParams != null)
                {
                    foreach (var param in this.nodes[i].eveParams)
                    {
                        // 如果param是BulletLauncher类型，进行克隆
                        if (param is BulletLauncher bulletLauncher)
                        {
                            BulletLauncher clonedBulletLauncher = bulletLauncher.Clone(); // 假设你有一个Clone方法来克隆BulletLauncher
                            clonedEveParams.Add(clonedBulletLauncher);
                        }
                        else if (param is AoeLauncher aoeLauncher)
                        {
                            AoeLauncher clonedAoeLauncher = aoeLauncher.Clone();
                            clonedEveParams.Add(clonedAoeLauncher);
                        }
                        else
                        {
                            // 如果不是BulletLauncher类型，直接添加到克隆的参数列表中
                            clonedEveParams.Add(param);
                        }
                    }
                }

                clonedNodes[i] = new TimelineNode(
                    this.nodes[i].timeElapsed,
                    this.nodes[i].TimelineEventName,
                    this.nodes[i].loopTimes,
                    this.nodes[i].loopIntervalTime,
                    clonedEveParams.ToArray()
                );
            }


            TimelineModel clonedModel = new TimelineModel(
                this.id,
                clonedNodes,
                this.duration,
                this.chargeGoBack,
                this.IsContainsLoop
            );

            return clonedModel;
        }

        public void ExtendTimelineIfNeeded()
        {
            List<TimelineNode> modifiedNodes = new List<TimelineNode>();
            float nodeTimeToAdd = 0f;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].loopTimes > 1 && !modifiedNodes.Contains(nodes[i]))
                {
                    nodeTimeToAdd += nodes[i].loopIntervalTime * (nodes[i].loopTimes - 1);
                    modifiedNodes.Add(nodes[i]);
                    ModifyLaterNodes(i + 1, nodeTimeToAdd);
                }
            }
        }
        private void ModifyLaterNodes(int index, float nodeTimeToAdd)
        {
            for (int i = index; i < nodes.Length; i++)
            {
                nodes[i].timeElapsed += nodeTimeToAdd;
            }
            this.duration += nodeTimeToAdd;
        }
    }

}
