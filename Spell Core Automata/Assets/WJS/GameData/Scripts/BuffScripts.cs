using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public static class BuffScripts
    {
        public static Dictionary<string, BuffOnOccur> onOccurFunc = new Dictionary<string, BuffOnOccur>()
        {

        };

        public static Dictionary<string, BuffOnRemoved> onRemovedFunc = new Dictionary<string, BuffOnRemoved>()
        {

        };


        public static Dictionary<string, BuffOnTick> onTickFunc = new Dictionary<string, BuffOnTick>()
        {

        };

        public static Dictionary<string, BuffOnCast> onCastFunc = new Dictionary<string, BuffOnCast>()
        {

        };

        public static Dictionary<string, BuffOnHit> onHitFunc = new Dictionary<string, BuffOnHit>()
        {

        };

        public static Dictionary<string, BuffOnBeHurt> beHurtFunc = new Dictionary<string, BuffOnBeHurt>()
        {

        };

        public static Dictionary<string, BuffOnKill> onKillFunc = new Dictionary<string, BuffOnKill>()
        {

        };

        public static Dictionary<string, BuffOnBeKilled> beKilledFunc = new Dictionary<string, BuffOnBeKilled>()
        {

        };
    }

}
