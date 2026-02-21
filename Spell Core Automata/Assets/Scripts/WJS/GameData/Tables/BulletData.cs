using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public static class BulletData
    {
        public static Dictionary<string, BulletModel> data = new Dictionary<string, BulletModel>();

        public static void Initialize()
        {
            data = new Dictionary<string, BulletModel>() { };

            // 火焰飞弹
            ParamDictionary fireballOnHitParams = new ParamDictionary();
            fireballOnHitParams.Add("攻击力加成", 1.0f);
            fireballOnHitParams.Add("基础暴击率", 0.05f);
            fireballOnHitParams.Add("命中视觉特效", "FlashExplosionBlue");
            fireballOnHitParams.Add("特效绑定点", "Body");
            BulletModel fireball = new BulletModel(
                "fireball", "FlashMissileBlue", "SparkleMuzzleBlue",
                "", new object[0],
                "CommonBulletHit", fireballOnHitParams,
                "CommonBulletRemoved", new object[] { "FlashExplosionBlue" }
            );
            data.Add("fireball", fireball);
        }
    }
}

