using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignerTables
{
    public static class DataEnhancedEffect
    {
        public static Dictionary<string, EnhanceEffectEvent> data =
            new Dictionary<string, EnhanceEffectEvent>
            {
                {"AddMoveSpeed", AddMoveSpeed},
                {"PoisonOnHit", PoisonOnHit},
                {"TripleLaunch", TripleLaunch},
                {"SecondLaunch",SecondLaunch },
                {"AddPenetrationCount",AddPenetrationCount },
                {"DeflectionOnHit",DeflectionOnHit },
                {"ScattershotFrenzy",ScattershotFrenzy },
                {"AutoTargeting",AutoTargeting },
                {"RevolutionAroundCaster",RevolutionAroundCaster },
                {"MouseGuided",MouseGuided },
                {"AddRelayDebuffOnHit",AddRelayDebuffOnHit },
                {"RadiusBoost",RadiusBoost },
            };

        private static SkillModel RadiusBoost(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();
            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.model.radius =  1.30f * value.model.sightEffectRadius;
                    return value;
                });
                ModifyParameterOfType<AoeLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (AoeLauncher value) =>
                {
                    value.radius =  1.30f * value.radius;
                    return value;
                });
            }
            return modifiedSkillModel;
        }

        private static SkillModel MouseGuided(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();
            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.tween = DesignerScripts.Bullet.bulletTween["FollowingMouse"];
                    return value;
                });
            }
            return modifiedSkillModel;
        }

        private static SkillModel AutoTargeting(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();
            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.tween = DesignerScripts.Bullet.bulletTween["SpeedUpFollowingTarget"];
                    value.targetFunc = DesignerScripts.Bullet.targettingFunc["GetNearestEnemy"];
                    return value;
                });
            }
            return modifiedSkillModel;
        }

        private static SkillModel RevolutionAroundCaster(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();
            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.tween = DesignerScripts.Bullet.bulletTween["RevolutionAroundCaster"];
                    return value;
                });
            }
            return modifiedSkillModel;
        }


        /// <summary>
        /// 弹幕狂潮
        /// 提升射击速度和子弹飞行速度，但子弹的散射度大幅增加，造成射击难以准确。
        /// </summary>
        /// <param name="skillModel"></param>
        /// <returns></returns>
        private static SkillModel ScattershotFrenzy(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();
            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.speed += 10f;
                    value.scatteringDegree += 360f;
                    return value;
                });
            }
            return modifiedSkillModel;
        }

        private static SkillModel AddPenetrationCount(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.model.hitTimes += 1;
                    if (value.model.sameTargetDelay <= 0.2f)
                    {
                        value.model.sameTargetDelay = 0.2f;
                    }
                    return value;
                });
            }

            return modifiedSkillModel;
        }

        private static SkillModel AddMoveSpeed(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.speed += 10f;
                    return value;
                });
            }

            return modifiedSkillModel;
        }

        private static SkillModel DeflectionOnHit(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.model.hitTimes += 2;
                    value.model.onHit.AddListener(DesignerScripts.Bullet.onHitFunc["BulletDeflectionOnHit"]);
                    return value;
                });
            }

            return modifiedSkillModel;
        }

        private static SkillModel PoisonOnHit(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.model.onHit.AddListener(DesignerScripts.Bullet.onHitFunc["AddPoisonOnHit"]);
                    return value;
                });
                ModifyParameterOfType<CharacterSpawnInfo>(modifiedSkillModel.effect.nodes[i].eveParams, (CharacterSpawnInfo value) =>
                {
                    value.GetSkillModels();
                    List<SkillModel> skills = new List<SkillModel>();
                    for (int j = 0; j < value.skills.Count; j++)
                    {
                        SkillModel modifiedCharacterSkillModel = value.skills[j].Clone();
                        for (int k = 0; k < modifiedCharacterSkillModel.effect.nodes.Length; k++)
                        {
                            ModifyParameterOfType<BulletLauncher>(modifiedCharacterSkillModel.effect.nodes[k].eveParams, (BulletLauncher bulletLauncher) =>
                            {
                                bulletLauncher.model.onHit.AddListener(DesignerScripts.Bullet.onHitFunc["AddPoisonOnHit"]);
                                return bulletLauncher;
                            });
                        }
                        skills.Add(modifiedCharacterSkillModel);
                    }
                    value.skills = skills; // 确保将修改后的技能列表重新赋值回CharacterSpawnInfo
                    return value;
                });
            }
            return modifiedSkillModel;
        }

        private static SkillModel AddRelayDebuffOnHit(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    value.model.onHit.AddListener(DesignerScripts.Bullet.onHitFunc["AddDeathRelayBuffOnHit"]);
                    return value;
                });
                ModifyParameterOfType<CharacterSpawnInfo>(modifiedSkillModel.effect.nodes[i].eveParams, (CharacterSpawnInfo value) =>
                {
                    value.GetSkillModels();
                    List<SkillModel> skills = new List<SkillModel>();
                    for (int j = 0; j < value.skills.Count; j++)
                    {
                        SkillModel modifiedCharacterSkillModel = value.skills[j].Clone();
                        for (int k = 0; k < modifiedCharacterSkillModel.effect.nodes.Length; k++)
                        {
                            ModifyParameterOfType<BulletLauncher>(modifiedCharacterSkillModel.effect.nodes[k].eveParams, (BulletLauncher bulletLauncher) =>
                            {
                                bulletLauncher.model.onHit.AddListener(DesignerScripts.Bullet.onHitFunc["AddDeathRelayBuffOnHit"]);
                                return bulletLauncher;
                            });
                        }
                        skills.Add(modifiedCharacterSkillModel);
                    }
                    value.skills = skills; // 确保将修改后的技能列表重新赋值回CharacterSpawnInfo
                    return value;
                });
            }
            return modifiedSkillModel;
        }

        private static SkillModel TripleLaunch(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                ModifyParameterOfType<BulletLauncher>(modifiedSkillModel.effect.nodes[i].eveParams, (BulletLauncher value) =>
                {
                    BulletLauncher bulletLauncher = value.Clone();
                    bulletLauncher.shotCount = value.shotCount * 3;
                    bulletLauncher.spreadAngle = value.spreadAngle + 30f;
                    return bulletLauncher;
                });
                ModifyParameterOfType<CharacterSpawnInfo>(modifiedSkillModel.effect.nodes[i].eveParams, (CharacterSpawnInfo value) =>
                {
                    value.GetSkillModels();
                    value.count *= 3;
                    return value;
                });
            }

            return modifiedSkillModel;
        }

        private static SkillModel SecondLaunch(SkillModel skillModel)
        {
            SkillModel modifiedSkillModel = skillModel.Clone();

            for (int i = 0; i < modifiedSkillModel.effect.nodes.Length; i++)
            {
                TimelineNode node = modifiedSkillModel.effect.nodes[i];
                if (ContainsParameterOfType<BulletLauncher>(node.eveParams) || ContainsParameterOfType<CharacterSpawnInfo>(node.eveParams))
                {
                    modifiedSkillModel.effect.IsContainsLoop = true;
                    modifiedSkillModel.effect.nodes[i].loopTimes += 1;
                    modifiedSkillModel.effect.nodes[i].loopIntervalTime += 0.2f;
                    modifiedSkillModel.effect.ExtendTimelineIfNeeded();
                }
            }
            return modifiedSkillModel;
        }

        public static void ModifyParameterOfType<T>(object[] eveParams, Func<T, T> modifier)
        {
            // 动态生成索引，只对当前的 eveParams 有效
            int[] indices = eveParams
                .Select((param, index) => (param, index))
                .Where(x => x.param is T)
                .Select(x => x.index)
                .ToArray();

            foreach (int index in indices)
            {
                if (eveParams[index] is T originalValue)
                {
                    eveParams[index] = modifier(originalValue);
                }
            }
        }
        public static bool ContainsParameterOfType<T>(object[] eveParams)
        {
            foreach (object param in eveParams)
            {
                if (param is T)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public delegate SkillModel EnhanceEffectEvent(SkillModel skillModel);
}
