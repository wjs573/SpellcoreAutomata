using System.Collections.Generic;
using JinShan;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 玩家数据
/// 游戏开始时创建，一个存档只有一个
/// 存档功能应该在这个类中
/// </summary>
public class MainCharacter : MonoSingleton<MainCharacter>
{
    /// <summary>
    /// 技能仓库
    /// </summary>
    public InventoryObject Skill_Inventory;
    /// <summary>
    /// 已装备技能仓库
    /// </summary>
    public InventoryObject Equipped_Skill_Inventory;
    /// <summary>
    /// 已装备技能仓库
    /// </summary>
    public InventoryObject Equipped_ComboSpell_Inventory;

    /// <summary>
    /// 法宝装备栏
    /// </summary>
    public InventoryObject FaBao_Equippment_Inventory;

    /// <summary>
    /// 主角背包
    /// </summary>
    public InventoryObject Main_inventory;

    /// <summary>
    /// 基础数据库
    /// </summary>
    public ItemDatabaseObject ItemDatabase;

    /// <summary>
    /// 角色姓名
    /// </summary>
    public string Name;

    /// <summary>
    /// 境界
    /// 例如 练气期·初期
    /// </summary>
    public Realm realm;

    /// <summary>
    /// 经验值
    /// </summary>
    public int exp;

    /// <summary>
    /// 晶石数量
    /// </summary>
    public int coin;

    /// <summary>
    /// 基础属性
    /// </summary>
    public ChaProperty BaseProperty;

    ///<summary>
    ///来自装备的属性
    ///</summary>
    public ChaProperty equipmentProp = ChaProperty.zero;

    ///<summary>
    ///角色当前的属性
    ///</summary>
    public ChaProperty property
    {
        get
        {
            UpdateEquipmentProp();
            return BaseProperty + equipmentProp;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Initialized();
    }

    /// <summary>
    /// 角色属性的初始化
    /// </summary>
    private void Initialized()
    {
        //练气初期起步
        realm = new Realm(RealmType.LianQi, StageType.ChuQi);
        exp = 0;
        //基础属性
        BaseProperty = new ChaProperty(
                100, 0, 100,
                1000, 10, 200, 20, 100,
                200, 100, 0,
                1.5f, 0.25f, 0.05f, 0.25f, 0.4f);
    }



    public void InitChaStateInBattle()
    {
        //给主角挂上装备栏仓库
        GameManager.Instance.mainActor.GetComponent<ChaState>().FaBao_Equippment_Inventory = FaBao_Equippment_Inventory;

        //给主角的背包添加一轮数据库的物品 省得我一次次手动添加
        for (int i = 0; i < ItemDatabase.ItemObjects.Length; i++)
        {
            if (Main_inventory.HasItemObject(ItemDatabase.ItemObjects[i]))
            {
                continue;
            }
            Debug.Log(ItemDatabase.ItemObjects[i].name);
            Main_inventory.AddItem(new Item(ItemDatabase.ItemObjects[i]), 1);
        }
    }

    /// <summary>
    /// 人物信息存档
    /// 在存档中，关于玩家角色的存档信息
    /// </summary>
    public void Save()
    {
    }

    private void UpdateEquipmentProp()
    {
        ////计算装备提供的属性
        equipmentProp = ChaProperty.zero;
        if (FaBao_Equippment_Inventory != null)
        {
            equipmentProp += FaBao_Equippment_Inventory.GetTotalProperty();
        }
    }

    /// <summary>
    /// 升级
    /// </summary>
    public void Upgrade()
    {
        if (realm.currentStage == StageType.DaCheng)
        { // 如果当前阶段已达到大成
            // 判断是否满足晋升至下一个境界的初期的条件
            if (realm.checkRealmUpgradeDelegate())
            {
                GetRequiredExp(realm.realmType, realm.currentStage);
                realm.currentStage = StageType.ChuQi; // 进入下一个境界的初期
                realm.realmType = (RealmType)((int)realm.realmType + 1); // 境界提升
                // 判断是否满足晋升至下一个境界的初期条件
                //进阶后
                //属性获得提升
                //弹出页面 显示属性提升
                UIManager.Instance.GetWindow<UIShenTongWindow>().Show();
                //此处留给策划填表 designscripts.common
                //realm.onUpgradeDelegate(realm.realmType, realm.currentStage);
            }
        }
        else
        { // 其他阶段
            // 判断当前阶段是否可以升级
            int currentStageIndex = (int)realm.currentStage;
            if (currentStageIndex < 3)
            { // 前三个阶段
                int requiredExp = GetRequiredExp(realm.realmType, realm.currentStage);
                if (exp >= requiredExp)
                { // 判断经验值是否达到升级要求
                    GetRequiredExp(realm.realmType, realm.currentStage);
                    realm.currentStage = (StageType)(currentStageIndex + 1);

                    //进阶后属性提升
                    //realm.onUpgradeDelegate(realm.realmType, realm.currentStage);
                    if (GameManager.Instance.IsInBattle)
                    {
                        UIManager.Instance.GetWindow<UIShenTongWindow>().Show();
                    }
                    exp -= requiredExp;
                }
            }
        }
    }

    // 基础经验值
    private const int baseExp = 100;
    private const float growthFactor = 1f;
    [ShowInInspector]
    private int requiredExp = 100;
    /// <summary>
    /// 获取升级到指定阶段所需经验值
    /// 从策划脚本中获取需要提升的经验
    /// </summary>
    /// <param name="realmType"></param>
    /// <param name="stageType"></param>
    /// <returns></returns>
    private int GetRequiredExp(RealmType realmType, StageType stageType)
    {
        int realmIndex = (int)realmType;
        int stageIndex = (int)stageType;

        float Exp = (baseExp * Mathf.Exp(growthFactor * realmIndex) * Mathf.Exp(growthFactor * stageIndex));
        requiredExp = Mathf.CeilToInt((float)Exp);
        return requiredExp;
    }


    public void AddExp(int exp)
    {
        this.exp += exp;
        bar.UpdateBar((float)this.exp / requiredExp);
        Upgrade();
    }

    public void AddCoin(int coinToAdd)
    {
        this.coin += coinToAdd;
    }
}