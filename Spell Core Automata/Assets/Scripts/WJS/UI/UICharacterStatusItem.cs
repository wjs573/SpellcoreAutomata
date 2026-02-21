
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WJS;

namespace WJS
{
    /// <summary>
    /// 角色状态UI项
    /// 用于显示单个Buff或Debuff
    /// </summary>
    public class UICharacterStatusItem : MonoBehaviour
    {
        [Header("Buff信息")]
        [SerializeField]
        private Image buffIcon;

        [SerializeField]
        private Text buffName;

        [SerializeField]
        private Text buffDuration;

        [SerializeField]
        private Image buffBackground;

        [SerializeField]
        private Color buffColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);

        [SerializeField]
        private Color debuffColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);

        private BuffObj buffObj;

        /// <summary>
        /// 设置Buff对象
        /// </summary>
        /// <param name="buff">Buff对象</param>
        public void SetBuff(BuffObj buff)
        {
            buffObj = buff;

            if (buff == null)
            {
                return;
            }

            // 设置Buff名称
            if (buffName != null)
            {
                buffName.text = buff.model.name;
            }

            // 设置Buff图标
            if (buffIcon != null)
            {
                buffIcon.sprite = BuffIconManager.Instance.GetBuffIcon(buff.model.id);
            }


            // 设置背景颜色
            if (buffBackground != null)
            {
                buffBackground.color = buff.model.tags.Contains("debuff") ? debuffColor : buffColor;
            }
        }

        private void Update()
        {
            if (buffObj == null)
            {
                return;
            }

            // 更新持续时间
            if (buffDuration != null && buffObj.duration > 0)
            {
                buffDuration.text = $"{buffObj.duration:F1}s";
            }
        }
    }
}
