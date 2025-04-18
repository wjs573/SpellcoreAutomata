using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSimpleAI : MonoBehaviour
{
    public MainCharacter mainCharacter;
    ChaState chaState;
    
    [Header("Debug Settings")]
    [Tooltip("是否显示移动方向Gizmos")]
    public bool showMovementGizmos = true;
    [Tooltip("方向线长度")]
    public float gizmoLineLength = 2f;
    [Tooltip("方向线颜色")]
    public Color gizmoColor = Color.cyan;

    private Vector3 currentDirection;

    private void Start()
    {
        chaState = GetComponent<ChaState>();
    }

    private void FixedUpdate()
    {
        mainCharacter = MainCharacter.Instance;
        if (mainCharacter == null) return;
        
        // 计算目标方向（XZ平面）
        currentDirection = mainCharacter.transform.position - transform.position;
        currentDirection.y = 0; // 消除垂直分量

        // 归一化方向（确保移动速度稳定）
        if (currentDirection != Vector3.zero)
        {
            currentDirection.Normalize();
        }
        else
        {
            currentDirection = Vector3.forward; // 默认方向避免零向量
        }
        
        chaState.RotateToTarget(mainCharacter.gameObject);
        chaState.OrderMove(currentDirection * chaState.property.moveSpeed/50);
    }

    private void OnDrawGizmos()
    {
        if (!showMovementGizmos) return;
        
        // 绘制移动方向线
        Gizmos.color = gizmoColor;
        Vector3 startPos = transform.position + Vector3.up * 0.1f; // 稍微抬高起点避免与地面重叠
        Gizmos.DrawLine(startPos, startPos + currentDirection * gizmoLineLength);
        
        // 在终点绘制箭头
        float arrowSize = 0.3f;
        Vector3 rightArrow = Quaternion.LookRotation(currentDirection) * Quaternion.Euler(0, 135, 0) * Vector3.forward;
        Vector3 leftArrow = Quaternion.LookRotation(currentDirection) * Quaternion.Euler(0, -135, 0) * Vector3.forward;
        Gizmos.DrawLine(startPos + currentDirection * gizmoLineLength, 
                       startPos + currentDirection * (gizmoLineLength - arrowSize) + rightArrow * arrowSize);
        Gizmos.DrawLine(startPos + currentDirection * gizmoLineLength, 
                       startPos + currentDirection * (gizmoLineLength - arrowSize) + leftArrow * arrowSize);
        
        // 绘制当前速度文本
        #if UNITY_EDITOR
        if (chaState != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, 
                                    $"Speed: {chaState.property.moveSpeed/50:F2}\nDir: {currentDirection}");
        }
        #endif
    }
}