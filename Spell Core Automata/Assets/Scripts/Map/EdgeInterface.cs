using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public enum Direction { North, East, South, West }

[System.Serializable]
public struct EdgeInterface
{
    public Direction direction;
    public int position; // 1-based索引

    public Vector3 worldPosition; // 实际世界坐标（基于5f单元）

    public void SetPosition(Vector3 tileCenter, Vector2Int tileSize)
    {
        float unitSize = 5f; // 每个单元格的Unity单位尺寸

        // 计算Tile的物理尺寸（世界坐标）
        Vector3 tileWorldSize = new Vector3(
            tileSize.x * unitSize,
            0,
            tileSize.y * unitSize
        );

        worldPosition = direction switch
        {
            Direction.North => tileCenter + new Vector3(
                (position - 1 - (tileSize.x - 1) * 0.5f) * unitSize,
                0,
                tileWorldSize.z * 0.5f
            ),
            Direction.South => tileCenter + new Vector3(
                (position - 1 - (tileSize.x - 1) * 0.5f) * unitSize,
                0,
                -tileWorldSize.z * 0.5f
            ),
            Direction.East => tileCenter + new Vector3(
                tileWorldSize.x * 0.5f,
                0,
                (position - 1 - (tileSize.y - 1) * 0.5f) * unitSize
            ),
            Direction.West => tileCenter + new Vector3(
                -tileWorldSize.x * 0.5f,
                0,
                (position - 1 - (tileSize.y - 1) * 0.5f) * unitSize
            ),
            _ => Vector3.zero
        };
        
    }

    /// <summary>
    /// ToDO: 获取边缘接口的偏移量
    /// </summary>
    /// <param name="tileSize"></param>
    /// <returns></returns>
    public Vector3 GetEdgeOffset(Vector2Int tileSize)
    {
        float unitSize = 5f; // 每个单元格的Unity单位尺寸

        // 计算Tile的物理尺寸（世界坐标）
        Vector3 tileWorldSize = new Vector3(
            tileSize.x * unitSize,
            0,
            tileSize.y * unitSize
        );

        Vector3 offset = Vector3.zero;

        if (tileSize.x == tileSize.y)
        {
            offset = direction switch
            {
                Direction.North => new Vector3(
                    (position - 1 - (tileSize.x - 1) * 0.5f) * unitSize,
                    0,
                    tileWorldSize.z * 0.5f
                ),
                Direction.South => new Vector3(
                    (position - 1 - (tileSize.x - 1) * 0.5f) * unitSize,
                    0,
                    -tileWorldSize.z * 0.5f
                ),
                Direction.East => new Vector3(
                    tileWorldSize.x * 0.5f,
                    0,
                    (position - 1 - (tileSize.y - 1) * 0.5f) * unitSize
                ),
                Direction.West => new Vector3(
                    -tileWorldSize.x * 0.5f,
                    0,
                    (position - 1 - (tileSize.y - 1) * 0.5f) * unitSize
                ),
                _ => Vector3.zero
            };
        }
        else
        {
            // 如果不是正方形，使用不同的计算方式
            // 根据方向计算接口的世界坐标
            offset = direction switch
            {
                Direction.North => new Vector3(
                    (position - 1 - (tileSize.x - 1) * 0.5f) * unitSize,
                    0,
                    tileWorldSize.z * 0.5f
                ),
                Direction.South => new Vector3(
                    (tileSize.x - (position - 1) - (tileSize.x - 1) * 0.5f) * unitSize,
                    0,
                    -tileWorldSize.z * 0.5f
                ),
                Direction.East => new Vector3(
                    tileWorldSize.x * 0.5f,
                    0,
                    (tileSize.y - (position - 1) - (tileSize.y - 1) * 0.5f) * unitSize
                ),
                Direction.West => new Vector3(
                    -tileWorldSize.x * 0.5f,
                    0,
                    (position - 1 - (tileSize.y - 1) * 0.5f) * unitSize
                ),
                _ => Vector3.zero
            };
        }
        return offset;
    }

    public EdgeInterface GetRotated(int steps,Vector2Int size)
    {
        Direction newDir = (Direction)(((int)direction + steps) % 4);

        if (newDir == Direction.South && size.x ==2)
        {
            position = position == 1 ? 2 : 1;
        }
        if (newDir == Direction.East && size.y == 2)
        {
            position = position == 1 ? 2 : 1;
        }

        int newPosition = position;

        return new EdgeInterface
        {
            direction = newDir,
            position = newPosition,
        };
    }

    // 方向向量辅助方法（与之前一致）
    private Vector3 GetDirectionVector(Direction dir)
    {
        return dir switch
        {
            Direction.North => Vector3.forward,
            Direction.South => Vector3.back,
            Direction.East => Vector3.right,
            Direction.West => Vector3.left,
            _ => Vector3.up
        };
    }

    /// <summary>
    /// 在接口位置绘制红色方向线
    /// </summary>
    /// <param name="tileCenter">Tile中心坐标</param>
    /// <param name="duration">显示持续时间(秒)</param>

    public void DrawRedLineAtEdge(Vector3 tileCenter, Vector2Int tileSize, float duration = 0)
    {
        SetPosition(tileCenter, tileSize);

        Color lineColor = Color.red;
        float lineLength = 2f;
        float lineThickness = 0.3f;

        Vector3 dirVector = GetDirectionVector(direction);
        Vector3 lineStart = worldPosition;
        Vector3 lineEnd = lineStart + dirVector * lineLength;

        Vector3 perpendicular = Vector3.Cross(dirVector, Vector3.up).normalized * lineThickness;

        Debug.DrawLine(lineStart - perpendicular, lineEnd - perpendicular, lineColor, duration);
        Debug.DrawLine(lineStart, lineEnd, lineColor, duration);
        Debug.DrawLine(lineStart + perpendicular, lineEnd + perpendicular, lineColor, duration);
    }
}
