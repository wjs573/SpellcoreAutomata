using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JinShan
{
    [CreateAssetMenu(fileName = "New EnhancedEffect Object", menuName = "Inventory System/Items/EnhancedEffect")]
    public class EnhancedEffectObject : ItemObject
    {
        private void Awake()
        {
            type = ItemType.技能强化;
            DrawCount = 1;
        }

        /// <summary>
        /// 影响基础效果类法术的范围
        /// 使用正数表示后方第一个基础效果类法术，负数表示前方。
        /// </summary>
        public int rangeModifier = 1;
        public float delayTimeModifier = 0f;
        public float chargeTimeModifier = 0f;
        /// <summary>
        /// 技能修正效果的清单
        /// </summary>
        public List<string> skillModifiers;
    }
}