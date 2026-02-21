using UnityEngine;
using WJS;

/// <summary>
/// 背包系统测试工具
/// 用于快速测试背包功能
/// </summary>
public class UIBackpackTester : MonoBehaviour
{
    [Header("测试设置")]
    public bool runTestsOnStart = false;
    public KeyCode openBackpackKey = KeyCode.B;
    public KeyCode addTestItemKey = KeyCode.Insert;
    public KeyCode removeTestItemKey = KeyCode.Delete;
    
    [Header("测试装备ID")]
    public string[] testEquipmentIds = new string[] { "星辉棒" };

    private UnitBackpack backpack;
    private UIBackpackWindow backpackWindow;

    private void Start()
    {
        if (runTestsOnStart)
        {
            Invoke(nameof(RunTests), 1f);
        }
    }

    private void Update()
    {
        // 打开背包
        if (Input.GetKeyDown(openBackpackKey))
        {
            ToggleBackpack();
        }
        
        // 添加测试物品
        if (Input.GetKeyDown(addTestItemKey))
        {
            AddTestItems();
        }
        
        // 移除所有物品
        if (Input.GetKeyDown(removeTestItemKey))
        {
            ClearBackpack();
        }
    }

    /// <summary>
    /// 切换背包显示
    /// </summary>
    private void ToggleBackpack()
    {
        if (backpackWindow == null)
        {
            backpackWindow = UIManager.Instance?.GetWindow<UIBackpackWindow>();
        }
        
        if (backpackWindow != null)
        {
            if (backpackWindow.visibleState)
            {
                backpackWindow.Close();
            }
            else
            {
                backpackWindow.Open();
            }
        }
        else
        {
            Debug.LogWarning("找不到UIBackpackWindow，请确保场景中已创建");
        }
    }

    /// <summary>
    /// 添加测试物品到背包
    /// </summary>
    [ContextMenu("添加测试物品")]
    public void AddTestItems()
    {
        if (backpack == null)
        {
            if (GameManager.Instance?.mainCharacter != null)
            {
                backpack = GameManager.Instance.mainCharacter.GetComponent<UnitBackpack>();
            }
        }
        
        if (backpack == null)
        {
            Debug.LogWarning("找不到UnitBackpack组件");
            return;
        }
        
        // 确保EquipmentData已初始化
        if (EquipmentData.data == null || EquipmentData.data.Count == 0)
        {
            EquipmentData.Initialize();
        }
        
        // 添加测试装备
        foreach (string id in testEquipmentIds)
        {
            if (EquipmentData.data.ContainsKey(id))
            {
                EquipmentModel model = EquipmentData.data[id];
                EquipmentObj equip = new EquipmentObj(model);
                backpack.AddItem(equip);
                Debug.Log($"添加装备: {model.name}");
            }
            else
            {
                Debug.LogWarning($"找不到装备ID: {id}");
            }
        }
        
        // 创建一些随机装备
        CreateRandomEquipments(3);
    }

    /// <summary>
    /// 创建随机测试装备
    /// </summary>
    private void CreateRandomEquipments(int count)
    {
        string[] itemNames = { "铁剑", "布甲", "皮盔", "戒指" };
        EquipmentType[] types = { EquipmentType.weapon, EquipmentType.armor, EquipmentType.helm, EquipmentType.trinket };
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, itemNames.Length);
            string itemName = itemNames[randomIndex] + $"_{Random.Range(1, 100)}";
            
            EquipmentModel model = new EquipmentModel(
                id: $"test_{i}_{System.Guid.NewGuid().ToString().Substring(0, 4)}",
                icon: "DefaultItem",
                name: itemName,
                tags: new string[] { "equipment" },
                equipment: new ChaProperty(),
                buffs: new AddBuffInfo[0],
                slot: types[randomIndex]
            );
            
            EquipmentObj equip = new EquipmentObj(model);
            backpack.AddItem(equip);
            Debug.Log($"创建随机装备: {model.name}, 类型: {model.type}");
        }
    }

    /// <summary>
    /// 清空背包
    /// </summary>
    [ContextMenu("清空背包")]
    public void ClearBackpack()
    {
        if (backpack == null)
        {
            if (GameManager.Instance?.mainCharacter != null)
            {
                backpack = GameManager.Instance.mainCharacter.GetComponent<UnitBackpack>();
            }
        }
        
        if (backpack == null) return;
        
        // 卸载所有装备
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            backpack.UnequipEquipment(type);
        }
        
        // 注意：当前实现没有提供清空背包的方法，这里只是刷新UI
        Debug.Log("已卸载所有装备");
    }

    /// <summary>
    /// 运行完整测试
    /// </summary>
    [ContextMenu("运行完整测试")]
    public void RunTests()
    {
        Debug.Log("===== 开始背包系统测试 =====");
        
        // 测试1: 添加物品
        Debug.Log("测试1: 添加物品到背包");
        AddTestItems();
        
        // 测试2: 打开背包
        Debug.Log("测试2: 打开背包窗口");
        ToggleBackpack();
        
        Debug.Log("===== 背包系统测试完成 =====");
    }
}
