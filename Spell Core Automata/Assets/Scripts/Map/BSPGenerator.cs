using UnityEngine;
using System.Collections.Generic;

public class BSPGenerator : MonoBehaviour
{
    [Header("Generator Settings")]
    public int dungeonWidth = 100;    // 地牢总宽度
    public int dungeonHeight = 100;   // 地牢总高度
    public int minRoomSize = 8;       // 房间最小尺寸
    public int maxSplitDepth = 5;     // 最大分割深度
    public int corridorWidth = 3;     // 走廊宽度
    
    [Header("Debug")]
    public bool drawGizmos = true;    // 场景视图调试绘制
    public Color roomColor = Color.green;
    public Color corridorColor = Color.yellow;
    private MapGrid mapGrid;          // 网格数据容器
    private List<BSPNode> leafNodes = new List<BSPNode>(); // 所有叶子节点

    // BSP树节点定义
    private class BSPNode
    {
        public RectInt space;         // 节点空间区域
        public BSPNode leftChild;     // 左子节点
        public BSPNode rightChild;    // 右子节点
        public Room room;             // 关联的房间数据
        public int depth;             // 节点深度

        public bool IsLeaf => leftChild == null && rightChild == null;
    }

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

    #region BSP分割逻辑
    private void SplitNode(BSPNode node)
    {
        // 终止条件：达到最大深度或空间太小
        if (node.depth >= maxSplitDepth || 
            node.space.width < minRoomSize * 2 || 
            node.space.height < minRoomSize * 2)
        {
            leafNodes.Add(node);
            return;
        }

        // 根据长宽比决定分割方向
        bool splitVertical = node.space.width > node.space.height;
        if (Mathf.Abs(node.space.width - node.space.height) < minRoomSize)
            splitVertical = Random.value > 0.5f;

        // 计算分割位置（保留最小空间）
        int splitPos = splitVertical ?
            Random.Range(node.space.x + minRoomSize, node.space.xMax - minRoomSize) :
            Random.Range(node.space.y + minRoomSize, node.space.yMax - minRoomSize);

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
    #endregion

    #region 房间生成
    private void GenerateRooms(BSPNode node)
    {
        if (node.IsLeaf)
        {
            // 计算房间尺寸（留出边距）
            int margin = 2;
            int roomWidth = Mathf.Max(
                node.space.width - margin * 2, 
                minRoomSize);
            int roomHeight = Mathf.Max(
                node.space.height - margin * 2, 
                minRoomSize);
            
            // 随机偏移位置
            int offsetX = Random.Range(margin, node.space.width - roomWidth - margin);
            int offsetY = Random.Range(margin, node.space.height - roomHeight - margin);
            
            RectInt roomRect = new RectInt(
                node.space.x + offsetX,
                node.space.y + offsetY,
                roomWidth,
                roomHeight);

            // 创建房间并注册到网格
            Room room = new Room(mapGrid.rooms.Count, roomRect);
            mapGrid.rooms.Add(room);
            node.room = room;

            // 标记网格
            for (int x = roomRect.x; x < roomRect.xMax; x++)
            {
                for (int y = roomRect.y; y < roomRect.yMax; y++)
                {
                    mapGrid.terrainGrid[x, y] = (int)TileType.Floor;
                    mapGrid.roomIdGrid[x, y] = room.id;
                    room.tiles.Add(new Vector2Int(x, y));
                }
            }
        }
        else
        {
            GenerateRooms(node.leftChild);
            GenerateRooms(node.rightChild);
        }
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
            for (int w = -corridorWidth/2; w <= corridorWidth/2; w++)
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
            for (int w = -corridorWidth/2; w <= corridorWidth/2; w++)
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