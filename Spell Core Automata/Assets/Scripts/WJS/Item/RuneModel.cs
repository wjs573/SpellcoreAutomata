using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public struct RuneModel
{
    ///<summary>
    ///道具id
    ///</summary>
    public string id;

    ///<summary>
    ///道具的icon
    ///</summary>
    public string icon;

    ///<summary>
    ///道具名称
    ///</summary>
    public string name;

    ///<summary>
    ///道具Tag
    ///</summary>
    public string[] tags;

    public string description;
    public SkillModel skill;
    public EnhanceEffectEvent enhanceEffectEvent;
}
