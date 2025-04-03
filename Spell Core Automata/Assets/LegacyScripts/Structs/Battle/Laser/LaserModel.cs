using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///激光的模板，也是策划填表的东西，当然游戏过程中所有的激光模板，未必都得由策划填表，也可以运行的脚本逻辑产生
///值得注意的是，这些信息只是构成“一道激光”，也就是描述了这道激光是怎样的，因此有很多数据并不属于这个结构
///比如激光的轨迹等，这些数据其实都是子弹的发射环境决定的，同道激光，可能被不同的人、地形、其他任何东西发射出来
///这些子弹的性质是一样的，例如激光造成的伤害信息、激光造成伤害的间隔，就是被填表的这些内容，但是他们可能轨迹之类都不同。
/// </summary>
public class LaserModel
{
    /// <summary>
    /// 这个激光的名字
    /// </summary>
    public string id;

    ///<summary>
    ///激光需要用的prefab，默认是Resources/Prefabs/Laser/下的，所以这个string需要省略前半部分
    ///比如是Laser0，就会创建自Resources/Prefabs/Laser/Laser0这个prefab
    ///</summary>
    public string prefab;

    /// <summary>
    /// 每次发射需要消耗的资源
    /// </summary>
    public ChaResource resource;

    /// <summary>
    /// 激光最大长度
    /// </summary>
    public float MaxLength;

    /// <summary>
    /// 激光可以穿透的次数
    /// </summary>
    public int penetrationCount;

    ///<summary>
    ///激光攻击同一个目标的延迟，单位：秒，最小值是Time.fixedDeltaTime（每帧发生一次）
    ///</summary>
    public float sameTargetDelay;

    ///<summary>
    ///激光是否会命中敌人
    ///</summary>
    public bool hitFoe;

    ///<summary>
    ///激光是否会命中盟军
    ///</summary>
    public bool hitAlly;

    ///<summary>
    ///激光被创建的事件
    ///</summary>
    public EventManager<LaserOnCreate> onCreate;

    /// <summary>
    /// OnCreate的参数
    /// </summary>
    public object[] onCreateParam;

    ///<summary>
    ///激光命中目标的时候触发的事件
    ///<summary>
    public EventManager<LaserOnHit> onHit;

    ///<summary>
    ///OnHit的参数
    ///</summary>
    public object[] onHitParams;

    ///<summary>
    ///激光在生命周期消耗殆尽之后发生的事件，生命周期消耗殆尽是laserState.duration归零或不满足释放条件。
    ///</summary>
    public EventManager<LaserOnRemoved> onRemoved;

    ///<summary>
    ///OnRemoved的参数
    ///</summary>
    public object[] onRemovedParams;

    public LaserModel(string id, string prefab,
                      string onCreate = "", object[] onCreateParam = null,
                      string onHit = "", object[] onHitParams = null,
                      string onRemoved = "", object[] onRemovedParams = null,
                      ChaResource resource = null, float MaxLength = 40f, int penetrationCount = 1,
                      float sameTargetDelay = 0.2f, bool hitFoe = true, bool hitAlly = false)
    {
        this.id = id;
        this.prefab = prefab;
        this.resource = resource;
        this.MaxLength = MaxLength;
        this.penetrationCount = penetrationCount;
        this.sameTargetDelay = sameTargetDelay;
        this.hitFoe = hitFoe;
        this.hitAlly = hitAlly;
        this.onHit = onHit == "" ? null : new EventManager<LaserOnHit>(DesignerScripts.DataLaserScripts.onHitFunc[onHit]);
        this.onRemoved = onRemoved == "" ? null : new EventManager<LaserOnRemoved>(DesignerScripts.DataLaserScripts.onRemovedFunc[onRemoved]);
        this.onCreate = onCreate == "" ? null : new EventManager<LaserOnCreate>(DesignerScripts.DataLaserScripts.onCreateFunc[onCreate]);
        this.onCreateParam = onCreateParam;
        this.onHitParams = onHitParams;
        this.onRemovedParams = onRemovedParams;
    }

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
}

///<summary>
///激光被创建的事件
///</summary>
public delegate void LaserOnCreate(GameObject laser);

///<summary>
///激光命中目标的时候触发的事件
///<param name="laser">发生碰撞的激光，应该是个携带laserState的游戏物体，但是在unity的逻辑下，他就是个GameObject，具体数据从GameObject拿了</param>
///<param name="target">被击中的角色</param>
///<summary>
public delegate void LaserOnHit(GameObject laser, GameObject target);

///<summary>
///激光在生命周期消耗殆尽之后发生的事件，生命周期消耗殆尽是laserState.duration归零或不满足释放条件。
///<param name="laser">发生碰撞的激光，应该是个携带laserState的游戏物体，但是在unity的逻辑下，他就是个GameObject，具体数据从GameObject拿了</param>
///</summary>
public delegate void LaserOnRemoved(GameObject laser);
