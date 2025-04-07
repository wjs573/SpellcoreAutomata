using System.Collections;
using JinShan;
using UnityEngine;

/// <summary>
/// 玩家操作的控件，理论上它只能被加在“主角身上”。
/// 但如果我们有类似wow牧师的精神控制之类的技能、或者是控制多个分身同步行动的，就需要给多目标添加了
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraAngle = 0f; // 设定相机的偏置角度

    private ChaState chaState;
    public MainCharacter mainCharacter;

    private float scrollAccumulator = 0f;
    public float scrollThreshold = 0.001f; // 滚轮切换阈值

    private void Start()
    {
        chaState = GetComponent<ChaState>();
        mainCharacter = MainCharacter.Instance;

        mainCharacter.FaBao_Equippment_Inventory.OnEquip += EquipWand;
    }

    public void EquipWand(int index)
    {
        InventorySlot inventorySlot = mainCharacter.FaBao_Equippment_Inventory.GetSlots[index];
        if (inventorySlot.item == null || inventorySlot.item.Id == "" || inventorySlot.amount == 0)
        {
            return;
        }
        GetComponent<SpellCombinationManagerContainer>().SwitchManager((FaBao)inventorySlot.ItemObject);
    }

    private void FixedUpdate()
    {
        if (!chaState || chaState.dead) return;

        // 获取输入
        float ix = Input.GetAxis("Horizontal");
        float iz = Input.GetAxis("Vertical");

        // 计算相机的旋转角度
        float angleRad = cameraAngle * Mathf.Deg2Rad;
        float cosAngle = Mathf.Cos(angleRad);
        float sinAngle = Mathf.Sin(angleRad);

        // 计算实际移动方向
        float moveX = ix * cosAngle - iz * sinAngle;
        float moveZ = ix * sinAngle + iz * cosAngle;
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ) * chaState.moveSpeed;

        // 处理角色旋转
        if (mainCamera)
        {
            Vector2 cursorPos = Input.mousePosition;
            Vector2 mScreenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, transform.position);
            float rotateTo = Mathf.Atan2(cursorPos.x - mScreenPos.x, cursorPos.y - mScreenPos.y) * Mathf.Rad2Deg;
            chaState.OrderRotateTo(rotateTo);
        }

        // 执行移动
        if (moveDirection != Vector3.zero)
        {
            chaState.OrderMove(moveDirection);
        }

        // 处理技能按键
        string[] skillId = GetSkillsFromInventory();
        bool[] sBtn = new bool[]{
            Input.GetButton("num3"),
            Input.GetButton("num2"),
            Input.GetButton("num1"),
            Input.GetButton("Jump")
        };

        bool btnHolding = false;
        for (int i = 0; i < sBtn.Length; i++)
        {
            if (sBtn[i])
            {
                chaState.CastSkill(skillId[i]);
                btnHolding = true;
            }
        }
        chaState.charging = btnHolding;

        // 临时代码：释放组合法术
        if (Input.GetButton("num1"))
        {
            chaState.GetComponent<SpellCombinationManagerContainer>().UseCurrentWand();
        }

        // 处理滚轮切换武器的功能
        HandleScrollWheel();
    }

    /// <summary>
    /// 从技能装备栏（实际上是一个仓库）中获取技能id
    /// </summary>
    /// <returns></returns>
    private string[] GetSkillsFromInventory()
    {
        string[] skillId = new string[4];
        InventorySlot[] skillSlots = mainCharacter.Equipped_Skill_Inventory.Container.Slots;
        for (int i = 0; i < 4; i++)
        {
            if (skillSlots[i].item != null && skillSlots[i].item.Id == "")
            {
                skillId[i] = skillSlots[i].item.GetSkillModel().id;
            }
        }
        return skillId;
    }

    /// <summary>
    /// 处理滚轮切换武器的功能
    /// </summary>
    private void HandleScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            scrollAccumulator += scroll;

            if (scrollAccumulator >= scrollThreshold)
            {
                mainCharacter.FaBao_Equippment_Inventory.EquipNext();
                scrollAccumulator = 0f; // 重置累积变量
            }
            else if (scrollAccumulator <= -scrollThreshold)
            {
                mainCharacter.FaBao_Equippment_Inventory.EquipPrevious();
                scrollAccumulator = 0f; // 重置累积变量
            }
        }
    }
}
