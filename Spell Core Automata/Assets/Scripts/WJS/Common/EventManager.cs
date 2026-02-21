using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class EventManager<T> where T : Delegate
{
    protected T _event;
    private readonly T _baseEvent;
    private EventTriggerCondition _condition;
    private object[] _conditionParams;
    private CallBackParams _callBackParams;

    public EventManager(T initialHandler)
    {
        _baseEvent = initialHandler;
        _event = initialHandler;
        _conditionParams = new object[] { };
    }

    /// <summary>
    /// 重置到最初的事件处理程序
    /// </summary>
    public void ResetEvent()
    {
        _event = _baseEvent;
        _condition = null;
        _conditionParams = new object[] { };
    }

    // 添加事件处理程序
    public void AddListener(T handler, EventTriggerCondition condition = null, object[] conditionParams = null)
    {
        _event = (T)Delegate.Combine(_event, handler);
        if (condition != null) _condition = condition;
        if (conditionParams != null) _conditionParams = conditionParams;
    }

    // 移除事件处理程序
    public void RemoveListener(T handler)
    {
        _event = (T)Delegate.Remove(_event, handler);
    }

    /// <summary>
    /// 添加timelineNModel
    /// </summary>
    /// <param name="model"></param>
    public void AddTimelineModel(TimelineModel model, EventTriggerCondition condition = null, object[] conditionParams = null)
    {
        // 创建一个临时的委托handler，根据不同的处理逻辑来封装
        T handler = CreateHandlerForTimelineModel(model);
        if (handler != null)
        {
            AddListener(handler, condition, conditionParams);
        }
    }

    /// <summary>
    /// 调用事件
    /// </summary>
    /// <param name="args"></param>
    public void Invoke(params object[] args)
    {
        _event?.DynamicInvoke(args);
    }

    /// <summary>
    /// 若有返回值 采用此方法调用事件
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public TResult Invoke<TResult>(params object[] args)
    {
        if (_event == null)
        {
            return default(TResult);
        }

        var result = default(TResult);

        foreach (var del in _event.GetInvocationList())
        {
            // 确保委托类型匹配
            if (del is Func<object[], TResult> func)
            {
                try
                {
                    result = func(args);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error invoking delegate: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Delegate {del} is not of type Func<object[], TResult>");
            }
        }

        return result;
    }


    /// <summary>
    /// 调用 BuffOnBeHurt 委托的事件
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="damageInfo"></param>
    /// <param name="attacker"></param>
    public void Invoke(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker)
    {
        if (_event == null) return;

        foreach (var del in _event.GetInvocationList())
        {
            if (del is Action<BuffObj, DamageInfo, GameObject> action)
            {
                try
                {
                    // 使用反射调用方法，并传递 ref 参数
                    var parameters = new object[] { buff, damageInfo, attacker };
                    var method = del.Method;
                    method.Invoke(del.Target, parameters);

                    // 更新 ref 参数值
                    damageInfo = (DamageInfo)parameters[1];
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error invoking delegate: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Delegate {del} is not of type Action<BuffObj, ref DamageInfo, GameObject>");
            }
        }
    }

    /// <summary>
    /// 调用 BuffOnHit 委托的事件
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="damageInfo"></param>
    /// <param name="target"></param>
    public void Invoke(BuffObj buff, ref DamageInfo damageInfo, GameObject target, bool isBuffOnHit = false)
    {
        if (_event == null) return;

        foreach (var del in _event.GetInvocationList())
        {
            if (isBuffOnHit && del is Action<BuffObj, DamageInfo, GameObject> action)
            {
                try
                {
                    // 使用反射调用方法，并传递 ref 参数
                    var parameters = new object[] { buff, damageInfo, target };
                    var method = del.Method;
                    method.Invoke(del.Target, parameters);

                    // 更新 ref 参数值
                    damageInfo = (DamageInfo)parameters[1];
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error invoking delegate: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Delegate {del} is not of type Action<BuffObj, ref DamageInfo, GameObject>");
            }
        }
    }


    private T CreateHandlerForTimelineModel(TimelineModel timelineModel)
    {
        if (typeof(T) == typeof(BulletOnHit))
        {
            BulletOnHit bulletOnHitDelegate = new BulletOnHit((bullet, target) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BulletOnHit, bullet, target });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)bulletOnHitDelegate;
        }
        else if (typeof(T) == typeof(BulletOnCreate))
        {
            BulletOnCreate bulletOnCreateDelegate = new BulletOnCreate((bullet) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BulletOnCreate, bullet });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)bulletOnCreateDelegate;
        }
        else if (typeof(T) == typeof(BulletOnRemoved))
        {
            BulletOnRemoved bulletOnRemovedDelegate = new BulletOnRemoved((bullet) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BulletOnRemoved, bullet });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)bulletOnRemovedDelegate;
        }
        else if (typeof(T) == typeof(AoeOnCreate))
        {
            AoeOnCreate aoeOnCreateDelegate = new AoeOnCreate((aoe) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.AoeOnCreate, aoe });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)aoeOnCreateDelegate;
        }
        else if (typeof(T) == typeof(AoeOnRemoved))
        {
            AoeOnRemoved aoeOnRemovedDelegate = new AoeOnRemoved((aoe) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.AoeOnRemoved, aoe });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)aoeOnRemovedDelegate;
        }
        else if (typeof(T) == typeof(AoeOnTick))
        {
            AoeOnTick aoeOnTickDelegate = new AoeOnTick((aoe) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.AoeOnTick, aoe });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)aoeOnTickDelegate;
        }
        else if (typeof(T) == typeof(AoeOnCharacterEnter))
        {
            AoeOnCharacterEnter aoeOnCharacterEnterDelegate = new AoeOnCharacterEnter((aoe, cha) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.AoeOnCharacterEnter, aoe, cha });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)aoeOnCharacterEnterDelegate;
        }
        else if (typeof(T) == typeof(AoeOnCharacterLeave))
        {
            AoeOnCharacterLeave aoeOnCharacterLeaveDelegate = new AoeOnCharacterLeave((aoe, cha) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.AoeOnCharacterLeave, aoe, cha });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)aoeOnCharacterLeaveDelegate;
        }
        else if (typeof(T) == typeof(AoeOnBulletEnter))
        {
            AoeOnBulletEnter aoeOnBulletEnterDelegate = new AoeOnBulletEnter((aoe, bullet) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.AoeOnBulletEnter, aoe, bullet });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)aoeOnBulletEnterDelegate;
        }
        else if (typeof(T) == typeof(AoeOnBulletLeave))
        {
            AoeOnBulletLeave aoeOnBulletLeaveDelegate = new AoeOnBulletLeave((aoe, bullet) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.AoeOnBulletLeave, aoe, bullet });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)aoeOnBulletLeaveDelegate;
        }
        else if (typeof(T) == typeof(BuffOnOccur))
        {
            BuffOnOccur buffOnOccurDelegate = new BuffOnOccur((buff, modifyStack) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnOccur, buff, modifyStack });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)buffOnOccurDelegate;
        }
        else if (typeof(T) == typeof(BuffOnRemoved))
        {
            BuffOnRemoved buffOnRemovedDelegate = new BuffOnRemoved((buff) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnRemoved, buff });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)buffOnRemovedDelegate;
        }
        else if (typeof(T) == typeof(BuffOnTick))
        {
            BuffOnTick buffOnTickDelegate = new BuffOnTick((buff) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnTick, buff });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)buffOnTickDelegate;
        }
        else if (typeof(T) == typeof(BuffOnHit))
        {
            BuffOnHit buffOnHitDelegate = new BuffOnHit((BuffObj buff, ref DamageInfo damageInfo, GameObject target) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnHit, buff, damageInfo, target });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)buffOnHitDelegate;
        }
        else if (typeof(T) == typeof(BuffOnBeHurt))
        {
            BuffOnBeHurt buffOnBeHurtDelegate = new BuffOnBeHurt((BuffObj buff, ref DamageInfo damageInfo, GameObject attacker) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnBeHurt, buff, damageInfo, attacker });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)buffOnBeHurtDelegate;
        }
        else if (typeof(T) == typeof(BuffOnKill))
        {
            BuffOnKill buffOnKillDelegate = new BuffOnKill((BuffObj buff, DamageInfo damageInfo, GameObject target) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnKill, buff, damageInfo, target });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)buffOnKillDelegate;
        }
        else if (typeof(T) == typeof(BuffOnBeKilled))
        {
            BuffOnBeKilled buffOnBeKilledDelegate = new BuffOnBeKilled((BuffObj buff, DamageInfo damageInfo, GameObject attacker) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnBeKilled, buff, damageInfo, attacker });
                DelegateCastTimelineObj(newTimelineobj);
            });
            return (T)(Delegate)buffOnBeKilledDelegate;
        }
        else if (typeof(T) == typeof(BuffOnCast))
        {
            BuffOnCast buffOnCastDelegate = new BuffOnCast((buff, skill, timeline) =>
            {
                TimelineObj newTimelineobj = new TimelineObj(timelineModel, null, new object[] { EventManagerType.BuffOnCast, buff, skill, timeline });
                DelegateCastTimelineObj(newTimelineobj);
                return timeline;
            });
            return (T)(Delegate)buffOnCastDelegate;
        }
        else
        {
            throw new NotSupportedException($"Unsupported delegate type: {typeof(T).Name}");
        }
    }


    private void DelegateCastTimelineObj(TimelineObj timelineObj)
    {
        EventManagerType eventType = (EventManagerType)timelineObj.param[0];

        switch (eventType)
        {
            case EventManagerType.BulletOnHit:
                HandleBulletOnHit(timelineObj);
                break;

            case EventManagerType.BulletOnCreate:
                HandleBulletOnCreate(timelineObj);
                break;

            case EventManagerType.BulletOnRemoved:
                HandleBulletOnRemoved(timelineObj);
                break;

            case EventManagerType.AoeOnCreate:
                HandleAoeOnCreate(timelineObj);
                break;

            case EventManagerType.AoeOnRemoved:
                HandleAoeOnRemoved(timelineObj);
                break;

            case EventManagerType.AoeOnTick:
                HandleAoeOnTick(timelineObj);
                break;

            case EventManagerType.AoeOnCharacterEnter:
                HandleAoeOnCharacterEnter(timelineObj);
                break;

            case EventManagerType.AoeOnCharacterLeave:
                HandleAoeOnCharacterLeave(timelineObj);
                break;

            case EventManagerType.AoeOnBulletEnter:
                HandleAoeOnBulletEnter(timelineObj);
                break;

            case EventManagerType.AoeOnBulletLeave:
                HandleAoeOnBulletLeave(timelineObj);
                break;
            case EventManagerType.BuffOnOccur:
                HandleBuffOnOccur(timelineObj);
                break;

            case EventManagerType.BuffOnRemoved:
                HandleBuffOnRemoved(timelineObj);
                break;

            case EventManagerType.BuffOnTick:
                HandleBuffOnTick(timelineObj);
                break;

            case EventManagerType.BuffOnHit:
                HandleBuffOnHit(timelineObj);
                break;

            case EventManagerType.BuffOnBeHurt:
                HandleBuffOnBeHurt(timelineObj);
                break;

            case EventManagerType.BuffOnKill:
                HandleBuffOnKill(timelineObj);
                break;

            case EventManagerType.BuffOnBeKilled:
                HandleBuffOnBeKilled(timelineObj);
                break;

            case EventManagerType.BuffOnCast:
                HandleBuffOnCast(timelineObj);
                break;

            default:
                throw new NotSupportedException($"Unsupported event type: {eventType}");
        }
    }

    private void CreateTimelineByBullet(GameObject bullet, GameObject target, TimelineObj timelineObj)
    {
        ChaState chaState = CreateBulletLauncherCharacter(bullet);
        timelineObj.caster = chaState.gameObject;
        timelineObj.values.Add("Caster", bullet.GetComponent<BulletState>().caster);

        if (target != null)
        {
            chaState.SetRotateToTarget(chaState.GetComponent<UnitGetTarget>().GetEnemies(1, new List<GameObject>() { target })[0]);
            //找到参数中的子弹发射器 添加避免命中hit敌人的限制
            for (int i = 0; i < timelineObj.model.nodes.Length; i++)
            {
                DataEnhancedEffect.ModifyParameterOfType<BulletLauncher>(timelineObj.model.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    BulletLauncher bulletLauncher = value.Clone();
                    if (bulletLauncher != null)
                    {
                        if (bulletLauncher.param == null)
                            bulletLauncher.param = new Dictionary<string, object>();
                        bulletLauncher.param["NotHitTargetOnCreate"] = target;
                    }

                    return bulletLauncher;
                });
            }
        }

        SceneVariants.CreateTimeline(timelineObj);
    }

    private ChaState CreateBulletLauncherCharacter(GameObject bullet)
    {
        BulletState bulletState = bullet.GetComponent<BulletState>();
        ChaState chaState = bulletState.caster.GetComponent<ChaState>();

        // 创建临时施法者
        GameObject DelegateCharacater = SceneVariants.CreateCharacter("DelegateCharacater", chaState.side, bullet.transform.position, chaState.property, 0f, "FireMage");
        ChaState delegateCharacaterState = DelegateCharacater.GetComponent<ChaState>();
        //添加探敌组件
        DelegateCharacater.AddComponent<UnitGetTarget>();
        //关闭碰撞体
        DelegateCharacater.GetComponent<CapsuleCollider>().isTrigger = true;
        DelegateCharacater.GetComponent<Rigidbody>().isKinematic = true;
        delegateCharacaterState.SetImmuneTime(10f);
        //添加定时销毁buff
        AddBuffInfo addBuffInfo = new AddBuffInfo(BuffData.data["ScheduledDead"], DelegateCharacater, DelegateCharacater, 1, 3f);
        delegateCharacaterState.AddBuff(addBuffInfo);
        return delegateCharacaterState;
    }
    private void HandleBulletOnHit(TimelineObj timelineObj)
    {
        var bullet = (GameObject)timelineObj.param[1];
        var target = timelineObj.param.Length > 2 ? (GameObject)timelineObj.param[2] : null;

        _callBackParams = new CallBackParams();
        _callBackParams.bullet = bullet;
        _callBackParams.target = target;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBullet(bullet, target, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBullet(bullet, target, timelineObj);
        }
    }

    private void HandleBulletOnCreate(TimelineObj timelineObj)
    {
        var bullet = (GameObject)timelineObj.param[1];

        _callBackParams = new CallBackParams();
        _callBackParams.bullet = bullet;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBullet(bullet, null, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBullet(bullet, null, timelineObj);
        }
    }

    private void HandleBulletOnRemoved(TimelineObj timelineObj)
    {
        var bullet = (GameObject)timelineObj.param[1];

        _callBackParams = new CallBackParams();
        _callBackParams.bullet = bullet;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBullet(bullet, null, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBullet(bullet, null, timelineObj);
        }
    }

    private void CreateTimelineByAoe(GameObject aoe, TimelineObj timelineObj)
    {
        // 处理 AoeOnCreate 事件的逻辑
        AoeState aoeState = aoe.GetComponent<AoeState>();
        ChaState chaState = aoeState.caster.GetComponent<ChaState>();

        // 创建临时施法者
        GameObject DelegateCharacater = SceneVariants.CreateCharacter("DelegateCharacater", chaState.side, aoe.transform.position, chaState.property, 0f, "Default_Gunner", null, false);
        DelegateCharacater.name = "DelegateCharacater";
        ChaState delegateCharacaterState = DelegateCharacater.GetComponent<ChaState>();
        //关闭碰撞体
        DelegateCharacater.GetComponent<CapsuleCollider>().enabled = false;
        delegateCharacaterState.SetImmuneTime(10f);
        //添加定时销毁buff
        AddBuffInfo addBuffInfo = new AddBuffInfo(BuffData.data["ScheduledDead"], DelegateCharacater, DelegateCharacater, 1, 0.1f);
        delegateCharacaterState.AddBuff(addBuffInfo);
        timelineObj.caster = DelegateCharacater;
        timelineObj.values.Add("Caster", aoeState.caster);
        SceneVariants.CreateTimeline(timelineObj);
    }

    private void HandleAoeOnCreate(TimelineObj timelineObj)
    {
        var aoe = (GameObject)timelineObj.param[1];

        _callBackParams = new CallBackParams();
        _callBackParams.aoe = aoe;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByAoe(aoe, timelineObj);
            }
        }
        else
        {
            CreateTimelineByAoe(aoe, timelineObj);
        }
    }

    private void HandleAoeOnRemoved(TimelineObj timelineObj)
    {
        var aoe = (GameObject)timelineObj.param[1];
        // 处理 AoeOnRemoved 事件的逻辑
        _callBackParams = new CallBackParams();
        _callBackParams.aoe = aoe;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByAoe(aoe, timelineObj);
            }
        }
        else
        {
            CreateTimelineByAoe(aoe, timelineObj);
        }
    }

    private void HandleAoeOnTick(TimelineObj timelineObj)
    {
        var aoe = (GameObject)timelineObj.param[1];
        // 处理 AoeOnTick 事件的逻辑
        _callBackParams = new CallBackParams();
        _callBackParams.aoe = aoe;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByAoe(aoe, timelineObj);
            }
        }
        else
        {
            CreateTimelineByAoe(aoe, timelineObj);
        }
    }

    private void HandleAoeOnCharacterEnter(TimelineObj timelineObj)
    {
        var aoe = (GameObject)timelineObj.param[1];
        var cha = (List<GameObject>)timelineObj.param[2];
        // 处理 AoeOnCharacterEnter 事件的逻辑
        _callBackParams = new CallBackParams();
        _callBackParams.aoe = aoe;
        _callBackParams.cha = cha;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByAoe(aoe, timelineObj);
            }
        }
        else
        {
            CreateTimelineByAoe(aoe, timelineObj);
        }
    }

    private void HandleAoeOnCharacterLeave(TimelineObj timelineObj)
    {
        var aoe = (GameObject)timelineObj.param[1];
        var cha = (List<GameObject>)timelineObj.param[2];
        // 处理 AoeOnCharacterLeave 事件的逻辑
        _callBackParams = new CallBackParams();
        _callBackParams.aoe = aoe;
        _callBackParams.cha = cha;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByAoe(aoe, timelineObj);
            }
        }
        else
        {
            CreateTimelineByAoe(aoe, timelineObj);
        }
    }

    private void HandleAoeOnBulletEnter(TimelineObj timelineObj)
    {
        var aoe = (GameObject)timelineObj.param[1];
        var bullets = (List<GameObject>)timelineObj.param[2];
        // 处理 AoeOnBulletEnter 事件的逻辑
        _callBackParams = new CallBackParams();
        _callBackParams.aoe = aoe;
        _callBackParams.bullets = bullets;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByAoe(aoe, timelineObj);
            }
        }
        else
        {
            CreateTimelineByAoe(aoe, timelineObj);
        }
    }

    private void HandleAoeOnBulletLeave(TimelineObj timelineObj)
    {
        var aoe = (GameObject)timelineObj.param[1];
        var bullets = (List<GameObject>)timelineObj.param[2];
        // 处理 AoeOnBulletLeave 事件的逻辑
        _callBackParams = new CallBackParams();
        _callBackParams.aoe = aoe;
        _callBackParams.bullets = bullets;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByAoe(aoe, timelineObj);
            }
        }
        else
        {
            CreateTimelineByAoe(aoe, timelineObj);
        }
    }


    private void CreateTimelineByBuff(BuffObj buff, TimelineObj timelineObj)
    {
        // 处理 BuffOnOccur 事件的逻辑
        ChaState chaState = buff.carrier.GetComponent<ChaState>();

        // 创建临时施法者
        GameObject DelegateCharacater = SceneVariants.CreateCharacter("DelegateCharacater", chaState.side, buff.carrier.transform.position, chaState.property, 0f);
        ChaState delegateCharacaterState = DelegateCharacater.GetComponent<ChaState>();
        //关闭碰撞体
        DelegateCharacater.GetComponent<CapsuleCollider>().enabled = false;
        delegateCharacaterState.SetImmuneTime(10f);
        //添加定时销毁buff
        AddBuffInfo addBuffInfo = new AddBuffInfo(BuffData.data["ScheduledDead"], DelegateCharacater, DelegateCharacater, 1, 0.1f);
        delegateCharacaterState.AddBuff(addBuffInfo);

        timelineObj.values.Add("Caster", buff.carrier);
        SceneVariants.CreateTimeline(timelineObj);
    }

    private void HandleBuffOnOccur(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];
        var modifyStack = timelineObj.param.Length > 2 ? (int)timelineObj.param[2] : 1;

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;
        _callBackParams.modifyStack = modifyStack;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

    private void HandleBuffOnRemoved(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

    private void HandleBuffOnTick(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

    private void HandleBuffOnHit(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];
        var damageInfo = (DamageInfo)timelineObj.param[2];
        var target = (GameObject)timelineObj.param[3];

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;
        _callBackParams.target = target;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

    private void HandleBuffOnBeHurt(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];
        var damageInfo = (DamageInfo)timelineObj.param[2];
        var attacker = (GameObject)timelineObj.param[3];

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;
        _callBackParams.attacker = attacker;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

    private void HandleBuffOnKill(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];
        var damageInfo = (DamageInfo)timelineObj.param[2];
        var target = (GameObject)timelineObj.param[3];

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;
        _callBackParams.target = target;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

    private void HandleBuffOnBeKilled(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];
        var damageInfo = (DamageInfo)timelineObj.param[2];
        var attacker = (GameObject)timelineObj.param[3];

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;
        _callBackParams.damageInfo = damageInfo;
        _callBackParams.attacker = attacker;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

    private void HandleBuffOnCast(TimelineObj timelineObj)
    {
        var buff = (BuffObj)timelineObj.param[1];
        var skill = (SkillObj)timelineObj.param[2];
        var timeline = (TimelineObj)timelineObj.param[3];

        _callBackParams = new CallBackParams();
        _callBackParams.buff = buff;
        _callBackParams.skill = skill;
        _callBackParams.timeline = timeline;

        if (_condition != null)
        {
            if (_condition(_callBackParams, _conditionParams))
            {
                CreateTimelineByBuff(buff, timelineObj);
            }
        }
        else
        {
            CreateTimelineByBuff(buff, timelineObj);
        }
    }

}

public enum EventManagerType
{
    AoeOnCreate,
    AoeOnTick,
    AoeOnRemoved,
    AoeOnCharacterEnter,
    AoeOnCharacterLeave,
    AoeOnBulletEnter,
    AoeOnBulletLeave,
    BulletOnCreate,
    BulletOnHit,
    BulletOnRemoved,
    BuffOnOccur,
    BuffOnTick,
    BuffOnRemoved,
    BuffOnCast,
    BuffOnHit,
    BuffOnBeHurt,
    BuffOnKill,
    BuffOnBeKilled
}
}
