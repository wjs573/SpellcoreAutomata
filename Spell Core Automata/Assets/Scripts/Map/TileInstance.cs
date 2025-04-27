using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TileInstance : MonoBehaviour
{
    [ShowInInspector]
    public TileVariant variant;
    private Bounds _bounds;

    [ShowInInspector]
    public Dictionary<Direction, List<EdgeInterface>> _edgeMap;

    // 用于存储需要绘制的Gizmo数据
    private List<GizmoDrawCommand> _gizmoDrawCommands = new List<GizmoDrawCommand>();
    private struct GizmoDrawCommand
    {
        public Vector3 position;
        public Direction direction;
        public Color color;
    }

    public void Initialize(TileVariant variant)
    {
        this.variant = variant;
        transform.rotation = variant.Rotation;

        UpdateBounds();
        BuildEdgeMap();
    }

    public Bounds GetBounds() => _bounds;

    public List<EdgeInterface> GetInterfaces(Direction direction)
    {
        return _edgeMap.TryGetValue(direction, out var edges) ? edges : new List<EdgeInterface>();
    }

    public bool TryFindConnection(TileInstance other, out EdgeInterface ourEdge, out EdgeInterface theirEdge)
    {
        ourEdge = default;
        theirEdge = default;

        foreach (var ourInterface in variant.interfaces)
        {
            foreach (var theirInterface in other.variant.interfaces)
            {
                if (Vector3.Distance(ourInterface.worldPosition, theirInterface.worldPosition) < 0.5f &&
                    ourInterface.direction == GetOppositeDirection(theirInterface.direction))
                {
                    ourEdge = ourInterface;
                    theirEdge = theirInterface;
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateBounds()
    {
        float unitSize = 5f;
        Vector3 size = new Vector3(
            variant.RotatedSize.x * unitSize,
            0.1f,
            variant.RotatedSize.y * unitSize
        );
        _bounds = new Bounds(transform.position, size);
    }

    private void BuildEdgeMap()
    {
        _edgeMap = new Dictionary<Direction, List<EdgeInterface>>();
        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            _edgeMap[dir] = new List<EdgeInterface>();
        }

        foreach (var edge in variant.interfaces)
        {
            _edgeMap[edge.direction].Add(edge);
        }
    }

    private Direction GetOppositeDirection(Direction dir)
    {
        return (Direction)(((int)dir + 2) % 4);
    }


    private void DrawInterfaceMarker(Vector3 position, Direction direction)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(position, 0.3f);

        Vector3 dirVector = direction switch
        {
            Direction.North => Vector3.forward,
            Direction.South => Vector3.back,
            Direction.East => Vector3.right,
            Direction.West => Vector3.left,
            _ => Vector3.up
        };

        Gizmos.DrawLine(position, position + dirVector * 1f);
    }

    [Button("Draw Interfaces")]
    public void DrawInterface()
    {
        if (variant == null || _edgeMap == null) return;

        Vector3 tileCenter = transform.position;
        Vector2Int rotatedSize = variant.RotatedSize;

        // 清空之前的绘制命令
        _gizmoDrawCommands.Clear();

        foreach (var edgeList in _edgeMap.Values)
        {
            foreach (var edge in edgeList)
            {
                // 绘制连接线（使用Debug.DrawLine）
                edge.DrawRedLineAtEdge(tileCenter, rotatedSize, 100f);

                // 记录Gizmo绘制命令
                _gizmoDrawCommands.Add(new GizmoDrawCommand
                {
                    position = edge.worldPosition,
                    direction = edge.direction,
                    color = Color.green
                });
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || variant == null) return;

        // 绘制Tile边界
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, _bounds.size);

        // 执行缓存的Gizmo绘制命令
        foreach (var cmd in _gizmoDrawCommands)
        {
            Gizmos.color = cmd.color;
            Gizmos.DrawSphere(cmd.position, 0.3f);

            Vector3 dirVector = cmd.direction switch
            {
                Direction.North => Vector3.forward,
                Direction.South => Vector3.back,
                Direction.East => Vector3.right,
                Direction.West => Vector3.left,
                _ => Vector3.up
            };

            Gizmos.DrawLine(cmd.position, cmd.position + dirVector * 1f);
        }
    }
}
