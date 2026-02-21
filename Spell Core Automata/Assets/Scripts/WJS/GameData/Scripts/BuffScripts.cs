using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public static class BuffScripts
    {
        public static Dictionary<string, BuffOnOccur> onOccurFunc = new Dictionary<string, BuffOnOccur>()
        {
            {"ResurrectOnCreate",ResurrectOnCreate }
        };

        public static Dictionary<string, BuffOnRemoved> onRemovedFunc = new Dictionary<string, BuffOnRemoved>()
        {
            {"ResurrectOnRemoved", ResurrectOnRemoved}
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

        private static void ResurrectOnCreate(BuffObj buff, int modifyStack)
        {
            if (buff.carrier.GetComponent<CapsuleCollider>())
            {
                buff.carrier.GetComponent<CapsuleCollider>().enabled = false;
            }
            buff.carrier.GetComponent<UnitAnim>().BufferAnimation = "Resurrect";
            buff.carrier.GetComponent<ChaState>().SetImmuneTime(0.80f);
        }

        private static void ResurrectOnRemoved(BuffObj buff)
        {
            if (buff.carrier.GetComponent<CapsuleCollider>())
            {
                buff.carrier.GetComponent<CapsuleCollider>().enabled = true;
            }
        }

        /// <summary>
        /// OnTick 每秒回复生命值和灵力值
        /// </summary>
        /// <param name="buff"></param>
        private static void BaseRecover(BuffObj buff)
        {
            ChaState carrier = buff.carrier.GetComponent<ChaState>();
            SceneVariants.CreateDamage(buff.carrier, buff.carrier, new Damage(-carrier.property.hp_recover), 0, 0, new DamageInfoTag[] { DamageInfoTag.periodHeal});
            carrier.ModResource(new ChaResource(0, carrier.property.mp_recover));
        }
    }
}
