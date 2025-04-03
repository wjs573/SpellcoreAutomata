using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DesignerScripts;
using UnityEngine;

public class LaserState : MonoBehaviour
{
    ///<summary>
    ///要发射的激光
    ///</summary>
    public LaserModel model;

    ///<summary>
    ///要发射激光的这个人的gameObject，这里就认角色（拥有ChaState的）
    ///当然可以是null发射的，但是写效果逻辑的时候得小心caster是null的情况
    ///</summary>
    public GameObject caster;

    ///<summary>
    ///激光发射时候，caster的属性，如果caster不存在，就会是一个ChaProperty.zero
    ///在一些设计中，比如wow的技能中，技能效果是跟发出时候的角色状态有关的，之后即使获得或者取消了buff，更换了装备，数值一样不会受到影响，所以得记录这个释放当时的值
    ///</summary>
    public ChaProperty propWhileCast = ChaProperty.zero;

    ///<summary>
    ///发射的坐标，y轴是无效的
    ///</summary>
    public Vector3 firePosition;
    public Transform firePositionTransform;

    public TimelineObj launchingLaserTimelineObj;

    /// <summary>
    /// 瞄准模式
    /// 分为 玩家鼠标瞄准 和 自动瞄准
    /// 自动瞄准依赖UnitGetTarget组件
    /// 处理逻辑放在LaserManager
    /// </summary>
    public AimType AimType;

    ///<summary>
    ///激光的生命周期，单位：秒
    ///激光应该是有个生命周期的，因为如果总是不命中，也不回收总不好
    ///</summary>
    public float duration;

    ///<summary>
    ///激光已经存在了多久了，单位：秒
    ///毕竟duration是可以被重设的，比如经过一个aoe，生命周期减半了
    ///</summary>
    public float timeElapsed = 0;

    /// <summary>
    /// 激光上次命中敌人的时间，单位：秒
    /// 当激光命中敌人的时候，设置为duration
    /// 创建时设置为duration
    /// 针对可以多次命中敌人的激光射击的，例如激光攻击a单位，过了一段时间后才允许攻击下一个单位
    /// </summary>
    public float lastHitTime;

    ///<summary>
    ///激光的一些特殊逻辑使用的参数，可以在创建时候传递给激光
    ///</summary>
    public Dictionary<string, object> param;

    ///<summary>
    ///命中纪录
    ///</summary>
    public List<BulletHitRecord> hitRecords = new List<BulletHitRecord>();

    private UnitRotate unitRotate;
    public GameObject viewContainer;

    /// <summary>
    /// 绘制激光的组件
    /// </summary>
    public LineRenderer Laser;

    public float MainTextureLength = 1f;
    public float NoiseTextureLength = 1f;
    public Vector4 Length = new Vector4(1, 1, 1, 1);
    public GameObject FlashEffectGameObject;
    public GameObject HitEffectGameObject;
    public ParticleSystem[] FlashEffects;
    public ParticleSystem[] HitEffects;

    public Dictionary<GameObject, GameObject> HitEffectsDict;

    private void Awake()
    {
        if (!unitRotate) unitRotate = gameObject.GetComponent<UnitRotate>();
        HitEffectsDict = new Dictionary<GameObject, GameObject>();
    }

    public void InitEffects()
    {
        //激光绘制组件及特效初始化
        Laser = viewContainer.transform.GetChild(0).GetComponent<LineRenderer>();
        FlashEffectGameObject = viewContainer.transform.GetChild(0).Find("Flash").gameObject;
        HitEffectGameObject = viewContainer.transform.GetChild(0).Find("Hit").gameObject;
        FlashEffects = FlashEffectGameObject.GetComponentsInChildren<ParticleSystem>();
        HitEffects = HitEffectGameObject.GetComponentsInChildren<ParticleSystem>();
    }

    public GameObject CreateHitEffect(GameObject target)
    {
        return Instantiate(HitEffectGameObject, target.transform.position, Quaternion.identity, HitEffectGameObject.transform.parent);
    }

    public void UpdateHitEffect(List<GameObject> newTargets)
    {
        //更新击中特效
        //清理不存在newTargets中的击中特效
        List<GameObject> keysToRemove = new List<GameObject>();
        foreach (var hitEffect in HitEffectsDict)
        {
            if (!newTargets.Contains(hitEffect.Key))
            {
                Destroy(hitEffect.Value);
                keysToRemove.Add(hitEffect.Key);
            }
        }
        // 在遍历完成后进行删除
        foreach (var key in keysToRemove)
        {
            HitEffectsDict.Remove(key);
        }
        //创建不存在HitEffectsDict中的击中特效
        foreach (var newTarget in newTargets)
        {
            if (!HitEffectsDict.ContainsKey(newTarget))
            {
                var hitEffect = CreateHitEffect(newTarget);
                HitEffectsDict.Add(newTarget, hitEffect);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position = firePositionTransform.position;
    }

    /// <summary>
    /// 通过发射器初始化激光的各项属性
    /// </summary>
    /// <param name="laserLauncher"></param>
    public void InitByLaserLauncher(LaserLauncher laserLauncher)
    {
        this.model = laserLauncher.model;
        this.caster = laserLauncher.caster;
        if (this.caster && caster.GetComponent<ChaState>())
        {
            this.propWhileCast = caster.GetComponent<ChaState>().property;
        }
        this.duration = laserLauncher.duration;
        this.lastHitTime = 0;
        this.timeElapsed = 0;
        this.firePositionTransform = laserLauncher.firePositionTransform;
        this.AimType = laserLauncher.aimType;

        this.param = new Dictionary<string, object>();
        if (laserLauncher.param != null)
        {
            foreach (KeyValuePair<string, object> kv in laserLauncher.param)
            {
                this.param.Add(kv.Key, kv.Value);
            }
        }

        SetRotationToMousePosition();

        //把视觉特效补充给bulletObj
        if (this.model.prefab != "")
        {
            GameObject laserEffect = Instantiate(
                Resources.Load<GameObject>("Prefabs/Laser/" + this.model.prefab),
                Vector3.zero,
                Quaternion.identity,
                viewContainer.transform
            );
            laserEffect.transform.localPosition = new Vector3(0, this.gameObject.transform.position.y, 0);
            laserEffect.transform.localRotation = Quaternion.identity;
            InitEffects();
        }
        //gameObject.name = model.id;
    }

    public void OrderRotateTo(Vector3 targetPos)
    {
        unitRotate.RotateTo(targetPos.x, targetPos.z);
    }

    public void OrderRotateTo(GameObject target)
    {
        unitRotate.RotateTo(target.transform.position.x, target.transform.position.z);
    }

    public void OrderRotateTo(float degree)
    {
        unitRotate.RotateTo(degree);
    }

    ///<summary>
    ///判断激光是否还能击中某个GameObject
    ///<param name="target">目标gameObject</param>
    ///</summary>
    public bool CanHit(GameObject target)
    {
        for (int i = 0; i < this.hitRecords.Count; i++)
        {
            if (hitRecords[i].target == target)
            {
                return false;
            }
        }

        ChaState cs = target.GetComponent<ChaState>();
        if (cs && cs.immuneTime > 0) return false;

        return true;
    }

    ///<summary>
    ///添加命中纪录
    ///<param name="target">目标GameObject</param>
    ///</summary>
    public void AddHitRecord(GameObject target)
    {
        hitRecords.Add(new BulletHitRecord(
            target,
            this.model.sameTargetDelay
        ));
    }

    public void RotateToMousePosition()
    {
        Vector2 cursorPos = Input.mousePosition;

        if (Camera.main)
        {
            Vector2 mScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, firePositionTransform.position);
            float rotateTo = 45f + Mathf.Atan2(cursorPos.x - mScreenPos.x, cursorPos.y - mScreenPos.y) * 180.00f / Mathf.PI;
            OrderRotateTo(rotateTo);
        }
    }

    public void SetRotationToMousePosition()
    {
        Vector2 cursorPos = Input.mousePosition;

        if (Camera.main)
        {
            Vector2 mScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, firePositionTransform.position);
            float rotateTo = 45f + Mathf.Atan2(cursorPos.x - mScreenPos.x, cursorPos.y - mScreenPos.y) * 180.00f / Mathf.PI;
            unitRotate.SetRotation(rotateTo);
        }
    }
}

public enum AimType { MouserPosition = 0, AutoAim = 1 }