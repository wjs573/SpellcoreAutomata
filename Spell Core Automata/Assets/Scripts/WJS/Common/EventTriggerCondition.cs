using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    /// <summary>
    /// 触发条件委托
    /// 返回值是
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate bool EventTriggerCondition(CallBackParams callBackParams, object[] conditionParams);

    public class DataEventTriggerCondition
    {
        public static Dictionary<string, EventTriggerCondition> data = new Dictionary<string, EventTriggerCondition>()
    {
        { "HitPoisonedEnemy",HitPoisonedEnemy },
        { "ProbabilityTrigger",ProbabilityTrigger }
    };

        /// <summary>
        /// 概率触发
        /// </summary>
        /// <param name="callBackParams"></param>
        /// <param name="conditionParams"></param>
        /// <returns></returns>
        private static bool ProbabilityTrigger(CallBackParams callBackParams, object[] conditionParams)
        {
            float Probability = conditionParams.Length > 0 ? (float)conditionParams[0] : 0f;
            return UnityEngine.Random.Range(0, 1f) < Probability;
        }

        /// <summary>
        /// 如果命中的单位携带有中毒buff则返回true
        /// </summary>
        /// <param name="callBackParams"></param>
        /// <returns></returns>
        public static bool HitPoisonedEnemy(CallBackParams callBackParams, object[] conditionParams)
        {
            List<GameObject> enemies = callBackParams.GetTargets();
            if (enemies.Count > 0)
            {
                foreach (GameObject cha in enemies)
                {
                    ChaState chaState = cha.GetComponent<ChaState>();
                    if (chaState != null && chaState.HasBuff("Poisoning"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}