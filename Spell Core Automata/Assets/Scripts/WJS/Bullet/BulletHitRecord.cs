using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    public class BulletHitRecord : MonoBehaviour
    {
        ///<summary>
        ///角色的GameObject
        ///</summary>
        public GameObject target;

        ///<summary>
        ///多久之后还能再次命中，单位秒
        ///</summary>
        public float timeToCanHit;

        public BulletHitRecord(GameObject character, float timeToCanHit)
        {
            this.target = character;
            this.timeToCanHit = timeToCanHit;
        }
    }
}

