using System.Collections.Generic;
using UnityEngine;

namespace DesignerTables
{
    ///<summary>
    ///BulletModel
    ///</summary>
    public class Bullet
    {
        public static Dictionary<string, BulletModel> data = new Dictionary<string, BulletModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, BulletModel>() { };

            BulletModel bulletModel;

            ParamDictionary FireBallOnHitParams = new ParamDictionary();
            FireBallOnHitParams.Add("攻击力加成", 1.0f);
            FireBallOnHitParams.Add("基础暴击率", 0.05f);
            FireBallOnHitParams.Add("命中视觉特效", "Effect/Hit/ExplosionFireballSharpBlue");
            FireBallOnHitParams.Add("特效绑定点", "Body");
            bulletModel = new BulletModel(
                "BlueFireBall", "Bullet/FireballMissileBlue", "Effect/Flash/MuzzleFireballBlue",
                "", new object[0],
                "CommonBulletHit", FireBallOnHitParams,
                "", new object[0],
                MoveType.fly, false, 0.1f, 1, 0.5f, true, false);
            data.Add("BlueFireBall", bulletModel);

            ParamDictionary bulletOnHitParams = new ParamDictionary();
            bulletOnHitParams.Add("攻击力加成", 1.0f);
            bulletOnHitParams.Add("基础暴击率", 0.05f);
            bulletOnHitParams.Add("命中视觉特效", "Effect/HovlHit/Hit 26");
            bulletOnHitParams.Add("特效绑定点", "Body");
            bulletModel = new BulletModel(
                "Blade", "HovlBullet/Projectile 8", "",
                "", new object[0],
                "CommonBulletHit", bulletOnHitParams,
                "", new object[0],
                MoveType.fly, false, 0.1f, 9999, 0.5f, true, false);
            data.Add("Blade", bulletModel);

            // HighSpeedShell
            ParamDictionary highSpeedShellParams = new ParamDictionary();
            highSpeedShellParams.Add("攻击力加成", 2.0f);
            highSpeedShellParams.Add("基础暴击率", 0.05f);
            highSpeedShellParams.Add("命中视觉特效", "Effect/Hit/Hit01");
            highSpeedShellParams.Add("特效绑定点", "Body");
            BulletModel highSpeedShell = new BulletModel(
                "HighSpeedShell", "HovlBullet/Projectile 21", "",
                "", new object[0],
                "CommonBulletHit", highSpeedShellParams,
                "", new object[0],
                MoveType.fly, false, 0.2f, 1, 0.5f, true, false
            );
            data.Add("HighSpeedShell", highSpeedShell);



            //暴雨飞弹
            // HeavyRainBullet
            ParamDictionary heavyRainBulletParams = new ParamDictionary();
            heavyRainBulletParams.Add("攻击力加成", 1.0f);
            heavyRainBulletParams.Add("基础暴击率", 0.05f);
            heavyRainBulletParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 9");
            heavyRainBulletParams.Add("特效绑定点", "Body");
            BulletModel heavyRainBullet = new BulletModel(
                "HeavyRainBullet", "HovlBullet/Projectile 9", "",
                "SetUnitRotateSpeed", new object[] { 1800f },
                "CommonBulletHit", heavyRainBulletParams,
                "", new object[0],
                MoveType.fly, false, 0.2f, 1, 0.5f, true, false
            );
            data.Add("HeavyRainBullet", heavyRainBullet);

            //佛怒火莲
            ParamDictionary flameLotusOfWrathParams = new ParamDictionary();
            flameLotusOfWrathParams.Add("攻击力加成", 1.0f);
            flameLotusOfWrathParams.Add("基础暴击率", 0.05f);
            flameLotusOfWrathParams.Add("命中视觉特效", "Effect/HovlHit/Hit 19");
            flameLotusOfWrathParams.Add("特效绑定点", "Body");
            BulletModel flameLotusOfWrath = new BulletModel(
                "FlameLotusOfWrath", "HovlBullet/Projectile 19", "",
                "CreateAoeFollowBullet", new object[] { new AoeLauncher(DesignerTables.AoE.data["GatheringEnemies"], null, Vector3.zero, 5f, 4.5f, 0f) },
                "CommonBulletHit", flameLotusOfWrathParams,
                "CreateAoeOnRemoved", new object[] { new AoeLauncher(DesignerTables.AoE.data["FireExplosion"], null, Vector3.zero, 5f, 1.0f, 0f) },
                MoveType.fly, false, 0.5f, 9999, 1, true, false
            );
            data.Add("FlameLotusOfWrath", flameLotusOfWrath);


            //飞叶子弹
            ParamDictionary greenLeafParams = new ParamDictionary();
            greenLeafParams.Add("攻击力加成", 1.0f);
            greenLeafParams.Add("基础暴击率", 0.05f);
            greenLeafParams.Add("命中视觉特效", "Effect/Hit/Hit01");
            greenLeafParams.Add("特效绑定点", "Body");
            BulletModel greenLeaf = new BulletModel(
                "GreenLeaf", "HovlBullet/Projectile 6", "",
                "", new object[0],
                "CommonBulletHit", greenLeafParams,
                "CommonBulletRemoved", new object[] { "Effect/Hit/Hit01" }
            );
            data.Add("GreenLeaf", greenLeaf);

            // 火焰飞弹
            ParamDictionary fireballOnHitParams = new ParamDictionary();
            fireballOnHitParams.Add("攻击力加成", 1.0f);
            fireballOnHitParams.Add("基础暴击率", 0.05f);
            fireballOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 3");
            fireballOnHitParams.Add("特效绑定点", "Body");
            BulletModel fireball = new BulletModel(
                "fireball", "HovlBullet/Projectile fireball", "Effect/Hovl/Flash/Flash 3",
                "", new object[0],
                "AddIgniteBuffOnHit", fireballOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hit/Hit03" }
            );
            data.Add("fireball", fireball);

            // 雷电球
            ParamDictionary thunderBallBulletOnHitParams = new ParamDictionary();
            thunderBallBulletOnHitParams.Add("攻击力加成", 1.0f);
            thunderBallBulletOnHitParams.Add("基础暴击率", 0.05f);
            thunderBallBulletOnHitParams.Add("命中视觉特效", "Effect/HitEffect_A");
            thunderBallBulletOnHitParams.Add("特效绑定点", "Body");
            BulletModel thunderBallBullet = new BulletModel(
                "ThunderBallBullet", "ThunderBallBullet", "",
                "", new object[0],
                "CommonBulletHit", thunderBallBulletOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/HitEffect_A" }
            );
            data.Add("ThunderBallBullet", thunderBallBullet);

            // GreenArrow
            ParamDictionary greenArrowOnHitParams = new ParamDictionary();
            greenArrowOnHitParams.Add("攻击力加成", 1.0f);
            greenArrowOnHitParams.Add("基础暴击率", 0.05f);
            greenArrowOnHitParams.Add("命中视觉特效", "Effect/Hit/Hit01");
            greenArrowOnHitParams.Add("特效绑定点", "Body");
            BulletModel greenArrow = new BulletModel(
                "GreenArrow", "HovlBullet/Projectile 17", "",
                "", new object[0],
                "CommonBulletHit", greenArrowOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hit/Hit01" }
            );
            data.Add("GreenArrow", greenArrow);

            // 冰锥术子弹
            ParamDictionary iceConeOnHitParams = new ParamDictionary();
            iceConeOnHitParams.Add("攻击力加成", 1.0f);
            iceConeOnHitParams.Add("基础暴击率", 0.05f);
            iceConeOnHitParams.Add("命中视觉特效", "Effect/Hit/HitIce");
            iceConeOnHitParams.Add("特效绑定点", "Body");
            BulletModel iceCone = new BulletModel(
                "IceCone", "HovlBullet/ProjectileIce", "Effect/Hovl/Flash/Flash 9",
                "", new object[0],
                "CommonBulletHit", iceConeOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hit/HitIce" }
            );
            data.Add("IceCone", iceCone);

            // Green Dart
            ParamDictionary greenDartOnHitParams = new ParamDictionary();
            greenDartOnHitParams.Add("攻击力加成", 0.75f);
            greenDartOnHitParams.Add("基础暴击率", 0.03f);
            greenDartOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 1");
            greenDartOnHitParams.Add("特效绑定点", "Body");
            BulletModel greenDart = new BulletModel(
                "Green Dart", "HovlBullet/Projectile 1", "Effect/Hovl/Flash/Flash 1",
                "", new object[0],
                "CommonBulletHit", greenDartOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 1" }
            );
            data.Add("Green Dart", greenDart);

            // Thunder Missile
            ParamDictionary thunderMissileOnHitParams = new ParamDictionary();
            thunderMissileOnHitParams.Add("攻击力加成", 1.0f);
            thunderMissileOnHitParams.Add("基础暴击率", 0.04f);
            thunderMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 2");
            thunderMissileOnHitParams.Add("特效绑定点", "Body");
            BulletModel thunderMissile = new BulletModel(
                "Thunder Missile", "HovlBullet/Projectile 2", "Effect/Hovl/Flash/Flash 2",
                "", new object[0],
                "CommonBulletHit", thunderMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 2" }
            );
            data.Add("Thunder Missile", thunderMissile);

            // Flame Missile
            ParamDictionary flameMissileOnHitParams = new ParamDictionary();
            flameMissileOnHitParams.Add("攻击力加成", 1.0f);
            flameMissileOnHitParams.Add("基础暴击率", 0.05f);
            flameMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 3");
            flameMissileOnHitParams.Add("特效绑定点", "Body");
            BulletModel flameMissile = new BulletModel(
                "Flame Missile", "HovlBullet/Projectile 3", "Effect/Hovl/Flash/Flash 3",
                "", new object[0],
                "CommonBulletHit", flameMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 3" }
            );
            data.Add("Flame Missile", flameMissile);

            // Yellow Flying Sword
            ParamDictionary yellowFlyingSwordOnHitParams = new ParamDictionary();
            yellowFlyingSwordOnHitParams.Add("攻击力加成", 0.8f);
            yellowFlyingSwordOnHitParams.Add("基础暴击率", 0.03f);
            yellowFlyingSwordOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 4");
            yellowFlyingSwordOnHitParams.Add("特效绑定点", "Body");
            BulletModel yellowFlyingSword = new BulletModel(
                "Yellow Flying Sword", "HovlBullet/Projectile 4", "Effect/Hovl/Flash/Flash 4",
                "", new object[0],
                "CommonBulletHit", yellowFlyingSwordOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 4" }
            );
            data.Add("Yellow Flying Sword", yellowFlyingSword);

            // Crimson Dart
            ParamDictionary crimsonDartOnHitParams = new ParamDictionary();
            crimsonDartOnHitParams.Add("攻击力加成", 0.75f);
            crimsonDartOnHitParams.Add("基础暴击率", 0.03f);
            crimsonDartOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 5");
            crimsonDartOnHitParams.Add("特效绑定点", "Body");
            BulletModel crimsonDart = new BulletModel(
                "Crimson Dart", "HovlBullet/Projectile 5", "Effect/Hovl/Flash/Flash 5",
                "", new object[0],
                "CommonBulletHit", crimsonDartOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 5" }
            );
            data.Add("Crimson Dart", crimsonDart);

            // Blue Green Arrow
            ParamDictionary blueGreenArrowOnHitParams = new ParamDictionary();
            blueGreenArrowOnHitParams.Add("攻击力加成", 0.9f);
            blueGreenArrowOnHitParams.Add("基础暴击率", 0.04f);
            blueGreenArrowOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 6");
            blueGreenArrowOnHitParams.Add("特效绑定点", "Body");
            BulletModel blueGreenArrow = new BulletModel(
                "Blue Green Arrow", "HovlBullet/Projectile 6", "Effect/Hovl/Flash/Flash 6",
                "", new object[0],
                "CommonBulletHit", blueGreenArrowOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 6" }
            );
            data.Add("Blue Green Arrow", blueGreenArrow);

            // Purple Firework
            ParamDictionary purpleFireworkOnHitParams = new ParamDictionary();
            purpleFireworkOnHitParams.Add("攻击力加成", 0.85f);
            purpleFireworkOnHitParams.Add("基础暴击率", 0.04f);
            purpleFireworkOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 7");
            purpleFireworkOnHitParams.Add("特效绑定点", "Body");
            BulletModel purpleFirework = new BulletModel(
                "Purple Firework", "HovlBullet/Projectile 7", "Effect/Hovl/Flash/Flash 7",
                "", new object[0],
                "CommonBulletHit", purpleFireworkOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 7" }
            );
            data.Add("Purple Firework", purpleFirework);

            // Dagger
            ParamDictionary daggerOnHitParams = new ParamDictionary();
            daggerOnHitParams.Add("攻击力加成", 0.6f);
            daggerOnHitParams.Add("基础暴击率", 0.02f);
            daggerOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 8");
            daggerOnHitParams.Add("特效绑定点", "Body");
            BulletModel dagger = new BulletModel(
                "Dagger", "HovlBullet/Projectile 8", "",
                "", new object[0],
                "CommonBulletHit", daggerOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 8" }
            );
            data.Add("Dagger", dagger);

            // Ice Blue Missile
            ParamDictionary iceBlueMissileOnHitParams = new ParamDictionary();
            iceBlueMissileOnHitParams.Add("攻击力加成", 0.1f);
            iceBlueMissileOnHitParams.Add("基础暴击率", 0.50f);
            iceBlueMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 9");
            iceBlueMissileOnHitParams.Add("特效绑定点", "Body");
            iceBlueMissileOnHitParams.Add("流血伤害", 20);
            BulletModel iceBlueMissile = new BulletModel(
                "Ice Blue Missile", "HovlBullet/Projectile 9", "Effect/Hovl/Flash/Flash 9",
                "", new object[0],
                "BleedingDamageOnHit", iceBlueMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 9" }
            );
            data.Add("Ice Blue Missile", iceBlueMissile);

            // High-Speed Blue Missile
            ParamDictionary highSpeedBlueMissileOnHitParams = new ParamDictionary();
            highSpeedBlueMissileOnHitParams.Add("攻击力加成", 1.3f);
            highSpeedBlueMissileOnHitParams.Add("基础暴击率", 0.065f);
            highSpeedBlueMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 10");
            highSpeedBlueMissileOnHitParams.Add("特效绑定点", "Body");
            BulletModel highSpeedBlueMissile = new BulletModel(
                "High-Speed Blue Missile", "HovlBullet/Projectile 10", "Effect/Hovl/Flash/Flash 10",
                "", new object[0],
                "CommonBulletHit", highSpeedBlueMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 10" }
            );
            data.Add("High-Speed Blue Missile", highSpeedBlueMissile);

            // Yellow Missile with Purple Tail
            ParamDictionary yellowMissileOnHitParams = new ParamDictionary();
            yellowMissileOnHitParams.Add("攻击力加成", 0.7f);
            yellowMissileOnHitParams.Add("基础暴击率", 0.03f);
            yellowMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 11");
            yellowMissileOnHitParams.Add("特效绑定点", "Body");
            BulletModel yellowMissile = new BulletModel(
                "Yellow Missile with Purple Tail", "HovlBullet/Projectile 11", "Effect/Hovl/Flash/Flash 11",
                "", new object[0],
                "CommonBulletHit", yellowMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 11" }
            );
            data.Add("Yellow Missile with Purple Tail", yellowMissile);

            // Green Blob with Wood Attribute and Poison
            ParamDictionary greenBlobOnHitParams = new ParamDictionary();
            greenBlobOnHitParams.Add("AoE发射信息", new AoeLauncher(DesignerTables.AoE.data["PoisonFog"], null, Vector3.zero, 4f, 4f, 0f));
            BulletModel greenBlob = new BulletModel(
                "Green Blob with Wood Attribute and Poison", "HovlBullet/Projectile 12", "Effect/Hovl/Flash/Flash 12",
                "", new object[0],
                "CreateAoEOnHit", greenBlobOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 12" },
                MoveType.fly, true, 0.25f
            );
            data.Add("Green Blob with Wood Attribute and Poison", greenBlob);

            // Fast Small Red Missile
            ParamDictionary fastSmallRedMissileOnHitParams = new ParamDictionary();
            fastSmallRedMissileOnHitParams.Add("攻击力加成", 1.1f);
            fastSmallRedMissileOnHitParams.Add("基础暴击率", 0.04f);
            fastSmallRedMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 13");
            fastSmallRedMissileOnHitParams.Add("特效绑定点", "Body");
            BulletModel fastSmallRedMissile = new BulletModel(
                "Fast Small Red Missile", "HovlBullet/Projectile 13", "Effect/Hovl/Flash/Flash 13",
                "", new object[0],
                "CommonBulletHit", fastSmallRedMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 13" }
            );
            data.Add("Fast Small Red Missile", fastSmallRedMissile);

            // Ice Blue Arrow that Shatters on Impact
            ParamDictionary iceBlueArrowOnHitParams = new ParamDictionary();
            iceBlueArrowOnHitParams.Add("攻击力加成", 0.9f);
            iceBlueArrowOnHitParams.Add("基础暴击率", 0.035f);
            iceBlueArrowOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 14");
            iceBlueArrowOnHitParams.Add("特效绑定点", "Body");
            BulletModel iceBlueArrow = new BulletModel(
                "Ice Blue Arrow that Shatters on Impact", "HovlBullet/Projectile 14", "Effect/Hovl/Flash/Flash 14",
                "", new object[0],
                "CommonBulletHit", iceBlueArrowOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 14" }
            );
            data.Add("Ice Blue Arrow that Shatters on Impact", iceBlueArrow);

            // Peach-colored Missile that Explodes on Impact
            ParamDictionary peachMissileOnHitParams = new ParamDictionary();
            peachMissileOnHitParams.Add("攻击力加成", 0.95f);
            peachMissileOnHitParams.Add("基础暴击率", 0.04f);
            peachMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 15");
            peachMissileOnHitParams.Add("特效绑定点", "Body");
            BulletModel peachMissile = new BulletModel(
                "Peach-colored Missile that Explodes on Impact", "HovlBullet/Projectile 15", "Effect/Hovl/Flash/Flash 15",
                "", new object[0],
                "CommonBulletHit", peachMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 15" }
            );
            data.Add("Peach-colored Missile that Explodes on Impact", peachMissile);

            // Flame Shot
            ParamDictionary flameShotOnHitParams = new ParamDictionary();
            flameShotOnHitParams.Add("攻击力加成", 1.0f);
            flameShotOnHitParams.Add("基础暴击率", 0.05f);
            flameShotOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 16");
            flameShotOnHitParams.Add("特效绑定点", "Body");
            BulletModel flameShot = new BulletModel(
                "Flame Shot", "HovlBullet/Projectile 16", "Effect/Hovl/Flash/Flash 16",
                "", new object[0],
                "CommonBulletHit", flameShotOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 16" }
            );
            data.Add("Flame Shot", flameShot);

            // Lightning Orb
            ParamDictionary lightningOrbOnHitParams = new ParamDictionary();
            lightningOrbOnHitParams.Add("AoE发射信息", new AoeLauncher(DesignerTables.AoE.data["LightningOrbExplosion"], null, Vector3.zero, 2f, 3f, 0f));
            BulletModel lightningOrb = new BulletModel(
                "Lightning Orb", "HovlBullet/Projectile 17", "Effect/Hovl/Flash/Flash 17",
                "", new object[0],
                "CreateAoEOnHit", lightningOrbOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 17" }
            );
            data.Add("Lightning Orb", lightningOrb);

            // Fire Orb
            ParamDictionary fireOrbOnHitParams = new ParamDictionary();
            fireOrbOnHitParams.Add("爆炸效果", new AoeLauncher(DesignerTables.AoE.data["FireOrbExplosion"], null, Vector3.zero, 2f, 3f, 0f));
            BulletModel fireOrb = new BulletModel(
                "Fire Orb", "HovlBullet/Projectile 18", "Effect/Hovl/Flash/Flash 18",
                "", new object[0],
                "CreateAoEOnHit", fireOrbOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 18" }
            );
            data.Add("Fire Orb", fireOrb);

            // Red Orb
            ParamDictionary redOrbOnHitParams = new ParamDictionary();
            redOrbOnHitParams.Add("爆炸效果", new AoeLauncher(DesignerTables.AoE.data["RedOrbExplosion"], null, Vector3.zero, 2f, 3f, 0f));
            BulletModel redOrb = new BulletModel(
                "Red Orb", "HovlBullet/Projectile 19", "Effect/Hovl/Flash/Flash 19",
                "", new object[0],
                "CreateAoEOnHit", redOrbOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 19" }
            );
            data.Add("Red Orb", redOrb);

            // Purple Arrow
            ParamDictionary purpleArrowOnHitParams = new ParamDictionary();
            purpleArrowOnHitParams.Add("攻击力加成", 0.9f);
            purpleArrowOnHitParams.Add("基础暴击率", 0.04f);
            purpleArrowOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 20");
            purpleArrowOnHitParams.Add("特效绑定点", "Body");
            BulletModel purpleArrow = new BulletModel(
                "Purple Arrow", "HovlBullet/Projectile 20", "",
                "", new object[0],
                "CommonBulletHit", purpleArrowOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 20" }
            );
            data.Add("Purple Arrow", purpleArrow);

            // High-Speed Rocket
            ParamDictionary highSpeedRocketOnHitParams = new ParamDictionary();
            highSpeedRocketOnHitParams.Add("攻击力加成", 1.3f);
            highSpeedRocketOnHitParams.Add("基础暴击率", 0.065f);
            highSpeedRocketOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 21");
            highSpeedRocketOnHitParams.Add("特效绑定点", "Body");
            BulletModel highSpeedRocket = new BulletModel(
                "High-Speed Rocket", "HovlBullet/Projectile 21", "",
                "", new object[0],
                "CommonBulletHit", highSpeedRocketOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 21" }
            );
            data.Add("High-Speed Rocket", highSpeedRocket);

            // Star Strike
            ParamDictionary starStrikeOnHitParams = new ParamDictionary();
            starStrikeOnHitParams.Add("攻击力加成", 1.0f);
            starStrikeOnHitParams.Add("基础暴击率", 0.05f);
            starStrikeOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 22");
            starStrikeOnHitParams.Add("特效绑定点", "Body");
            BulletModel starStrike = new BulletModel(
                "Star Strike", "HovlBullet/Projectile 22", "Effect/Hovl/Flash/Flash 22",
                "", new object[0],
                "CommonBulletHit", starStrikeOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 22" }
            );
            data.Add("Star Strike", starStrike);

            // Gold Nugget Strike
            ParamDictionary goldNuggetStrikeOnHitParams = new ParamDictionary();
            goldNuggetStrikeOnHitParams.Add("攻击力加成", 0.85f);
            goldNuggetStrikeOnHitParams.Add("基础暴击率", 0.04f);
            goldNuggetStrikeOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 23");
            goldNuggetStrikeOnHitParams.Add("特效绑定点", "Body");
            BulletModel goldNuggetStrike = new BulletModel(
                "Gold Nugget Strike", "HovlBullet/Projectile 23", "Effect/Hovl/Flash/Flash 23",
                "", new object[0],
                "CommonBulletHit", goldNuggetStrikeOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 23" }
            );
            data.Add("Gold Nugget Strike", goldNuggetStrike);

            // Green Cannon
            ParamDictionary greenCannonOnHitParams = new ParamDictionary();
            greenCannonOnHitParams.Add("攻击力加成", 1.1f);
            greenCannonOnHitParams.Add("基础暴击率", 0.055f);
            greenCannonOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 24");
            greenCannonOnHitParams.Add("特效绑定点", "Body");
            BulletModel greenCannon = new BulletModel(
                "Green Cannon", "HovlBullet/Projectile 24", "Effect/Hovl/Flash/Flash 24",
                "", new object[0],
                "CommonBulletHit", greenCannonOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 24" }
            );
            data.Add("Green Cannon", greenCannon);

            // Red Cannon
            ParamDictionary redCannonOnHitParams = new ParamDictionary();
            redCannonOnHitParams.Add("攻击力加成", 1.2f);
            redCannonOnHitParams.Add("基础暴击率", 0.06f);
            redCannonOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 25");
            redCannonOnHitParams.Add("特效绑定点", "Body");
            BulletModel redCannon = new BulletModel(
                "Red Cannon", "HovlBullet/Projectile 25", "Effect/Hovl/Flash/Flash 25",
                "", new object[0],
                "CommonBulletHit", redCannonOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 25" }
            );
            data.Add("Red Cannon", redCannon);

            // Lightning Missile
            ParamDictionary lightningMissileOnHitParams = new ParamDictionary();
            lightningMissileOnHitParams.Add("攻击力加成", 1.0f);
            lightningMissileOnHitParams.Add("基础暴击率", 0.05f);
            lightningMissileOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 26");
            lightningMissileOnHitParams.Add("特效绑定点", "Body");
            BulletModel lightningMissile = new BulletModel(
                "Lightning Missile", "HovlBullet/Projectile 26", "Effect/Hovl/Flash/Flash 26",
                "", new object[0],
                "CommonBulletHit", lightningMissileOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 26" }
            );
            data.Add("Lightning Missile", lightningMissile);

            // Enchanting Red Heart
            ParamDictionary enchantingRedHeartOnHitParams = new ParamDictionary();
            enchantingRedHeartOnHitParams.Add("攻击力加成", 0.75f);
            enchantingRedHeartOnHitParams.Add("基础暴击率", 0.03f);
            enchantingRedHeartOnHitParams.Add("命中视觉特效", "Effect/Hovl/Hit/Hit 27");
            enchantingRedHeartOnHitParams.Add("特效绑定点", "Body");
            BulletModel enchantingRedHeart = new BulletModel(
                "Enchanting Red Heart", "HovlBullet/Projectile 27", "Effect/Hovl/Flash/Flash 27",
                "", new object[0],
                "CommonBulletHit", enchantingRedHeartOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/Hovl/Hit/Hit 27" }
            );
            data.Add("Enchanting Red Heart", enchantingRedHeart);

            // The Bone-Chilling Spiritual Fire Bullet
            ParamDictionary boneChillingOnHitParams = new ParamDictionary();
            boneChillingOnHitParams.Add("攻击力加成", 1.0f);
            boneChillingOnHitParams.Add("基础暴击率", 0.05f);
            boneChillingOnHitParams.Add("命中视觉特效", "Effect/HovlHit/Hit 3");
            boneChillingOnHitParams.Add("特效绑定点", "Body");
            BulletModel boneChilling = new BulletModel(
                "TheBoneChillingSpiritualFireBullet", "HovlBullet/Projectile 3", "",
                "", new object[0],
                "TheBoneChillingSpiritualFireBulletHit", boneChillingOnHitParams,
                "CommonBulletRemoved", new object[] { "Effect/HitEffect_A" }
            );
            data.Add("TheBoneChillingSpiritualFireBullet", boneChilling);

            // Cloak Boomerang
            ParamDictionary cloakBoomerangOnHitParams = new ParamDictionary();
            cloakBoomerangOnHitParams.Add("攻击力加成", 1.0f);
            cloakBoomerangOnHitParams.Add("基础暴击率", 0.05f);
            cloakBoomerangOnHitParams.Add("命中视觉特效", "Effect/HitEffect_A");
            BulletModel cloakBoomerang = new BulletModel(
                "cloakBoomerang", "Boomerang", "",
                "", new object[0],
                "CloakBoomerangHit", cloakBoomerangOnHitParams,
                "", new object[0],
                MoveType.fly, false, 0.5f, 99999, 0.5f, true, true
            );
            data.Add("cloakBoomerang", cloakBoomerang);
        }
    }
}