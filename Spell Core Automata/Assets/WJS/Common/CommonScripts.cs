using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WJS
{
    public static class CommonScripts
    {
        /// <summary>
        /// 伤害公式
        /// </summary>
        /// <param name="damageInfo"></param>
        /// <param name="asHeal"></param>
        /// <returns></returns>
        public static DamageInfo DamageValue(DamageInfo damageInfo, bool asHeal)
        {
            return damageInfo;
        }

        /// <summary>
        /// 修改参数
        /// </summary>
        /// <param name="eveParams"></param>
        /// <param name="modifier"></param>
        /// <typeparam name="T"></typeparam>
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
    }
}

