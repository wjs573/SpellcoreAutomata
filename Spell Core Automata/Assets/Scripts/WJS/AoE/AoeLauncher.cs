using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///AoE发射器，创建aoe依赖的数据都在这里了
    ///</summary>
    public class AoeLauncher
    {
        ///<summary>
        ///要释放的aoe
        ///</summary>
        public AoeModel model;

        ///<summary>
        ///释放的中心坐标
        ///</summary>
        public Vector3 position;

        ///<summary>
        ///释放aoe的角色的GameObject，当然可能是null的
        ///</summary>
        public GameObject caster;

        /// <summary>
        /// 释放aoe的游戏物体
        /// 是Character或者Character的Weapon 
        /// </summary>
        public GameObject parent;

        ///<summary>
        ///aoe的半径，单位：米
        ///目前这游戏的设计中，aoe只有圆形，所以只有一个半径，也不存在角度一说，如果需要可以扩展
        ///</summary>
        public float radius;
        ///<summary>
        ///aoe存在的时间，单位：秒
        ///</summary>
        public float duration;

        ///<summary>
        ///aoe的角度
        ///</summary>
        public float degree;

        ///<summary>
        ///aoe移动轨迹函数
        ///</summary>
        public AoeTween tween;
        public object[] tweenParam = new object[0];

        public bool isDieWithParent;

        ///<summary>
        ///aoe的传入参数，比如可以吸收次数之类的
        ///</summary>
        public Dictionary<string, object> param = new Dictionary<string, object>();

        public AoeLauncher(
            AoeModel model, GameObject caster, Vector3 position, float radius, float duration, float degree,
            AoeTween tween = null, object[] tweenParam = null, Dictionary<string, object> aoeParam = null,
            bool isDieWithParent = false, GameObject parent = null)
        {
            this.model = model;
            this.caster = caster;
            this.position = position;
            this.radius = radius;
            this.duration = duration;
            this.degree = degree;
            this.tween = tween;
            if (aoeParam != null) this.param = aoeParam;
            if (tweenParam != null) this.tweenParam = tweenParam;
            this.isDieWithParent = isDieWithParent;
            if (parent == null)
            {
                this.parent = caster;
            }
            else
            {
                this.parent = parent;
            }
        }

        public AoeLauncher Clone()
        {
            return new AoeLauncher(
                this.model,
                this.caster,
                this.position,
                this.radius,
                this.duration,
                this.degree,
                this.tween,
                this.tweenParam,
                this.param
            );
        }
    }
}
