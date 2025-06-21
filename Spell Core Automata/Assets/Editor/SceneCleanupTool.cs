using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneCleanupTool : EditorWindow
{
    [MenuItem("Tools/Scene Cleanup")]
    public static void ShowWindow()
    {
        // 创建自定义编辑器窗口
        GetWindow<SceneCleanupTool>("Scene Cleanup");
    }

    private void OnGUI()
    {
        GUILayout.Label("SceneIDMap Cleanup", EditorStyles.boldLabel);
        if (GUILayout.Button("Clean SceneIDMap Objects"))
        {
            CleanSceneIDMap();
        }
        
        GUILayout.Space(15);
        GUILayout.Label("Missing Script Finder", EditorStyles.boldLabel);
        if (GUILayout.Button("Find Missing Scripts"))
        {
            FindMissingScripts();
        }
        
        GUILayout.Space(15);
        GUILayout.Label("Object Selection", EditorStyles.boldLabel);
        if (GUILayout.Button("Select Missing Script Objects"))
        {
            SelectMissingScriptObjects();
        }
    }

    private static void CleanSceneIDMap()
    {
        Debug.Log("Starting SceneIDMap cleanup...");
        int cleanedCount = 0;
        
        while (GameObject.Find("SceneIDMap") != null)
        {
            GameObject obj = GameObject.Find("SceneIDMap");
            if (obj != null)
            {
                EditorUtility.DisplayProgressBar(
                    "Cleaning SceneIDMap",
                    $"Cleaning {obj.name}",
                    cleanedCount / (cleanedCount + 1f)
                );
                
                GameObject.DestroyImmediate(obj);
                Debug.Log($"Cleared: {obj.name}", obj);
                cleanedCount++;
            }
        }
        
        EditorUtility.ClearProgressBar();
        Debug.Log($"SceneIDMap cleanup completed! Removed {cleanedCount} objects.");
    }

    public static void FindMissingScripts()
    {
        Debug.Log("Searching for missing scripts...");
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int missingCount = 0;
        
        foreach (GameObject go in allObjects)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(go) && 
                PrefabUtility.IsAnyPrefabInstanceRoot(go))
            {
                continue;
            }

            Component[] components = go.GetComponents<Component>();
            bool hasMissing = false;
            
            foreach (var component in components)
            {
                if (component == null)
                {
                    hasMissing = true;
                    break;
                }
            }
            
            if (hasMissing)
            {
                Debug.LogWarning($"Missing script found on: {go.name}", go);
                missingCount++;
            }
        }
        
        if (missingCount > 0)
        {
            Debug.LogError($"Found {missingCount} objects with missing scripts! Check them in the console.");
        }
        else
        {
            Debug.Log("No missing scripts found! Scene is clean.");
        }
    }

    public static void SelectMissingScriptObjects()
    {
        List<GameObject> objectsWithMissing = new List<GameObject>();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        
        foreach (GameObject go in allObjects)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(go) && 
                PrefabUtility.IsAnyPrefabInstanceRoot(go))
            {
                continue;
            }

            Component[] components = go.GetComponents<Component>();
            bool hasMissing = false;
            
            foreach (var component in components)
            {
                if (component == null)
                {
                    hasMissing = true;
                    break;
                }
            }
            
            if (hasMissing)
            {
                objectsWithMissing.Add(go);
            }
        }
        
        Selection.objects = objectsWithMissing.ToArray();
        Debug.Log($"Selected {objectsWithMissing.Count} objects with missing scripts.");
    }
}