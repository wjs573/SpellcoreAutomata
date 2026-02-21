using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///AoE的模板数据
    ///</summary>
    public struct AoeModel
    {
        public string id;

        ///<summary>
        ///aoe的视觉特效，如果是空字符串，就不会添加视觉特效
        ///这里需要的是在Prefabs/下的路径，因为任何东西都可以是视觉特效
        ///</summary>
        public string prefab;


        ///<summary>
        ///aoe是否碰撞到阻挡就摧毁了（removed），如果不是，移动就是smooth的，如果移动的话……
        ///</summary>
        public bool removeOnObstacle;

        ///<summmary>
        ///aoe的tag
        ///</summary>
        public string[] tags;

        ///<summary>
        ///aoe每一跳的时间，单位：秒
        ///如果这个时间小于等于0，或者没有onTick，则不会执行aoe的onTick事件
        ///</summary>
        public float tickTime;

        ///<summary>
        ///aoe创建时的事件
        ///</summary>
        public EventManager<AoeOnCreate> onCreate;

        ///<summary>
        ///aoe创建的参数
        ///</summary>
        public object[] onCreateParams;

        ///<summary>
        ///aoe每一跳的事件，如果没有，就不会发生每一跳
        ///</summary>
        public EventManager<AoeOnTick> onTick;
        public object[] onTickParams;

        ///<summary>
        ///aoe结束时的事件
        ///</summary>
        public EventManager<AoeOnRemoved> onRemoved;
        public object[] onRemovedParams;

        ///<summary>
        ///有角色进入aoe时的事件，onCreate时候位于aoe范围内的人不会触发这个，但是在onCreate里面会已经存在
        ///</summary>
        public EventManager<AoeOnCharacterEnter> onChaEnter;
        public object[] onChaEnterParams;

        ///<summary>
        ///有角色离开aoe结束时的事件
        ///</summary>
        public EventManager<AoeOnCharacterLeave> onChaLeave;
        public object[] onChaLeaveParams;

        ///<summary>
        ///有子弹进入aoe时的事件，onCreate时候位于aoe范围内的子弹不会触发这个，但是在onCreate里面会已经存在
        ///</summary>
        public EventManager<AoeOnBulletEnter> onBulletEnter;
        public object[] onBulletEnterParams;

        ///<summary>
        ///有子弹离开aoe时的事件
        ///</summary>
        public EventManager<AoeOnBulletLeave> onBulletLeave;
        public object[] onBulletLeaveParams;

        /// <summary>
        /// 基础版本圆形AOE的构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prefab"></param>
        /// <param name="tags"></param>
        /// <param name="tickTime"></param>
        /// <param name="removeOnObstacle"></param>
        /// <param name="onCreate"></param>
        /// <param name="onCreateParam"></param>
        /// <param name="onRemoved"></param>
        /// <param name="onRemovedParam"></param>
        /// <param name="onTick"></param>
        /// <param name="onTickParam"></param>
        /// <param name="onChaEnter"></param>
        /// <param name="onChaEnterParam"></param>
        /// <param name="onChaLeave"></param>
        /// <param name="onChaLeaveParam"></param>
        /// <param name="onBulletEnter"></param>
        /// <param name="onBulletEnterParam"></param>
        /// <param name="onBulletLeave"></param>
        /// <param name="onBulletLeaveParam"></param>
        public AoeModel(
            string id, string prefab, string[] tags, float tickTime, bool removeOnObstacle,
            string onCreate, object[] onCreateParam,
            string onRemoved, object[] onRemovedParam,
            string onTick, object[] onTickParam,
            string onChaEnter, object[] onChaEnterParam,
            string onChaLeave, object[] onChaLeaveParam,
            string onBulletEnter, object[] onBulletEnterParam,
            string onBulletLeave, object[] onBulletLeaveParam
        )
        {
            this.id = id;
            this.prefab = prefab;
            this.tags = tags;
            this.tickTime = tickTime;
            this.removeOnObstacle = removeOnObstacle;
            this.onCreate = onCreate == "" ? null : new EventManager<AoeOnCreate>(AoEScripts.onCreateFunc[onCreate]);
            this.onCreateParams = onCreateParam;
            this.onRemoved = onRemoved == "" ? null : new EventManager<AoeOnRemoved>(AoEScripts.onRemovedFunc[onRemoved]);
            this.onRemovedParams = onRemovedParam;
            this.onTick = onTick == "" ? null : new EventManager<AoeOnTick>(AoEScripts.onTickFunc[onTick]);
            this.onTickParams = onTickParam;
            this.onChaEnter = onChaEnter == "" ? null : new EventManager<AoeOnCharacterEnter>(AoEScripts.onChaEnterFunc[onChaEnter]);
            this.onChaEnterParams = onChaEnterParam;
            this.onChaLeave = onChaLeave == "" ? null : new EventManager<AoeOnCharacterLeave>(AoEScripts.onChaLeaveFunc[onChaLeave]);
            this.onChaLeaveParams = onChaLeaveParam;
            this.onBulletEnter = onBulletEnter == "" ? null : new EventManager<AoeOnBulletEnter>(AoEScripts.onBulletEnterFunc[onBulletEnter]);
            this.onBulletEnterParams = onBulletEnterParam;
            this.onBulletLeave = onBulletLeave == "" ? null : new EventManager<AoeOnBulletLeave>(AoEScripts.onBulletLeaveFunc[onBulletLeave]);
            this.onBulletLeaveParams = onBulletLeaveParam;
        }

        public void ResetEvent()
        {
            if (onCreate != null)
            {
                onCreate.ResetEvent();
            }
            if (onTick != null)
            {
                onTick.ResetEvent();
            }
            if (onRemoved != null)
            {
                onRemoved.ResetEvent();
            }
            if (onChaEnter != null)
            {
                onChaEnter.ResetEvent();
            }
            if (onChaLeave != null)
            {
                onChaLeave.ResetEvent();
            }
            if (onBulletEnter != null)
            {
                onBulletEnter.ResetEvent();
            }
            if (onBulletLeave != null)
            {
                onBulletLeave.ResetEvent();
            }
        }
    }
}