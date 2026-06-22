using HexagonUtils;
using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TilePass : WFCPass
{
    private readonly uint types = (uint)TileType.ALL;
    private readonly uint[] rules = Array.ConvertAll(GameHandler.TileRules, item => (uint)item.ValidNeighbours);

    public TilePass(AxialIndexArray2D<MapNode> initialNodes) : base(initialNodes) { }

    protected override void PrepareSearch(MapNode[] predetermined)
    {
        HashSet<(int q, int r)> coordsSet = new HashSet<(int q, int r)>(Array.ConvertAll(predetermined, item => (item.Q, item.R)));
        for (int r = 0; r < nodes.GetLength(1); r++)
        {
            int qMin = HexGrid.QMin(r);
            for (int q = 0 + qMin; q < qMin + nodes.GetLength(0); q++)
            {
                if (coordsSet.Contains((q, r)))
                {
                    continue;
                }
                searchStack.Push((q, r));
            }
        }
    }

    protected override void CollapsePredetermined(MapNode[] predetermined)
    {
        for(int i = 0; i < predetermined.Length; i++)
        {
            int q = predetermined[i].Q;
            int r = predetermined[i].R;
            nodes[q, r] = predetermined[i];
            Propagate(q, r);
        }
    }

    protected override void CollapseNode(int q, int r)
    {
        uint available = nodes[q, r].TileType;
        if (BitwiseUtils.CountActiveBits(available) == 1) return;
        int index = BitwiseUtils.GetRandomActiveIndex(available);
        uint result = available & 1u << index;
        nodes[q, r].TileType = result;
        Propagate(q, r);
    }

    protected override void Propagate(int q, int r)
    {
        Stack<(int, int)> nodesToEvaluate = new Stack<(int, int)>();
        nodesToEvaluate.Push((q, r));

        while (nodesToEvaluate.Count != 0)
        {
            (q, r) = nodesToEvaluate.Pop();
            uint validAdjacent = GetAvailableAdjacentMask(nodes[q, r].TileType, types, rules);
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
            uint availableTiles = nodes[q, r].TileType;
            uint maskedAvailableTiles = availableTiles & tileMask;

            if (availableTiles != maskedAvailableTiles)
            {
                nodes[q, r].TileType = maskedAvailableTiles;
                nodesToEvaluate.Push((q, r));
            }
        }
    }

    protected override PassResult GenerateResult()
    {
        return new TilePassResult(nodes);
    }
}
