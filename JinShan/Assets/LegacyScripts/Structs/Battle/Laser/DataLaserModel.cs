using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DesignerScripts
{
    public class DataLaserModel
    {
        public static Dictionary<string, LaserModel> data;
        public static void Init()
        {
            data = new Dictionary<string, LaserModel>()
            {
                //绿色流动波纹射线: GreenFlowingRippleRay
                //蓝白闪电射线: BlueWhiteLightningRay
                //红色环状火焰波纹射线: RedCircularFlameRippleRay

                //紫色波纹射线: PurpleRippleRay
                //蓝色冰晶: BlueIceCrystalRay
                //金色极细射线: GoldenThinRay
                //青色射线: CyanRay
                //紫色雷电波纹射线: PurpleThunderRippleRay
                //红色火焰射线: RedFlameRay
            };

            data.Add(
                "GreenRay", new LaserModel(
                       "GreenRay", "GreenFlowingRippleRay",
                       "", new object[0],
                       "AddPoisonBuffLaserHit", new object[0],
                       "", new object[0],
                       ChaResource.Null, 40f, 1, 0.25f, true, false
                   ));

            data.Add(
                "LightningRay", new LaserModel(
                       "LightningRay", "BlueWhiteLightningRay",
                       "", new object[0],
                       "IncreaseDamageOverTimeLaserHit", new object[] { 50.0f },
                       "", new object[0],
                       ChaResource.Null, 40f, 1, 0.25f, true, false
                   ));

            data.Add(
                "FlameRay", new LaserModel(
                       "FlameRay", "RedCircularFlameRippleRay",
                       "", new object[0],
                       "CommonLaserHit", new object[0],
                       "", new object[0],
                       ChaResource.Null, 40f, 1, 0.25f, true, false
                   ));
            //猩红射线: CrimsonRay
            //光剑 距离极短
            data.Add(
                "BloodyCrimsonRay", new LaserModel(
                       "BloodyCrimsonRay", "CrimsonRay",
                       "", new object[0],
                       "CommonLaserHit", new object[] { 0.10f },
                       "", new object[0],
                       ChaResource.Null, 4f, 999, 0.25f, true, false
                   ));

            data.Add(
                "PurpleRay", new LaserModel(
                       "PurpleRay", "PurpleRippleRay",
                       "", new object[0],
                       "CommonLaserHit", new object[] { 0.10f },
                       "", new object[0],
                       ChaResource.Null, 40f, 1, 0.25f, true, false
                   ));

            data.Add(
                "IceCrystalRay", new LaserModel(
                       "IceCrystalRay", "BlueIceCrystalRay",
                       "", new object[0],
                       "AddColdBuffLaserHit", new object[] { 0.10f },
                       "", new object[0],
                       ChaResource.Null, 40f, 3, 0.25f, true, false
                   ));

            data.Add(
                "GoldenRay", new LaserModel(
                       "GoldenRay", "GoldenThinRay",
                       "", new object[0],
                       "KnockbackLaserHit", new object[0],
                       "", new object[0],
                       ChaResource.Null, 40f, 5, 0.25f, true, false
                   ));

            data.Add(
                "WaterWaveRay", new LaserModel(
                       "WaterWaveRay", "CyanRay",
                       "", new object[0],
                       "CommonLaserHit", new object[] { 0.10f },
                       "", new object[0],
                       ChaResource.Null, 40f, 1, 0.25f, true, false
                   ));

            data.Add(
                "PurpleThunderRay", new LaserModel(
                       "PurpleThunderRay", "PurpleThunderRippleRay",
                       "", new object[0],
                       "CommonLaserHit", new object[] { 0.10f },
                       "", new object[0],
                       ChaResource.Null, 40f, 5, 0.25f, true, false
                   ));

            data.Add(
                "RedFlameRay", new LaserModel(
                       "RedFlameRay", "RedFlameRay",
                       "", new object[0],
                       "CommonLaserHit", new object[] { 0.10f },
                       "", new object[0],
                       ChaResource.Null, 40f, 1, 0.25f, true, false
                   ));
        }
    }
}

