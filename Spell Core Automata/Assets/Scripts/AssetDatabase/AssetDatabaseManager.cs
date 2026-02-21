using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public class AssetDatabaseManager : MonoSingleton<AssetDatabaseManager>
{
    public AssetDatabaseSO AssetDatabase;

    public GameObject GetPrefab(string prefabName)
    {
        return AssetDatabase.GetPrefab(prefabName);
    }
}
