using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///子弹的发射信息，专门有个系统会处理这个发射信息，然后往地图上放置出子弹的GameObject
    ///所有脚本中，需要创建一个子弹，也应该传递这个结构作为产生子弹的参数
    ///</summary>
    public class BulletLauncher
    {
        ///<summary>
        ///要发射的子弹
        ///</summary>
        public BulletModel model;

        ///<summary>
        ///要发射子弹的这个人的gameObject，这里就认角色（拥有ChaState的）
        ///当然可以是null发射的，但是写效果逻辑的时候得小心caster是null的情况
        ///</summary>
        public GameObject caster;

        ///<summary>
        ///发射的坐标，y轴是无效的
        ///</summary>
        public Vector3 firePosition;

        ///<summary>
        ///发射的角度，单位：角度
        ///</summary>
        public float fireDegree;

        ///<summary>
        /// 发射的子弹数量
        ///</summary>
        public int shotCount;

        ///<summary>
        /// 子弹的散射角度，单位：度
        ///</summary>
        public float spreadAngle;

        ///<summary>
        /// 投射物与目标方向的随机偏差范围，以度数计算。
        ///</summary>
        public float scatteringDegree;

        ///<summary>
        ///子弹的初速度，单位：米/秒
        ///</summary>
        public float speed;

        ///<summary>
        ///子弹的生命周期，单位：秒
        ///子弹应该是有个生命周期的，因为如果总是不命中，也不回收总不好
        ///当然更多的还是因为有些子弹射程非常短
        ///</summary>
        public float duration;

        ///<summary>
        ///子弹在发射瞬间，可以捕捉一个GameObject作为目标，并且将这个目标传递给BulletTween，作为移动参数
        ///<param name="bullet">是当前的子弹GameObject，不建议公式中用到这个</param>
        ///<param name="targets">所有可以被选作目标的对象，这里是GameManager的逻辑决定的传递过来谁，比如这个游戏子弹只能捕捉角色作为对象，那就是只有角色的GameObject，当然如果需要，加入子弹也不麻烦</param>
        ///<return>在创建子弹的瞬间，根据这个函数获得一个GameObject作为followingTarget</return>
        ///</summary>
        public BulletTargettingFunction targetFunc;

        ///<summary>
        ///子弹的轨迹函数，传入一个时间点，返回出一个Vector3，作为这个时间点的速度和方向，这是个相对于正在飞行的方向的一个偏移（*speed的）
        ///正在飞行的方向按照z轴，来算，也就是说，当你只需要子弹匀速行动的时候，你可以让这个函数只做一件事情——return Vector3.forward。
        ///如果这个值是null，就会跟return Vector3.forward一样处理，性能还高一些。
        ///虽然是vector3，但是y坐标是无效的，只是为了统一单位
        ///比如手榴弹这种会一跳一跳的可不得y变化吗？是要变化，但是这个变化归我管，这是render的事情
        ///简单地说就是做一个跳跳的Component，update（而非fixedupdate）里面去管理跳吧
        ///<param name="t">子弹飞行了多久的时间点，单位秒。</param>
        ///<return>返回这一时间点上的速度和偏移，Vector3就是正常速度正常前进</return>
        ///</summary>
        public BulletTween tween = null;

        ///<summary>
        ///子弹的移动轨迹是否严格遵循发射出来的角度
        ///如果是true，则子弹每一帧Tween返回的角度是按照fireDegree来偏移的
        ///如果是false，则会根据子弹正在飞的角度(transform.rotation)来算下一帧的角度
        ///</summary>
        public bool useFireDegreeForever = false;

        ///<summary>
        ///子弹创建后多久是没有碰撞的，这样比如子母弹之类的，不会在创建后立即命中目标，但绝大多子弹还应该是0的
        ///单位：秒
        ///</summary>
        public float canHitAfterCreated = 0f;

        public BulletLauncher IncreaseSpeed(float amount)
        {
            return new BulletLauncher(
                model, caster, firePosition, fireDegree, speed + amount, duration,
                canHitAfterCreated, tween, targetFunc, useFireDegreeForever, param
            );
        }

        ///<summary>
        ///子弹的一些特殊逻辑使用的参数，可以在创建子的时候传递给子弹
        ///</summary>
        public Dictionary<string, object> param;

        public BulletLauncher(
            BulletModel model, GameObject caster, Vector3 firePos, float degree, float speed, float duration,
            float canHitAfterCreated = 0,
            BulletTween tween = null, BulletTargettingFunction targetFunc = null, bool useFireDegree = false,
            Dictionary<string, object> param = null,
            int shotCount = 1, float spreadAngle = 0f, float scatteringDegree = 0f)
        {
            this.model = model;
            this.caster = caster;
            this.firePosition = firePos;
            this.fireDegree = degree;
            this.speed = speed;
            this.duration = duration;
            this.canHitAfterCreated = canHitAfterCreated;
            this.tween = tween;
            this.useFireDegreeForever = useFireDegree;
            this.targetFunc = targetFunc;
            this.param = param;
            this.shotCount = shotCount;
            this.spreadAngle = spreadAngle;
            this.scatteringDegree = scatteringDegree;

        }

        public static List<BulletLauncher> SplitBulletLauncher(BulletLauncher originalLauncher)
        {
            List<BulletLauncher> bulletLaunchers = new List<BulletLauncher>();

            if (originalLauncher.shotCount == 1)
            {
                // 如果原始的shotCount已经为1，则无需分割，直接返回原始的BulletLauncher
                bulletLaunchers.Add(originalLauncher);
                return bulletLaunchers;
            }

            // 计算每个子弹的散射角度
            float singleSpreadAngle = originalLauncher.spreadAngle / originalLauncher.shotCount;

            // 创建多个新的BulletLauncher对象，每个对象的shotCount为1，fireDegree根据散射角度计算
            for (int i = 0; i < originalLauncher.shotCount; i++)
            {
                float randomDegree = originalLauncher.scatteringDegree;
                if(originalLauncher.caster.GetComponent<UnitWandManager>() != null)
                 randomDegree = Mathf.Clamp(originalLauncher.scatteringDegree +
                 originalLauncher.caster.GetComponent<UnitWandManager>().wand.model.wandData.BaseScatter, 0, 360f);
                float newFireDegree = originalLauncher.fireDegree - (originalLauncher.spreadAngle / 2) + (i * singleSpreadAngle) +
                    Random.Range(-randomDegree, randomDegree);
                BulletLauncher newLauncher = new BulletLauncher(
                    originalLauncher.model, originalLauncher.caster, originalLauncher.firePosition,
                    newFireDegree, originalLauncher.speed, originalLauncher.duration,
                    originalLauncher.canHitAfterCreated, originalLauncher.tween,
                    originalLauncher.targetFunc, originalLauncher.useFireDegreeForever, originalLauncher.param,
                    shotCount: 1, spreadAngle: 0f, scatteringDegree: 0f
                );

                bulletLaunchers.Add(newLauncher);
            }

            return bulletLaunchers;
        }
        public BulletLauncher Clone()
        {
            // 创建一个新的 BulletLauncher 对象，复制所有属性和字段
            BulletLauncher clonedLauncher = new BulletLauncher(
                this.model, this.caster, this.firePosition, this.fireDegree, this.speed, this.duration,
                this.canHitAfterCreated, this.tween, this.targetFunc, this.useFireDegreeForever, this.param,
                this.shotCount, this.spreadAngle, this.scatteringDegree
            );
            return clonedLauncher;
        }

    }
}
