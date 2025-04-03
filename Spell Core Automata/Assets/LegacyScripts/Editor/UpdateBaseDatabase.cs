using System.Collections.Generic;
using System.Linq;
using JinShan;
using UnityEditor;

public static class UpdateBaseDatabase
{
    // ����BaseDatabase�е���Ʒ��Ϣ
    // Ŀǰ��ʵ�ַ����ͼ���
    [MenuItem("Tools/Update BaseDatabase")]
    public static void UpdateBaseDatabaseWithNewItem()
    {
        string baseDatabasePath = "Assets/Inventory/BaseDatabase.asset";

        ItemDatabaseObject baseDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabaseObject>(baseDatabasePath);
        // ��������
        UpdateDatabaseFromPath_FaBao(baseDatabase, "Assets/Inventory/ScriptableObjects/����");

        // ���ܲ���
        UpdateDatabaseFromPath_Skill(baseDatabase, "Assets/Inventory/ScriptableObjects/Skill");

        // �����������ݿ�����Ʒ��ID
        baseDatabase.UpdateItems();
    }

    // ��ָ��·����ȡ��Ʒ���������ݿ�
    private static void UpdateDatabaseFromPath_FaBao(ItemDatabaseObject baseDatabase, string path)
    {
        // ��·����ȡ�µ���Ʒ����
        List<FaBao> newItemObjects = MyScriptableObjectReader.LoadAllAssets<FaBao>(path);

        // ����ԭ�е���Ʒ��Ϣ
        List<ItemObject> itemObjects = baseDatabase.ItemObjects.ToList();

        foreach (ItemObject newItemObject in newItemObjects)
        {
            // �����Ƿ�����ͬ���ֵ���Ʒ
            ItemObject foundObject = itemObjects.FirstOrDefault(item => item.data.Name == newItemObject.data.Name);
            if (foundObject != null)
            {
                // ����ҵ��ˣ�����ԭ�е���Ʒ��Ϣ
                int index = itemObjects.IndexOf(foundObject);
                itemObjects[index] = newItemObject;
            }
            else
            {
                // ���û���ҵ��������µ���Ʒ�����ݿ�
                itemObjects.Add(newItemObject);
            }
        }

        // ���º����Ʒ��Ϣ ת����ItemObject[]
        baseDatabase.ItemObjects = itemObjects.ToArray();
    }

    // ��ָ��·����ȡ��Ʒ���������ݿ�
    private static void UpdateDatabaseFromPath_Skill(ItemDatabaseObject baseDatabase, string path)
    {
        // ��·����ȡ�µ���Ʒ����
        List<SkillScriptableObject> newItemObjects = MyScriptableObjectReader.LoadAllAssets<SkillScriptableObject>(path);

        // ����ԭ�е���Ʒ��Ϣ
        List<ItemObject> itemObjects = baseDatabase.ItemObjects.ToList();

        foreach (ItemObject newItemObject in newItemObjects)
        {
            // �����Ƿ�����ͬ���ֵ���Ʒ
            ItemObject foundObject = itemObjects.FirstOrDefault(item => item.data.Name == newItemObject.data.Name);
            if (foundObject != null)
            {
                // ����ҵ��ˣ�����ԭ�е���Ʒ��Ϣ
                int index = itemObjects.IndexOf(foundObject);
                itemObjects[index] = newItemObject;
            }
            else
            {
                // ���û���ҵ��������µ���Ʒ�����ݿ�
                itemObjects.Add(newItemObject);
            }
        }

        // ���º����Ʒ��Ϣ ת����ItemObject[]
        baseDatabase.ItemObjects = itemObjects.ToArray();
    }
}