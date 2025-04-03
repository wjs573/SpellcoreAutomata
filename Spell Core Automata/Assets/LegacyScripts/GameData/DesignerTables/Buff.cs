using System.Collections.Generic;

namespace DesignerTables
{
    ///<summary>
    ///buff的效果
    ///</summary>
    public class Buff
    {
        public static Dictionary<string, BuffModel> data = new Dictionary<string, BuffModel>()
            {
                //DeathRelayDebuff
                //灾厄传播
                { "DeathRelayDebuff", new BuffModel("DeathRelayDebuff", "灾厄传播","","",
                    new string[]{"Passive","debuff"}, 0, 1, 0f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "RelayDebuffOnDeath", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    null
                )},

                //怪物出生buff
                { "Resurrect", new BuffModel("Resurrect", "复苏中","","Body",
                    new string[]{"Passive"}, 0, 1, 0f,
                    "ResurrectOnCreate", new object[0],  //occur
                    "ResurrectOnRemoved", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(false,false,false),
                    null
                )},

                //HeavenlyFireThreeProfoundTransformation 天火三玄变
                //天火三玄变 33%生命值时移除此buff 获得天火三玄变生效版buff
                { "HeavenlyFireThreeProfoundTransformation", new BuffModel("HeavenlyFireThreeProfoundTransformation", "天火三玄变","","Body",
                    new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "AddBuff", new object[]{ "HeavenlyFireThreeProfoundTransformationEffect",1,33f,true,true},  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "RemoveBuffOnPercentHp", new object[]{ 0.33f},  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    null
                )},

                { "HeavenlyFireThreeProfoundTransformationEffect", new BuffModel("HeavenlyFireThreeProfoundTransformationEffect", "天火三玄变","","Body",
                    new string[]{"Passive"}, 0, 1, 1f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "PercentDamageOnTick", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    new ChaProperty[2]{
                        new ChaProperty(100,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,MoveType.ground,false),
                        new ChaProperty(0,0,0,
                        0,0,0,0,
                        0,25,25,
                        0,0,0,0,0,0,MoveType.ground,false)}
                )},


                //定时死亡
                { "ScheduledDead", new BuffModel("ScheduledDead", "定时死亡","","Body",
                    new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "DeadOnRemoved", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    new ChaProperty[2]{
                        new ChaProperty(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,MoveType.ground,true),
                        new ChaProperty(0,0,0,
                        0,0,0,0,
                        0,0,0,0,0,0,0,0,0,MoveType.ground,false)}
                )},

                //落雷亡语
                { "ThunderStrikeOnDead", new BuffModel("ThunderStrikeOnDead", "落雷亡语","","Body",
                    new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "CreateAoEOnDead", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    null
                )},

                // 点燃：
                // 持续性伤害：每秒对目标造成一定数值的伤害，伤害值与点燃层数相关。
                // 叠加机制：点燃可以叠加，每层增加额外伤害，并刷新持续时间。
                // 持续时间：每层点燃默认持续5秒，叠加后重新计时。
                { "Ignite", new BuffModel("Ignite", "点燃","Effect/Buff/IgniteSightEffect","Body",
                    new string[]{"debuff"}, 0, 10, 1f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "IgniteDamageOnTick", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    null
                )},

                //吞噬标记
                { "DevourMark", new BuffModel("DevourMark", "吞噬标记","","Body",
                    new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "AddMaxHPToCasterOnBeKilled", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    null
                )},

                //吞噬属性记录
                { "DevourChaPropertyRecord", new BuffModel("DevourChaPropertyRecord", "吞噬属性记录","","Body",
                    new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    new ChaProperty[2]{ChaProperty.zero,ChaProperty.zero }
                )},

                //减速，降低50%移动速度。
                { "Deceleration", new BuffModel("Deceleration", "减速", "","Body",new string[]{"Passive","debuff","Control"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true), new ChaProperty[2]{
                        new ChaProperty(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,MoveType.ground,false),
                        new ChaProperty(-50,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,MoveType.ground,false)}
                )},

                //击飞，完全使角色无法行动（移动、转向、施法），持续一定时间。
                { "KnockUp", new BuffModel("KnockUp", "击飞", "","Body",new string[]{"Passive","debuff","Control"}, 0, 1, 0,
                    "JumpUpOnCreate", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(false,false,false), null
                )},

                //炮塔控制 关联skill_CreateTurret
                //记录已创建的炮台
                //OnHit 记录正在攻击的目标
                //OnTick 创建一轮齐射
                {
                    "TurretControl",new BuffModel("TurretControl", "炮塔控制", "","Body",new string[]{"Passive"}, 0, 1, 1f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "TurretSalvo", new object[0],  //tick
                    "", new object[0],  //cast
                    "RecordHitTarget", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true), null)
                },

                //训练场的不死稻草人
                //每次受到致死伤害的时候都会免疫改次伤害并获得9999的治疗
                {
                    "ImmortalTrainingTarget",new BuffModel("ImmortalTrainingTarget", "不死训练靶", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "GetHealOnDead", new object[]{new DamageInfo(null,null,new Damage(-9999),0f,0f,new DamageInfoTag[]{DamageInfoTag.directHeal }) },  //dead
                    new ChaControlState(true,true,true), null)
                },

                //召唤物 神念附着
                //召唤物会消耗召唤者的神念
                //当召唤物死去时会返回对应的神念强度
                //神念强度储存在buffParam中：召唤者、消耗的神念强度
                {"SummonedEntity ",new BuffModel("SummonedEntity", "神念附着", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "ReturnMind", new object[0],  //dead
                    new ChaControlState(true,true,true), null
                )},

                //隐身buff 角色处于隐身状态下，IsInvisible为true，同时获得半透明化美术效果
                {
                    "Invisibility",new BuffModel("Invisibility","隐身", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "EnterInvisibilityState", new object[0],  //occur
                    "ExitInvisibilityState", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true),
                    new ChaProperty[]{
                    new ChaProperty(0,0,50,0,0,0,0,0,0,0,0,0,0,0,0,0,MoveType.ground,true),
                    ChaProperty.zero
                    })
                },

                //不朽
                //单位死亡时会进入重生状态
                { "Immortality", new BuffModel("Immortality", "不朽", "","Body",new string[]{"Passive","Pop"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "ImmortalityOnDead", new object[0],  //dead
                    new ChaControlState(true,true,true), null
                )},

                //重生
                //单位无法移动或施法
                //防御力归零
                //可以受到伤害
                //每0.2秒回复10%最大生命值
                //持续2秒
                { "Resurgence", new BuffModel("Resurgence", "重生", "","Body",new string[]{"Passive","Pop"}, 0, 1, 0.2f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "RecoverPercentMaxHp", new object[]{ 0.1f},  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(true,true,true), new ChaProperty[]{ChaProperty.zero,
                        new ChaProperty(0,0,0,0,0,0,0,0,0,-1,0,0,0,0,0,0,MoveType.ground)}
                )},

                //晕眩是一种严重的负面效果，完全使角色无法行动（移动、转向、施法），持续一定时间。
                //角色完全无法进行任何行动，
                //无法自卫或对攻击做出反应。
                //晕眩通常被用作游戏中强力的控制效果。
                { "Stun", new BuffModel("Stun", "眩晕", "Effect/Buff/Stunned","Head",new string[]{"Passive","debuff","Control"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(false,false,false), null
                )},
                //自爆
                { "ExplosionBuff", new BuffModel("ExplosionBuff", "自爆", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "CreateAoEOnCreate", new object[]{},  //occur
                    "DeadOnRemoved", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(false,false,false), null
                )},
                //寒霜护甲 受到所有伤害减少百分之三十
                { "FrostArmor", new BuffModel("FrostArmor", "寒霜护甲", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "CalculateModifiedDamage", new object[3]{"","",-0.3f},  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                //畏火 受到火属性伤害翻倍
                { "AfraidOfFire", new BuffModel("AfraidOfFire", "畏火", "","Body",new string[]{"Passive","debuff"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "CalculateModifiedDamage", new object[3]{DamageInfoTag.directDamage,"火属性伤害",2.0f },  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                //苦心人 天不负 卧薪尝胆，三千越甲可吞吴
                //开局给角色添加一层卧薪尝胆buff 输出降低百分之五十
                //释放越王勾践剑时移除buff，并获得百分之1乘积蓄秒数的伤害提升
                { "Endured", new BuffModel("Endured", "卧薪尝胆", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "EnduredDamageModified", new object[]{true},  //cast
                    "DamageModification", new object[]{ 0.5f},  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                //越王勾践剑
                { "The legendary Sword of King Goujian", new BuffModel("The legendary Sword of King Goujian", "越王勾践剑", "","Body",new string[]{"Passive"}, 0, 10, 1,
                    "", new object[0],  //occur
                    "SwordOnRemove", new object[0],  //remove
                    "FlyingSwordTick", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                //青竹蜂云剑
                { "The Green Bamboo Wasp Cloud Sword", new BuffModel("The Green Bamboo Wasp Cloud Sword", "青竹蜂云剑", "","Body",new string[]{"Passive"}, 0, 10, 1,
                    "", new object[0],  //occur
                    "SwordOnRemove", new object[0],  //remove
                    "FlyingSwordTick", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                //骨冷灵火
                { "The  Bone-chilling spiritual fire", new BuffModel("The  Bone-chilling spiritual fire", "骨冷灵火", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                { "The Falling Heart Flame", new BuffModel("The Falling Heart Flame", "陨落心炎", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "TheFallingHeartFlameOnCreate", new object[0],  //occur
                    "TheFallingHeartFlameOnRemove", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                //飞剑术
                //如果处于收剑状态 无效果
                //处于出剑状态，每秒消耗法力值，法力值低于使用每秒消耗值时，自动使用飞剑术技能，进行收剑。
                { "FlyingSword", new BuffModel("FlyingSword", "飞剑术", "","Body",new string[]{"Passive"}, 0, 1, 1f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "FlyingSwordTick", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                { "TeleportBulletPassive", new BuffModel("TeleportBulletPassive", "传送弹技能被动效果", "","Body",new string[]{"Passive"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "FireTeleportBullet", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null
                )},
                { "TeleportTo", new BuffModel("TeleportTo", "直接把GameObject传送到某个世界坐标（非常危险）", "","Body",new string[]{"Dangerous"}, 0, 1, 0,
                    "", new object[0],  //occur
                    "TeleportCarrier", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.stun, null
                )},
                { "ExplosionBarrel", new BuffModel("ExplosionBarrel", "爆炸的桶子用的", "","Body",new string[]{"Passive"}, -1, 1, 5.0f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "BarrelDurationLose", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "OnlyTakeOneDirectDamage", new object[0],  //hurt
                    "", new object[0],  //kill
                    "BarrelExplosed", new object[0],  //dead
                    ChaControlState.stun, null  //桶子也是被昏迷的
                )},

                //基础回复 读取chastate中的 生命恢复值和灵力恢复值
                //每秒进行一次回复 生命恢复值是创建damageinfo 灵力恢复值是直接修改
                { "BaseRecover", new BuffModel("BaseRecover", "基础回复", "","Body",new string[]{"Passive","Recover"}, 0, 1, 1f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "BaseRecover", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null  //
                )},
                //中毒
                { "Poisoning", new BuffModel("Poisoning", "中毒", "","Body",new string[]{"Passive","debuff"}, 0, 9999, 1f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "PoisoningDamageOnTick", new object[0],  //tick
                    "PoisoningDamageOnCast", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null  //
                )},
                //烧伤
                { "Burn", new BuffModel("Burn", "烧伤", "","Body",new string[]{"Passive","debuff"}, 0, 9999, 0f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "BurnDamageOnHurt", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null  //
                )},
                //僵直 无法移动、转向或施法
                { "StunLock", new BuffModel("StunLock", "僵直", "","Body",new string[]{}, 0, 1, 0f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(false,false,false), null  //
                )},
                //冻结 无法移动无法施法，但是会受到额外的水属性伤害
                { "Freeze", new BuffModel("Freeze", "冻结", "","Body",new string[]{"Passive","debuff"}, 0, 1, 0f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "FreezeOnHurt", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    new ChaControlState(false,false,false), null  //
                )},
                { "Bleeding", new BuffModel("Bleeding", "流血", "","Body",new string[]{"Passive","debuff"}, 0, 1, 0.25f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "DoBleedingDamageOnTick", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin, null  //
                )},
                //寒冷 降低行动速度和移动速度，最多叠加五层，
                //每次添加层数都重置冷却时间
                //达到五层时进入冻结状态
                { "Cold", new BuffModel("Cold", "冻结", "","Body",new string[]{"Passive","debuff"}, 0, 5, 0f,
                    "ColdOnOccur", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin,new ChaProperty[2]{ new ChaProperty(-10,0,-10,0,0,0,0,0,0,0,0,0f,0f,0f,0f,0f,MoveType.ground),ChaProperty.zero}  //
                )},
                //暴怒 提升50%攻击力 降低25%防御力 提高100点cd冷却胜率 100点行动速率 100点移动速度
                { "Rage", new BuffModel("Rage", "暴怒", "","Body",new string[]{"Passive"}, 0, 1, 0f,
                    "", new object[0],  //occur
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //cast
                    "", new object[0],  //hit
                    "", new object[0],  //hurt
                    "", new object[0],  //kill
                    "", new object[0],  //dead
                    ChaControlState.origin,new ChaProperty[2]{ new ChaProperty(
                        100,100,100,
                        100,-100,0,0,0,
                        0,0,0,
                        0f,0f,0f,0f,0f,MoveType.ground),
                        ChaProperty.zero}  //
                )}
            };
    }
}