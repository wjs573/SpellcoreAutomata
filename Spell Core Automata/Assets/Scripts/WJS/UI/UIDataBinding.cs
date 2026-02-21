
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI数据绑定系统
    /// 用于将游戏数据绑定到UI元素上
    /// </summary>
    public class UIDataBinding : MonoBehaviour
    {
        [System.Serializable]
        public class Binding
        {
            public enum BindingType
            {
                Text,
                Image,
                Slider,
                Toggle,
                Dropdown,
                ProgressBar
            }

            public string dataPath;
            public BindingType type;
            public Component targetComponent;
            public string format = "{0}";
            public bool updateEveryFrame = false;
        }

        [SerializeField]
        private Binding[] bindings;

        private object dataContext;

        /// <summary>
        /// 设置数据上下文
        /// </summary>
        /// <param name="context">数据上下文</param>
        public void SetDataContext(object context)
        {
            dataContext = context;
            UpdateAllBindings();
        }

        /// <summary>
        /// 更新所有绑定
        /// </summary>
        public void UpdateAllBindings()
        {
            if (dataContext == null || bindings == null)
            {
                return;
            }

            foreach (Binding binding in bindings)
            {
                UpdateBinding(binding);
            }
        }

        /// <summary>
        /// 更新单个绑定
        /// </summary>
        private void UpdateBinding(Binding binding)
        {
            if (binding == null || binding.targetComponent == null)
            {
                return;
            }

            // 获取数据值
            object value = GetDataValue(binding.dataPath);
            if (value == null)
            {
                return;
            }

            // 根据绑定类型更新UI
            switch (binding.type)
            {
                case Binding.BindingType.Text:
                    UpdateTextBinding(binding, value);
                    break;

                case Binding.BindingType.Image:
                    UpdateImageBinding(binding, value);
                    break;

                case Binding.BindingType.Slider:
                    UpdateSliderBinding(binding, value);
                    break;

                case Binding.BindingType.Toggle:
                    UpdateToggleBinding(binding, value);
                    break;

                case Binding.BindingType.Dropdown:
                    UpdateDropdownBinding(binding, value);
                    break;

                case Binding.BindingType.ProgressBar:
                    UpdateProgressBarBinding(binding, value);
                    break;
            }
        }

        /// <summary>
        /// 更新文本绑定
        /// </summary>
        private void UpdateTextBinding(Binding binding, object value)
        {
            Text textComponent = binding.targetComponent as Text;
            if (textComponent == null)
            {
                return;
            }

            string formattedValue = string.Format(binding.format, value);
            textComponent.text = formattedValue;
        }

        /// <summary>
        /// 更新图片绑定
        /// </summary>
        private void UpdateImageBinding(Binding binding, object value)
        {
            Image imageComponent = binding.targetComponent as Image;
            if (imageComponent == null)
            {
                return;
            }

            if (value is Sprite sprite)
            {
                imageComponent.sprite = sprite;
            }
            else if (value is Color color)
            {
                imageComponent.color = color;
            }
        }

        /// <summary>
        /// 更新滑块绑定
        /// </summary>
        private void UpdateSliderBinding(Binding binding, object value)
        {
            Slider sliderComponent = binding.targetComponent as Slider;
            if (sliderComponent == null)
            {
                return;
            }

            if (value is float floatValue)
            {
                sliderComponent.value = floatValue;
            }
            else if (value is int intValue)
            {
                sliderComponent.value = intValue;
            }
        }

        /// <summary>
        /// 更新开关绑定
        /// </summary>
        private void UpdateToggleBinding(Binding binding, object value)
        {
            Toggle toggleComponent = binding.targetComponent as Toggle;
            if (toggleComponent == null)
            {
                return;
            }

            if (value is bool boolValue)
            {
                toggleComponent.isOn = boolValue;
            }
        }

        /// <summary>
        /// 更新下拉框绑定
        /// </summary>
        private void UpdateDropdownBinding(Binding binding, object value)
        {
            Dropdown dropdownComponent = binding.targetComponent as Dropdown;
            if (dropdownComponent == null)
            {
                return;
            }

            if (value is int intValue)
            {
                dropdownComponent.value = intValue;
            }
        }

        /// <summary>
        /// 更新进度条绑定
        /// </summary>
        private void UpdateProgressBarBinding(Binding binding, object value)
        {
            Slider sliderComponent = binding.targetComponent as Slider;
            if (sliderComponent == null)
            {
                return;
            }

            if (value is float floatValue)
            {
                sliderComponent.value = floatValue;
            }
            else if (value is int intValue)
            {
                sliderComponent.value = intValue;
            }
        }

        /// <summary>
        /// 获取数据值
        /// </summary>
        private object GetDataValue(string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath) || dataContext == null)
            {
                return null;
            }

            // 分割数据路径
            string[] parts = dataPath.Split('.');
            object current = dataContext;

            // 遍历路径获取值
            for (int i = 0; i < parts.Length; i++)
            {
                if (current == null)
                {
                    return null;
                }

                string part = parts[i];

                // 处理数组或列表
                if (part.Contains("[") && part.Contains("]"))
                {
                    int indexStart = part.IndexOf('[') + 1;
                    int indexEnd = part.IndexOf(']');
                    string indexStr = part.Substring(indexStart, indexEnd - indexStart);
                    string propertyName = part.Substring(0, indexStart - 1);

                    if (int.TryParse(indexStr, out int index))
                    {
                        var property = current.GetType().GetProperty(propertyName);
                        if (property != null)
                        {
                            var collection = property.GetValue(current);
                            if (collection is System.Collections.IList list && index >= 0 && index < list.Count)
                            {
                                current = list[index];
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    // 处理普通属性
                    var property = current.GetType().GetProperty(part);
                    if (property != null)
                    {
                        current = property.GetValue(current);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return current;
        }

        private void Update()
        {
            if (bindings == null)
            {
                return;
            }

            foreach (Binding binding in bindings)
            {
                if (binding.updateEveryFrame)
                {
                    UpdateBinding(binding);
                }
            }
        }
    }
}
