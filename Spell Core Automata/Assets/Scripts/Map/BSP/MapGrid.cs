using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TileType { Empty, Floor, Corridor, Wall }
// 配套的网格数据类
public class MapGrid
{
    public enum TileType { Empty, Floor, Wall }

    public int[,] terrainGrid;  // 地形网格
    public int[,] roomIdGrid;   // 房间ID网格
    public List<Room> rooms = new List<Room>();

    public MapGrid(int width, int height)
    {
        terrainGrid = new int[width, height];
        roomIdGrid = new int[width, height];
        
        // 初始化网格
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                roomIdGrid[x, y] = -1;
                terrainGrid[x, y] = (int)TileType.Empty;
            }
        }
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < terrainGrid.GetLength(0) && 
               y >= 0 && y < terrainGrid.GetLength(1);
    }
}