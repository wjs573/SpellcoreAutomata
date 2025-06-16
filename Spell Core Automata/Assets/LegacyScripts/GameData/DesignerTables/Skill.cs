using System.Collections.Generic;

namespace DesignerTables
{
    ///<summary>
    ///BulletModel
    ///</summary>
    public class Skill
    {
        public static Dictionary<string, SkillModel> data = new Dictionary<string, SkillModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, SkillModel>() {
                //***********自走棋************
                 { "BaseAttack",new SkillModel("BaseAttack",new ChaResource(0,0),new ChaResource(0,0),
                "ChessPieceBaseAttack",null,1.00f) },
                //*********基础技能********
                //火球
                {
                    "FireBall",new SkillModel("FireBall",new ChaResource(0,30),new ChaResource(0,30),
                    "FireBall",null,1.00f,1,null,null,false)
                },
                //烈焰斩
                { "FireSlash",new SkillModel("skill_FireSlash",new ChaResource(0,30),new ChaResource(0,30),
                "skill_FireSlash",null,1.00f,1,null,null,false) },

                //三千雷动 skill_ThreeThousandThunderMovements
                { "ThreeThousandThunderMovements",new SkillModel("ThreeThousandThunderMovements",new ChaResource(0,30),new ChaResource(0,30),
                "skill_ThreeThousandThunderMovements",null,5.00f) },
                //地心之火
                { "InfernoPurgatory",new SkillModel("InfernoPurgatory",new ChaResource(0,30),new ChaResource(0,30),
                "skill_InfernoPurgatory",null,8.00f,1,null,null,false) },
                //焰分噬浪尺
                { "FireWaveSlash",new SkillModel("FireWaveSlash",new ChaResource(0,30),new ChaResource(0,30),
                "skill_FireWaveSlash",null,20.00f,1,null,null,false) },
                //创建吞噬领域
                { "CreateDevourField",new SkillModel("CreateDevourField",new ChaResource(0,30),new ChaResource(0,30),
                "skill_CreateDevourField",null,12.00f,1,null,null,false) },
                //势不可挡 石头人大招
                { "UnstoppableForce",new SkillModel("UnstoppableForce",new ChaResource(0,30),new ChaResource(0,30),"skill_UnstoppableForce",null,0.6f) },
                //激光系列
                { "LaunchingLightningRay",new SkillModel("LaunchingLightningRay",new ChaResource(0,30),new ChaResource(0,30),"skill_LightningRay",null,1f) },
                { "LaunchingPurpleRay",new SkillModel("LaunchingPurpleRay",new ChaResource(0,30),new ChaResource(0,30),"skill_PurpleRay",null,1f) },
                { "LaunchingIceCrystalRay",new SkillModel("LaunchingIceCrystalRay",new ChaResource(0,30),new ChaResource(0,30),"skill_IceCrystalRay",null,1f) },
                { "LaunchingGoldenRay",new SkillModel("LaunchingGoldenRay",new ChaResource(0,30),new ChaResource(0,30),"skill_GoldenRay",null,1f) },
                { "LaunchingWaterWaveRay",new SkillModel("LaunchingWaterWaveRay",new ChaResource(0,30),new ChaResource(0,30),"skill_WaterWaveRay",null,1f) },
                { "LaunchingPurpleThunderRay",new SkillModel("LaunchingPurpleThunderRay",new ChaResource(0,30),new ChaResource(0,30),"skill_PurpleThunderRay",null,1f) },
                { "LaunchingRedFlameRay",new SkillModel("LaunchingRedFlameRay",new ChaResource(0,30),new ChaResource(0,30),"skill_RedFlameRay",null,1f) },
                { "LaunchingFlameRay",new SkillModel("LaunchingFlameRay",new ChaResource(0,30),new ChaResource(0,30),"skill_FlameRay",null,1f) },
                { "LaunchingBloodyCrimsonRay",new SkillModel("LaunchingBloodyCrimsonRay",new ChaResource(0,30),new ChaResource(0,30),"skill_BloodyCrimsonRay",null,1f) },
                { "LaunchingLaser",new SkillModel("LaunchingLaser",new ChaResource(0,30),new ChaResource(0,30),"skill_laser",null,1f) },
                //剑影
                { "LaunchingBladeMirage",new SkillModel("LaunchingBladeMirage",new ChaResource(0,10),new ChaResource(0,10),"skill_LaunchingBladeMirage",null,2f)},
                //剑影飞弹
                { "LaunchingBlade",new SkillModel("LaunchingBlade",new ChaResource(0,10),new ChaResource(0,10),"skill_FireBladeBullet",null,2f)},
                //佛怒火莲
                { "LaunchingFlameLotusOfWrath",new SkillModel("LaunchingFlameLotusOfWrath",new ChaResource(0,100),new ChaResource(0,100),"skill_LaunchingFlameLotusOfWrath",null,10f,1,null,null,false)},
                //创建炮台
                { "CreateTurret", new SkillModel("CreateTurret", new ChaResource(0, 0), ChaResource.Null, "skill_CreateTurret",null,2f)},
                //高速炮弹
                { "LaunchingHighSpeedShell", new SkillModel("LaunchingHighSpeedShell", new ChaResource(0, 0), ChaResource.Null, "skill_LaunchingHighSpeedShell",null,0.1f)},
                //暴雨弹幕
                { "HeavyRainBarrage",new SkillModel("HeavyRainBarrage",new ChaResource(0,50),new ChaResource(0,50),"skill_HeavyRainBarrage",null,2f) },
                //心火领域
                { "HeartFlameField", new SkillModel("HeartFlameField", new ChaResource(0, 2), ChaResource.Null, "skill_HeartFlameField",null,2f)},
                //减速领域
                { "DecelerationField", new SkillModel("DecelerationField", new ChaResource(0, 2), ChaResource.Null, "skill_DecelerationField",null,2f)},
                //以下为20231215大范围重构前的技能
                {"fire", new SkillModel("fire", new ChaResource(0, 10), ChaResource.Null, "skill_fire",null)}, //即使没有子弹也可以用这个技能，但是因为有buff会让他自动转向另一个reload的timeline
                {"spaceMonkeyBall", new SkillModel("spaceMonkeyBall", new ChaResource(0, 3), ChaResource.Null, "skill_spaceMonkeyBall", null)},
                {"grenade", new SkillModel("grenade", ChaResource.Null, ChaResource.Null, "skill_grenade", null)},
                {"explosiveBarrel", new SkillModel("explosiveBarrel", ChaResource.Null, ChaResource.Null, "skill_exploseBarrel", null)},
                {"homingMissle", new SkillModel("homingMissle", new ChaResource(0, 2), ChaResource.Null, "skill_followfire", null)},
                {"cloakBoomerang", new SkillModel("cloakBoomerang", ChaResource.Null, ChaResource.Null, "skill_cloakBoomerang", null)},

                {"roll", new SkillModel("roll", ChaResource.Null, ChaResource.Null, "skill_roll", null,0.5f)},
                {"collide", new SkillModel("collide", ChaResource.Null, ChaResource.Null, "skill_collide", null,1f)},
                {"dash", new SkillModel("dash", ChaResource.Null, ChaResource.Null, "skill_dash", null,1f)},
                {"flyingSword", new SkillModel("flyingSword", ChaResource.Null, ChaResource.Null, "skill_flyingSword", null)},
                {"ringTheBell", new SkillModel("ringTheBell", ChaResource.Null, ChaResource.Null, "skill_ringTheBell", null)},
                //旋转飞叶 高速旋转，在0-360度之间划分12个角度，每0.1秒发射一枚绿色子弹，持续3.6秒
                {"Spinning Leaves", new SkillModel("Spinning Leaves", ChaResource.Null, ChaResource.Null, "skill_SpinningLeaves", null,5f)},
                //斩击 在面前创造一个aoe 0.2f秒后移除 移除时造成伤害
                {"slash", new SkillModel("slash", ChaResource.Null, ChaResource.Null, "skill_slash", null)},
                //绿色斩击 在面前创造一个aoe 0.2f秒后移除 移除时造成伤害
                {"BambooSlash", new SkillModel("BambooSlash", ChaResource.Null, ChaResource.Null, "skill_BambooSlash", null,1f)},
                //翻天印
                {"The Overturning Seal", new SkillModel("The Overturning Seal", new ChaResource(0,10,0), new ChaResource(0,10,0), "skill_The Overturning Seal", null, 1.0f) },
                //青竹蜂云剑 控剑
                {"The Green Bamboo Wasp Cloud Sword", new SkillModel("The Green Bamboo Wasp Cloud Sword", new ChaResource(0,10,0), new ChaResource(0,10,0), "skill_The Green Bamboo Wasp Cloud Sword", null, 1f) },
                //越王勾践剑 控剑
                {"The legendary Sword of King Goujian", new SkillModel("The legendary Sword of King Goujian", new ChaResource(0,10,0), new ChaResource(0,10,0), "skill_The legendary Sword of King Goujian", null, 1f) },
                //骨冷灵火 火弹1
                {"The  Bone-chilling spiritual fire",new SkillModel("The  Bone-chilling spiritual fire",new ChaResource(0,2,0),new ChaResource(0,2,0),"skill_The  Bone-chilling spiritual fire",null,1f)  },
                //火焰飞弹
                //{"FireBall",new SkillModel("FireBall",new ChaResource(0,20,0),new ChaResource(0,20,0),"skill_fireball",null,0.80f,10)  },
                //随机飞弹
                {"FireRandomBall",new SkillModel("FireRandomBall",new ChaResource(0,20,0),new ChaResource(0,20,0),
                "skill_fireRandomBullet",null,0.80f,10)  },
                {"CreateThunderField",new SkillModel("CreateThunderField",new ChaResource(0,120,0),new ChaResource(0,20,0),
                "skill_CreateThunderField",null,0.10f)  },

                //毒雾飞弹
                {"FirePoisonFogBullet",new SkillModel("FirePoisonFogBullet",new ChaResource(0,20,0),new ChaResource(0,20,0),
                "skill_FirePoisonFogBullet",null,0.80f)  },
                //机关枪飞弹
                {"FireMachineGunBullet",new SkillModel("FireMachineGunBullet",new ChaResource(0,50,0),new ChaResource(0,50,0),
                "skill_MachineGunBullet",null,0.10f,10)  },
                //星星飞弹
                {"FireStarBullet",new SkillModel("FireStarBullet",new ChaResource(0,50,0),new ChaResource(0,50,0),
                "skill_FireStarBullet",null,0.10f,10)  },
                //skill_LightningOrb
                {"FireLightningOrb",new SkillModel("skill_LightningOrb",new ChaResource(0,50,0),new ChaResource(0,50,0),
                "skill_LightningOrb",null,0.10f)  },
                //龙拳
                {"DragonPunch",new SkillModel("skill_DragonPunch",new ChaResource(0,50,0),new ChaResource(0,50,0),
                "skill_DragonPunch",null,0.10f)  },

                //冰锥术
                {"Ice Spike spell",new SkillModel("Ice Spike spell",new ChaResource(0,20,0),new ChaResource(0,20,0),"skill_IceSpikeSpell",null,0.5f)  },
                //带指示器的冰锥术
                {"IceSpikespellWithIndicator",new SkillModel("IceSpikespellWithIndicator",new ChaResource(0,2,0),new ChaResource(0,2,0),"skill_IceSpikeSpellWithIndicator",null,3f)  },
                //冰锥连发
                {"Ice Spike Barrage",new SkillModel("Ice Spike Barrage",new ChaResource(0,20,0),new ChaResource(0,20,0),"skill_IceSpikeBarrage",null,2.5f)  },
                //冰锥爆裂
                {"Ice Spike Explosion",new SkillModel("Ice Spike Explosion",new ChaResource(0,30,0),new ChaResource(0,30,0),"skill_Ice Spike Explosion",null,10f)  },
                //召唤寒冰蟾
                {"SummoningIceToad",new SkillModel("SummoningIceToad",new ChaResource(0,10,0),new ChaResource(0,30,0),"skill_SummoningIceToad",null,10f )},
                {"Jump",new SkillModel("Jump",ChaResource.Null,ChaResource.Null,"Move_Jump" ,null,3f)},
                {"BoneShield",new SkillModel("BoneShield",ChaResource.Null,ChaResource.Null,"BoneShield" ,null,10f)},
                {"SkeletonSlam",new SkillModel("SkeletonSlam",ChaResource.Null,ChaResource.Null,"SkeletonSlam" ,null,1.5f)},
                {"SkeletonCollide",new SkillModel("SkeletonCollide",ChaResource.Null,ChaResource.Null,"SkeletonCollide" ,null,6f)},
                {"HeavyHammer",new SkillModel("HeavyHammer",ChaResource.Null,ChaResource.Null,"HeavyHammer" ,null,8f)},
                //召唤骷髅
                {"SummonSkeleton",new SkillModel("SummonSkeleton",new ChaResource(0,30,0,5),new ChaResource(0,30,0,5),"skill_SummonSkeleton",null,1f) },
                //召唤骷髅
                {"SummonSkeletons",new SkillModel("SummonSkeletons",new ChaResource(0,30,0,5),new ChaResource(0,30,0,5),"skill_SpawnSkeletons",null,1f) },
                //召唤霜灵
                {"SummonIceGhosts",new SkillModel("SummonIceGhosts",new ChaResource(0,30,0,5),new ChaResource(0,30,0,5),"skill_SpawnIceGhosts",null,1f) },
                
                //隐身 进入隐身状态
                {"EnterInvisibilityState",new SkillModel("EnterInvisibilityState",new ChaResource(0,30,0,0),new ChaResource(0,30,0,0),"skill_EnterInvisibilityState",null,5f) }
            };
        }
    }
}