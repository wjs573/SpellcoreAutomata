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
        }
    }
}

