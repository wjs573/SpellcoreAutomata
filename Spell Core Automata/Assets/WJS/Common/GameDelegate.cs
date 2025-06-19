using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WJS
{
    public class GameDelegate
    {
        public delegate void PointerEventHandler(PointerEventData eventData);
        ///<summary>
        ///子弹被创建的事件
        ///</summary>
        public delegate void BulletOnCreate(GameObject bullet);

        ///<summary>
        ///子弹命中目标的时候触发的事件
        ///<param name="bullet">发生碰撞的子弹，应该是个bulletObj，但是在unity的逻辑下，他就是个GameObject，具体数据从GameObject拿了</param>
        ///<param name="target">被击中的角色</param>
        ///<summary>
        public delegate void BulletOnHit(GameObject bullet, GameObject target);

        ///<summary>
        ///子弹在生命周期消耗殆尽之后发生的事件，生命周期消耗殆尽是因为BulletState.duration<=0，或者是因为移动撞到了阻挡。
        ///<param name="bullet">发生碰撞的子弹，应该是个bulletObj，但是在unity的逻辑下，他就是个GameObject，具体数据从GameObject拿了</param>
        ///</summary>
        public delegate void BulletOnRemoved(GameObject bullet);

        ///<summary>
        ///子弹的轨迹函数，传入一个时间点，返回出一个Vector3，作为这个时间点的速度和方向，这是个相对于正在飞行的方向的一个偏移（*speed的）
        ///正在飞行的方向按照z轴，来算，也就是说，当你只需要子弹匀速行动的时候，你可以让这个函数只做一件事情——return Vector3.forward。
        ///<param name="t">子弹飞行了多久的时间点，单位秒。</param>
        ///<param name="bullet">是当前的子弹GameObject，不建议公式中用到这个</param>
        ///<param name="following">是正在跟踪的对象的GameObject，除非要做“跟踪弹”不然不建议使用</param>
        ///<return>返回这一时间点上的速度和偏移，Vector3就是正常速度正常前进</return>
        ///</summary>
        public delegate Vector3 BulletTween(float t, GameObject bullet, GameObject target);

        ///<summary>
        ///子弹在发射瞬间，可以捕捉一个GameObject作为目标，并且将这个目标传递给BulletTween，作为移动参数
        ///<param name="bullet">是当前的子弹GameObject，不建议公式中用到这个</param>
        ///<param name="targets">所有可以被选作目标的对象，这里是GameManager的逻辑决定的传递过来谁，比如这个游戏子弹只能捕捉角色作为对象，那就是只有角色的GameObject，当然如果需要，加入子弹也不麻烦</param>
        ///<return>在创建子弹的瞬间，根据这个函数获得一个GameObject作为followingTarget</return>
        ///</summary>
        public delegate GameObject BulletTargettingFunction(GameObject bullet, GameObject[] targets);
    }


    public delegate void BuffOnOccur(BuffObj buff, int modifyStack);
    public delegate void BuffOnRemoved(BuffObj buff);
    public delegate void BuffOnTick(BuffObj buff);
    public delegate void BuffOnHit(BuffObj buff, ref DamageInfo damageInfo, GameObject target);
    public delegate void BuffOnBeHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker);
    public delegate void BuffOnKill(BuffObj buff, DamageInfo damageInfo, GameObject target);
    public delegate void BuffOnBeKilled(BuffObj buff, DamageInfo damageInfo, GameObject attacker);
    public delegate TimelineObj BuffOnCast(BuffObj buff, SkillObj skill, TimelineObj timeline);

    public delegate void TimelineEvent(TimelineObj timeline, params object[] args);

    ///<summary>
    ///aoe创建时的事件
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///</summary>
    public delegate void AoeOnCreate(GameObject aoe);

    ///<summary>
    ///aoe移除时候的事件
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///</summary>
    public delegate void AoeOnRemoved(GameObject aoe);

    ///<summary>
    ///aoe每一跳的事件
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///</summary>
    public delegate void AoeOnTick(GameObject aoe);

    ///<summary>
    ///当有角色进入aoe范围的时候触发
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///<param name="cha">进入aoe范围的那些角色，他们现在还不在aoeState的角色列表里</param>
    ///</summary>
    public delegate void AoeOnCharacterEnter(GameObject aoe, List<GameObject> cha);

    ///<summary>
    ///当有角色离开aoe范围的时候
    ///<param name="aoe">离开aoe的gameObject</param>
    ///<param name="cha">离开aoe范围的那些角色，他们现在已经不在aoeState的角色列表里</param>
    ///</summary>
    public delegate void AoeOnCharacterLeave(GameObject aoe, List<GameObject> cha);

    ///<summary>
    ///当有子弹进入aoe范围的时候
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///<param name="bullet">离开aoe范围的那些子弹，他们现在已经不在aoeState的子弹列表里</param>
    ///</summary>
    public delegate void AoeOnBulletEnter(GameObject aoe, List<GameObject> bullet);

    ///<summary>
    ///当有子弹离开aoe范围的时候
    ///<param name="aoe">离开的aoe的gameObject</param>
    ///<param name="bullet">离开aoe范围的那些子弹，他们现在已经不在aoeState的子弹列表里</param>
    ///</summary>
    public delegate void AoeOnBulletLeave(GameObject aoe, List<GameObject> bullet);

    ///<summary>
    ///aoe的移动轨迹函数
    ///<param name="aoe">要执行的aoeObj</param>
    ///<param name="t">这个tween在aoe中运行了多久了，单位：秒</param>
    ///<return>aoe在这时候的移动信息</param>
    public delegate AoeMoveInfo AoeTween(GameObject aoe, float t);
}

