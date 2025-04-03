using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JinShan;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpellCombinationManager : MonoBehaviour
{
    //已装备法术仓库
    public InventoryObject spellInventory;
    /// <summary>
    /// 普通攻击
    /// </summary>
    public ItemObject baseAttack;
    [ShowInInspector]
    public Queue<Item> DrawPile = new Queue<Item>();
    [ShowInInspector]
    public Queue<Item> Hand = new Queue<Item>();
    [ShowInInspector]
    public Queue<Item> DiscardPile = new Queue<Item>();

    public Dictionary<Item, bool> triggerDict = new Dictionary<Item, bool>();

    public int defaultDrawCount = 1;
    public int DrawCount = 1;
    public bool HasUpdateDrawPile = false;

    /// <summary>
    /// 当前法力值
    /// </summary>
    public int curMp;
    ///最大法力值
    public int MaxMp;
    ///法力基础恢复速度（加上角色法力恢复值就是法宝最终回复值）
    public float BaseMpRecover;

    /// <summary>
    /// 当前法术延迟时间
    /// </summary>
    public float DelayTime;
    public float baseDelayTime;
    public float MaxDelayTime;

    /// <summary>
    /// 法术充能
    /// </summary>
    public float ChargeTime;
    public float baseChargeTime;
    public float MaxChargeTime;

    public bool IsCharging = false;
    // 新增变量用于追踪是否有待施法的法术组合
    private bool isSpellReady = false;
    /// <summary>
    /// 预载手牌后得到的skill model
    /// </summary>
    [ShowInInspector]
    Dictionary<Item, SkillModel> skillModelDict = new Dictionary<Item, SkillModel>();

    public void Initialize(FaBao fabao)
    {
        spellInventory = new InventoryObject
        {
            database = Resources.Load<ItemDatabaseObject>("Inventory/baseDatabase")
        };
        spellInventory.Container.Slots = new InventorySlot[fabao.SlotCount];
        for (int i = 0; i < fabao.SlotCount; i++)
        {
            spellInventory.Container.Slots[i] = new InventorySlot();
            if (fabao.Slots[i].itemObject != null)
            {
                spellInventory.Container.Slots[i].UpdateSlot(new Item(fabao.Slots[i].itemObject), 1);
                spellInventory.Container.Slots[i].isLocked = fabao.Slots[i].isLock;
            }
            spellInventory.Container.Slots[i].inventory = spellInventory;
        }
        MaxMp = fabao.MaxMp;
        curMp = MaxMp;
        BaseMpRecover = fabao.BaseMpRecover;
        baseChargeTime = fabao.BaseChargeTime;
        baseDelayTime = fabao.BaseDelayTime;
        DrawCount = fabao.DrawTimes;
    }

    /// <summary>
    /// 抽牌：从抽牌堆抽取卡牌
    /// </summary>
    public void DrawCard()
    {
        while (DrawCount > 0)
        {
            if (DrawPile.Count == 0)
            {
                Reload();
                if (DrawPile.Count == 0)
                {
                    break;
                }
            }
            Item drawnCard = DrawPile.Dequeue();
            Hand.Enqueue(drawnCard);
            DrawCount += drawnCard.itemObject.DrawCount;
            DrawCount--;
        }
        if (DrawPile.Count > 0 && DrawPile.Peek().itemObject.type == ItemType.触发器)
        {
            DrawCount = 1;
            DrawCard();
        }
    }


    /// <summary>
    /// 重载：弃牌堆卡牌全部移至抽牌堆
    /// </summary>
    public void Reload()
    {
        while (DiscardPile.Count > 0)
        {
            DrawPile.Enqueue(DiscardPile.Dequeue());
        }
        IsCharging = true;
        ChargeTime = CalculateChargeTime();
    }

    /// <summary>
    /// 预载：对手牌进行效果结算
    /// </summary>
    public void PreloadSpells()
    {
        List<Item> spells = new List<Item>();
        foreach (Item card in Hand)
        {
            if (card == null || card.Id < 0)
            {
                continue;
            }
            spells.Add(card);
        }

        skillModelDict = new Dictionary<Item, SkillModel>();

        foreach (Item item in spells)
        {
            //基础类法术
            if (item.itemObject.type == ItemType.技能)
            {
                SkillScriptableObject skillObject = (SkillScriptableObject)item.itemObject;
                skillModelDict[item] = skillObject.Model.Clone();
                skillModelDict[item].ResetEventManager();
            }
        }

        ApplyEnhancement(spells);
        ProcessTrigger(spells);
    }

    /// <summary>
    /// 处理触发器逻辑
    /// </summary>
    /// <param name="spells"></param>
    private void ProcessTrigger(List<Item> spells)
    {
        for (int i = 0; i < spells.Count; i++)
        {
            Item item = spells[i];
            if (item.itemObject.type == ItemType.触发器)
            {
                TriggerObject trigger = (TriggerObject)item.itemObject;
                if (trigger.condition == TriggerType.OnHit)
                {
                    Item triggerSkillItem = GetItemsAfterEnhanceSpell(spells, item, -1)[0];
                    Item beTriggeredSkillItem = GetItemsAfterEnhanceSpell(spells, item, 1)[0];
                    if (triggerSkillItem == null || beTriggeredSkillItem == null) continue;
                    SkillModel triggerSkillModel = skillModelDict[triggerSkillItem];
                    SkillModel beTriggeredSkillModel = skillModelDict[beTriggeredSkillItem];
                    triggerSkillModel = DesignerTables.DataTrigger.data["OnHit"](triggerSkillModel, beTriggeredSkillModel, trigger.EventTriggerCondition, trigger.EventTriggerConditionParams);
                    skillModelDict[triggerSkillItem] = triggerSkillModel;
                    triggerDict[beTriggeredSkillItem] = true;
                }
            }
        }
    }



    /// <summary>
    /// 应用技能强化效果
    /// </summary>
    /// <param name="spells"></param>
    private void ApplyEnhancement(List<Item> spells)
    {
        foreach (Item item in spells)
        {
            //增强类法术
            if (item.itemObject.type == ItemType.技能强化)
            {
                EnhancedEffectObject enhancedEffectObject = (EnhancedEffectObject)item.itemObject;
                int rangeModifier = enhancedEffectObject.rangeModifier;
                List<Item> skillItems = GetItemsAfterEnhanceSpell(spells, item, rangeModifier);
                foreach (Item skillItem in skillItems)
                {
                    foreach (string enhanceEffectId in enhancedEffectObject.skillModifiers)
                    {
                        if (enhanceEffectId == null)
                        {
                            continue;
                        }
                        if (skillItem != null &&skillModelDict.ContainsKey(skillItem))
                        {
                            SkillModel skillModel = skillModelDict[skillItem];
                            skillModelDict[skillItem] = DesignerTables.DataEnhancedEffect.data[enhanceEffectId](skillModel);
                        }
                    }
                }
            }
        }
    }

    private void UseSkill()
    {
        ChaState State = transform.parent.GetComponent<ChaState>();
        foreach (Item item in skillModelDict.Keys)
        {
            if (triggerDict[item] == true) continue;
            SkillModel skill = skillModelDict[item];
            SkillObj skillObj = new SkillObj(skill);
            if (State.resource.Enough(skillObj.model.condition) == false) continue;
            if (State.resource.mp > curMp) continue;
            TimelineObj timeline = new TimelineObj(
                    skillObj.model.effect.Clone(), transform.parent.gameObject, new object[] { skillObj }
                );
            timeline.timelineType = TimelineType.ComboSkill;
            //技能生成的TimelineObj要从技能参数中继承技能参数
            if (skillObj.model.skillParams != null)
            {
                foreach (var kvp in skillObj.model.skillParams)
                {
                    timeline.values[kvp.Key] = kvp.Value;
                }
            }
            if (timeline != null)
            {
                SceneVariants.CreateTimeline(timeline);
            }
            //释放技能 消耗资源
            curMp -= skillObj.model.cost.mp;
        }
    }

    /// <summary>
    /// 释放法术
    /// </summary>
    private void CastSpell()
    {
        UseSkill();
        DelayTime = CalculateTotalDelayTime();
        while (Hand.Count > 0)
        {
            Item card = Hand.Dequeue();
            DiscardPile.Enqueue(card);
        }
    }

    /// <summary>
    /// 计算延迟时间
    /// </summary>
    /// <returns></returns>
    private float CalculateTotalDelayTime()
    {
        float totalDelay = 0;
        foreach (Item card in Hand)
        {
            if (card.itemObject.type == ItemType.技能)
            {
                totalDelay += ((SkillScriptableObject)card.itemObject).delayTimeModifier;
            }
            else if (card.itemObject.type == ItemType.技能强化)
            {
                totalDelay += ((EnhancedEffectObject)card.itemObject).delayTimeModifier;
            }
            else if (card.itemObject.type == ItemType.触发器)
            {
                totalDelay += ((TriggerObject)card.itemObject).delayTimeModifier;
            }
        }
        MaxDelayTime = totalDelay + baseDelayTime;
        return Mathf.Clamp(MaxDelayTime, 0f, 10f);
    }

    /// <summary>
    /// 计算充能时间
    /// 抽牌堆里有所有的牌
    /// </summary>
    /// <returns></returns>
    private float CalculateChargeTime()
    {
        // 根据需要计算充能时间
        float totalChargeTime = 0;
        List<Queue<Item>> queues = new List<Queue<Item>> { DrawPile, Hand, DiscardPile };
        foreach (Queue<Item> queue in queues)
        {
            foreach (Item card in queue)
            {
                if (card.itemObject.type == ItemType.技能)
                {
                    totalChargeTime += ((SkillScriptableObject)card.itemObject).chargeTimeModifier;
                }
                else if (card.itemObject.type == ItemType.技能强化)
                {
                    totalChargeTime += ((EnhancedEffectObject)card.itemObject).chargeTimeModifier;
                }
                else if (card.itemObject.type == ItemType.触发器)
                {
                    totalChargeTime += ((TriggerObject)card.itemObject).chargeTimeModifier;
                }
            }
        }
        MaxChargeTime = totalChargeTime + baseChargeTime;
        return Mathf.Clamp(MaxChargeTime, 0f, 10f);
    }

    private void FixedUpdate()
    {

        curMp += (int)(BaseMpRecover / 50);
        // 将 curMp 限制在 0 和 MaxMp 之间
        curMp = Mathf.Clamp(curMp, 0, MaxMp);
        if (DelayTime > 0)
        {
            DelayTime -= Time.deltaTime;
        }

        if (IsCharging)
        {
            ChargeTime -= Time.deltaTime;
            if (ChargeTime <= 0)
            {
                IsCharging = false;
            }
        }

    }

    /// <summary>
    /// 使用法术
    /// </summary>
    public void UseWand()
    {
        if (!HasUpdateDrawPile)
        {
            UpdateDrawPile();
        }

        if (IsCharging)
        {
            return;
        }

        if (DelayTime > 0)
        {
            return;
        }

        // 抽牌：确保每次调用UseWand时仅执行一次
        if (!isSpellReady)
        {
            DrawCount = defaultDrawCount;  // 设置初始抽取数
            DrawCard();  // 抽牌

            // 检查抽牌后手牌是否为空，如果为空则触发重载
            if (Hand.Count == 0)
            {
                Reload();  // 重载抽牌堆
                DrawCard();  // 重载后再尝试抽牌一次
            }

            PreloadSpells();  // 组合并准备法术
            isSpellReady = true;  // 标记已准备好法术
        }

        if (Hand.Count > 0)
        {
            ExecuteSpell();  // 判断是否施法或移动
        }
    }

    /// <summary>
    /// 执行施法
    /// </summary>
    private void ExecuteSpell()
    {
        CastSpell();  // 施法
        isSpellReady = false;  // 重置标记，表示施法完成
    }



    /// <summary>
    /// 更新抽牌堆 清空手牌堆和弃牌堆
    /// </summary>
    public void UpdateDrawPile()
    {
        DrawPile = new Queue<Item>();
        Hand = new Queue<Item>();
        DiscardPile = new Queue<Item>();
        triggerDict = new Dictionary<Item, bool>();

        //空置的slot放置普通攻击
        foreach (InventorySlot slot in spellInventory.GetSlots)
        {
            if (slot.amount > 0 && slot.item != null && slot.item.Id > -1)
            {
                DrawPile.Enqueue(slot.item);
                triggerDict[slot.item] = false;
            }
        }
        HasUpdateDrawPile = true;
    }

    public List<Item> GetItemsAfterEnhanceSpell(List<Item> spells, Item enhancespell, int range)
    {

        List<Item> result = new List<Item>();
        // 获取 "enhancespell" 在 "spells" 列表中的索引
        int index = spells.FindIndex(spell => spell == enhancespell);

        if (index >= 0)
        {
            // 从 "enhancespell" 之后或之前获取符合条件的项
            var filteredItems = spells
                .Skip(index + 1) // 仅包括 "enhancespell" 之后的项
                .Where(item => item.itemObject.type == ItemType.技能)
                .ToList();

            if (range >= 0)
            {
                // 正数 range: 返回符合条件的前几个项
                result = filteredItems.Take(range).ToList();
            }
            else
            {
                // 负数 range: 返回 "enhancespell" 之前符合条件的第一个项
                int positiveRange = -range;
                var itemsBeforeEnhanceSpell = spells
                    .Take(index)
                    .Where(item => item.itemObject.type == ItemType.技能)
                    .Reverse()
                    .Take(positiveRange)
                    .ToList();

                result = itemsBeforeEnhanceSpell;
            }
        }
        if (result == null || result.Count == 0)
        {
            result = new List<Item>() { null };
        }

        return result;
    }
}