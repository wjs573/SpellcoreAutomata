using System.Collections;
using System.Collections.Generic;
using DesignerScripts;
using UnityEngine;

public class LaserLauncher
{
    ///<summary>
    ///要发射的激光
    ///</summary>
    public LaserModel model;

    ///<summary>
    ///要发射激光的这个人的gameObject，这里就认角色（拥有ChaState的）
    ///当然可以是null发射的，但是写效果逻辑的时候得小心caster是null的情况
    ///</summary>
    public GameObject caster;

    ///<summary>
    ///激光的生命周期，单位：秒
    ///激光应该是有个生命周期的，因为如果总是不命中，也不回收总不好
    ///</summary>
    public float duration;

    ///<summary>
    ///发射的坐标，y轴是无效的
    ///</summary>
    public Transform firePositionTransform;

    /// <summary>
    /// 瞄准模式
    /// </summary>
    public AimType aimType;


    ///<summary>
    ///激光的一些特殊逻辑使用的参数，可以在创建子的时候传递给激光
    ///</summary>
    public Dictionary<string, object> param;

    /// <summary>
    /// 构造函数，用于初始化 LaserLauncher 对象
    /// </summary>
    /// <param name="laserModel">要发射的激光模型</param>
    /// <param name="casterObject">发射激光的角色 GameObject</param>
    /// <param name="laserDuration">激光的生命周期（秒）</param>
    /// <param name="firePos">激光发射的坐标</param>
    /// <param name="aim">瞄准模式</param>
    /// <param name="hitDelay">激光创建后多久可以碰撞（秒）</param>
    /// <param name="parameters">激光的特殊参数</param>
    public LaserLauncher(LaserModel laserModel, GameObject casterObject, float laserDuration, Transform firePosTransform, AimType aim, Dictionary<string, object> parameters)
    {
        model = laserModel;
        caster = casterObject;
        duration = laserDuration;
        firePositionTransform = firePosTransform;
        aimType = aim;
        param = parameters;
    }
}
