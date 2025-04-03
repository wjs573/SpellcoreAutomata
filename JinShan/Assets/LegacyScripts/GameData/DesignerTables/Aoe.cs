using System.Collections.Generic;

namespace DesignerTables
{
    ///<summary>
    ///AoeModel
    ///</summary>
    public class AoE
    {
        public static Dictionary<string, AoeModel> data = new Dictionary<string, AoeModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, AoeModel>()
            {
                // 毒雾Aoe
                {"PoisonFog", new AoeModel(
                    "PoisonFog", "Effect/Circle/Healing circle", new string[0], 0.5f, false,
                    "", new object[0],  //create
                    "", new object[0],  //remove
                    "AddBuffOnTick", new object[]{ "Poisoning"},  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                // 雷球爆炸Aoe
                {"LightningOrbExplosion", new AoeModel( //移除时造成伤害
                    "LightningOrbExplosion", "Effect/Hovl/Hit/Hit 17", new string[0], 0, false,
                    "DoDamageOnCreate", new object[]{ new Damage(500), 1f, true, false, true},  //create
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                // 火球爆炸Aoe
                {"FireOrbExplosion", new AoeModel( //移除时造成伤害
                    "FireOrbExplosion", "Effect/Hovl/Hit/Hit 18", new string[0], 0, false,
                    "DoDamageOnCreate", new object[]{ new Damage(666), 1f, true, false, true},  //create
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                // 红球爆炸Aoe
                {"RedOrbExplosion", new AoeModel( //移除时造成伤害
                    "RedOrbExplosion", "Effect/Hovl/Hit/Hit 19", new string[0], 0, false,
                    "DoDamageOnCreate", new object[]{ new Damage(888), 1f, true, false, true},  //create
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //雷击领域 ThunderStrikeField
                {"ThunderStrikeField", new AoeModel(
                    "ThunderStrikeField", "Effect/Circle/ThunderStrikeField", new string[0], 0.55f, false,
                    "", new object[0],
                    "", new object[0],
                    "DoDamageOnTick", new object[]{ new Damage(300),0.1f,true,false,true, "Effect/Hit/Electro hit", "Body" },  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //地心之火领域，移除时对范围内的敌人造成一次火属性伤害。
                {"InfernoPurgatoryField", new AoeModel(
                    "InfernoPurgatoryField", "Effect/Circle/InfernoPurgatoryField", new string[0], 0f, false,
                    "", new object[0],
                    "KnockUpAndDamageEnemyOnRemoved", new object[]{new Damage(100), 0.1f, true, false, true, "Effect/Hit03", "Body"},
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},

                {//红色挥击
                    "RedSlash", new AoeModel( //移除时造成伤害
                    "RedSlash", "Effect/Slash/RedSlash", new string[0], 0, false,
                    "", new object[0],  //create
                    "DoDamageOnRemoved", new object[]{ new Damage(100), 0.1f, true, false, true, "Effect/HitEffect_A", "Body"},  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},

                {//焰浪
                    "FireWave",new AoeModel(
                        "FireWave", "Effect/Slash/RedSlash", new string[0], 0f, false,
                        "IgniteChaOnCreate", new object[0],  //create
                        "", new object[0],  //removed
                        "", new object[0],  //tick
                        "", new object[0],  //chaEnter
                        "", new object[0],  //chaLeave
                        "", new object[0],  //bulletEnter
                        "", new object[0]   //bulletLeave

                )},

                {//吞噬领域 Devour
                    "DevourField",new AoeModel(
                        "DevourField", "Effect/Circle/DevourField", new string[0], 0.2f, false,
                        "", new object[0],  //create
                        "", new object[0],  //removed
                        "DragAndDamageEnemy", new object[0],  //tick
                        "AddDevourMarkOnCharacterEnter", new object[0],  //chaEnter
                        "RemoveDevourMarkOnCharacterEnter", new object[0],  //chaLeave
                        "", new object[0],  //bulletEnter
                        "", new object[0]   //bulletLeave

                )},

                {//减速
                    "DecelerationField",new AoeModel(
                        "DecelerationField", "", new string[0], 0f, false,
                        "", new object[0],  //create
                        "", new object[0],  //removed
                        "", new object[0],  //tick
                        "AddDecelerationOnCharacterEnter", new object[0],  //chaEnter
                        "RemoveDecelerationOnCharacterEnter", new object[0],  //chaLeave
                        "", new object[0],  //bulletEnter
                        "", new object[0]   //bulletLeave

                )},

                //范围击飞 在aoe创建时击飞
                {
                    "KnockUpAoeOnCreate", new AoeModel(
                    "KnockUpAoeOnCreate", "", new string[0], 0f, false,
                    "KnockUpEnemyOnCreate", new object[0],  //create
                    "", new object[0],  //removed
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},

                //范围击飞 在aoe移除时击飞
                {"KnockUpAoeOnRemoved", new AoeModel(
                    "KnockUpAoeOnRemoved", "Effect/CollideEffect", new string[0], 0f, false,
                    "", new object[0],  //create
                    "KnockUpEnemyOnRemoved", new object[0],  //removed
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},

                //心火领域，移除时对范围内的敌人造成一次火属性伤害。
                {"HeartFlame", new AoeModel( //每0.5秒造成伤害
                    "HeartFlame", "", new string[0], 0f, false,
                    "", new object[0],
                    "DoDamageOnRemoved", new object[]{new Damage(10), 0.1f, true, false, true, "Effect/Hit03", "Body"},
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},

                //聚敌
                {"GatheringEnemies", new AoeModel( //每0.5秒造成伤害
                    "GatheringEnemies", "", new string[0], 0.1f, false,
                    "", new object[0],
                    "", new object[0],
                    "GatheringEnemiesOnTick", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //龙卷风
                {"Storm", new AoeModel( //每0.5秒造成伤害
                    "Storm", "Effect/Skill/Spell_Storm_7", new string[0], 0.25f, false,
                    "", new object[0],
                    "", new object[0],
                    "GatheringEnemiesOnTick", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //火焰爆炸
                {"FireExplosion", new AoeModel( //炸弹爆炸
                    "FireExplosion", "", new string[0], 0, false,
                    "CreateSightEffect", new object[]{"Effect/Skill/Spell_Fire_12"},
                    "DoDamageOnRemoved", new object[]{new Damage(100), 0.1f, true, false, true, "Effect/Hit03", "Body"},    //10%攻击力加成
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //碎石爆炸
                {"RockExplosion", new AoeModel( //炸弹爆炸
                    "RockExplosion", "", new string[0], 0, false,
                    "CreateSightEffect", new object[]{"Effect/Skill/RockExplosion"},
                    "DoDamageOnRemoved", new object[]{new Damage(100), 0.1f, true, false, true, "Effect/Hit03", "Body"},    //10%攻击力加成
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //陨石打击
                {"MetorHit", new AoeModel( //炸弹爆炸
                    "MetorHit", "", new string[0], 0, false,
                    "CreateSightEffect", new object[]{"Effect/Skill/Metor"},
                    "DoDamageOnRemoved", new object[]{ new Damage(10), 0.1f, true, false, true, "Effect/HitEffect_A", "Body"},    //10%攻击力加成
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //aoe 冻结领域
                {"AoE_FrozenDomain", new AoeModel(
                    "AoE_FrozenDomain", "Effect/Circle/Freeze Circle", new string[0], 1f, false,
                    "", new object[0],  //create
                    "", new object[0],  //remove
                    "AddBuffOnTick", new object[]{"Cold"},  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //aoe 召唤character
                {"AoE_Smummoning", new AoeModel(
                    "AoE_Smummoning", "Effect/Circle/Magic circle", new string[0], 0, false,
                    "", new object[0],  //create
                    "SmummoningOnRemoved", new object[]{1f,"IceToad"},  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                //冰锥爆裂
                {"AoE_Ice Spike Explosion", new AoeModel(
                    "AoE_Ice Spike Explosion", "Effect/Skill/Ice Spike Explosion", new string[0], 0, false,
                    "FreezesEnemyInRange", new object[0],  //create
                    "DoDamageOnRemoved", new object[]{new Damage(20, 20), 0.1f, true, false, true, "Effect/Hit/HitIce", "Body" },  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"BulletShield", new AoeModel(
                    "BulletShield", "", new string[0], 0, true,
                    "", new object[0],  //create
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"Collide", new AoeModel(//冲撞aoe 对范围内的敌人造成伤害
                    "Collide", "Effect/CollideEffect", new string[0], 0, true,
                    "", new object[0],  //create
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "DoDamageToEnterCha", new object[]{new Damage(0, 20), 0.2f, true, false, true, "Effect/HitEffect_A", "Body"},  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"SuperCollide", new AoeModel(//超级冲撞aoe 对范围内的敌人造成伤害，并且击飞
                    "Collide", "Effect/CollideEffect", new string[0], 0, true,
                    "", new object[0],  //create
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "DoDamageAndForceMoveToEnterCha", new object[]{new Damage(100), 0.2f, true, false, true, "Effect/HitEffect_A", "Body"},  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"ThunderStrike", new AoeModel(//雷击
                    "ThunderStrike", "Effect/MarkerRedCircle", new string[0], 0, false,
                    "", new object[0],  //create
                    "DoDamageOnRemoved", new object[]{new Damage(20, 20), 0.1f, true, false, true, "Effect/HitEffect_A", "Body" },  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0], //bulletEnter
                    "", new object[0]  //bulletLeave
                )},
                {"SpaceMonkeyBall", new AoeModel(
                    "SpaceMonkeyBall", "Effect/EffectSpikeBall", new string[0], 0, true,
                    "", new object[0],  //create
                    "", new object[0],  //remove
                    "", new object[0],  //tick
                    "DoDamageToEnterCha", new object[]{new Damage(0, 20), 0.2f, true, false, true, "Effect/HitEffect_A", "Body"},  //chaEnter
                    "", new object[0],  //chaLeave
                    "SpaceMonkeyBallHit", new object[]{0.05f},  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"BlackHole", new AoeModel(
                    "BlackHole", "Effect/ShockWave", new string[0], 0.02f, true,
                    "", new object[0],  //create
                    "", new object[0],  //remove
                    "BlackHole", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"BoomExplosive", new AoeModel( //炸弹爆炸
                    "BoomExplosive", "", new string[0], 0, false,
                    "CreateSightEffect", new object[]{"Effect/Explosion_A"},
                    "DoDamageOnRemoved", new object[]{new Damage(0, 20), 0.1f, true, false, true, "Effect/HitEffect_A", "Body"},    //10%攻击力加成
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"BellRing", new AoeModel( //每0.5秒造成伤害
                    "BellRing", "", new string[0], 1f, false,
                    "", new object[0],
                    "", new object[0],
                    "DoDamageOnTick", new object[]{new Damage(30),0.1f,true,false,true,"Effect/Hit/LoveHit","Body" },  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"DoDamageOnRemovedSkill", new AoeModel( //移除时造成伤害
                    "DoDamageOnRemove", "", new string[0], 0, false,
                    "", new object[0],  //create
                    "DoDamageOnRemoved", new object[]{ new Damage(50), 0.1f, true, false, true, "Effect/HitEffect_A", "Body"},  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                 {"HeavyHammer", new AoeModel( //移除时造成伤害并添加眩晕
                    "DoDamageOnRemove", "Effect/Circle/RedCircle", new string[0], 0, false,
                    "", new object[0],  //create
                    "DoDamageAndAddBuffOnRemoved", new object[]{ new Damage(100), 0.1f, true, false, true, "Effect/HitEffect_A", "Body",new AddBuffInfo(DesignerTables.Buff.data["Stun"],null,null,1,2f,true)},  //remove
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )},
                {"StayingBoom", new AoeModel(   //炸弹掉在地上的样子
                    "StayingBoom", "Bullet/BombBall", new string[0], 0, false,
                    "", new object[0],
                    "CreateAoeOnRemoved", new object[]{"BoomExplosive", 1.5f, 0f},
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                )}
            };
        }
    }
}