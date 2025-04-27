using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public int id;
    public RectInt bounds;
    public List<Vector2Int> tiles = new List<Vector2Int>();
    public Color debugColor;

    public Room(int id, RectInt bounds)
    {
        this.id = id;
        this.bounds = bounds;
        this.debugColor = Random.ColorHSV(0, 1, 0.5f, 1, 0.8f, 1);
    }

    public Vector2Int GetCenter()
    {
        return new Vector2Int(
            bounds.x + bounds.width / 2,
            bounds.y + bounds.height / 2
        );
    }
}