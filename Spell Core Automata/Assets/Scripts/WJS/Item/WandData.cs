using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public struct WandData
{
    public int SlotCount;             // 插槽数量
    public float CastInterval;    // 施法间隔：每次施法的间隔
    public int BaseScatter;       // 基础散射角度
    public List<RuneSlot> RuneSlots; // 插槽中的物品
}

