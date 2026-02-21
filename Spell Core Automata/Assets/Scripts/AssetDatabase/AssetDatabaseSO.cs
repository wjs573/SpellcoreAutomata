using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AssetDatabase", menuName = "Asset Management/Asset Database")]
public class AssetDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class AssetEntry
    {
        public string id;
        public GameObject prefab;
    }

    [SerializeField] private List<AssetEntry> assetEntries = new List<AssetEntry>();
    private Dictionary<string, GameObject> assetLookup;

    public void InitializeLookup()
    {
        assetLookup = new Dictionary<string, GameObject>();
        foreach (var entry in assetEntries)
        {
            if (!assetLookup.ContainsKey(entry.id))
            {
                assetLookup.Add(entry.id, entry.prefab);
            }
        }
    }

    public GameObject GetPrefab(string id)
    {
        if (assetLookup == null) InitializeLookup();

        if (assetLookup.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogError($"Prefab with ID '{id}' not found in database");
        return null;
    }

#if UNITY_EDITOR
    public void ScanFolder(string folderPath = "Assets/Resources/Prefabs")
{
    assetEntries.Clear();
    
    // 获取文件夹下所有预制件
    string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

    foreach (string guid in guids)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

        if (prefab != null)
        {
            // 仅使用预制件名称作为ID
            string id = Path.GetFileNameWithoutExtension(assetPath);
            
            // 检查是否已存在同名ID
            if (assetEntries.Any(e => e.id == id))
            {
                Debug.LogWarning($"重复的预制件名称: {id} (路径: {assetPath})");
                continue;
            }

            assetEntries.Add(new AssetEntry
            {
                id = id,
                prefab = prefab
            });
        }
    }
    
    EditorUtility.SetDirty(this);
    AssetDatabase.SaveAssets();
}
#endif
}