using UnityEngine;
using UnityEngine.InputSystem;
using WJS;

public class UnitSkillTester : MonoBehaviour
{
    public ChaState chaState;
    public string skillId = "FireBall";

    [SerializeField] private InputActionAsset inputActions;
    private InputAction fireAction;
    private bool isHoldingFire; // 标记是否按住

    private void Awake()
    {
        chaState = GetComponent<ChaState>();
        fireAction = inputActions.FindAction("Fire");
        
        // 绑定事件
        fireAction.started += ctx => isHoldingFire = true; // 按下时标记
        fireAction.canceled += ctx => isHoldingFire = false; // 松开时取消
    }

    private void OnEnable() => fireAction?.Enable();
    private void OnDisable() => fireAction?.Disable();

    private void FixedUpdate()
    {
        if (isHoldingFire)
        {
            Shoot(); // 按住时每帧触发
        }
    }

    public void LearnSkill()
    {
        if (chaState == null)
        {
            Debug.LogError("ChaState is not initialized!");
            return;
        }
        chaState.InitBaseProp(new ChaProperty(100, 0, 100, 100, 10, 100, 10));
        chaState.LearnSkill(SkillData.data[skillId]);
    }

    void Shoot()
    {
        chaState.CastSkill(skillId);
    }
}