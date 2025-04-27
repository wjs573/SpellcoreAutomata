using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public TileData startTile;
    public int maxIterations = 100;
    public bool visualizeGizmos = true;
    public float tileUnitSize = 5f; // 每个单元的物理尺寸

    [Header("Tile Pool")]
    public List<TileData> tilePool = new List<TileData>();

    private List<TileInstance> _placedTiles = new List<TileInstance>();
    private Queue<EdgeInterface> _openEdges = new Queue<EdgeInterface>();
    private Dictionary<Vector3Int, List<TileInstance>> _spatialGrid = new Dictionary<Vector3Int, List<TileInstance>>();

    void Start()
    {
    }

    [Button("Load Tile Data")]
    private void LoadTileData()
    {
        tilePool = Resources.LoadAll<TileData>("ScriptableObjects/Map").ToList();
    }

    [Button("Update Tile Size")]
    private void UpdateTileSize()
    {
        foreach (TileData tile in tilePool)
        {
            tile.UpdateTileSize();
        }
    }

    [Button("Generate Map")]
    public void GenerateMap()
    {
        ClearMap();

        // 使用物理坐标初始化起始Tile
        TileVariant startVariant = new TileVariant(startTile, 0);
        PlaceTile(startVariant, Vector3.zero);

        int iterations = 0;
        while (_openEdges.Count > 0 && iterations++ < maxIterations)
        {
            EdgeInterface currentEdge = _openEdges.Dequeue();
            ProcessEdge(currentEdge);
        }
    }

    void ProcessEdge(EdgeInterface edge)
    {
        // 获取源Tile实例（通过接口位置反查）
        TileInstance source = FindTileByInterface(edge);
        if (source == null) return;

        // 计算需要匹配的接口方向
        Direction requiredDir = GetOppositeDirection(edge.direction);

        // 寻找所有兼容的Tile变体
        List<TileVariant> candidates = FindCompatibleTiles(
            requiredDir
        );

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"No compatible tile found for {edge.direction} edge at {edge.worldPosition}");
            return;
        }

        // 随机尝试候选Tile
        foreach (TileVariant variant in Shuffle(candidates))
        {
            // 计算新Tile的放置位置
            Vector3 newPos = CalculatePlacementPosition(edge, source, variant);

            // 检查碰撞和接口兼容性
            if (!CheckCollision(variant, newPos) &&
                CheckEdgeCompatibility(variant, newPos))
            {
                PlaceTile(variant, newPos);
                return;
            }
        }
    }

    GameObject PlaceTile(TileVariant variant, Vector3 centerPosition)
    {
        // 先设置所有接口的位置（传入 RotatedSize）
        foreach (EdgeInterface edge in variant.interfaces)
        {
            edge.SetPosition(centerPosition, variant.RotatedSize);
        }

        // 然后实例化和初始化
        GameObject tileGO = Instantiate(variant.tile.prefab, centerPosition, variant.Rotation, transform);
        TileInstance instance = tileGO.AddComponent<TileInstance>();
        instance.Initialize(variant);

        _placedTiles.Add(instance);
        UpdateSpatialGrid(instance);

        // 将未连接的接口加入开放队列
        foreach (EdgeInterface edge in variant.interfaces)
        {
            if (!IsEdgeConnected(edge, instance))
            {
                _openEdges.Enqueue(edge);
            }
        }

        return tileGO;
    }

    #region Core Algorithms
    List<TileVariant> FindCompatibleTiles(Direction targetDir)
    {
        List<TileVariant> candidates = new List<TileVariant>();

        foreach (TileData tile in tilePool)
        {
            for (int rot = 0; rot < 4; rot++)
            {
                // 创建临时变体用于测试
                TileVariant variant = new TileVariant(tile, rot);

                // 检查是否有接口能匹配目标位置
                foreach (EdgeInterface edge in variant.interfaces)
                {
                    if (edge.direction == targetDir)
                    {
                        candidates.Add(variant);
                        break;
                    }
                }
            }
        }
        return candidates;
    }

    Vector3 CalculatePlacementPosition(EdgeInterface edge, TileInstance source, TileVariant newVariant)
    {
        // 找到新 Tile 的匹配接口（方向相反）
        EdgeInterface newEdge = newVariant.interfaces.Find(
            e => e.direction == GetOppositeDirection(edge.direction));


        // 计算新 Tile 的 offset（传入旋转后的尺寸）
        Vector3 offset = -newEdge.GetEdgeOffset(newVariant.RotatedSize);

        // 计算最终位置（使两个接口位置重合）
        return offset + edge.worldPosition;
    }
    #endregion

    #region Spatial Query
    bool CheckCollision(TileVariant variant, Vector3 position)
    {
        Bounds newBounds = new Bounds(
            position,
            new Vector3(
                variant.RotatedSize.x * tileUnitSize,
                0.1f,
                variant.RotatedSize.y * tileUnitSize
            )
        );

        // 空间网格优化查询
        Vector3Int gridKey = new Vector3Int(
            Mathf.RoundToInt(position.x / tileUnitSize),
            0,
            Mathf.RoundToInt(position.z / tileUnitSize)
        );

        if (_spatialGrid.TryGetValue(gridKey, out var nearbyTiles))
        {
            foreach (TileInstance tile in nearbyTiles)
            {
                if (tile.GetBounds().Intersects(newBounds))
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool CheckEdgeCompatibility(TileVariant variant, Vector3 position)
    {
        Bounds probeBounds = new Bounds(
            position,
            new Vector3(
                variant.RotatedSize.x * tileUnitSize + 0.1f,
                0.1f,
                variant.RotatedSize.y * tileUnitSize + 0.1f
            )
        );

        // 创建一个临时游戏对象用于检查连接性
        GameObject tempGO = new GameObject("TempTileCheck");
        tempGO.transform.position = position;
        tempGO.transform.rotation = variant.Rotation;

        TileInstance tempInstance = tempGO.AddComponent<TileInstance>();
        tempInstance.Initialize(variant);

        bool isCompatible = true;

        foreach (TileInstance neighbor in _placedTiles)
        {
            if (!neighbor.GetBounds().Intersects(probeBounds)) continue;

            // 检查所有可能的连接方向
            if (!neighbor.TryFindConnection(tempInstance, out _, out _))
            {
                isCompatible = false;
                break;
            }
        }

        // 销毁临时对象
        Destroy(tempGO);

        return isCompatible;
    }

    void UpdateSpatialGrid(TileInstance instance)
    {
        Bounds bounds = instance.GetBounds();
        for (float x = bounds.min.x; x < bounds.max.x; x += tileUnitSize)
        {
            for (float z = bounds.min.z; z < bounds.max.z; z += tileUnitSize)
            {
                Vector3Int key = new Vector3Int(
                    Mathf.RoundToInt(x / tileUnitSize),
                    0,
                    Mathf.RoundToInt(z / tileUnitSize)
                );

                if (!_spatialGrid.ContainsKey(key))
                {
                    _spatialGrid[key] = new List<TileInstance>();
                }
                _spatialGrid[key].Add(instance);
            }
        }
    }
    #endregion

    #region Helper Methods
    TileInstance FindTileByInterface(EdgeInterface edge)
    {
        foreach (TileInstance tile in _placedTiles)
        {
            foreach (EdgeInterface e in tile.variant.interfaces)
            {
                if (Vector3.Distance(e.worldPosition, edge.worldPosition) < 0.1f)
                {
                    return tile;
                }
            }
        }
        return null;
    }

    bool IsEdgeConnected(EdgeInterface edge, TileInstance instance)
    {
        Bounds probeBounds = new Bounds(
            edge.worldPosition,
            new Vector3(0.5f, 0.1f, 0.5f)
        );

        foreach (TileInstance tile in _placedTiles)
        {
            if (tile == instance) continue;

            if (tile.GetBounds().Intersects(probeBounds))
            {
                return true;
            }
        }

        return false;
    }

    Direction GetOppositeDirection(Direction dir)
    {
        return (Direction)(((int)dir + 2) % 4);
    }

    List<T> Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }
    #endregion

    #region Utilities
    public void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        _placedTiles.Clear();
        _openEdges.Clear();
        _spatialGrid.Clear();
    }

    #endregion

    [Header("Debug Settings")]
    public bool showAllVariants = false;
    public Vector2Int gridLayout = new Vector2Int(5, 5); // 排列布局
    public float tileSpacing = 10f; // 间隔距离

    [Button("Debug Generate All Variants")]
    [ContextMenu("Generate All Variants Test")]
    public void GenerateAllVariantsTest()
    {
        ClearMap();

        if (tilePool.Count == 0)
        {
            Debug.LogError("Tile pool is empty!");
            return;
        }

        // 计算布局原点
        Vector3 startPos = transform.position - new Vector3(
            gridLayout.x * tileSpacing * 0.5f,
            0,
            gridLayout.y * tileSpacing * 0.5f
        );

        int index = 0;
        foreach (TileData tile in tilePool)
        {
            for (int rot = 0; rot < 4; rot++) // 0°, 90°, 180°, 270°
            {
                // 计算网格位置
                int x = index % gridLayout.x;
                int z = index / gridLayout.x;
                Vector3 position = startPos + new Vector3(
                    x * tileSpacing,
                    0,
                    z * tileSpacing
                );

                // 创建并放置Tile
                TileVariant variant = new TileVariant(tile, rot);

                GameObject tileInstance = PlaceTile(variant, position);
                tileInstance.name = $"{tile.name}_Rot{rot * 90}";
                tileInstance.GetComponent<TileInstance>().DrawInterface();

                index++;
                if (index >= gridLayout.x * gridLayout.y) return;

            }
        }

        Debug.Log("All variants generated!");
    }
}