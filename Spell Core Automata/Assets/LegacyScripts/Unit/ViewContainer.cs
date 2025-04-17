using System;
using UnityEngine;


///<summary>
///这是一个装视觉物件的容器
///</summary>
public class ViewContainer : MonoBehaviour
{
    
    /// <summary>
    /// 委托，用于动态获取技能的范围参数
    /// </summary>
    public Func<float> GetSkillSize;
    private void Update()
    {
        // 如果绑定了获取范围的委托，动态同步美术大小
        if (GetSkillSize != null)
        {
            float size = GetSkillSize.Invoke();
            SyncViewSize(size);
        }
    }

    private void SyncViewSize(float size)
    {
        Vector3 currentScale = transform.GetChild(0).localScale; // 获取当前缩放值
        Vector3 originalPosition = transform.GetChild(0).localPosition; // 获取当前位置
        // 计算新的位置，保持中心不变
        float offsetY = (currentScale.y - size); // Y 轴调整，保持中心位置
        transform.GetChild(0).localScale = new Vector3(size, size, size);
        transform.GetChild(0).localPosition = new Vector3(0, originalPosition.y + 2*offsetY, 0);
    }

}