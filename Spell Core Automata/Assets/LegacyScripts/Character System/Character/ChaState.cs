using System.Collections.Generic;
using System.Text.RegularExpressions;
using JinShan;
using Sirenix.OdinInspector;
using UnityEngine;

///<summary>
///角色的“状态”，用来管理当前应该怎么移动、应该怎么旋转、应该怎么播放动画的。
///是一个角色的总的“调控中心”。
///</summary>
public class ChaState : MonoBehaviour
{
    ///<summary>
    //角色最终的可操作性状态
    ///</summary>
    private ChaControlState _controlState = new ChaControlState(true, true, true);

    ///<summary>
    ///GameTimeline专享的ChaControlState
    ///</summary>
    [ShowInInspector]
    public ChaControlState timelineControlState = new ChaControlState(true, true, true);

    public ChaControlState controlState
    {
        get
        {
            return this._controlState + this.timelineControlState;
        }
    }

    ///<summary>
    ///角色的无敌状态持续时间，如果在无敌状态中，子弹不会碰撞，DamageInfo处理无效化
    ///单位：秒
    ///</summary>
    public float immuneTime
    {
        get
        {
            return _immuneTime;
        }
        set
        {
            _immuneTime = Mathf.Max(_immuneTime, value);
        }
    }
    [ShowInInspector]
    private float _immuneTime = 0.00f;

    ///<summary>
    ///角色是否处于一种蓄力的状态
    ///</summary>
    public bool charging = false;

    ///<summary>
    ///角色主动期望的移动方向
    ///</summary>
    public float moveDegree
    {
        get
        {
            return _wishToMoveDegree;
        }
    }

    private float _wishToMoveDegree = 0.00f;

    ///<summary>
    ///角色主动期望的面向
    ///</summary>
    public float faceDegree
    {
        get
        {
            return _wishToFaceDegree;
        }
    }

    private float _wishToFaceDegree = 0.00f;

    ///<summary>
    ///角色是否已经死了，这不由我这个系统判断，其他系统应该告诉我
    ///</summary>
    public bool dead = false;

    //来自操作或者ai的移动请求信息
    private Vector3 moveOrder = new Vector3();

    [ShowInInspector]
    private List<MovePreorder> forceMove = new List<MovePreorder>();

    //收到的来自各方的播放动画的请求
    private List<string> animOrder = new List<string>();

    //来自操作或者ai的旋转角度请求
    private float rotateToOrder;

    //来自强制执行的旋转角度
    private List<float> forceRotate = new List<float>();

    ///<summary>
    ///角色现有的资源，比如hp之类的
    ///</summary>
    public ChaResource resource = new ChaResource(1);

    [Tooltip("角色所处阵营，阵营不同就会对打")]
    public int side = 0;

    ///<summary>
    ///根据tags可以判断出这是什么样的人
    ///</summary>
    public string[] tags = new string[0];

    ///<summary>
    ///角色当前的属性
    ///</summary>
    public ChaProperty property
    {
        get
        {
            return _prop;
        }
    }

    [ShowInInspector]
    public ChaProperty currentProperty;

    private ChaProperty _prop = ChaProperty.zero;

    ///<summary>
    ///角色移动力，单位：米/秒
    ///</summary>
    public float moveSpeed
    {
        get
        {
            //这个公式也可以通过给策划脚本接口获得，这里就写代码里了，不走策划脚本了
            //设定，值=0.2+5.6*x/(x+100)，初始速度是100，移动力3米/秒，最小值0.2米/秒。
            //如果是负数 就不能移动
            return this._prop.moveSpeed > 0 ? this._prop.moveSpeed * 5.600f / (this._prop.moveSpeed + 100.000f) + 0.200f : 0f;
        }
    }

    ///<summary>
    ///角色行动速度，是一个timescale，最小0.1，初始行动速度值也是100。
    ///</summary>
    public float actionSpeed
    {
        get
        {
            return this._prop.actionSpeed * 4.90f / (_prop.actionSpeed + 390.00f) + 0.100f;
        }
    }

    ///<summary>
    ///角色的基础属性，也就是每个角色“裸体”且不带任何buff的“纯粹的属性”
    ///先写死，正式的应该读表
    ///</summary>
    public ChaProperty baseProp = new ChaProperty(
        100, 0, 100,
        100, 10, 200, 20, 100,
        10, 100, 0,
        1.5f, 0.25f, 0.05f, 0.25f, 0.4f, MoveType.ground
    );

    ///<summary>
    ///角色来自buff的属性
    ///这个数组并不是说每个buff可以占用一条数据，而是分类总和
    ///在这个游戏里buff带来的属性总共有2类，plus和times，用策划设计的公式就是plus的属性加完之后乘以times的属性
    ///所以数组长度其实只有2：[0]buffPlus, [1]buffTimes
    ///</summary>
    public ChaProperty[] buffProp = new ChaProperty[2] { ChaProperty.zero, ChaProperty.zero };

    ///<summary>
    ///来自装备的属性
    ///</summary>
    public ChaProperty equipmentProp = ChaProperty.zero;

    ///<summary>
    ///角色的技能
    ///</summary>
    [ShowInInspector]
    public List<SkillObj> skills = new List<SkillObj>();

    ///<summary>
    ///角色身上的buff
    ///</summary>
    [ShowInInspector]
    public List<BuffObj> buffs = new List<BuffObj>();

    /// <summary>
    /// 法宝装备栏
    /// </summary>
    public InventoryObject FaBao_Equippment_Inventory;

    private UnitMove unitMove;
    private UnitAnim unitAnim;
    private UnitRotate unitRotate;
    private Animator animator;
    private UnitBindManager bindPoints;
    private GameObject viewContainer;
    private UnitFeedback unitFeedbacks;

    private bool isForceMoving = false;

    private void Start()
    {

        rotateToOrder = transform.rotation.eulerAngles.y;
        SynchronizedUnits();

        AttrRecheck();
    }

    private void FixedUpdate()
    {
        float timePassed = Time.fixedDeltaTime;
        if (dead == false)
        {
            //如果角色没死，做这些事情：

            //无敌时间减少
            if (_immuneTime > 0) _immuneTime -= timePassed;

            //技能冷却时间
            for (int i = 0; i < this.skills.Count; i++)
            {
                if (this.skills[i].cooldown > 0)
                {
                    this.skills[i].cooldown -= timePassed;
                }
            }

            //对身上的buff进行管理
            List<BuffObj> toRemove = new List<BuffObj>();
            for (int i = 0; i < this.buffs.Count; i++)
            {
                if (buffs[i].permanent == false) buffs[i].duration -= timePassed;
                buffs[i].timeElapsed += timePassed;

                if (buffs[i].model.tickTime > 0 && buffs[i].model.onTick != null)
                {
                    //float取模不精准，所以用x1000后的整数来
                    if (Mathf.RoundToInt(buffs[i].timeElapsed * 1000) % Mathf.RoundToInt(buffs[i].model.tickTime * 1000) <= 19)
                    {
                        buffs[i].model.onTick.Invoke(buffs[i]);
                        buffs[i].ticked += 1;
                    }
                }

                //只要duration <= 0，不管是否是permanent都移除掉
                if (buffs[i].duration <= 0 || buffs[i].stack <= 0)
                {
                    buffs[i].model.onRemoved?.Invoke(buffs[i]);
                    toRemove.Add(buffs[i]);

                    //移除buff回调点 在移除这个buff的时候尝试移除这个buff对应的美术效果
                    StopSightEffect(buffs[i].model.bindPointKey, buffs[i].model.id);
                }
            }
            if (toRemove.Count > 0)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    this.buffs.Remove(toRemove[i]);
                }
                AttrRecheck();
            }

            toRemove = null;

            //给各个系统发消息
            bool wishToMove = moveOrder != Vector3.zero;
            if (wishToMove == true)
                _wishToMoveDegree = Mathf.Atan2(moveOrder.x, moveOrder.z) * 180 / Mathf.PI;

            ChaControlState curCS = this.controlState + timelineControlState;

            //首先是合并移动信息，发送给UnitMove
            bool tryRun = curCS.canMove == true && moveOrder != Vector3.zero;
            float tryMoveDegree = Mathf.Atan2(moveOrder.x, moveOrder.z) * 180 / Mathf.PI;
            if (tryMoveDegree > 180) tryMoveDegree -= 360;
            if (unitMove)
            {
                if (curCS.canMove == false) moveOrder = Vector3.zero;
                int fmIndex = 0;
                while (fmIndex < forceMove.Count)
                {
                    moveOrder += forceMove[fmIndex].VeloInTime(timePassed);
                    if (forceMove[fmIndex].duration <= 0)
                    {
                        forceMove.RemoveAt(fmIndex);
                    }
                    else
                    {
                        fmIndex++;
                    }
                }
                unitMove.MoveBy(moveOrder);
                moveOrder = Vector3.zero;

                // 在 forceMove 结束时更新状态
                if (isForceMoving && forceMove.Count == 0)
                {
                    isForceMoving = false;
                    unitMove.StopForceMove();  // 假设你在 UnitMove 中添加了 StopForceMove 方法
                }
            }

            _wishToFaceDegree = rotateToOrder;
            if (wishToMove == false) _wishToMoveDegree = _wishToFaceDegree;
            //然后是旋转信息
            if (unitRotate)
            {
                if (curCS.canRotate == false) rotateToOrder = transform.rotation.eulerAngles.y;
                for (int i = 0; i < forceRotate.Count; i++)
                {
                    //这里全是增量，而不是设定为，所以可以直接加
                    rotateToOrder += forceRotate[i];
                }
                unitRotate.RotateTo(rotateToOrder);
                forceRotate.Clear();
            }
            //再是动画处理
            if (unitAnim)
            {
                unitAnim.timeScale = this.actionSpeed;
                //先计算默认（规则下）的动画，并且添加到动画组
                if (tryRun == false)
                {
                    animOrder.Add("Stand");    //如果没有要求移动，就用站立
                }
                else
                {
                    string tt = Utils.GetTailStringByDegree(transform.rotation.eulerAngles.y, tryMoveDegree);
                    animOrder.Add("Move" + tt);
                }
                //送给动画系统处理
                for (int i = 0; i < animOrder.Count; i++)
                {
                    unitAnim.Play(animOrder[i]);
                }
                animOrder.Clear();
            }
            if (animator)
            {
                animator.speed = this.actionSpeed;
            }
        }
        else
        {
            _wishToFaceDegree = transform.rotation.eulerAngles.y * 180.00f / Mathf.PI;
            _wishToMoveDegree = _wishToFaceDegree;
        }
    }

    private void SynchronizedUnits()
    {
        if (!unitMove) unitMove = this.gameObject.GetComponent<UnitMove>();
        if (!unitAnim) unitAnim = this.gameObject.GetComponent<UnitAnim>();
        if (!unitRotate) unitRotate = this.gameObject.GetComponent<UnitRotate>();
        if (!animator) animator = this.gameObject.GetComponent<Animator>();
        if (!bindPoints) bindPoints = this.gameObject.GetComponent<UnitBindManager>();
        if (!viewContainer) viewContainer = this.gameObject.GetComponentInChildren<ViewContainer>().gameObject;
        if (!unitFeedbacks) unitFeedbacks = this.gameObject.GetComponentInChildren<UnitFeedback>();
    }

    ///<summary>
    ///命令移动
    ///<param name="move">移动力</param>
    ///</summary>
    public void OrderMove(Vector3 move)
    {
        if (controlState.canMove)
        {
            this.moveOrder.x = move.x;
            this.moveOrder.z = move.z;
        }
    }

    ///<summary>
    ///强制移动
    ///<param name="moveInfo">移动信息</param>
    ///</summary>
    public void AddForceMove(MovePreorder move)
    {
        if (forceMove.Count == 0)
        {
            // 当开始 forceMove 时
            isForceMoving = true;
            unitMove.StartForceMove();
        }
        this.forceMove.Add(move);
    }

    ///<summary>
    ///命令旋转到多少度
    ///<param name="degree">旋转目标</param>
    ///</summary>
    public void OrderRotateTo(float degree)
    {
        this.rotateToOrder = degree;
    }

    ///<summary>
    ///强制旋转的力量
    ///<param name="degree">偏移角度</param>
    ///</summary>
    public void AddForceRotate(float degree)
    {
        this.forceRotate.Add(degree);
    }

    ///<summary>
    ///添加角色要做的动作请求
    ///<param name="animName">要做的动作</param>
    ///</summary>
    public void Play(string animName)
    {
        animOrder.Add(animName);
    }

    ///<summary>
    ///杀死这个角色
    ///</summary>
    [Button("Kill")]
    public void Kill()
    {
        this.dead = true;
        if (unitAnim)
        {
            unitAnim.Play("Dead");
        }

        //删除人物表中的引用
        if (GameManager.Instance.Characters.Contains(gameObject))
        {
            GameManager.Instance.Characters.Remove(gameObject);

            if (MobSpawnManager.Instance.battleSpawnData != null)
            {
                foreach (MobSpawnInfo mobSpawnInfo in MobSpawnManager.Instance.battleSpawnData.data.Values)
                {
                    if (mobSpawnInfo.GetEnemyData().data.Name == Regex.Replace(gameObject.name, @"\d+$", "") && mobSpawnInfo.gameObjectsOfMobs.Contains(gameObject))
                    {
                        mobSpawnInfo.gameObjectsOfMobs.Remove(gameObject);
                        //现存数量减一
                        mobSpawnInfo.currentCount -= 1;
                        //死亡数量加一
                        mobSpawnInfo.deathCount += 1;
                    }
                }
            }
        }
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false; //禁用碰撞
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true; //禁用物理
        this.gameObject.AddComponent<UnitRemover>().duration = 1f;
        if (SceneVariants.MainActor() == null) return; ;
        //如果不是主角，说明是主角击杀怪物，主角可以获得经验
        if (this.side != SceneVariants.MainActor().GetComponent<ChaState>().side)
        {
            //MainCharacter.Instance.AddExp(10);
            MainCharacter.Instance.AddCoin(10);
        }
    }

    ///<summary>
    ///重新计算所有属性，并且获得一个最终属性
    ////其实这个应该走脚本函数返回，抛给脚本函数多个ChaProperty，由脚本函数运作他们的运算关系，并返回结果
    ///</summary>
    public void AttrRecheck()
    {
        _controlState.Origin();
        this._prop.Zero();

        for (var i = 0; i < buffProp.Length; i++) buffProp[i].Zero();
        for (int i = 0; i < this.buffs.Count; i++)
        {
            for (int j = 0; j < Mathf.Min(buffProp.Length, buffs[i].model.propMod.Length); j++)
            {
                buffProp[j] += buffs[i].model.propMod[j] * buffs[i].stack;
            }
            _controlState += buffs[i].model.stateMod;
        }

        ////计算装备提供的属性
        equipmentProp = ChaProperty.zero;
        if (FaBao_Equippment_Inventory != null)
        {
            equipmentProp += FaBao_Equippment_Inventory.GetTotalProperty();
        }

        this._prop = (this.baseProp + this.equipmentProp + this.buffProp[0]) * this.buffProp[1];

        if (unitMove)
        {
            unitMove.bodyRadius = this._prop.bodyRadius;
        }

        currentProperty = new ChaProperty();
        currentProperty = this._prop;
    }

    ///<summary>
    ///增加角色的血量等资源，直接改变数字的，属于最后一步操作了
    ///<param name="value">要改变的量，负数为减少</param>
    ///</summary>
    public void ModResource(ChaResource value)
    {
        //Debug.Log(value.hp);
        //护盾处理
        //处理完护盾值后，再处理生命值
        this.resource.shield += value.shield;
        //角色护盾值必须大于等于0
        this.resource.shield = Mathf.Clamp(this.resource.shield, 0, 99999);

        //生命值处理
        if (value.hp < 0)
        {
            //当前护盾值大于零 且 护盾值足够抵消伤害
            if (this.resource.shield > 0 && (value.hp + this.resource.shield) >= 0)
            {
                this.resource.shield += value.hp;
            }
            //当前护盾值大于零 且 护盾值不足够抵消伤害
            else if (this.resource.shield > 0 && (value.hp + this.resource.shield) < 0)
            {
                int unblocked_damage = this.resource.shield + value.hp;
                this.resource.shield = 0;
                this.resource.hp += unblocked_damage;
            }
            //当前护盾值小于等于零
            else
            {
                this.resource.hp += value.hp;
            }
        }
        else
        {
            //Debug.Log("回复生命值");
            this.resource.hp += value.hp;
        }

        //灵力值处理
        this.resource.mp += value.mp;

        //角色当前生命值和灵力值必须小于等于最大生命值和最大灵力值且大于等于0
        this.resource.hp = Mathf.Clamp(this.resource.hp, 0, this._prop.hp);
        this.resource.mp = Mathf.Clamp(this.resource.mp, 0, this._prop.mp);
        //护盾值必须大于等于0
        this.resource.shield = Mathf.Clamp(this.resource.shield, 0, 99999);
        if (this.resource.hp <= 0)
        {
            this.Kill();
        }
    }

    ///<summary>
    ///在角色身上放一个特效，其实是挂在一个gameObject而已
    ///<param name="bindPointKey">绑点名称，角色有Muzzle/Head/Body这3个，需要再加</param>
    ///<param name="effect">要播放的特效文件名，统一走Prefabs/下拿</param>
    ///<param name="effectKey">这个特效的key，要删除的时候就有用了</param>
    ///<param name="effect">要播放的特效</param>
    ///</summary>
    public void PlaySightEffect(string bindPointKey, string effect, string effectKey = "", bool loop = false)
    {
        if (effect == "")
        {
            return;
        }
        bindPoints.AddBindGameObject(bindPointKey, "Prefabs/" + effect, effectKey, loop);
    }

    ///<summary>
    ///删除角色身上的一个特效
    ///<param name="bindPointKey">绑点名称，角色有Muzzle/Head/Body这3个，需要再加</param>
    ///<param name="effectKey">这个特效的key，要删除的时候就有用了</param>
    ///</summary>
    public void StopSightEffect(string bindPointKey, string effectKey)
    {
        bindPoints.RemoveBindGameObject(bindPointKey, effectKey);
    }

    ///<summary>
    ///判断这个角色是否会被这个damageInfo所杀
    ///<param name="dInfo">要判断的damageInfo</param>
    ///<return>如果是true代表角色可能会被这次伤害所杀</return>
    ///</summary>
    public bool CanBeKilledByDamageInfo(DamageInfo damageInfo)
    {
        if (this.immuneTime > 0 || damageInfo.isHeal() == true) return false;
        int dValue = damageInfo.DamageValue(false).result.damage;
        return dValue >= this.resource.hp;
    }

    ///<summary>
    ///为角色添加buff，当然，删除也是走这个的
    ///</summary>
    public void AddBuff(AddBuffInfo buff)
    {
        List<GameObject> bCaster = new List<GameObject>();
        if (buff.caster) bCaster.Add(buff.caster);
        List<BuffObj> hasOnes = GetBuffById(buff.buffModel.id, bCaster);
        int modStack = Mathf.Min(buff.addStack, buff.buffModel.maxStack);
        bool toRemove = false;
        BuffObj toAddBuff = null;
        if (hasOnes.Count > 0)
        {
            //已经存在
            hasOnes[0].buffParam = new Dictionary<string, object>();
            if (buff.buffParam != null)
            {
                foreach (KeyValuePair<string, object> kv in buff.buffParam) { hasOnes[0].buffParam[kv.Key] = kv.Value; };
            }

            hasOnes[0].duration = (buff.durationSetTo == true) ? buff.duration : (buff.duration + hasOnes[0].duration);
            int afterAdd = hasOnes[0].stack + modStack;
            modStack = afterAdd >= hasOnes[0].model.maxStack ?
                (hasOnes[0].model.maxStack - hasOnes[0].stack) :
                (afterAdd <= 0 ? (0 - hasOnes[0].stack) : modStack);
            hasOnes[0].stack += modStack;
            hasOnes[0].permanent = buff.permanent;
            toAddBuff = hasOnes[0];
            toRemove = hasOnes[0].stack <= 0;
        }
        else
        {
            //新建
            toAddBuff = new BuffObj(
                buff.buffModel,
                buff.caster,
                this.gameObject,
                buff.duration,
                buff.addStack,
                buff.permanent,
                buff.buffParam
            );
            buffs.Add(toAddBuff);
            buffs.Sort((a, b) =>
            {
                return a.model.priority.CompareTo(b.model.priority);
            });

            //添加buff回调点 读取buff的id 在资源中找到对应的美术效果 添加给buff携带者 设置绑点为body 循环播放
            PlaySightEffect(toAddBuff.model.bindPointKey, toAddBuff.model.prefab, toAddBuff.model.id, true);

            //添加debuff时，跳字buff的id
            for (int i = 0; i < toAddBuff.model.tags.Length; i++)
            {
                if (toAddBuff.model.tags[i] == "debuff" || toAddBuff.model.tags[i] == "Control" || toAddBuff.model.tags[i] == "Pop")
                {
                    if (toAddBuff.model.id == "Poisoning")
                    {
                        //ToDo:跳字
                    }
                    break;
                }
            }
        }
        if (toRemove == false && buff.buffModel.onOccur != null)
        {
            buff.buffModel.onOccur.Invoke(toAddBuff, modStack);
        }
        AttrRecheck();
    }

    public bool HasBuff(string id)
    {
        if (GetBuffById(id).Count > 0)
        {
            return true;
        }
        return false;
    }

    ///<summary>
    ///获取角色身上对应的buffObj
    ///<param name="id">buff的model的id</param>
    ///<param name="caster">如果caster不是空，那么就代表只有buffObj.caster在caster里面的才符合条件</param>
    ///<return>符合条件的buffObj数组</return>
    ///</summary>
    public List<BuffObj> GetBuffById(string id, List<GameObject> caster = null)
    {
        List<BuffObj> res = new List<BuffObj>();
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].model.id == id && (caster == null || caster.Count <= 0 || caster.Contains(buffs[i].caster) == true))
            {
                res.Add(buffs[i]);
            }
        }
        return res;
    }

    ///<summary>
    ///根据id获得角色学会的技能（skillObj），如果没有则返回null
    ///<param name="id">技能的id</param>
    ///<return>skillObj or null</return>
    ///</summary>
    public SkillObj GetSkillById(string id)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].model.id == id)
            {
                return skills[i];
            }
        }
        return null;
    }

    ///<summary>
    ///释放一个技能，释放技能并不总是成功的，如果你一直发释放技能的命令，那失败率应该是骤增的
    ///<param name="id">要释放的技能的id</param>
    ///<return>是否释放成功</return>
    ///</summary>
    public bool CastSkill(string id)
    {
        if (id == null)
        {
            return false;
        }
        //目前认为 法宝在状态上是和角色独立的 角色进入无法施法状态 不影响法宝施法
        if (this.controlState.canUseSkill == false) return false; //不能用技能就不放了

        SkillObj skillObj = GetSkillById(id);
        bool castSuccess = false;
        if (skillObj == null || skillObj.cooldown > 0) return false;

        //施法资源 如果是法宝 释放技能应该是消耗父亲的资源
        if (resource.Enough(skillObj.model.condition) == true)
        {
            TimelineObj timeline = new TimelineObj(
                skillObj.model.effect, this.gameObject, new object[] { skillObj }
            );

            //技能生成的TimelineObj要从技能参数中继承技能参数
            if (skillObj.model.skillParams != null)
            {
                foreach (var kvp in skillObj.model.skillParams)
                {
                    timeline.values[kvp.Key] = kvp.Value;
                }
            }

            //buff的oncast  如果是法宝 应该走父亲的buff
            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i].model.onCast != null)
                {
                    timeline = buffs[i].model.onCast.Invoke<TimelineObj>(buffs[i], skillObj, timeline);
                }
            }
            if (timeline != null)
            {
                ModResource(-1 * skillObj.model.cost);
                SceneVariants.CreateTimeline(timeline);
                castSuccess = true;
            }
        }

        if (castSuccess)
        {
            //每100点冷却速率可以降低一半冷却时间
            skillObj.cooldown = skillObj.model.cooldown / (property.cd_speed / 100 + 1);
        }
        else
        {
            skillObj.cooldown = 0.2f;   //无论成功与否，都会进入gcd
        }

        return castSuccess;
    }

    ///<summary>
    ///初始化角色的属性
    ///</summary>
    public void InitBaseProp(ChaProperty cProp)
    {
        this.baseProp = cProp;
        this.AttrRecheck();
        this.resource.hp = _prop.hp;
        this.resource.mp = _prop.mp;
        this.resource.shield = _prop.shield;
        this.resource.mind = _prop.mind;
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            collider.radius = cProp.bodyRadius;
        }
    }

    ///<summary>
    ///学习某个技能
    ///<param name="skillModel">技能的模板</param>
    ///<param name="level">技能等级</param>
    ///</summary>
    public SkillObj LearnSkill(SkillModel skillModel)
    {
        SkillObj skillObj = new SkillObj(skillModel);
        if (!skillModel.canUseOnStart)
        {
            skillObj.cooldown = skillObj.model.cooldown / (property.cd_speed / 100 + 1);
        }

        this.skills.Add(skillObj);
        if (skillModel.buff != null)
        {
            for (int i = 0; i < skillModel.buff.Length; i++)
            {
                AddBuffInfo abi = skillModel.buff[i];
                abi.permanent = true;
                abi.duration = 10;
                abi.durationSetTo = true;
                this.AddBuff(abi);
            }
        }
        return skillObj;
    }

    public SkillObj LearnSkill(SkillObj skillObj)
    {
        this.skills.Add(skillObj);
        if (skillObj.model.buff != null)
        {
            for (int i = 0; i < skillObj.model.buff.Length; i++)
            {
                AddBuffInfo abi = skillObj.model.buff[i];
                abi.permanent = true;
                abi.duration = 10;
                abi.durationSetTo = true;
                this.AddBuff(abi);
            }
        }
        return skillObj;
    }

    public void ForgetSkill(string id)
    {
        SkillObj skillObjToRemove = null;
        foreach (SkillObj skillObj in this.skills)
        {
            if (skillObj.model.id == id)
            {
                skillObjToRemove = skillObj;
                break;
            }
        }

        if (skillObjToRemove != null) this.skills.Remove(skillObjToRemove);
    }

    ///<summary>
    ///设置视觉元素
    ///</summary>
    public void SetView(GameObject view, Dictionary<string, AnimInfo> animInfo)
    {
        if (view == null) return;
        SynchronizedUnits();
        view.transform.SetParent(viewContainer.transform);
        view.transform.position = new Vector3(0, this.gameObject.transform.position.y, 0);
        this.gameObject.transform.position = new Vector3(
            this.gameObject.transform.position.x,
            0,
            this.gameObject.transform.position.z
        );
        this.gameObject.GetComponent<UnitAnim>().animInfo = animInfo;
    }

    ///<summary>
    ///设置无敌时间
    ///<param name="time">无敌的时间，单位：秒</param>
    ///</summary>
    public void SetImmuneTime(float time)
    {
        this._immuneTime = Mathf.Max(this._immuneTime, time);
    }

    public void ImmuneTimeToZero()
    {
        this._immuneTime = 0;
    }

    ///<summary>
    ///是否拥有某个tag
    ///</summary>
    public bool HasTag(string tag)
    {
        if (this.tags == null || this.tags.Length <= 0) return false;
        for (int i = 0; i < this.tags.Length; i++)
        {
            if (tags[i] == tag)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指向最近的敌人
    /// </summary>
    public void RotateToTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }
        float _x = (target.transform.position.x - gameObject.transform.position.x);
        float _z = (target.transform.position.z - gameObject.transform.position.z);
        float degree = (_z > 0) ? Mathf.Atan(_x / _z) * 180.00f / Mathf.PI : Mathf.Atan(_x / _z) * 180.00f / Mathf.PI - 180f;

        OrderRotateTo(degree);
    }

    public void SetRotateToTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }
        float _x = (target.transform.position.x - gameObject.transform.position.x);
        float _z = (target.transform.position.z - gameObject.transform.position.z);
        float degree = (_z > 0) ? Mathf.Atan(_x / _z) * 180.00f / Mathf.PI : Mathf.Atan(_x / _z) * 180.00f / Mathf.PI - 180f;

        unitRotate.SetRotation(degree);
    }

    /// <summary>
    /// 播放feedbacks
    /// </summary>
    /// <param name="name"></param>
    public void PlayFeedbacks(string name)
    {
        unitFeedbacks.play(name);
    }
}