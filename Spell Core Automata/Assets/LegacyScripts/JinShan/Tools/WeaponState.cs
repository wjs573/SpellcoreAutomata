using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponState : MonoBehaviour
{
    /// <summary>
    /// 是否采用自动瞄准模式
    /// 自动瞄准模式 要求武器装备自动瞄准组件
    /// 自动瞄准组件设置好间隔时间后
    /// 会自动寻找最近的目标，发射子弹
    /// 非自动瞄准模式
    /// 武器会自动使用cd已好的技能
    /// </summary>
    public bool IsAutoAim;

    ///<summary>
    ///角色的技能
    ///</summary>
    [ShowInInspector]
    public List<SkillObj> skills = new List<SkillObj>();

    /// <summary>
    /// 武器拥有者
    /// </summary>
    public ChaState ownerState;

    ///<summary>
    ///武器主动期望的移动方向
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
    ///武器主动期望的面向
    ///</summary>
    public float faceDegree
    {
        get
        {
            return _wishToFaceDegree;
        }
    }
    private float _wishToFaceDegree = 0.00f;

    //来自操作或者ai的移动请求信息
    private Vector3 moveOrder = new Vector3();

    //来自强制发生的位移信息，通常是技能效果等导致的，比如翻滚、被推开等
    private List<MovePreorder> forceMove = new List<MovePreorder>();

    //来自操作或者ai的旋转角度请求
    private float rotateToOrder;

    //来自强制执行的旋转角度
    private List<float> forceRotate = new List<float>();


    private UnitMove unitMove;
    private UnitRotate unitRotate;
    private GameObject viewContainer;

    // Start is called before the first frame update
    void Start()
    {
        //默认为非自动瞄准模式
        IsAutoAim = false;

        //默认转向
        rotateToOrder = 0f;

        rotateToOrder = transform.rotation.eulerAngles.y;

        synchronizedUnits();

    }

    void FixedUpdate()
    {
        //更新物体的转向情况
        unitRotate.RotateTo(rotateToOrder);

        float timePassed = Time.fixedDeltaTime;
        //技能冷却时间
        for (int i = 0; i < this.skills.Count; i++)
        {
            if (this.skills[i].cooldown > 0)
            {
                this.skills[i].cooldown -= timePassed;
            }
        }
    }


    public void SetOwner(ChaState chaState)
    {
        ownerState = chaState;
    }

    ///<summary>
    ///学习某个技能
    ///<param name="skillModel">技能的模板</param>
    ///<param name="level">技能等级</param>
    ///</summary>
    public void LearnSkill(SkillModel skillModel)
    {
        SkillObj skillObj = new SkillObj(skillModel);
        this.skills.Add(skillObj);
        if (!skillModel.canUseOnStart)
        {
            skillObj.cooldown = skillObj.model.cooldown / (ownerState.property.cd_speed / 100 + 1);
        }
        if (skillModel.buff != null)
        {
            for (int i = 0; i < skillModel.buff.Length; i++)
            {
                AddBuffInfo abi = skillModel.buff[i];
                abi.permanent = true;
                abi.duration = 10;
                abi.durationSetTo = true;
                ownerState.AddBuff(abi);
            }
        }
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
        SkillObj skillObj = GetSkillById(id);

        if (skillObj == null || skillObj.cooldown > 0)
        {
            return false;
        }

        bool castSuccess = false;

        if (ownerState.resource.Enough(skillObj.model.condition) == true)
        {
            TimelineObj timeline = new TimelineObj(
                skillObj.model.effect, ownerState.gameObject, new object[] { skillObj }, TimelineType.Weapon, gameObject
            );

            for (int i = 0; i < ownerState.buffs.Count; i++)
            {
                if (ownerState.buffs[i].model.onCast != null)
                {
                    timeline = ownerState.buffs[i].model.onCast.Invoke<TimelineObj>(ownerState.buffs[i], skillObj, timeline);
                }
            }

            if (timeline != null)
            {
                ownerState.ModResource(-1 * skillObj.model.cost);
                SceneVariants.CreateTimeline(timeline);
                castSuccess = true;
            }
            else
            {
                Debug.Log("技能 " + id + " 无法释放：未能创建 TimelineObj。");
            }
        }
        else
        {
            Debug.Log("技能 " + id + " 无法释放：资源不足。");
        }

        if (castSuccess)
        {
            //每100点冷却速率可以降低一半冷却时间
            skillObj.cooldown = skillObj.model.cooldown / (ownerState.property.cd_speed / 100 + 1);
        }
        else
        {
            skillObj.cooldown = 0.2f;   //无论成功与否，都会进入gcd
        }

        return castSuccess;
    }


    private void synchronizedUnits()
    {
        if (!unitMove) unitMove = this.gameObject.GetComponent<UnitMove>();
        if (!unitRotate) unitRotate = this.gameObject.GetComponent<UnitRotate>();
        if (!viewContainer) viewContainer = this.gameObject.GetComponentInChildren<ViewContainer>().gameObject;
    }

    ///<summary>
    ///命令移动
    ///<param name="move">移动力</param>
    ///</summary>
    public void OrderMove(Vector3 move)
    {
        this.moveOrder.x = move.x;
        this.moveOrder.z = move.z;
    }

    ///<summary>
    ///强制移动
    ///<param name="moveInfo">移动信息</param>
    ///</summary>
    public void AddForceMove(MovePreorder move)
    {
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
    ///设置视觉元素
    ///</summary>
    public void SetView(GameObject view)
    {
        if (view == null) return;
        synchronizedUnits();
        view.transform.SetParent(viewContainer.transform);
        view.transform.position = new Vector3(0, this.gameObject.transform.position.y, 0);
        this.gameObject.transform.position = new Vector3(
            this.gameObject.transform.position.x,
            0,
            this.gameObject.transform.position.z
        );
    }
}
