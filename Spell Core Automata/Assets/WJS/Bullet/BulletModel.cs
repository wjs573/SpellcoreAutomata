using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///子弹的模板，也是策划填表的东西，当然游戏过程中所有的子弹模板，未必都得由策划填表，也可以运行的脚本逻辑产生
    ///值得注意的是，这些信息只是构成“一个子弹”，也就是描述了这个子弹是怎样的，因此有很多数据并不属于这个结构
    ///比如子弹的飞行速度、轨迹等，这些数据其实都是子弹的发射环境决定的，同一个导弹，可能被不同的人、地形、其他任何东西发射出来
    ///这些子弹的性质是一样的，就是被填表的这些内容，但是他们可能轨迹之类都不同。
    ///</summary>
    ///</summary>
    public struct BulletModel
    {
        public string id;

        ///<summary>
        ///子弹需要用的prefab，默认是Resources/Prefabs/Bullet/下的，所以这个string需要省略前半部分
        ///比如是BlueRocket0，就会创建自Resources/Prefabs/Bullet/BlueRocket0这个prefab
        ///</summary>
        public string prefab;

        public string flash;

        ///<summary>
        ///子弹的碰撞半径，单位：米。这个游戏里子弹在逻辑世界都是圆形的，当然是这个游戏设定如此，实际策划的需求未必只能是圆形。
        ///</summary>
        public float radius;

        /// <summary>
        /// 美术特效子弹半径
        /// </summary>
        public float sightEffectRadius;

        ///<summary>
        ///子弹可以碰触的次数，每次碰到合理目标-1，到0的时候子弹就结束了。
        ///</summary>
        public int hitTimes;

        ///<summary>
        ///子弹碰触同一个目标的延迟，单位：秒，最小值是Time.fixedDeltaTime（每帧发生一次）
        ///</summary>
        public float sameTargetDelay;

        ///<summary>
        ///子弹被创建的事件
        ///<param name="bullet">要创建的子弹</param>
        ///</summary>
        public EventManager<BulletOnCreate> onCreate;

        public object[] onCreateParam;

        ///<summary>
        ///子弹命中目标时候发生的事情
        ///<param name="bullet">发生碰撞的子弹，应该是个bulletObj，但是在unity的逻辑下，他就是个GameObject，具体数据从GameObject拿了</param>
        ///<param name="target">被击中的角色</param>
        ///</summary>
        public EventManager<BulletOnHit> onHit;

        ///<summary>
        ///OnHit的参数
        ///</summary>
        public ParamDictionary onHitParams;

        ///<summary>
        ///子弹生命周期结束时候发生的事情
        ///<param name="bullet">发生碰撞的子弹，应该是个bulletObj，但是在unity的逻辑下，他就是个GameObject，具体数据从GameObject拿了</param>
        ///</summary>
        public EventManager<BulletOnRemoved> onRemoved;

        ///<summary>
        ///OnRemoved的参数
        ///</summary>
        public object[] onRemovedParams;

        ///<summary>
        ///子弹的移动方式，一般来说都是飞行的，也有部分是手雷这种属于走路的
        ///</summary>
        public MoveType moveType;

        ///<summary>
        ///子弹的是否碰到障碍物就爆炸，不会的话会沿着障碍物移动
        ///</summary>
        public bool removeOnObstacle;

        ///<summary>
        ///子弹是否会命中敌人
        ///</summary>
        public bool hitFoe;

        ///<summary>
        ///子弹是否会命中盟军
        ///</summary>
        public bool hitAlly;

        ///<summary>
        /// 发射的子弹数量
        ///</summary>
        public int shotCount;

        ///<summary>
        /// 子弹的散射角度，单位：度
        ///</summary>
        public float spreadAngle;

        public void ResetEvent()
        {
            if (onCreate != null)
            {
                onCreate.ResetEvent();
            }
            if (onHit != null)
            {
                onHit.ResetEvent();
            }
            if (onRemoved != null)
            {
                onRemoved.ResetEvent();
            }
        }

        public BulletModel(
            string id, string prefab, string flash,
            string onCreate = "",
            object[] createParams = null,
            string onHit = "",
            ParamDictionary onHitParams = null,
            string onRemoved = "",
            object[] onRemovedParams = null,
            MoveType moveType = MoveType.fly, bool removeOnObstacle = true,
            float radius = 0.1f, int hitTimes = 1, float sameTargetDelay = 0.5f,
            bool hitFoe = true, bool hitAlly = false,
            int shotCount = 1, float spreadAngle = 0f,
            float sightEffectRadius = 0f) // Added these two parameters
        {
            this.id = id;
            this.prefab = prefab;
            this.flash = flash;
            this.onHit = onHit == "" ? null : new EventManager<BulletOnHit>(BulletScripts.onHitFunc[onHit]);
            this.onRemoved = onRemoved == "" ? null : new EventManager<BulletOnRemoved>(BulletScripts.onRemovedFunc[onRemoved]);
            this.onCreate = onCreate == "" ? null : new EventManager<BulletOnCreate>(BulletScripts.onCreateFunc[onCreate]);
            this.onCreateParam = createParams ?? (new object[0]);
            this.onHitParams = onHitParams;
            this.onRemovedParams = onRemovedParams ?? (new object[0]);
            this.radius = radius;
            this.hitTimes = hitTimes;
            this.sameTargetDelay = sameTargetDelay;
            this.moveType = moveType;
            this.removeOnObstacle = removeOnObstacle;
            this.hitAlly = hitAlly;
            this.hitFoe = hitFoe;
            this.shotCount = shotCount; // Set the shot count
            this.spreadAngle = spreadAngle; // Set the spread angle
            if (sightEffectRadius == 0f)
            {
                this.sightEffectRadius = radius;
            }
            else
            {
                this.sightEffectRadius = sightEffectRadius;
            }

        }
    }

}