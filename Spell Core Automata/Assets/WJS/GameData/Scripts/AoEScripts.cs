using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class AoEScripts
    {
        public static Dictionary<string, AoeOnCreate> onCreateFunc = new Dictionary<string, AoeOnCreate>()
        {

        };

        public static Dictionary<string, AoeOnRemoved> onRemovedFunc = new Dictionary<string, AoeOnRemoved>()
        {

        };


        public static Dictionary<string, AoeOnTick> onTickFunc = new Dictionary<string, AoeOnTick>()
        {

        };

        public static Dictionary<string, AoeOnCharacterEnter> onChaEnterFunc = new Dictionary<string, AoeOnCharacterEnter>()
        {

        };

        public static Dictionary<string, AoeOnCharacterLeave> onChaLeaveFunc = new Dictionary<string, AoeOnCharacterLeave>()
        {

        };

        public static Dictionary<string, AoeOnBulletEnter> onBulletEnterFunc = new Dictionary<string, AoeOnBulletEnter>()
        {

        };

        public static Dictionary<string, AoeOnBulletLeave> onBulletLeaveFunc = new Dictionary<string, AoeOnBulletLeave>()
        {

        };

        public static Dictionary<string, AoeTween> aoeTweenFunc = new Dictionary<string, AoeTween>()
        {

        };
    }
}
