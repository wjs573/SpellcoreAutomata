#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssetDatabaseSO))]
public class AssetDatabaseSOEditor : Editor
{
    private string folderPath = "Assets/Resources/Prefabs";
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Database Initialization", EditorStyles.boldLabel);
        
        folderPath = EditorGUILayout.TextField("Scan Folder Path", folderPath);
        
        if (GUILayout.Button("Scan Folder and Import Prefabs"))
        {
            ((AssetDatabaseSO)target).ScanFolder(folderPath);
        }
        
        if (GUILayout.Button("Initialize Lookup Dictionary"))
        {
            ((AssetDatabaseSO)target).InitializeLookup();
        }
    }
}
#endif