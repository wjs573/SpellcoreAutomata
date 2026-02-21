using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WJS
{
    public static class DataEnhancedEffect
    {
        public static Dictionary<string, EnhanceEffectEvent> data =
            new Dictionary<string, EnhanceEffectEvent>
            {
                {"AddMoveSpeed", AddMoveSpeed},
                {"AddPenetrationCount",AddPenetrationCount },
                {"DeflectionOnHit",DeflectionOnHit },
                {"ScattershotFrenzy",ScattershotFrenzy },
                {"AutoTargeting",AutoTargeting },
                {"RevolutionAroundCaster",RevolutionAroundCaster },
                {"MouseGuided",MouseGuided },
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
                    value.tween = BulletScripts.bulletTween["FollowingMouse"];
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
                    value.tween = BulletScripts.bulletTween["SpeedUpFollowingTarget"];
                    value.targetFunc = BulletScripts.targettingFunc["GetNearestEnemy"];
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
                    value.tween = BulletScripts.bulletTween["RevolutionAroundCaster"];
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
                    value.model.onHit.AddListener(BulletScripts.onHitFunc["BulletDeflectionOnHit"]);
                    return value;
                });
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
}