using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///注意：和unity的timeline不是一个东西，这个概念出来的时候unity都还没出来。
    ///这是一段预约的事情的记录，也就是当timelineObj产生之后，就会开始计时，并且在每个“关键帧”（类似flash的概念）做事情。
    ///所有的道具使用效果、技能效果都可以抽象为一个timeline，由timeline来“指导”后续的事件发生。
    ///</summary>
    public class TimelineObj
    {
        ///<summary>
        ///Timeline的基础信息
        ///</summary>
        public TimelineModel model;

        ///<summary>
        ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
        ///</summary>
        public GameObject caster;

        ///<summary>
        ///倍速，1=100%，0.1=10%是最小值
        ///</summary>
        public float timeScale
        {
            get
            {
                return _timeScale;
            }
            set
            {
                _timeScale = Mathf.Max(0.100f, value);
            }
        }

        private float _timeScale = 1.00f;

        ///<summary>
        ///Timeline的创建参数，如果是一个技能，这就是一个skillObj
        ///</summary>
        public object[] param;

        ///<summary>
        ///Timeline已经运行了多少秒了（不包括跳转重置的时间）
        ///</summary>
        public float timeElapsed = 0;

        /// <summary>
        /// Timeline实际上已经运行了多少秒了（包括跳转重置的时间）
        /// </summary>
        public float realTimeElapsed = 0;

        ///<summary>
        ///一些重要的逻辑参数，是根据游戏机制在程序层提供的，这里目前需要的是
        ///[faceDegree] 发生时如果有caster，则caster企图面向的角度（主动）。
        ///[moveDegree] 发生时如果有caster，则caster企图移动向的角度（主动）。
        ///</summary>
        public Dictionary<string, object> values;

        public TimelineObj(TimelineModel model, GameObject caster, object[] param)
        {
            this.model = model;
            this.caster = caster;
            this.values = new Dictionary<string, object>();
            this._timeScale = 1.00f;

            if (caster)
            {
                ChaState cs = caster.GetComponent<ChaState>();
                if (cs)
                {
                    this.values.Add("faceDegree", cs.faceDegree);
                    this.values.Add("moveDegree", cs.moveDegree);
                }
                this._timeScale = cs.actionSpeed;
            }
            this.param = param;
        }

        ///<summary>
        ///尝试从values获得某个值
        ///<param name="key">这个值的key{faceDegree, moveDegree}</param>
        ///<return>取出对应的值，如果不存在就是null</return>
        ///</summary>
        public object GetValue(string key)
        {
            if (values.ContainsKey(key) == false) return null;
            return values[key];
        }
    }
}