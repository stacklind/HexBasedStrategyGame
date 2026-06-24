using UnityEngine;

public class MapNode
{
    public readonly int Q;
    public readonly int R;
    public readonly (int q, int r) Coords;
    public uint TileType;
    public uint ElevationRaw;

    public MapNode((int q, int r) coords, uint tileType, uint elevation)
    {
        Q = coords.q;
        R = coords.r;
        Coords = coords;
        TileType = tileType;
        ElevationRaw = elevation;
    }
    public MapNode(int q, int r, uint tileType, uint elevation)
    {
        Q = q;
        R = r;
        Coords = (q, r);
        TileType = tileType;
        ElevationRaw = elevation;
    }

    public int GetElevation()
    {
        return BitwiseUtils.GetFirstActiveBitIndex(ElevationRaw);
    }
}
