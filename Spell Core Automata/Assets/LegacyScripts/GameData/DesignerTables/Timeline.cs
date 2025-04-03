using System.Collections.Generic;
using DesignerScripts;
using MoreMountains.Tools;
using UnityEngine;

namespace DesignerTables
{
    public class Timeline
    {
        public static Dictionary<string, TimelineModel> data = new Dictionary<string, TimelineModel>();
        // 添加一个方法来获取完全拷贝的 TimelineModel
        public static TimelineModel GetTimelineCopy(string id)
        {
            if (data.ContainsKey(id))
            {
                // 使用 Clone 方法创建完全拷贝的副本
                return data[id].Clone();
            }
            else
            {
                // 如果 id 不存在，可以根据需要返回一个默认值或者抛出异常
                // 以下是返回 null 的示例
                return data["base"];
            }
        }
        public static void Initialize()
        {
            data = new Dictionary<string, TimelineModel>();

            //空
            data.Add("base", new TimelineModel("base", new TimelineNode[] { }, 0.00f, TimelineGoTo.Null));

            //*******基础技能*******
            data.Add(
                //烈焰斩
                "skill_FireSlash", new TimelineModel("skill_FireSlash", new TimelineNode[] {
                    new TimelineNode(0.00f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["FireWave"],
                        null, Vector3.zero, 4.00f, 0.60f, 0f,
                        null,null,null,true),false
                    }),
            }, 0.30f, TimelineGoTo.Null));


            //*******自走棋*******
            //棋子普通攻击
            data.Add("ChessPieceBaseAttack", new TimelineModel("ChessPieceBaseAttack", new TimelineNode[] {
                new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "BaseAttack", false }),
                new TimelineNode(0.01f, "SetCasterControlState",new object[] { false, false, false }),
                new TimelineNode(0.40f, "BaseAttack",new object[] {"GetCurrentTarget"}),
                new TimelineNode(0.80f, "SetCasterControlState",new object[] { true, true, true }),
                }, 0.80f, TimelineGoTo.Null));

            //三千雷动
            data.Add("skill_ThreeThousandThunderMovements", new TimelineModel("skill_ThreeThousandThunderMovements", new TimelineNode[] {
                    new TimelineNode(0.01f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.05f, "CloneCharacter",new object[] {}),
                    new TimelineNode(0.05f, "CasterBlinkMove",new object[] {4.0f}),
                    new TimelineNode(0.20f, "SetCasterControlState",new object[] { true, true, true })    //早0.1秒恢复操作状态手感好点
                }, 0.30f, TimelineGoTo.Null));

            //召唤骷髅群
            data.Add("skill_SpawnSkeletons", new TimelineModel("skill_SpawnSkeletons", new TimelineNode[] {
                    new TimelineNode(0.00f, "SummonCharacterByEnemyData",new object[]{ new CharacterSpawnInfo("骷髅",3)}),
                }, 0.10f, TimelineGoTo.Null));

            //召唤霜灵群
            data.Add("skill_SpawnIceGhosts", new TimelineModel("skill_SpawnIceGhosts", new TimelineNode[] {
                    new TimelineNode(0.00f, "SummonCharacterByEnemyData",new object[]{ new CharacterSpawnInfo("霜灵",2)}),
                }, 0.10f, TimelineGoTo.Null));

            data.Add(
                //创建地心之火领域
                "skill_InfernoPurgatory", new TimelineModel("skill_InfernoPurgatory", new TimelineNode[] {
                    new TimelineNode(0.00f, "AICreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["InfernoPurgatoryField"],
                        null, Vector3.zero, 4.00f, 4.0f, 0f,
                        null,null,null,true),false,true,0f
                    }),
            }, 0.20f, TimelineGoTo.Null));

            data.Add(
                //焰分噬浪尺
                "skill_FireWaveSlash", new TimelineModel("skill_FireWaveSlash", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.00f, "PlaySightEffectOnCaster",new object[] { "Foot", "Effect/Circle/FireCharging", "FireCharging", false  }),
                    new TimelineNode(0.10f,"PopChargingTime",null,null),
                    new TimelineNode(0.21f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["RedSlash"],
                        null, Vector3.zero, 5.00f, 0.40f, 0f,
                        null,null,null,true),false
                    }),
                    new TimelineNode(0.30f, "CreateAoEAndSetDurationRadiusByParams",new object[] { }),
                    new TimelineNode(0.30f, "StopSightEffectOnCaster",new object[] { "Foot", "FireCharging" }),
                    new TimelineNode(0.31f, "SetCasterControlState",new object[] { true, true, true }),
            }, 0.31f, new TimelineGoTo(0.2f, 0.01f)));

            data.Add(
                //创建吞噬领域
                "skill_CreateDevourField", new TimelineModel("skill_CreateDevourField", new TimelineNode[] {
                    new TimelineNode(0.00f, "AICreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["DevourField"],
                        null, Vector3.zero, 4.00f, 9.0f, 0f,
                        null,null,null,true),false,true,4f
                    }),
            }, 0.20f, TimelineGoTo.Null));

            data.Add(
                //创建雷击领域
                "skill_CreateThunderField", new TimelineModel("skill_CreateThunderField", new TimelineNode[] {
                    new TimelineNode(0.00f, "AICreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["ThunderStrikeField"],
                        null, Vector3.zero, 2.50f, 0.9f, 0f,
                        null,null,null,true),false,true,0.5f
                    }),
                    //Effect/Circle/ThunderStrikeField
            }, 0.10f, TimelineGoTo.Null));

            data.Add(
                //发射剑影
                "skill_LaunchingBladeMirage", new TimelineModel("skill_LaunchingBladeMirage", new TimelineNode[] {
                    new TimelineNode(0.00f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["Blade"], null, Vector3.zero, 0, 10f, 3f,0.02f,
                        null,
                        null,
                        false, new Dictionary<string, object> { { "RemainingSplittingTimes",  2},{ "SplitBulletCount", 2 } } ),
                        "Muzzle"}),
                }, 0.25f, TimelineGoTo.Null, true));

            data.Add(
                //发射佛怒火莲
                "skill_LaunchingFlameLotusOfWrath", new TimelineModel("skill_LaunchingFlameLotusOfWrath", new TimelineNode[] {
                    new TimelineNode(0.00f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["FlameLotusOfWrath"], null, Vector3.zero, 0, 3.0f, 5f,0f,
                        null,
                        null,
                        false, new Dictionary<string, object>() { },1,0f
                    ), "Muzzle"}),
                }, 0.25f, TimelineGoTo.Null, true));

            data.Add(
                //创建炮台
                "skill_CreateTurret", new TimelineModel("skill_CreateTurret", new TimelineNode[] {
                    new TimelineNode(0.10f, "CreateTurret",new object[] {
                        "Turret",
                        new ChaProperty(100, 0, 100,
                        1000, 2, 100,0,
                        50,20,10,0,
                        1.5f,0.1f,0,0.5f,0.5f,MoveType.ground),
                        0f, "",
                        new string[]{},
                        new AddBuffInfo[] {}
                    }),
            }, 0.20f, TimelineGoTo.Null));

            data.Add(//发射高射炮弹
                "skill_LaunchingHighSpeedShell", new TimelineModel("skill_LaunchingHighSpeedShell", new TimelineNode[] {
                    new TimelineNode(0.00f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["HighSpeedShell"], null, Vector3.zero, 0, 15.0f, 5f,0f,
                        null,
                        null,
                        false, new Dictionary<string, object>() { }
                    ), "Muzzle"}),
                }, 0.25f, TimelineGoTo.Null, true));

            data.Add(//暴雨弹幕
                "skill_HeavyRainBarrage", new TimelineModel("skill_HeavyRainBarrage", new TimelineNode[] {
                    new TimelineNode(0.00f, "FireBulletAtRandomAngles",5,0.2f,
                    new object[] {
                        new BulletLauncher(
                        Bullet.data["HeavyRainBullet"], null, Vector3.zero, 0, 3.0f, 5f,0f,
                        DesignerScripts.Bullet.bulletTween["SpeedUpFollowingTarget"],
                        DesignerScripts.Bullet.targettingFunc["GetNearestEnemy"],
                        false, new Dictionary<string, object>() { }
                    ), "Body"}),
                }, 1.2f, TimelineGoTo.Null, true));

            //心火领域
            //创建一个心火aoe,0.5f后造成火属性伤害
            data.Add("skill_HeartFlameField", new TimelineModel("skill_HeartFlameField", new TimelineNode[] {
                    new TimelineNode(0.00f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["HeartFlame"],
                        null, Vector3.zero, 4.00f, 1.00f, 0f,
                        DesignerScripts.AoE.aoeTweenFunc["ScaleAoe"],null,
                        new Dictionary<string, object>(){ { "targetRadius", 8f },{ "startRadius",4f } })
                    }),
            }, 1.00f, TimelineGoTo.Null));

            //减速领域
            //创建一个减速aoe
            data.Add("skill_DecelerationField", new TimelineModel("skill_DecelerationField", new TimelineNode[] {
                    new TimelineNode(0.00f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["DecelerationField"],
                        null, Vector3.zero, 6.00f, 999f, 0f,
                        DesignerScripts.AoE.aoeTweenFunc["FollowCaster"],null,null)
                    }),
            }, 1.00f, TimelineGoTo.Null));

            //翻天印
            //随机选择最近的敌人
            //投下一枚陨石
            data.Add("skill_The Overturning Seal", new TimelineModel("The Overturning Seal", new TimelineNode[] {
                    new TimelineNode(0.00f, "CreateAoEInEnemy",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["MetorHit"],
                        null, Vector3.zero, 3f, 0.5f, 0f) }),
                }, 0.5f, TimelineGoTo.Null));

            //进入隐身状态
            //播放施法动画
            data.Add("skill_EnterInvisibilityState", new TimelineModel("skill_EnterInvisibilityState", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new string[]{},new object[] { false, false, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Cast", false }),
                    new TimelineNode(0.00f, "AddBuffToCaster",new object[] {
                        new AddBuffInfo(DesignerTables.Buff.data["Invisibility"],null,null,1,5f)
                    }),
                    new TimelineNode(0.45f, "SetCasterControlState",new object[] { true, true, true })
            }, 0.50f, TimelineGoTo.Null));

            //骷髅拍击
            //创建一个拍击aoe,0.2f后造成伤害
            data.Add("SkeletonSlam", new TimelineModel("SkeletonSlam", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Slash Attack", false }),
                    new TimelineNode(0.20f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["DoDamageOnRemovedSkill"],
                        null, Vector3.zero, 2f, 0.2f, 0f,
                        DesignerScripts.AoE.aoeTweenFunc["SpawnInFront"]), false
                    }),
                    new TimelineNode(1.2f, "SetCasterControlState",new object[] { true, true, true }),
                    new TimelineNode(1.2f,"ResetViewContainer",new object[]{ })
            }, 1.2f, TimelineGoTo.Null));

            //骷髅冲锋 向面朝方向冲撞的技能效果
            data.Add("SkeletonCollide", new TimelineModel("SkeletonCollide", new TimelineNode[] {
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "CastSpell", false }),
                    new TimelineNode(0.00f, "ShowChargeDashIndicator",new object[] { "Root",1.0f,2.0f,10.0f}),
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(1.50f, "CreateAoE",new object[] {
                        new AoeLauncher(
                        AoE.data["SuperCollide"], null, Vector3.zero, 2.0f, 0.8f, 0,
                        DesignerScripts.AoE.aoeTweenFunc["FollowCaster"], new object[0],null,true
                    ), true
                    }),
                    new TimelineNode(1.50f, "CasterPlayAnim",new object[] { "DashForward", false }),
                    new TimelineNode(1.50f, "CasterForceMove",new object[] { 10.0f, 0.8f, 0.00f, false, false }),
                    new TimelineNode(2.30f, "ResetViewContainer",new object[]{ }),
                    new TimelineNode(2.30f, "SetCasterControlState",new object[] { true, true, true })    //早0.1秒恢复操作状态手感好点
                }, 2.30f, TimelineGoTo.Null));

            //重锤
            //创建一个重锤aoe,0.2f后造成伤害和眩晕效果
            data.Add("HeavyHammer", new TimelineModel("HeavyHammer", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Jump Smash Attack In Place", false }),
                    new TimelineNode(0.25f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["HeavyHammer"],
                        null, Vector3.zero, 2.5f, 0.45f, 0f,
                        DesignerScripts.AoE.aoeTweenFunc["SpawnInFront"]), false
                    }),
                    new TimelineNode(0.50f, "PlayFeedbacksByManager",new object[] { "CameraShaker" }),
                    new TimelineNode(1.80f, "SetCasterControlState",new object[] { true, true, true }),
                    new TimelineNode(1.80f,"ResetViewContainer",new object[]{ })
            }, 1.80f, TimelineGoTo.Null));

            //骨盾
            //播放施法动画,给自己添加500点护盾值
            data.Add("BoneShield", new TimelineModel("BoneShield", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Cast Spell", false }),
                    new TimelineNode(0.10f, "AddShield",new object[] {500}),
                    new TimelineNode(0.45f, "SetCasterControlState",new object[] { true, true, true })
            }, 0.50f, TimelineGoTo.Null));

            //陨石打击
            //选择最近的敌人
            //创建一个陨石打击aoe,2f后造成伤害
            data.Add("MentorHit", new TimelineModel("MentorHit", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.25f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["MetorHit"],
                        null, Vector3.zero, 2f, 2f, 0f), false
                    }),
                    new TimelineNode(1.00f, "SetCasterControlState",new object[] { true, true, true })
            }, 1f, TimelineGoTo.Null));

            //skill_IceSpikeBarrage 冰锥连发
            //冰锥术 寒冰蟾的技能
            //冰锥术是一种强大的攻击法术,可以将寒冰蟾身边的水分凝结成冰锥发射出去,造成极大的伤害。
            data.Add("skill_IceSpikeBarrage", new TimelineModel("skill_IceSpikeBarrage", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false,"noloop"  }),

                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/FlashIce", "", false ,"noloop" }),
                    new TimelineNode(0.01f, "FireBullet",6,0.1f,new object[] {
                        new BulletLauncher(
                        Bullet.data["IceCone"], null, Quaternion.Euler(0, Random.Range(-30, 30), 0) * Vector3.forward, 0, 8.0f, 5.0f
                    ), "Muzzle"}),
                    new TimelineNode(0.80f, "SetCasterControlState",new object[] { true, true, true,"noloop" })
                }, 0.80f, TimelineGoTo.Null, true));
            //召唤小寒冰蟾
            data.Add("skill_SummoningIceToad", new TimelineModel("skill_SummoningIceToad", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "PlayFeedbacksOnCaster",new object[] { "MummoningMinions" }),
                    new TimelineNode(0.00f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["AoE_Smummoning"],
                        null, Vector3.zero,1f,1f,0f)
                    }),
                    new TimelineNode(1.00f, "SetCasterControlState",new object[] { true, true, true })
                }, 1.00f, TimelineGoTo.Null));

            //召唤大量小寒冰蟾
            data.Add("skill_SummoningFiveIceToad", new TimelineModel("skill_SummoningIceToad", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false ,"noloop"}),
                    new TimelineNode(0.00f, "PlayFeedbacksOnCaster",new object[] { "MummoningMinions" ,"noloop"}),
                    new TimelineNode(0.00f, "CreateAoE",5,0.1f,new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["AoE_Smummoning"],
                        null, Vector3.zero,1f,1f,0f),true
                    }),
                    new TimelineNode(1.00f, "SetCasterControlState",new object[] { true, true, true ,"noloop"})
                }, 0.60f, TimelineGoTo.Null, true));

            //跳跃 朝当前朝向跳跃
            //距离为3*（1～2）随机
            data.Add("Move_Jump", new TimelineModel("Move_Jump", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "PlayFeedbacksOnCaster",new object[] { "JumpLand"}),
                    new TimelineNode(0.25f, "CasterForceJump",new object[] { 4.0f, 0.5f, 0.00f, false, false }),
                    new TimelineNode(0.75f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["DoDamageOnRemovedSkill"],
                        null, Vector3.zero, 3f, 0.2f, 0f), false
                    }),
                    new TimelineNode(0.75f, "SetCasterControlState",new object[] { true, true, true })
                }, 1f, TimelineGoTo.Null));

            //龙拳
            data.Add("skill_DragonPunch", new TimelineModel("skill_DragonPunch", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "PlaySightEffectOnCaster",new object[] { "Body", "Effect/Hovl/Dragon Punch", "", false }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.55f, TimelineGoTo.Null));

            //发射蓝色飞弹
            data.Add("skill_MachineGunBullet", new TimelineModel("skill_MachineGunBullet", new TimelineNode[] {
                    new TimelineNode(0.00f, "CasterPlayAnim", new object[]{"Fire", false}),
                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/Hovl/Flash/Flash 9", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["Ice Blue Missile"], null, Vector3.zero, 0, 20.0f, 3.0f,0,null,null,false,null,1,0f
                    ), "Muzzle"
                    })
                }, 0.05f, TimelineGoTo.Null));

            //发射浮游法球
            data.Add("skill_LightningOrb", new TimelineModel("skill_LightningOrb", new TimelineNode[] {
                    new TimelineNode(0.00f, "CasterPlayAnim", new object[]{"Fire", false}),
                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/Hovl/Flash/Flash 17", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["Lightning Orb"], null, Vector3.zero, 0, 5.0f, 30.0f,0,DesignerScripts.Bullet.bulletTween["Wandering"],null,false,null,1,0f
                    ), "Muzzle"
                    })
                }, 0.05f, TimelineGoTo.Null));

            //发射星星子弹 FireStarBullet
            data.Add("skill_FireStarBullet", new TimelineModel("skill_FireStarBullet", new TimelineNode[] {
                    new TimelineNode(0.00f, "CasterPlayAnim", new object[]{"Fire", false}),
                    new TimelineNode(0.00f, "PlaySound", new object[]{ "PistolFire1", MMSoundManager.MMSoundManagerTracks.Sfx}),
                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/Hovl/Flash/Flash 22", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["Star Strike"], null, Vector3.zero, 0, 10f, 5.0f,0,DesignerScripts.Bullet.bulletTween["SineWaveTween"],null,false,null,1,0f
                    ), "Muzzle"
                    }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["Star Strike"], null, Vector3.zero, 0, 10f, 5.0f,0,DesignerScripts.Bullet.bulletTween["SineWaveTweenHalfTDelay"],null,false,null,1,0f
                    ), "Muzzle"
                    })
                }, 0.10f, TimelineGoTo.Null));

            data.Add("skill_FireBladeBullet", new TimelineModel("skill_FireBladeBullet", new TimelineNode[] {
                    new TimelineNode(0.00f, "CasterPlayAnim", new object[]{"Fire", false}),
                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/Hovl/Flash/Flash 8", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["Blade"], null, Vector3.zero, 0, 15f, 5.0f,0
                    ), "Muzzle"
                    })
                }, 0.05f, TimelineGoTo.Null));

            //冰锥术 寒冰蟾的技能
            //冰锥术是一种强大的攻击法术,可以将寒冰蟾身边的水分凝结成冰锥发射出去,造成极大的伤害。
            data.Add("skill_IceSpikeSpell", new TimelineModel("skill_IceSpikeSpell", new TimelineNode[] {
                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/FlashIce", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["IceCone"], null, Vector3.zero, 0, 18.0f, 3.0f,0,null,null,false,null,3,15f
                    ), "Muzzle"
                    })
                }, 0.50f, TimelineGoTo.Null));

            //带有指示器的冰锥术
            data.Add("skill_IceSpikeSpellWithIndicator", new TimelineModel("skill_IceSpikeSpell", new TimelineNode[] {
                    new TimelineNode(0.00f, "ShowIndicator",new object[] { 2.1f }),
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { false, true, false }),
                    new TimelineNode(1.50f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(2.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/FlashIce", "", false }),
                    new TimelineNode(2.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["IceCone"], null, Vector3.zero, 0, 20.0f, 3.0f,0,null,null,false,null
                    ), "Muzzle"
                    }),
                    new TimelineNode(2.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 2.50f, TimelineGoTo.Null));

            //冰锥爆裂 寒冰蟾王兽的技能
            //冰锥爆裂 在身边创造大量冰锥,将周围的敌人冻成冰块,短暂时间后引爆冰锥造成巨大的伤害。
            data.Add("skill_Ice Spike Explosion", new TimelineModel("skill_Ice Spike Explosion", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    
                    //new TimelineNode(0.00f, "PlaySightEffectOnCaster", new object[]{"Body", "Effect/Skill/Ice Spike Explosion", "",false}),
                    new TimelineNode(0.00f, "CreateAoE",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["AoE_Ice Spike Explosion"],
                        null, Vector3.zero, 7f, 0.9f, 0f), false
                    }),
                    new TimelineNode(1.00f, "SetCasterControlState",new object[] { true, true, true })
                }, 1.00f, TimelineGoTo.Null));

            //开火技能 发射子弹
            data.Add("skill_fire", new TimelineModel("skill_fire", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/FlashIce", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["IceCone"], null, Vector3.zero, 0, 10.0f, 5.0f
                    ), "Muzzle"
                    }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            //发射毒雾子弹
            data.Add("skill_FirePoisonFogBullet", new TimelineModel("skill_FirePoisonFogBullet", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),

                    new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/FlashIce", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["Green Blob with Wood Attribute and Poison"], null, Vector3.zero, 0, 6.0f, 3.0f
                    ), "Muzzle"
                    }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            //开火技能 发射火球子弹 小火灵的技能
            data.Add("skill_fireball", new TimelineModel("skill_fireball", new TimelineNode[] {
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[]{"Fire", false}),
                    new TimelineNode(0.00f, "PlaySound", new object[]{ "FireSpell01", MMSoundManager.MMSoundManagerTracks.Sfx}),
                    new TimelineNode(0.10f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["fireball"], null, Vector3.zero, 0, 16.0f, 5.0f,0
                    ), "Muzzle"
                    })
                }, 0.20f, TimelineGoTo.Null));

            //开火技能 发射随机子弹 
            data.Add("skill_fireRandomBullet", new TimelineModel("skill_fireRandomBullet", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, false, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[]{"Fire", false}),
                    new TimelineNode(0.02f, "FireRandomBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["fireball"], null, Vector3.zero, 0, 10.0f, 5.0f,0), "Muzzle"
                    }),
                    new TimelineNode(0.15f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.20f, TimelineGoTo.Null));

            //发射骨冷灵火子弹
            data.Add("skill_The  Bone-chilling spiritual fire", new TimelineModel("skill_The  Bone-chilling spiritual fire", new TimelineNode[] {
                    //new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    //new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    //new TimelineNode(0.01f, "PlaySightEffectOnCaster",new object[] { "Muzzle", "Effect/MuzzleFlash", "", false }),
                    new TimelineNode(0.01f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["TheBoneChillingSpiritualFireBullet"], null, Vector3.zero, 0, 10.0f, 10.0f
                    ), "Muzzle"
                    }),
                    //new TimelineNode(0.80f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.80f, TimelineGoTo.Null));

            //发射氪漏氪回力标
            data.Add("skill_cloakBoomerang", new TimelineModel("skill_cloakBoomerang", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.10f, "PlaySightEffectOnCaster",new object[] { "Head", "Effect/Heart", "", false }),
                    new TimelineNode(0.10f, "FireBullet",new object[] {
                        new BulletLauncher(
                        Bullet.data["cloakBoomerang"], null, Vector3.zero, 0, 20.0f, 10.0f, 0,
                        DesignerScripts.Bullet.bulletTween["CloakBoomerangTween"],
                        DesignerScripts.Bullet.targettingFunc["BulletCasterSelf"],
                        true, new Dictionary<string, object>() { { "backTime", 0.50f } }
                    ), "Muzzle"
                    }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            //敲钟
            //创建一个aoe,每隔一段时间对范围内的敌人造成伤害
            data.Add("skill_ringTheBell", new TimelineModel("skill_ringTheBell", new TimelineNode[] {
                    
                    //new TimelineNode(0.10f, "PlaySightEffectOnCaster", new object[]{"Head","Effect/Heart","",false}),
                    new TimelineNode(0.5f, "CreateAoE",new object[] {
                        new AoeLauncher(
                        AoE.data["BellRing"], null, Vector3.zero, 3f, 100f, 0,
                        DesignerScripts.AoE.aoeTweenFunc["FollowCaster"], new object[0]
                    ), true
                    })
                }, 0.5f, TimelineGoTo.Null));

            //斩击
            //创建一个aoe,移除时造成伤害
            data.Add("skill_slash", new TimelineModel("skill_slash", new TimelineNode[] {
                    //new TimelineNode(0.10f, "PlaySightEffectOnCaster",new object[] { "Head", "Effect/Heart", "", false }),
                    new TimelineNode(0.10f, "PlaySightEffectOnCaster",new object[] { "Body", "Effect/Slash/GreenSlash", "", false }),
                    new TimelineNode(0.10f, "CreateAoE",new object[] {
                        new AoeLauncher(
                        AoE.data["DoDamageOnRemovedSkill"], null, Vector3.zero, 6f, 0.05f, 0,
                        DesignerScripts.AoE.aoeTweenFunc["FollowCaster"], new object[0]
                    ), true
                    })
                }, 0.60f, TimelineGoTo.Null));

            //竹枝横扫
            //创建一个aoe,移除时造成伤害
            data.Add("skill_BambooSlash", new TimelineModel("skill_BambooSlash", new TimelineNode[] {
                    new TimelineNode(0.01f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.10f, "PlaySightEffectOnCaster",new object[] { "Body", "Effect/Slash/GreenSlash", "", false }),
                    new TimelineNode(0.05f, "CreateAoE",new object[] {
                        new AoeLauncher(
                        AoE.data["DoDamageOnRemovedSkill"], null, Vector3.zero, 8f, 0.05f, 0,
                        DesignerScripts.AoE.aoeTweenFunc["FollowCaster"], new object[0]
                    ), true
                    }),
                    new TimelineNode(0.40f, "SetCasterControlState",new object[] { true, true, true }),
                }, 0.50f, TimelineGoTo.Null));

            //角色向移动方向打滚的技能效果
            data.Add("skill_roll", new TimelineModel("skill_roll", new TimelineNode[] {
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Roll", true }),
                    new TimelineNode(0.00f, "PlaySightEffectOnCaster",new object[] { "Body", "Effect/Fire_B", "fire_following", true }),
                    new TimelineNode(0.01f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.10f, "CasterImmune",new object[] { 0.70f }),
                    new TimelineNode(0.20f, "CasterForceMove",new object[] { 2.0f, 0.5f, 0.00f, true, false }),
                    new TimelineNode(0.80f, "StopSightEffectOnCaster",new object[] { "Body", "fire_following" }),
                    new TimelineNode(0.80f, "PlaySightEffectOnCaster",new object[] { "Body", "Effect/ShockWave", "shockWave", false }),
                    new TimelineNode(0.80f, "SetCasterControlState",new object[] { true, true, true })    //早0.1秒恢复操作状态手感好点
                }, 0.90f, TimelineGoTo.Null));

            //角色向面朝方向冲撞的技能效果
            data.Add("skill_collide", new TimelineModel("skill_collide", new TimelineNode[] {
                    new TimelineNode(0.01f, "SetCasterControlState",new object[] { false, false, false }),
                    //new TimelineNode(0.10f, "CasterImmune", new object[]{0.70f}),
                    new TimelineNode(0.50f, "CreateAoE",new object[] {
                        new AoeLauncher(
                        AoE.data["Collide"], null, Vector3.zero, 1f, 0.50f, 0,
                        DesignerScripts.AoE.aoeTweenFunc["FollowCaster"], new object[0]
                    ), true
                    }),
                    new TimelineNode(0.50f, "CasterForceMoveByInput",new object[] { 5.0f, 0.50f, 0.00f, false, false }),
                    new TimelineNode(0.90f, "SetCasterControlState",new object[] { true, true, true })    //早0.1秒恢复操作状态手感好点
                }, 1.00f, TimelineGoTo.Null));

            //角色向面朝方向冲刺的技能效果 无伤害
            data.Add("skill_dash", new TimelineModel("skill_dash", new TimelineNode[] {
                    new TimelineNode(0.01f, "SetCasterControlState",new object[] { false, false, false }),
                    new TimelineNode(0.05f, "CasterForceMoveByInput",new object[] { 5.0f, 0.15f, 0.00f, false, false }),
                    new TimelineNode(0.40f, "SetCasterControlState",new object[] { true, true, true })    //早0.1秒恢复操作状态手感好点
                }, 0.40f, TimelineGoTo.Null));

            //旋转飞叶
            data.Add("skill_SpinningLeaves", new TimelineModel("skill_SpinningLeaves", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { false, false, false,"noloop"}),
                    new TimelineNode(0.01f, "PlayFeedbacksByManagerOnCaster",new object[] { "spin","noloop" }) ,
                    new TimelineNode(0.20f, "SpinFireBullet",60,0.10f,new object[] {
                        new BulletLauncher(
                        Bullet.data["GreenLeaf"], null, Vector3.zero, 0, 10.0f, 3f
                    ), "Muzzle"
                    }),
                    new TimelineNode(6.60f, "SetCasterControlState",new object[] { true, true, true ,"noloop"})
                }, 6.80f, TimelineGoTo.Null, true));

            //召唤骷髅
            data.Add("skill_SummonSkeleton", new TimelineModel("skill_SummonSkeleton", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    //new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.10f, "SummonAICharacter",new object[] {
                        "Skeleton",
                        new ChaProperty(100, 0, 100, 1000, 2, 100,0,
                        50,20,10,0,
                        1.5f,0.1f,0,0.5f,0.5f,MoveType.ground),
                        0f, "Skeleton",
                        new string[] { "Skeleton" },
                        new AddBuffInfo[] {
                        },new string[]{"SkeletonSlam"},"AISkeleton"
                    }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            //测试用例：发射激光
            data.Add("skill_laser", new TimelineModel("skill_laser", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["GreenRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, new TimelineGoTo(0.4f, 0.2f)));

            data.Add("skill_LightningRay", new TimelineModel("skill_LightningRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["LightningRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_FlameRay", new TimelineModel("skill_FlameRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["FlameRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            //猩红光剑
            data.Add("skill_BloodyCrimsonRay", new TimelineModel("skill_BloodyCrimsonRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["BloodyCrimsonRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_PurpleRay", new TimelineModel("skill_PurpleRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["PurpleRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_IceCrystalRay", new TimelineModel("skill_IceCrystalRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["IceCrystalRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_GoldenRay", new TimelineModel("skill_GoldenRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["GoldenRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_WaterWaveRay", new TimelineModel("skill_WaterWaveRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["WaterWaveRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_PurpleThunderRay", new TimelineModel("skill_PurpleThunderRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["PurpleThunderRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_RedFlameRay", new TimelineModel("skill_RedFlameRay", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "Fire", false }),
                    new TimelineNode(0.00f, "CreateLaser",new object[]{new LaserLauncher(DataLaserModel.data["RedFlameRay"], null,0.5f,null,AimType.MouserPosition,null),"Muzzle" }),
                    //new TimelineNode(0.20f, "LaserAddDuration",new object[]{ }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));

            data.Add("skill_UnstoppableForce", new TimelineModel("skill_UnstoppableForce", new TimelineNode[] {
                    new TimelineNode(0.00f, "SetCasterControlState",new object[] { true, true, false }),
                    new TimelineNode(0.00f, "CasterPlayAnim",new object[] { "RollForward", false }),
                    new TimelineNode(0.00f, "CasterForceMove",new object[] { 8.0f, 0.50f, 0.00f, false, false }),
                    new TimelineNode(0.02f, "CreateAoEAndSetDurationByParams",new object[] {
                        new AoeLauncher(DesignerTables.AoE.data["KnockUpAoeOnRemoved"],
                        null, Vector3.zero, 4.00f, 0.20f, 0f,
                        DesignerScripts.AoE.aoeTweenFunc["FollowCaster"]), false,"CasterForceMoveTime",0.50f
                    }),
                    new TimelineNode(0.50f, "SetCasterControlState",new object[] { true, true, true })
                }, 0.50f, TimelineGoTo.Null));
        }
    }
}