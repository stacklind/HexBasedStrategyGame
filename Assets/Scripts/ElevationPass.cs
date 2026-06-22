using HexagonUtils;
using UnityEngine;
using System;
using System.Collections.Generic;

public class ElevationPass : WFCPass
{
    private readonly uint types = BitwiseUtils.SetAllActiveBits(GameHandler.MapSettings.MaxElevation + 1);
    private readonly uint[] sameRules = Array.ConvertAll(GameHandler.TileRules, item => item.ValidElevations);
    private readonly uint[] neighbourRules = {  0b00000111, 
                                                0b00001111, 
                                                0b00001111, 
                                                0b00001110};

    private Stack<(int, int)> predeterminedStack = new Stack<(int, int)>();

    public ElevationPass(AxialIndexArray2D<MapNode> initialNodes) : base(initialNodes) {    }

    protected override void PrepareSearch(MapNode[] predetermined)
    {
        foreach(MapNode node in nodes.ToArray())
        {
            (int q, int r) = node.Coords;
            int index = BitwiseUtils.GetFirstActiveBitIndex(node.TileType);
            nodes[q, r].Elevation = sameRules[index];
            if (((TileType)node.TileType & (TileType.OCEAN | TileType.COAST)) != 0)
            {
                predeterminedStack.Push(node.Coords);
            }
            else
            {
                searchStack.Push(node.Coords);
            }

        }
    }

    protected override void CollapsePredetermined(MapNode[] predetermined)
    {
        while (predeterminedStack.Count > 0)
        {
            (int q, int r) = predeterminedStack.Pop();
            nodes[q, r].Elevation = BitwiseUtils.TwoToPowerOf(0);
            Propagate(q, r);
        }
    }

    protected override void CollapseNode(int q, int r)
    {
        uint available = nodes[q, r].Elevation;
        if (BitwiseUtils.CountActiveBits(available) == 1) return;
        int index = BitwiseUtils.GetRandomActiveIndex(available);
        uint result = available & 1u << index;
        nodes[q, r].Elevation = result;
        Debug.Log("Set result to " + result);
        Propagate(q, r);
    }

    protected override void Propagate(int q, int r)
    {
        Stack<(int, int)> nodesToEvaluate = new Stack<(int, int)>();
        nodesToEvaluate.Push((q, r));

        while (nodesToEvaluate.Count != 0)
        {
            (q, r) = nodesToEvaluate.Pop();
            uint validAdjacent = GetAvailableAdjacentMask(nodes[q, r].Elevation, types, neighbourRules);
            (int, int)[] neighBours = HexGrid.GetNeighboursAxial(q, r);

            foreach ((int q, int r) neighbour in neighBours)
            {
                int qMin = HexGrid.QMin(neighbour.r);
                if (MapHandler.Instance.IsValidAxial(neighbour.q, neighbour.r))
                {
                    PropagateNeighbour(neighbour.q, neighbour.r, validAdjacent);
                }
            }
        }

        void PropagateNeighbour(int q, int r, uint tileMask)
        {
            uint availableTiles = nodes[q, r].Elevation;
            uint maskedAvailableTiles = availableTiles & tileMask;

            if (availableTiles != maskedAvailableTiles)
            {
                nodes[q, r].Elevation = maskedAvailableTiles;
                nodesToEvaluate.Push((q, r));
            }
        }
    }

    protected override PassResult GenerateResult()
    {
        return new ElevationPassResult(nodes);
    }
}
