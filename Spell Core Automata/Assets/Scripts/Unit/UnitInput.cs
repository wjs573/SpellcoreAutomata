using UnityEngine;
using UnityEngine.InputSystem;

public class UnitInput : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionReference fireActionRef; // 在Inspector中分配
    
    [Header("Character Settings")]
    private ChaState chaState;

    private void Awake()
    {
        // 初始化组件引用
        chaState = GetComponent<ChaState>();
        if (chaState == null)
        {
            Debug.LogError("ChaState component not found!", this);
        }
    }

    private void OnEnable()
    {
        // 确保所有引用有效
        if (fireActionRef != null && fireActionRef.action != null && chaState != null)
        {
        
            // 启用并订阅输入
            fireActionRef.action.Enable();
            fireActionRef.action.performed += OnFireActionPerformed;
        }
        else
        {
            Debug.LogError("Input setup failed! Check fireActionRef and chaState assignments.", this);
        }
    }

    private void OnDisable()
    {
        // 安全取消订阅和禁用
        if (fireActionRef != null && fireActionRef.action != null)
        {
            fireActionRef.action.performed -= OnFireActionPerformed;
            fireActionRef.action.Disable();
        }
    }

    // 注意：方法名和签名必须完全匹配
    private void OnFireActionPerformed(InputAction.CallbackContext context)
    {
        // 添加安全检查
        if (chaState != null)
        {
            chaState.CastSkill("FireBall");
        }
    }
}