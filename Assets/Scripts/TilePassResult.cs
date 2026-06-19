using HexagonUtils;
using UnityEngine;

public class TilePassResult : PassResult
{
    public readonly AxialIndexArray2D<MapNode> Nodes;
    public TilePassResult(AxialIndexArray2D<MapNode> nodes)
    {
        Nodes = nodes;
    }
}
