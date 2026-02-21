
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using WJS;

namespace WJS
{
    /// <summary>
    /// 角色属性面板
    /// 用于详细展示角色的属性信息
    /// </summary>
    public class UICharacterPropertyPanel : MonoBehaviour
    {
        [Header("基础属性")]
        [SerializeField]
        private Text basePropertyText;

        [Header("Buff属性")]
        [SerializeField]
        private Text buffPropertyText;

        [Header("装备属性")]
        [SerializeField]
        private Text equipmentPropertyText;

        [Header("当前属性")]
        [SerializeField]
        private Text currentPropertyText;

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

            // 更新基础属性
            UpdateBaseProperty();

            // 更新Buff属性
            UpdateBuffProperty();

            // 更新装备属性
            UpdateEquipmentProperty();

            // 更新当前属性
            UpdateCurrentProperty();
        }

        /// <summary>
        /// 更新基础属性
        /// </summary>
        private void UpdateBaseProperty()
        {
            if (basePropertyText == null)
            {
                return;
            }

            sb.Clear();
            sb.AppendLine("<b>基础属性</b>");
            sb.Append(targetChaState.baseProp.GetDescription());

            basePropertyText.text = sb.ToString();
        }

        /// <summary>
        /// 更新Buff属性
        /// </summary>
        private void UpdateBuffProperty()
        {
            if (buffPropertyText == null)
            {
                return;
            }

            sb.Clear();
            sb.AppendLine("<b>Buff属性</b>");

            // 显示Buff Plus属性
            if (targetChaState.buffProp != null && targetChaState.buffProp.Length > 0)
            {
                sb.AppendLine("<b>Buff Plus:</b>");
                sb.Append(targetChaState.buffProp[0].GetDescription());

                // 显示Buff Times属性
                if (targetChaState.buffProp.Length > 1)
                {
                    sb.AppendLine("<b>Buff Times:</b>");
                    sb.Append(targetChaState.buffProp[1].GetDescription());
                }
            }
            else
            {
                sb.AppendLine("无Buff属性");
            }

            buffPropertyText.text = sb.ToString();
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        private void UpdateEquipmentProperty()
        {
            if (equipmentPropertyText == null)
            {
                return;
            }

            sb.Clear();
            sb.AppendLine("<b>装备属性</b>");
            sb.Append(targetChaState.equipmentProp.GetDescription());

            equipmentPropertyText.text = sb.ToString();
        }

        /// <summary>
        /// 更新当前属性
        /// </summary>
        private void UpdateCurrentProperty()
        {
            if (currentPropertyText == null)
            {
                return;
            }

            sb.Clear();
            sb.AppendLine("<b>当前属性</b>");
            sb.Append(targetChaState.currentProperty.GetDescription());

            currentPropertyText.text = sb.ToString();
        }

        /// <summary>
        /// 设置目标角色
        /// </summary>
        /// <param name="chaState">角色状态</param>
        public void SetTargetCharacter(ChaState chaState)
        {
            targetChaState = chaState;
        }
    }
}
