using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///这是一个装视觉物件的容器
    ///</summary>
    public class ViewContainer : MonoBehaviour
    {

        /// <summary>
        /// 委托，用于动态获取技能的范围参数
        /// </summary>
        public Func<float> GetSkillSize;
        private void FixedUpdate()
        {
            // 如果绑定了获取范围的委托，动态同步美术大小
            if (GetSkillSize != null)
            {
                float size = GetSkillSize.Invoke();
            }
        }
    }
}
