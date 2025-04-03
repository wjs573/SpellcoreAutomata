using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "New Skill Object", menuName = "Inventory System/Items/Skill")]
public class SkillScriptableObject : ItemObject
{
    private void Awake()
    {
        type = ItemType.技能;
    }
    public string skillId;
    public SkillModel Model => GetSkillModel();
    public float delayTimeModifier = 0f;
    public float chargeTimeModifier = 0f;
    public SkillModel GetSkillModel()
    {
        return DesignerTables.Skill.data[skillId];
    }
}