/// <summary>
/// 游戏事件类
/// </summary>
public abstract class GameEvent
{
    /// <summary>
    /// 事件是否会重复发生
    /// </summary>
    public bool IsRepeatable;
    /// <summary>
    /// 判断事件是否发生的条件
    /// 满足所有条件 才允许发生
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckCondition();
    /// <summary>
    /// 事件本身
    /// </summary>
    public abstract void ExecuteEvent();
}

/// <summary>
/// 随机事件
/// 满足条件 随时都可能发生
/// </summary>
public class RandomEvent : GameEvent
{
    // 添加随机事件所需的其他属性和方法
    public override bool CheckCondition()
    {
        // 检查随机事件的条件
        return true;
    }

    public override void ExecuteEvent()
    {
        // 执行随机事件
    }
}


/// <summary>
/// 固定事件
/// 满足条件一定会发生
/// </summary>
public class FixedEvent : GameEvent
{
    // 添加固定事件所需的其他属性和方法
    public override bool CheckCondition()
    {
        // 检查固定事件的条件
        return true;
    }

    public override void ExecuteEvent()
    {
        // 执行固定事件
    }
}

/// <summary>
/// 在固定时间一定会发生的事件
/// </summary>
public class FixedTimeEvent : GameEvent
{
    public int FixedYear;
    public int FixedMonth;
    public int FixedDay;

    // 添加固定时间事件所需的其他属性和方法
    public override bool CheckCondition()
    {
        // 检查固定时间事件的条件
        return true;
    }

    public override void ExecuteEvent()
    {
        // 执行固定时间事件
    }
}

/// <summary>
/// 周期性事件，例如节假日活动
/// </summary>
public class PeriodicEvent : GameEvent
{
    public int PeriodInDays;

    // 添加周期性事件所需的其他属性和方法
    public override bool CheckCondition()
    {
        // 检查周期性事件的条件
        return true;
    }

    public override void ExecuteEvent()
    {
        // 执行周期性事件
    }
}
