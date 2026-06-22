using HexagonUtils;

public abstract class PassResult
{
    public readonly AxialIndexArray2D<MapNode> Nodes;
    public PassResult(AxialIndexArray2D<MapNode> nodes)
    {
        Nodes = nodes;
    }
}
