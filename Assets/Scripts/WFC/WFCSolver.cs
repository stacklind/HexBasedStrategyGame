using HexagonUtils;
using System;
using System.Collections.Generic;

public class WFCSolver
{
    public AxialIndexArray2D<MapNode> Solve()
    {
        AxialIndexArray2D<MapNode> nodes = PrepareNodes(GameHandler.MapSettings.Width, GameHandler.MapSettings.Height);
        MapNode[] predetermined = MakePredeterminedTiles();
        TilePass tp = new TilePass(nodes);
        TilePassResult tilePassResult = (TilePassResult)tp.DoPass(predetermined);
        ElevationPass ep = new ElevationPass(tilePassResult.Nodes);
        ElevationPassResult elevationPassResult = (ElevationPassResult)ep.DoPass(new MapNode[0]);
        return elevationPassResult.Nodes;
    }
    private AxialIndexArray2D<MapNode> PrepareNodes(int width, int height)
    {
        uint allTileTypes = (uint)TileType.ALL;
        uint allElevations = BitwiseUtils.SetAllActiveBits(GameHandler.MapSettings.MaxElevation + 1);
        AxialIndexArray2D<MapNode> nodes = new AxialIndexArray2D<MapNode>(width, height);
        for (int r = 0; r < nodes.GetLength(1); r++)
        {
            int qMin = HexGrid.QMin(r);
            for (int q = 0 + qMin; q < qMin + nodes.GetLength(0); q++)
            {
                nodes[q, r] = new MapNode(q, r, allTileTypes, allElevations);
            }
        }
        return nodes;
    }

    private MapNode[] MakePredeterminedTiles()
    {
        List<MapNode> preparedNodes = new List<MapNode>();
        preparedNodes.Add(new MapNode(HexGrid.OddRToAxial(0, 0), (uint)TileType.OCEAN, 0));
        preparedNodes.Add(new MapNode(HexGrid.OddRToAxial(0, GameHandler.MapSettings.Height - 1), (uint)TileType.OCEAN, 0));
        preparedNodes.Add(new MapNode(HexGrid.OddRToAxial(GameHandler.MapSettings.Width - 1, GameHandler.MapSettings.Height - 1), (uint)TileType.OCEAN, 0));
        preparedNodes.Add(new MapNode(HexGrid.OddRToAxial(GameHandler.MapSettings.Width - 1, 0), (uint)TileType.OCEAN, 0));

        return preparedNodes.ToArray();
    }
}