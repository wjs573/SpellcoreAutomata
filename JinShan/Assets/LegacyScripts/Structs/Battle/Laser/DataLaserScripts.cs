using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;

namespace DesignerScripts
{
    public class DataLaserScripts
    {
        //Create
        public static Dictionary<string, LaserOnCreate> onCreateFunc = new Dictionary<string, LaserOnCreate>()
        {
        };
        //Hit
        public static Dictionary<string, LaserOnHit> onHitFunc = new Dictionary<string, LaserOnHit>()
        {
            {"CommonLaserHit",CommonLaserHit },
            {"IncreaseDamageOverTimeLaserHit",IncreaseDamageOverTimeLaserHit },
            {"AddColdBuffLaserHit", AddColdBuffLaserHit},
            {"AddPoisonBuffLaserHit" ,AddPoisonBuffLaserHit},
            {"KnockbackLaserHit",KnockbackLaserHit}
        };


        //Removed
        public static Dictionary<string, LaserOnRemoved> onRemovedFunc = new Dictionary<string, LaserOnRemoved>()
        {
        };

        /// <summary>
        /// 通用激光命中函数，造成伤害
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="target"></param>
        private static void CommonLaserHit(GameObject laser, GameObject target)
        {
            LaserState laserState = laser.GetComponent<LaserState>();
            if (!laserState) return;
            object[] onHitParam = laserState.model.onHitParams;

            float attackMultiplier = onHitParam.Length > 0 ? (float)onHitParam[0] : 0f;
            ChaProperty attackerChaProperty = laserState.propWhileCast;
            if (laserState.caster != null && laserState.caster.GetComponent<ChaState>() != null)
            {
                ChaState chaState = laserState.caster.GetComponent<ChaState>();
                attackerChaProperty = chaState.property;
            }

            SceneVariants.CreateDamage(laserState.caster, target,
                new Damage(10 + (int)(attackerChaProperty.attack * attackMultiplier)), 0f, 0.25f,
                new DamageInfoTag[] { DamageInfoTag.directDamage });
        }

        private static void IncreaseDamageOverTimeLaserHit(GameObject laser, GameObject target)
        {
            LaserState laserState = laser.GetComponent<LaserState>();
            if (!laserState) return;
            object[] onHitParam = laserState.model.onHitParams;
            float timeMultiplier = onHitParam.Length > 0 ? (float)onHitParam[0] : 0f;
            SceneVariants.CreateDamage(laserState.caster, target,
                new Damage(50 + (int)(timeMultiplier * laserState.timeElapsed)), 0f, 0.25f,
                new DamageInfoTag[] { DamageInfoTag.directDamage });
        }

        /// <summary>
        /// 造成伤害并施加寒冷buff
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="target"></param>
        private static void AddColdBuffLaserHit(GameObject laser, GameObject target)
        {
            LaserState laserState = laser.GetComponent<LaserState>();
            AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["Cold"], laserState.caster, target, 1, 3f);
            ChaState targetChaState = target.GetComponent<ChaState>();
            targetChaState.AddBuff(addBuffInfo);
            CommonLaserHit(laser, target);
        }

        /// <summary>
        /// 施加中毒buff
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="target"></param>
        private static void AddPoisonBuffLaserHit(GameObject laser, GameObject target)
        {
            LaserState laserState = laser.GetComponent<LaserState>();
            AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["Poisoning"], laserState.caster, target, 1, 999f);
            ChaState targetChaState = target.GetComponent<ChaState>();
            targetChaState.AddBuff(addBuffInfo);
        }

        private static void KnockbackLaserHit(GameObject laser, GameObject target)
        {
            ChaState targetChaState = target.GetComponent<ChaState>();
            // 获取GameObject的旋转作为四元数
            Quaternion rotation = laser.transform.rotation;

            // 将四元数转换为前方向量
            Vector3 forwardVector = rotation * Vector3.forward;
            MovePreorder movePreorder = new MovePreorder(forwardVector, 0.1f);
            targetChaState.AddForceMove(movePreorder);
        }
    }
}

