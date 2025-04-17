using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSimpleAI : MonoBehaviour
{
    public MainCharacter mainCharacter;
    ChaState chaState;

    private void Start()
    {
        chaState = GetComponent<ChaState>();
    }

    private void FixedUpdate()
    {
        mainCharacter = MainCharacter.Instance;
        if (mainCharacter == null) return;
        // 计算目标方向（XZ平面）
        Vector3 direction = mainCharacter.transform.position - transform.position;
        direction.y = 0; // 消除垂直分量

        // 归一化方向（确保移动速度稳定）
        if (direction != Vector3.zero)
        {
            direction.Normalize();
        }
        else
        {
            direction = Vector3.forward; // 默认方向避免零向量
        }
        chaState.RotateToTarget(mainCharacter.gameObject);
        chaState.OrderMove(direction * chaState.property.moveSpeed/50);
    }
}
