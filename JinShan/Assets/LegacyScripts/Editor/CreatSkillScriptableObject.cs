using JinShan;
using UnityEditor;
using UnityEngine;

public class CreatSkillScriptableObject : MonoBehaviour
{
    [MenuItem("Tools/Create Skill ScriptableObjects")]
    public static void CreateSkillScriptableObjects()
    {
        Debug.Log("Starting to create Skill ScriptableObjects.");
        foreach (SkillModel skillmodel in DesignerTables.Skill.data.Values)
        {
            Debug.Log("Creating Skill ScriptableObject for: " + skillmodel.id);
            CreateSkillScriptableObject(skillmodel);
        }
    }

    private static void CreateSkillScriptableObject(SkillModel skillModel)
    {
        Debug.Log("Creating new SkillScriptableObject instance for: " + skillModel.id);
        SkillScriptableObject newItem = ScriptableObject.CreateInstance<SkillScriptableObject>();

        string iconPath = "Assets/Resources/Icons/Skill/" + skillModel.id + ".png";
        if (AssetDatabase.LoadMainAssetAtPath(iconPath))
        {
            Debug.Log("Loading icon from: " + iconPath);
            newItem.uiDisplay = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        }
        else
        {
            Debug.Log("Could not load icon from: " + iconPath);
            newItem.uiDisplay = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Icons/Skill/default.png");
        }

        newItem.data.Name = skillModel.id;
        newItem.data.Rank = ItemRank.默认;
        newItem.skillId = skillModel.id;
        // path has to start at "Assets"
        string path = "Assets/Resources/Inventory/ScriptableObjects/技能/" + skillModel.id + ".asset";
        Debug.Log("Creating asset at path: " + path);
        AssetDatabase.CreateAsset(newItem, path);

        Debug.Log("Asset created successfully for: " + skillModel.id);
    }
}