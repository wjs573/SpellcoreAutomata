using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CircularGridLayoutGroup : LayoutGroup
{
    [SerializeField] public int rows = 4; // 网格行数
    [SerializeField] public int columns = 4; // 网格列数
    [SerializeField] public Vector2 cellSize = new Vector2(100f, 100f); // 每个单元格的尺寸
    [SerializeField] public Vector2 spacing = new Vector2(10f, 10f); // 单元格间距
    [SerializeField] public bool isUniformDistribution = false; // 是否启用均匀分布
    [SerializeField] public bool isDynamicGridSize = false; // 是否动态调整网格大小
    [SerializeField] public RectTransform centerImage; // 中心元素（非子元素）

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        ArrangeChildren();
    }

    public override void CalculateLayoutInputVertical()
    {
        ArrangeChildren();
    }

    public override void SetLayoutHorizontal()
    {
        ArrangeChildren();
    }

    public override void SetLayoutVertical()
    {
        ArrangeChildren();
    }

    private void ArrangeChildren()
    {
        int totalChildren = rectChildren.Count;
        if (totalChildren == 0 || rows <= 0 || columns <= 0) return;

        if (isDynamicGridSize)
        {
            AdjustGridSize(totalChildren);
        }

        if (isUniformDistribution)
        {
            ApplyUniformSymmetricDistribution(totalChildren);
        }
        else
        {
            ApplyCircularGrid(totalChildren);
        }

        if (centerImage != null)
        {
            PlaceCenterImage();
        }
    }

    private void AdjustGridSize(int totalChildren)
    {
        if (totalChildren <= 8)
        {
            rows = 3;
            columns = 3;
        }
        else if (totalChildren <= 10)
        {
            rows = 3;
            columns = 4;
        }
        else if (totalChildren <= 12)
        {
            rows = 3;
            columns = 5;
        }
        else if (totalChildren <= 14)
        {
            rows = 3;
            columns = 6;
        }
    }

private void PlaceCenterImage()
{
    // 计算网格中心的世界坐标
    Vector3 gridCenterWorldPosition = CalculateGridCenterWorldPosition();

    // 修正中心元素的锚点偏移
    Vector3 anchorOffset = new Vector3(
        (centerImage.anchorMax.x + centerImage.anchorMin.x - 1) * centerImage.rect.width / 2,
        (centerImage.anchorMax.y + centerImage.anchorMin.y - 1) * centerImage.rect.height / 2,
        0
    );

    // 设置中心元素的位置，修正锚点偏移
    centerImage.position = gridCenterWorldPosition + anchorOffset;
}


private Vector3 CalculateGridCenterWorldPosition()
{
    // 计算网格的宽度和高度
    float gridWidth = (columns - 1) * (cellSize.x + spacing.x) + cellSize.x;
    float gridHeight = (rows - 1) * (cellSize.y + spacing.y) + cellSize.y;

    // 计算网格中心的本地坐标
    float localCenterX = padding.left + gridWidth / 2;
    float localCenterY = -padding.top - gridHeight / 2; // Y 轴为负方向

    // 转换为世界坐标
    Vector3 localCenter = new Vector3(localCenterX, localCenterY, 0);
    return transform.TransformPoint(localCenter);
}


    private void ApplyUniformSymmetricDistribution(int totalChildren)
    {
        int ringLength = 2 * (rows + columns) - 4; // 圆环总格子数
        if (ringLength <= 0) return;

        for (int i = 0; i < totalChildren; i++)
        {
            RectTransform child = rectChildren[i];
            Vector2 position;

            int targetIndex = CalculateSymmetricIndex(i, totalChildren, ringLength);
            position = CalculateRingPosition(targetIndex);

            SetChildAlongAxis(child, 0, position.x);
            SetChildAlongAxis(child, 1, position.y);
        }
    }

    private int CalculateSymmetricIndex(int index, int totalChildren, int ringLength)
    {
        float step = (float)ringLength / totalChildren;
        return Mathf.RoundToInt(index * step) % ringLength;
    }

    private Vector2 CalculateRingPosition(int index)
    {
        int topEdge = columns - 1;
        int rightEdge = rows - 1;
        int bottomEdge = columns - 1;
        int leftEdge = rows - 1;

        float x = padding.left;
        float y = padding.top;

        if (index < topEdge) // 顶边
        {
            x += index * (cellSize.x + spacing.x);
        }
        else if (index < topEdge + rightEdge) // 右边
        {
            x += (columns - 1) * (cellSize.x + spacing.x);
            y += (index - topEdge) * (cellSize.y + spacing.y);
        }
        else if (index < topEdge + rightEdge + bottomEdge) // 底边
        {
            x += (columns - 1 - (index - topEdge - rightEdge)) * (cellSize.x + spacing.x);
            y += (rows - 1) * (cellSize.y + spacing.y);
        }
        else // 左边
        {
            y += (rows - 1 - (index - topEdge - rightEdge - bottomEdge)) * (cellSize.y + spacing.y);
        }

        return new Vector2(x, y);
    }

    private void ApplyCircularGrid(int totalChildren)
    {
        int ringLength = 2 * (rows + columns) - 4;
        int pathCount = Mathf.Min(totalChildren, ringLength);

        float startX = padding.left;
        float startY = padding.top;

        for (int i = 0; i < totalChildren; i++)
        {
            RectTransform child = rectChildren[i];
            Vector2 position;

            if (i < pathCount)
            {
                position = CalculateRingPosition(i);
            }
            else
            {
                position = new Vector2(startX + (columns - 1) * (cellSize.x + spacing.x) / 2,
                                       startY + (rows - 1) * (cellSize.y + spacing.y) / 2);
            }

            SetChildAlongAxis(child, 0, position.x);
            SetChildAlongAxis(child, 1, position.y);
        }
    }

    protected override void OnTransformChildrenChanged()
    {
        base.OnTransformChildrenChanged();
        if (!Application.isPlaying)
        {
            ArrangeChildren();
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        ArrangeChildren();
    }
#endif
}
