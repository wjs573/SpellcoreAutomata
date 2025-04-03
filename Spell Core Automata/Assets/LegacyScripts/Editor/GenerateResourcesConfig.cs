using System.IO;
using UnityEditor;
using UnityEngine;
public class GenerateResourcesConfig : MonoBehaviour
{
    [MenuItem("Tools/Resources/Generate ResourcesConfig File")]
    public static void Generate()
    {
        //寻找resources里的prefab的完整路径
        string[] resFiles = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });
        //记录路径
        for (int i = 0; i < resFiles.Length; i++)
        {
            resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);

            //名字和路径对应关系
            string fileName = Path.GetFileNameWithoutExtension(resFiles[i]);
            string filePath = resFiles[i].Replace("Assets/Resources", string.Empty).Replace(".prefab", string.Empty);

            resFiles[i] = fileName + "=" + filePath;

        }
        //保存 StreamingAssets 目录中的文件不会被压缩，适合在移动端读取资源
        //Application.persistentDataPath路径可以在运行时进行读写操作 unity外部目录 发布以后才可以用
        File.WriteAllLines("Assets/StreamingAssets/mapConfig.txt", resFiles);

        //刷新 让更新后的文件出现在编辑器里
        AssetDatabase.Refresh();

    }
}