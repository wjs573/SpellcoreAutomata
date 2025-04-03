using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JinShan
{
    [CreateAssetMenu(fileName = "New Trigger Object", menuName = "Inventory System/Items/Trigger")]
    public class TriggerObject : ItemObject
    {
        private void Awake()
        {
            type = ItemType.触发器;
            DrawCount = 1;
        }
        /// <summary>
        /// 触发条件类型
        /// </summary>
        public TriggerType condition = TriggerType.OnHit;
        /// <summary>
        /// 触发条件逻辑
        /// </summary>
        public string EventTriggerCondition;
        /// <summary>
        /// 触发条件参数
        /// </summary>
        public object[] EventTriggerConditionParams;

        [Header("修正属性")]
        public float delayTimeModifier = 0f;
        public float chargeTimeModifier = 0f;
    }

    public enum TriggerType
    {
        OnHit,
        OnKill
    };
}
