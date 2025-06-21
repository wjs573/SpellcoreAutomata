using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    /// <summary>
    /// 每个回调点都有一系列参数
    /// 这个结构体包含所有回调点的参数
    /// </summary>
    public struct CallBackParams
    {
        /// <summary>
        /// BulletOnCreate
        /// BulletOnHit
        /// BulletOnRemoved
        /// </summary>
        public GameObject bullet;

        /// <summary>
        /// BulletOnHit
        /// BuffOnHit
        /// LaserOnHit
        /// </summary>
        public GameObject target;

        /// <summary>
        /// BuffOnOccur
        /// BuffOnRemoved
        /// BuffOnTick
        /// BuffOnBeHurt
        /// BuffOnKill
        /// BuffOnBeKilled
        /// BuffOnCast
        /// </summary>
        public BuffObj buff;

        /// <summary>
        /// BuffOnOccur
        /// </summary>
        public int modifyStack;

        /// <summary>
        /// BuffOnHit
        /// BuffOnBeHurt
        /// BuffOnKill
        /// BuffOnBeKilled
        /// </summary>
        public DamageInfo damageInfo;

        /// <summary>
        /// BuffOnBeHurt
        /// BuffOnBeKilled
        /// </summary>
        public GameObject attacker;

        /// <summary>
        /// BuffOnCast
        /// </summary>
        public SkillObj skill;

        /// <summary>
        /// AoeOnCreate
        /// AoeOnRemoved
        /// AoeOnTick
        /// AoeOnCharacterEnter
        /// AoeOnCharacterLeave
        /// AoeOnBulletEnter
        /// AoeOnBulletLeave
        /// </summary>
        public GameObject aoe;

        /// <summary>
        /// AoeOnCharacterEnter
        /// AoeOnCharacterLeave
        /// </summary>
        public List<GameObject> cha;

        /// <summary>
        /// AoeOnBulletEnter
        /// AoeOnBulletLeave
        /// </summary>
        public List<GameObject> bullets;

        /// <summary>
        /// LaserOnCreate
        /// LaserOnRemoved
        /// LaserOnHit
        /// </summary>
        public GameObject laser;

        /// <summary>
        /// BuffOnCast
        /// </summary>
        public TimelineObj timeline;

        public List<GameObject> GetTargets()
        {
            List<GameObject> targets = new List<GameObject>();
            if (target != null)
            {
                targets.Add(target);
            }
            if (cha != null)
            {
                targets.AddRange(cha);
            }
            return targets;
        }
    }
}