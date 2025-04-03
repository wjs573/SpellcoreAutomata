using System.Collections;
using System.Collections.Generic;
using JinShan;
using UnityEngine;

public class MartialArtObject : ItemObject
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        type = ItemType.功法;
    }

    public string MartialArtName;
}
