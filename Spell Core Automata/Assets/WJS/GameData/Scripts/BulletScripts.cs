using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class BulletScripts
    {
        public static Dictionary<string, BulletOnCreate> onCreateFunc = new Dictionary<string, BulletOnCreate>()
        {

        };

        public static Dictionary<string, BulletOnHit> onHitFunc = new Dictionary<string, BulletOnHit>()
        {

        };

        public static Dictionary<string, BulletOnRemoved> onRemovedFunc = new Dictionary<string, BulletOnRemoved>()
        {

        };

        public static Dictionary<string, BulletTween> bulletTween = new Dictionary<string, BulletTween>()
        {

        };

        public static Dictionary<string, BulletTargettingFunction> targettingFunc = new Dictionary<string, BulletTargettingFunction>()
        {
        };
    }

}
