using System.Collections.Generic;
using UnityEngine;

public class TileVariant
{
    public TileData tile;
    public int rotationSteps;
    public Vector2Int RotatedSize;

    public List<EdgeInterface> interfaces = new List<EdgeInterface>();

    public TileVariant(TileData tile, int rotationSteps)
    {
        this.tile = tile;
        this.rotationSteps = rotationSteps;
        this.RotatedSize = rotationSteps % 2 == 0 ? 
        tile.baseSize : 
        new Vector2Int(tile.baseSize.y, tile.baseSize.x);

        foreach (EdgeInterface edge in tile.interfaces)
        {
            EdgeInterface rotatedEdge = edge.GetRotated(rotationSteps);
            interfaces.Add(rotatedEdge);
        }
    }

    public Quaternion Rotation => Quaternion.Euler(0, rotationSteps * 90f, 0);
}