using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class BSPGenerator : MonoBehaviour
{
    [Header("Generator Settings")]
    public int dungeonWidth = 100;    // 地牢总宽度
    public int dungeonHeight = 100;   // 地牢总高度
    public int minRoomSize = 8;       // 房间最小尺寸
    public int maxSplitDepth = 5;     // 最大分割深度
    public int corridorWidth = 3;     // 走廊宽度
    [Header("Room Padding")]
    public int roomMargin = 2; // 新增：房间边距

    [Header("Debug")]
    public bool drawGizmos = true;    // 场景视图调试绘制
    public Color roomColor = Color.green;
    public Color corridorColor = Color.yellow;
    private MapGrid mapGrid;          // 网格数据容器
    private List<BSPNode> leafNodes = new List<BSPNode>(); // 所有叶子节点

    [Button("Generate Dungeon")]
    // 生成入口
    public MapGrid GenerateDungeon()
    {
        mapGrid = new MapGrid(dungeonWidth, dungeonHeight);
        leafNodes.Clear();

        // 创建根节点
        BSPNode root = new BSPNode()
        {
            space = new RectInt(0, 0, dungeonWidth, dungeonHeight),
            depth = 0
        };

        // 递归分割
        SplitNode(root);

        // 生成房间
        GenerateRooms(root);

        // 连接房间
        ConnectRooms(root);

        return mapGrid;
    }
    #region 改进的分割逻辑
    private void SplitNode(BSPNode node)
    {
        // 终止条件：空间不足或达到深度
        if (node.depth >= maxSplitDepth ||
            !CanSplitFurther(node.space))
        {
            leafNodes.Add(node);
            return;
        }

        // 动态选择分割方向（优化长宽比判断）
        bool splitVertical = ShouldSplitVertical(node.space);
        int minChildSize = minRoomSize * 2 + roomMargin * 4;

        // 计算有效分割范围
        int minSplit, maxSplit;
        if (splitVertical)
        {
            minSplit = node.space.x + minChildSize;
            maxSplit = node.space.xMax - minChildSize;
        }
        else
        {
            minSplit = node.space.y + minChildSize;
            maxSplit = node.space.yMax - minChildSize;
        }

        // 安全分割位置计算
        if (minSplit >= maxSplit)
        {
            Debug.LogWarning($"Cannot split {node.space} {(splitVertical ? "vertically" : "horizontally")}");
            leafNodes.Add(node);
            return;
        }

        int splitPos = Random.Range(minSplit, maxSplit);
        // 创建子节点
        node.leftChild = new BSPNode() { depth = node.depth + 1 };
        node.rightChild = new BSPNode() { depth = node.depth + 1 };

        if (splitVertical)
        {
            node.leftChild.space = new RectInt(
                node.space.x,
                node.space.y,
                splitPos - node.space.x,
                node.space.height);

            node.rightChild.space = new RectInt(
                splitPos,
                node.space.y,
                node.space.xMax - splitPos,
                node.space.height);
        }
        else
        {
            node.leftChild.space = new RectInt(
                node.space.x,
                node.space.y,
                node.space.width,
                splitPos - node.space.y);

            node.rightChild.space = new RectInt(
                node.space.x,
                splitPos,
                node.space.width,
                node.space.yMax - splitPos);
        }

        // 递归分割
        SplitNode(node.leftChild);
        SplitNode(node.rightChild);
    }
    private bool CanSplitFurther(RectInt space)
    {
        bool canVertical = space.width >= (minRoomSize + roomMargin * 2) * 2;
        bool canHorizontal = space.height >= (minRoomSize + roomMargin * 2) * 2;
        return canVertical || canHorizontal;
    }

    private bool ShouldSplitVertical(RectInt space)
    {
        float ratio = (float)space.width / space.height;
        if (ratio > 1.25f) return true;
        if (ratio < 0.75f) return false;
        return Random.value > 0.5f;
    }
    #endregion

    #region 房间生成（修复版）
    private void GenerateRooms(BSPNode node)
    {
        if (node.IsLeaf)
        {
            // 确保节点空间足够生成房间
            if (!IsSpaceValidForRoom(node.space))
            {
                Debug.LogWarning($"Node {node.space} is too small for room");
                return;
            }

            // 计算可用空间
            int availableWidth = node.space.width - roomMargin * 2;
            int availableHeight = node.space.height - roomMargin * 2;

            // 动态调整房间尺寸
            int roomWidth = Mathf.Clamp(
                Random.Range(minRoomSize, availableWidth),
                minRoomSize,
                availableWidth
            );
            int roomHeight = Mathf.Clamp(
                Random.Range(minRoomSize, availableHeight),
                minRoomSize,
                availableHeight
            );

            // 安全计算偏移量
            int maxOffsetX = Mathf.Max(0, availableWidth - roomWidth);
            int maxOffsetY = Mathf.Max(0, availableHeight - roomHeight);

            int offsetX = roomMargin + (maxOffsetX > 0 ? Random.Range(0, maxOffsetX) : 0);
            int offsetY = roomMargin + (maxOffsetY > 0 ? Random.Range(0, maxOffsetY) : 0);

            RectInt roomRect = new RectInt(
                node.space.x + offsetX,
                node.space.y + offsetY,
                roomWidth,
                roomHeight
            );

            // 二次边界检查
            roomRect.x = Mathf.Clamp(
                roomRect.x,
                node.space.x + roomMargin,
                node.space.xMax - roomMargin - roomWidth
            );
            roomRect.y = Mathf.Clamp(
                roomRect.y,
                node.space.y + roomMargin,
                node.space.yMax - roomMargin - roomHeight
            );

            // 标记网格（添加边界检查）
            for (int x = roomRect.x; x < Mathf.Min(roomRect.xMax, mapGrid.terrainGrid.GetLength(0)); x++)
            {
                for (int y = roomRect.y; y < Mathf.Min(roomRect.yMax, mapGrid.terrainGrid.GetLength(1)); y++)
                {
                    if (x >= 0 && x < mapGrid.terrainGrid.GetLength(0) &&
                        y >= 0 && y < mapGrid.terrainGrid.GetLength(1))
                    {
                        mapGrid.terrainGrid[x, y] = (int)MapGrid.TileType.Floor;
                        mapGrid.roomIdGrid[x, y] = mapGrid.rooms.Count;
                    }
                }
            }

            // 注册房间
            Room room = new Room(mapGrid.rooms.Count, roomRect);
            mapGrid.rooms.Add(room);
            node.room = room;
        }
        else
        {
            GenerateRooms(node.leftChild);
            GenerateRooms(node.rightChild);
        }
    }

    private bool IsSpaceValidForRoom(RectInt space)
    {
        return space.width >= minRoomSize + roomMargin * 2 &&
               space.height >= minRoomSize + roomMargin * 2;
    }
    #endregion

    #region 走廊连接
    private void ConnectRooms(BSPNode node)
    {
        if (node.IsLeaf) return;

        // 获取左右子树中的随机房间
        Room roomA = GetRandomRoom(node.leftChild);
        Room roomB = GetRandomRoom(node.rightChild);

        // 在两个房间之间生成走廊
        GenerateCorridor(roomA, roomB);

        // 递归连接子树
        ConnectRooms(node.leftChild);
        ConnectRooms(node.rightChild);
    }

    private Room GetRandomRoom(BSPNode node)
    {
        if (node.IsLeaf) return node.room;
        return Random.value > 0.5f ?
            GetRandomRoom(node.leftChild) :
            GetRandomRoom(node.rightChild);
    }

    private void GenerateCorridor(Room a, Room b)
    {
        Vector2Int start = a.GetCenter();
        Vector2Int end = b.GetCenter();

        // 生成L型走廊
        if (Random.value > 0.5f)
        {
            CreateHorizontalCorridor(start.x, end.x, start.y);
            CreateVerticalCorridor(start.y, end.y, end.x);
        }
        else
        {
            CreateVerticalCorridor(start.y, end.y, start.x);
            CreateHorizontalCorridor(start.x, end.x, end.y);
        }
    }

    private void CreateHorizontalCorridor(int xStart, int xEnd, int y)
    {
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
        {
            for (int w = -corridorWidth / 2; w <= corridorWidth / 2; w++)
            {
                if (mapGrid.IsInBounds(x, y + w))
                {
                    mapGrid.terrainGrid[x, y + w] = (int)TileType.Floor;
                }
            }
        }
    }

    private void CreateVerticalCorridor(int yStart, int yEnd, int x)
    {
        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
        {
            for (int w = -corridorWidth / 2; w <= corridorWidth / 2; w++)
            {
                if (mapGrid.IsInBounds(x + w, y))
                {
                    mapGrid.terrainGrid[x + w, y] = (int)TileType.Floor;
                }
            }
        }
    }
    #endregion

    #region 调试绘制
    private void OnDrawGizmos()
    {
        if (!drawGizmos || mapGrid == null) return;

        // 绘制房间
        Gizmos.color = roomColor;
        foreach (Room room in mapGrid.rooms)
        {
            Vector3 center = new Vector3(
                room.bounds.x + room.bounds.width / 2f,
                0,
                room.bounds.y + room.bounds.height / 2f
            );
            Vector3 size = new Vector3(room.bounds.width, 0.1f, room.bounds.height);
            Gizmos.DrawCube(center, size);
        }

        // 绘制走廊
        Gizmos.color = corridorColor;
        for (int x = 0; x < mapGrid.terrainGrid.GetLength(0); x++)
        {
            for (int y = 0; y < mapGrid.terrainGrid.GetLength(1); y++)
            {
                if (mapGrid.terrainGrid[x, y] == (int)TileType.Floor &&
                    mapGrid.roomIdGrid[x, y] == -1)
                {
                    Gizmos.DrawCube(
                        new Vector3(x + 0.5f, 0, y + 0.5f),
                        Vector3.one * 0.8f
                    );
                }
            }
        }
    }
    #endregion
}