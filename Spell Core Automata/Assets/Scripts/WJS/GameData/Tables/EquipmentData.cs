using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public class EquipmentData
{
    public static Dictionary<string, EquipmentModel> data = new Dictionary<string, EquipmentModel>();

    // 初始化方法
    public static void Initialize()
    {
        data = new Dictionary<string, EquipmentModel>()
        {
            {"星辉棒", new EquipmentModel()
            {
                id = "20001",
                name = "星辉棒",
                icon = "starRod",
                type = EquipmentType.weapon,
                equipmentProperty = new ChaProperty(0,0,0,100,0,100,0,10,0),
                wandData = new WandData
                {
                    SlotCount = 4,
                    CastInterval = 0.25f,
                    BaseScatter = 10,
                    RuneSlots = new List<RuneSlot>(4)
                    {
                        new RuneSlot(RuneData.data["FireBall"])
                    }
                }
            }}
        };
    }
}
