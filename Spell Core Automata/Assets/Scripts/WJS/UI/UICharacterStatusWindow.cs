
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using WJS;
using TMPro;

namespace WJS
{
    /// <summary>
    /// 角色状态UI窗口
    /// 用于显示角色的属性和资源信息
    /// </summary>
    public class UICharacterStatusWindow : UIWindow
    {
        [Header("角色信息")]
        [SerializeField]
        private TMP_Text characterNameText;

        [SerializeField]
        private TMP_Text characterLevelText;

        [Header("资源信息")]
        [SerializeField]
        private Slider hpSlider;

        [SerializeField]
        private TMP_Text hpText;

        [SerializeField]
        private Slider mpSlider;

        [SerializeField]
        private TMP_Text mpText;

        [Header("属性信息")]
        [SerializeField]
        private TMP_Text propertyText;

        [Header("状态信息")]
        [SerializeField]
        private TMP_Text statusText;

        [Header("Buff信息")]
        [SerializeField]
        private Transform buffContainer;

        [SerializeField]
        private GameObject buffItemPrefab;

        private ChaState targetChaState;
        private StringBuilder sb = new StringBuilder();

        private void Start()
        {
            // 获取主角的ChaState
            if (GameManager.Instance.mainCharacter != null)
            {
                targetChaState = GameManager.Instance.mainCharacter.GetComponent<ChaState>();
            }
        }

        private void Update()
        {
            if (targetChaState == null)
            {
                return;
            }

            // 更新资源信息
            UpdateResourceInfo();

            // 更新属性信息
            UpdatePropertyInfo();

            // 更新状态信息
            UpdateStatusInfo();

            // 更新Buff信息
            UpdateBuffInfo();
        }

        /// <summary>
        /// 更新资源信息
        /// </summary>
        private void UpdateResourceInfo()
        {
            if (targetChaState.resource == null)
            {
                return;
            }

            // 更新HP
            if (hpSlider != null)
            {
                int maxHp = targetChaState.property.hp;
                hpSlider.maxValue = maxHp > 0 ? maxHp : 1;
                hpSlider.value = targetChaState.resource.hp;
            }

            if (hpText != null)
            {
                hpText.text = $"{targetChaState.resource.hp} / {targetChaState.property.hp}";
            }

            // 更新MP
            if (mpSlider != null)
            {
                int maxMp = targetChaState.property.mp;
                mpSlider.maxValue = maxMp > 0 ? maxMp : 1;
                mpSlider.value = targetChaState.resource.mp;
            }

            if (mpText != null)
            {
                mpText.text = $"{targetChaState.resource.mp} / {targetChaState.property.mp}";
            }
        }

        /// <summary>
        /// 更新属性信息
        /// </summary>
        private void UpdatePropertyInfo()
        {
            if (propertyText == null)
            {
                return;
            }

            sb.Clear();

            // 获取当前属性
            ChaProperty prop = targetChaState.property;

            // 添加属性信息
            sb.AppendLine("<b>基础属性</b>");
            sb.AppendLine($"攻击力: {prop.attack}");
            sb.AppendLine($"防御力: {prop.defence}");
            sb.AppendLine($"移动速度: {prop.moveSpeed}");
            sb.AppendLine($"施法速度: {prop.actionSpeed}");
            sb.AppendLine($"冷却速度: {prop.cd_speed}");
            sb.AppendLine();

            sb.AppendLine("<b>战斗属性</b>");
            sb.AppendLine($"暴击率: {prop.critic_rate * 100:F1}%");
            sb.AppendLine($"暴击倍率: {prop.critic_multiplier:F2}x");
            sb.AppendLine($"闪避率: {prop.dodge_rate * 100:F1}%");
            sb.AppendLine();

            sb.AppendLine("<b>回复属性</b>");
            sb.AppendLine($"最大生命: {prop.hp}");
            sb.AppendLine($"生命回复: {prop.hp_recover}/秒");
            sb.AppendLine($"最大灵力: {prop.mp}");
            sb.AppendLine($"灵力回复: {prop.mp_recover}/秒");

            propertyText.text = sb.ToString();
        }

        /// <summary>
        /// 更新状态信息
        /// </summary>
        private void UpdateStatusInfo()
        {
            if (statusText == null)
            {
                return;
            }

            sb.Clear();

            // 添加状态信息
            sb.AppendLine("<b>状态信息</b>");

            if (targetChaState.dead)
            {
                sb.AppendLine("<color=red>状态: 死亡</color>");
            }
            else if (targetChaState.charging)
            {
                sb.AppendLine("<color=yellow>状态: 蓄力中</color>");
            }
            else
            {
                sb.AppendLine("状态: 正常");
            }

            if (targetChaState.immuneTime > 0)
            {
                sb.AppendLine($"<color=cyan>无敌时间: {targetChaState.immuneTime:F1}秒</color>");
            }

            // 添加控制状态
            ChaControlState controlState = targetChaState.ControlState;
            sb.AppendLine();
            sb.AppendLine("<b>控制状态</b>");
            sb.AppendLine($"可移动: {(controlState.canMove ? "是" : "否")}");
            sb.AppendLine($"可旋转: {(controlState.canRotate ? "是" : "否")}");
            sb.AppendLine($"可攻击: {(controlState.canUseSkill ? "是" : "否")}");

            statusText.text = sb.ToString();
        }

        /// <summary>
        /// 更新Buff信息
        /// </summary>
        private void UpdateBuffInfo()
        {
            if (buffContainer == null || buffItemPrefab == null)
            {
                return;
            }

            // 清除旧的Buff项
            foreach (Transform child in buffContainer)
            {
                Destroy(child.gameObject);
            }

            // 创建新的Buff项
            foreach (BuffObj buff in targetChaState.buffs)
            {
                if (buff == null)
                {
                    continue;
                }

                GameObject buffItem = Instantiate(buffItemPrefab, buffContainer);
                Text buffText = buffItem.GetComponentInChildren<Text>();

                if (buffText != null)
                {
                    buffText.text = buff.model.name;
                }
            }
        }

        /// <summary>
        /// 设置目标角色
        /// </summary>
        /// <param name="chaState">角色状态</param>
        public void SetTargetCharacter(ChaState chaState)
        {
            targetChaState = chaState;

            if (characterNameText != null && chaState != null)
            {
                characterNameText.text = chaState.name;
            }
        }
    }
}
