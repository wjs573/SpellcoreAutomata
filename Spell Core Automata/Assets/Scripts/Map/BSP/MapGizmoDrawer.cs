using UnityEngine;

[ExecuteInEditMode] // 允许在编辑模式下运行
public class MapGizmoDrawer : MonoBehaviour
{
    public bool drawInEditMode = true;
    public bool drawInPlayMode = true;
    public Color groundPassableColor = Color.green;
    public Color groundBlockedColor = Color.red;
    public Color flyPassableColor = Color.cyan;
    public Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    public float cellAlpha = 0.3f;

    private void OnDrawGizmos()
    {
        if (!ShouldDraw()) return;
        if (SceneVariants.map == null) return;

        DrawGridCells();
        DrawGridLines();
    }

    private bool ShouldDraw()
    {
        return (Application.isPlaying && drawInPlayMode) || 
               (!Application.isPlaying && drawInEditMode);
    }

    private void DrawGridCells()
    {
        GridInfo[,] grid = SceneVariants.map.grid;
        Vector2 cellSize = SceneVariants.map.gridSize;

        for (int x = 0; x < SceneVariants.map.MapWidth(); x++)
        {
            for (int y = 0; y < SceneVariants.map.MapHeight(); y++)
            {
                Vector3 center = new Vector3(
                    x * cellSize.x,
                    0,
                    y * cellSize.y
                );

                // 绘制地面通行性
                Gizmos.color = grid[x, y].groundCanPass ? 
                    groundPassableColor : groundBlockedColor;
                Gizmos.color *= new Color(1, 1, 1, cellAlpha);
                Gizmos.DrawCube(center, new Vector3(cellSize.x, 0.1f, cellSize.y));

                // 绘制飞行通行性（叠加半透明层）
                if (grid[x, y].flyCanPass)
                {
                    Gizmos.color = flyPassableColor * new Color(1, 1, 1, cellAlpha/2);
                    Gizmos.DrawWireCube(center, new Vector3(cellSize.x, 0.2f, cellSize.y));
                }
            }
        }
    }

    private void DrawGridLines()
    {
        Gizmos.color = gridLineColor;
        Vector2 cellSize = SceneVariants.map.gridSize;
        int width = SceneVariants.map.MapWidth();
        int height = SceneVariants.map.MapHeight();

        // 垂直线
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize.x, 0, 0);
            Vector3 end = new Vector3(x * cellSize.x, 0, height * cellSize.y);
            Gizmos.DrawLine(start, end);
        }

        // 水平线
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(0, 0, y * cellSize.y);
            Vector3 end = new Vector3(width * cellSize.x, 0, y * cellSize.y);
            Gizmos.DrawLine(start, end);
        }
    }
}