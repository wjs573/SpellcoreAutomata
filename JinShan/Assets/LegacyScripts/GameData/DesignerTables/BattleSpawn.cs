using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignerTables
{
    public class BattleSpawn
    {
        public static Dictionary<string, BattleSpawnData> data = new Dictionary<string, BattleSpawnData>();

        public static void Initialize()
        {
            data = new Dictionary<string, BattleSpawnData>()
            {

                {"Train",new BattleSpawnData(
                    new Dictionary<string, MobSpawnInfo>()
                    {
                    }, 0.1f) },

                {"Level2",new BattleSpawnData(
                    new Dictionary<string, MobSpawnInfo>()
                    {
                        { "Skeleton",new MobSpawnInfo(
                             "Skeleton",
                             5,
                             "NoCondition",null,
                             "RandomPosition",null,
                             8,1000)
                        },
                        { "IceGhost",new MobSpawnInfo(
                             "IceGhost",
                             2,
                             "NoCondition",null,
                             "RandomPosition",null,
                             4,100)
                        },
                        { "SkeletonGiant",new MobSpawnInfo(
                             "SkeletonGiant",
                             1,
                             "SpawnWithProbability",new object[]{1f },
                             "RandomPosition",null,
                             2,100)
                        }
                    }, 2f) },


                {"Level1",new BattleSpawnData(
                    new Dictionary<string, MobSpawnInfo>()
                    {
                         { "SkeletonGiant",new MobSpawnInfo("SkeletonGiant",
                         1,
                         "NoCondition",null,
                         "RandomPosition",null,
                         1,10)},

                        { "FireSpirit",new MobSpawnInfo("FireSpirit",
                        2,
                        "NoCondition",null,
                        "RandomPosition",null,
                        4,50)},

                         { "ParasiticRat",new MobSpawnInfo("ParasiticRat",
                         2,
                         "NoCondition",null,
                         "RandomPosition",null,
                         3,50)},

                        { "IceToad",new MobSpawnInfo("IceToad",
                        4,
                        "SpawnAfterSeconds",new object[]{ 5f},
                        "RandomPosition",null,
                        3,50)},

                         { "IceToadKing",new MobSpawnInfo("IceToadKing",
                         1,
                         "SpawnWithKilledMonster",new object[]{ "IceToad",10},
                         "RandomPosition",null,
                         1,2)},

                         { "BambooMonster",new MobSpawnInfo("BambooMonster",
                         1,
                         "SpawnWithProbability",new object[]{0.25f },
                         "RandomPosition",null,
                         1,10)}
                    }, 1f) }

            };
        }
    }

}