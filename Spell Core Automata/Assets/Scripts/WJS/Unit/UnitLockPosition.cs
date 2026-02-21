using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLockPosition : MonoBehaviour
{
    void LateUpdate()
    {
        transform.localPosition = Vector3.zero; // 在 LateUpdate 中设置，避免与其他逻辑冲突
    }
}
