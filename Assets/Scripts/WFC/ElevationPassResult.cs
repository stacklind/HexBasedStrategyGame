using HexagonUtils;
using UnityEngine;

public class ElevationPassResult : PassResult
{
    public readonly AxialIndexArray2D<MapNode> Nodes;
    public ElevationPassResult(AxialIndexArray2D<MapNode> nodes)
    {
        Nodes = nodes;
    }
}
