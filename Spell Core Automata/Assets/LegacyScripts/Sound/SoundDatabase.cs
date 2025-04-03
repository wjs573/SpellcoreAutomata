using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Inventory System/Items/SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
    public List<SoundClip> data;
}


[System.Serializable]
public class SoundClip
{
    public string id;
    public AudioClip AudioClip;
}

