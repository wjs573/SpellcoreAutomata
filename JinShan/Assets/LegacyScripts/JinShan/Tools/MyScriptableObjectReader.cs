using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MyScriptableObjectReader
{
    public static List<T> LoadAllAssets<T>(string path) where T : ScriptableObject
    {
        List<T> result = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new string[] { path });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                result.Add(asset);
            }
        }

        return result;
    }
}