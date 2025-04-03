using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI_M/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
    [MenuItem("GameObject/UI_M/Radial Progress Bar")]
    public static void AddRadialProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Radial Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif
    public float minimum;
    public float maximum;
    public float current;
    public Image mask;
    public Color color;
    public Image fill;
    public GameObject tickContainer; // 刻度容器
    private int _previousTickCount = 0;

    private List<GameObject> ticks = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
        UpdateTicks();
    }

    void GetCurrentFill()
    {
        // 确保最小值不大于最大值
        minimum = Mathf.Min(minimum, maximum);

        // 确保当前值在最小值和最大值之间
        current = Mathf.Clamp(current, minimum, maximum);

        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum; // 避免除以零
        float fillAmount = currentOffset / maximumOffset;
        if (maximumOffset == 0f)
        {
            fillAmount = 1f;
        }
        fill.fillAmount = fillAmount;
        fill.color = color;
    }

    void UpdateTicks()
    {
        // 如果不在运行模式，直接返回，避免初始化问题
        if (!Application.isPlaying)
        {
            return;
        }

        if (tickContainer == null || maximum <= 0)
        {
            return;
        }

        // 计算刻度数量，考虑包含最后的最大值刻度（如果最大值不整除100，也要创建最大刻度）
        int tickCount = Mathf.FloorToInt(maximum / 100);

        if (tickCount <= 0) return; // 如果没有刻度需要生成，直接返回

        float progressBarWidth = mask.rectTransform.rect.width;

        // 生成刻度，确保包括最大刻度（如果 maximum 不是100的倍数）
        for (int i = 1; i <= tickCount; i++)
        {
            GameObject tick;

            if (i - 1 < ticks.Count) // 如果已有刻度对象，复用它
            {
                tick = ticks[i - 1];
            }
            else // 如果没有足够的对象，从对象池获取新的对象
            {
                tick = GameManager.Instance.ObjectPooler.GetPooledGamObjectAtIndex(1);

                if (tick == null)
                {
                    Debug.LogWarning("Failed to retrieve a pooled object for tick " + i);
                    continue; // 跳过这个刻度，防止错误
                }

                ticks.Add(tick);
                tick.transform.SetParent(tickContainer.transform);
            }

            // 计算每个刻度的 X 位置，按比例在 0 到 progressBarWidth 范围内分布
            float xPosition = (progressBarWidth / tickCount) * i;

            // 设置 tick 的位置，只调整 X 轴
            RectTransform tickRectTransform = tick.GetComponent<RectTransform>();
            tickRectTransform.anchoredPosition = new Vector2(xPosition, 0);

            tick.SetActive(true); // 确保对象处于激活状态
        }

        // 如果 maximum 不是100的倍数，并且我们需要创建最后一个刻度
        if (maximum % 100 != 0)
        {
            GameObject lastTick;

            // 复用或者创建最后一个 tick
            if (tickCount < ticks.Count)
            {
                lastTick = ticks[tickCount];
            }
            else
            {
                lastTick = GameManager.Instance.ObjectPooler.GetPooledGamObjectAtIndex(1);
                ticks.Add(lastTick);
                lastTick.transform.SetParent(tickContainer.transform);
            }

            // 计算最后一个 tick 的位置，应该在最右侧（maximum 对应的位置）
            float lastPosition = progressBarWidth;
            RectTransform lastTickRectTransform = lastTick.GetComponent<RectTransform>();
            lastTickRectTransform.anchoredPosition = new Vector2(lastPosition, lastTickRectTransform.anchoredPosition.y);
            lastTick.SetActive(true);
        }

        // 禁用多余的对象
        for (int i = tickCount + 1; i < ticks.Count; i++)
        {
            if (ticks[i] != null)
            {
                ticks[i].SetActive(false);
            }
        }
    }

}
