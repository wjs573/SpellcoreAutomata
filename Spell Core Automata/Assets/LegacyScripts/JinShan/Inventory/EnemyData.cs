using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using JinShan;

[CreateAssetMenu(fileName = "New EnemyData Object", menuName = "Inventory System/Items/EnemyData")]
public class EnemyData : ItemObject
{
    /// <summary>
    /// 属性
    /// </summary>
    [ShowInInspector]
    public ChaProperty ChaProperty = ChaProperty.zero;

    /// <summary>
    /// 美术资源id
    /// 在Resources/Prefabs/Charater/
    /// </summary>
    [FoldoutGroup("Visual")]
    [FilePath(ParentFolder = "Assets/Resources/Prefabs/Charater", Extensions = "prefab")]
    [OnValueChanged("LoadPrefab")]
    public string View;

    [FoldoutGroup("Visual"), InlineEditor(InlineEditorModes.LargePreview)]
    [ShowInInspector]
    [OnValueChanged("UpdatePrefab")]
    [HideLabel]
    private GameObject viewPreview;

    [FoldoutGroup("AI")]
    [FilePath(ParentFolder = "Assets/Resources/Prefabs/AI", Extensions = "prefab")]
    [OnValueChanged("LoadPrefab")]
    public string FSM;

    [FoldoutGroup("AI")]
    [ShowInInspector]
    [OnValueChanged("UpdatePrefab")]
    private GameObject AIBrainPreview;

    /// <summary>
    /// 是否为精英
    /// 如果是精英，则需要给它添加一个血量条
    /// </summary>
    public bool IsElite = false;

    public int attackRange = 1;

    /// <summary>
    /// 创建时添加的buff
    /// </summary>
    public List<EnemyDataBuff> addBuffInfos = new List<EnemyDataBuff>();

    /// <summary>
    /// 怪物使用的技能
    /// </summary>
    public List<string> skills = new List<string>();

    /// <summary>
    /// 怪物使用的技能
    /// </summary>
    public ItemObject[] spells = new ItemObject[] { };


    /// <summary>
    /// 获取生成数据
    /// </summary>
    /// <returns></returns>
    public CharacterSpawnInfo GetCharacterSpawnInfo()
    {
        CharacterSpawnInfo characterSpawnInfo = new CharacterSpawnInfo
        {
            Name = this.data.Name,
            ChaProperty = this.ChaProperty,
            View = this.View,
            FSM = this.FSM,
            IsElite = this.IsElite
        };

        // 复制Buff信息
        foreach (var buffInfo in addBuffInfos)
        {
            characterSpawnInfo.addBuffInfos.Add(buffInfo.AddBuffInfo);
        }

        // 复制技能信息
        foreach (var skillId in skills)
        {
            if (DesignerTables.Skill.data.ContainsKey(skillId))
            {
                characterSpawnInfo.skills.Add(DesignerTables.Skill.data[skillId].Clone());
            }
        }

        return characterSpawnInfo;
    }

    private void OnEnable()
    {
        LoadPrefab();
    }

    private void LoadPrefab()
    {
        if (!string.IsNullOrEmpty(View))
        {
            viewPreview = Resources.Load<GameObject>("Prefabs/Character/" + View);
        }

        if (!string.IsNullOrEmpty(FSM))
        {
            AIBrainPreview = Resources.Load<GameObject>("Prefabs/AI/" + FSM);
        }
    }

    private void UpdatePrefab()
    {
        View = viewPreview.name;
        FSM = AIBrainPreview.name;
    }
}


public class EnemyDataBuff
{
    [OnValueChanged("UpdateAddBuffInfo")]
    public string BuffId;
    public AddBuffInfo AddBuffInfo;
    public void UpdateAddBuffInfo()
    {
        if (DesignerTables.Buff.data.ContainsKey(BuffId))
        {
            AddBuffInfo = new AddBuffInfo(DesignerTables.Buff.data[BuffId], null, null, 1, 1);
        }
    }
}

public class CharacterSpawnInfo
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name;

    /// <summary>
    /// 数量
    /// </summary>
    public int count;

    /// <summary>
    /// 属性
    /// </summary>
    public ChaProperty ChaProperty = ChaProperty.zero;

    /// <summary>
    /// 美术资源id
    /// 在Resources/Prefabs/Charater/
    /// </summary>
    public string View;

    public string FSM;

    /// <summary>
    /// 是否为精英
    /// 如果是精英，则需要给它添加一个血量条
    /// </summary>
    public bool IsElite = false;

    /// <summary>
    /// 创建时添加的buff
    /// </summary>
    public List<AddBuffInfo> addBuffInfos = new List<AddBuffInfo>();

    /// <summary>
    /// 怪物使用的技能
    /// </summary>
    public List<SkillModel> skills = new List<SkillModel>();

    public List<string> skillIds = new List<string>();

    public void GetSkillModels()
    {
        if (skills.Count <= skillIds.Count)
        {
            // 复制技能信息
            foreach (string skillId in skillIds)
            {
                if (DesignerTables.Skill.data.ContainsKey(skillId))
                {
                    skills.Add(DesignerTables.Skill.data[skillId].Clone());
                }
            }
        }
    }

    public CharacterSpawnInfo(string enemyDataId, int count)
    {
        EnemyData data = (EnemyData)GameManager.Instance.database.GetItemObjectByName(enemyDataId);
        if (data != null)
        {
            Name = data.data.Name;
            ChaProperty = data.ChaProperty;
            View = data.View;
            FSM = data.FSM;
            IsElite = data.IsElite;
            this.count = count;
            this.skillIds = data.skills;
            // 复制Buff信息
            foreach (var buffInfo in data.addBuffInfos)
            {
                addBuffInfos.Add(buffInfo.AddBuffInfo);
            }
        }
        else
        {
            Debug.LogError("EnemyData not found for id: " + enemyDataId);
        }
    }
    public CharacterSpawnInfo() { }
}