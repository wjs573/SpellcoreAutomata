using System.Collections.Generic;

///<summary>
///技能是角色拥有的东西，因为角色有技能，玩家或者ai才能操作角色释放技能
///</summary>
public class SkillObj
{
    ///<summary>
    ///技能的模板，创建于skillModel，但运行中还是会允许改变
    ///</summary>
    public SkillModel model;

    ///<summary>
    ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
    ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
    ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
    ///</summary>
    public float cooldown;

    public SkillObj(SkillModel model)
    {
        this.model = model;
        this.cooldown = 0;
    }
}

///<summary>
///策划填表的技能
///</summary>
public struct SkillModel
{
    ///<summary>
    ///技能的id
    ///</summary>
    public string id;

    /// <summary>
    /// 技能描述
    /// </summary>
    public string descriptionTemplate;

    ///<summary>
    ///技能使用的条件，这个游戏中只有资源需求，比如hp、ammo之类的
    ///</summary>
    public ChaResource condition;

    public int attackRange;

    /// <summary>
    /// 技能基础冷却时间
    /// </summary>
    public float cooldown;

    ///<summary>
    ///技能的消耗，成功之后会扣除这些资源
    ///</summary>
    public ChaResource cost;

    ///<summary>
    ///技能的效果，必然是一个timeline
    ///</summary>
    public TimelineModel effect;

    ///<summary>
    ///学会技能的时候，同时获得的buff
    ///</summary>
    public AddBuffInfo[] buff;

    public bool canUseOnStart;

    /// <summary>
    /// 技能参数
    /// </summary>
    public Dictionary<string, object> skillParams;

    public SkillModel(string id, ChaResource cost, ChaResource condition, string effectTimeline, AddBuffInfo[] buff, float cooldown = 0.1f, int attackRange = 1, Dictionary<string, object> skillParams = null, string descriptionTemplate = "", bool canUseOnStart = true)
    {
        this.id = id;
        this.cost = cost;
        this.condition = condition;
        this.effect = DesignerTables.Timeline.GetTimelineCopy(effectTimeline);

        //SceneVariants.desingerTables.timeline.data[effectTimeline];
        this.buff = buff;
        this.cooldown = cooldown;
        this.attackRange = attackRange;
        this.skillParams = skillParams;
        this.descriptionTemplate = descriptionTemplate;
        martialArt = null;
        this.canUseOnStart = canUseOnStart;
    }

    public void ResetEventManager()
    {
        this.effect.ResetEventManager();
    }

    public SkillModel Clone()
    {
        SkillModel clonedSkill = new SkillModel();
        clonedSkill.id = this.id;
        clonedSkill.descriptionTemplate = this.descriptionTemplate;
        clonedSkill.condition = this.condition;
        clonedSkill.cooldown = this.cooldown;
        clonedSkill.attackRange = this.attackRange;
        clonedSkill.cost = this.cost;
        clonedSkill.effect = this.effect.Clone();
        clonedSkill.buff = this.buff;
        clonedSkill.martialArt = this.martialArt;
        clonedSkill.canUseOnStart = this.canUseOnStart;

        // 检查 skillParams 是否为 null
        if (this.skillParams != null)
        {
            // 克隆技能参数字典
            clonedSkill.skillParams = new Dictionary<string, object>(this.skillParams);
        }
        else
        {
            // 如果 skillParams 为 null，创建一个新的空字典
            clonedSkill.skillParams = new Dictionary<string, object>();
        }

        return clonedSkill;
    }

    /// <summary>
    /// 动态生成一个基于技能参数的技能描述
    /// </summary>
    /// <returns></returns>
    public string GetDynamicDescription()
    {
        string description = this.descriptionTemplate;
        foreach (var param in skillParams)
        {
            // 这里假设占位符的形式是 {参数名称}
            string placeholder = "{" + param.Key + "}";
            string value = param.Value.ToString();

            // 替换占位符
            description = description.Replace(placeholder, value);
        }
        return description;
    }
}