using HexagonUtils;
using MyCollections;
using System;
using System.Collections.Generic;
public class AStarSolver
{
    public MapTile[] Solve((int x, int y) start, (int x, int y) goal, Func<TileType, bool> isWalkable)
    {
        start = HexGrid.OddRToAxial(start);
        goal = HexGrid.OddRToAxial(goal);
        PriorityQueue<(int q, int r)> openSet = new PriorityQueue<(int q, int r)>();
        var cameFrom = new Dictionary<(int q, int r), (int q, int r)>();
        var gScore = new Dictionary<(int q, int r), int>();
        var fScore = new Dictionary<(int q, int r), int>();
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);
        openSet.Enqueue(start, fScore[start]);
        while(openSet.Count > 0)
        {
            (int q, int r) current = openSet.Pop();
            if(current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }
            foreach((int q, int r) neighbour in HexGrid.GetNeighboursAxial(current.q, current.r))
            {
                if(MapHandler.Instance.GetTileTypeAxial(neighbour.q, neighbour.r, out TileType neighbourTile))
                {
                    if (!isWalkable(neighbourTile)) continue;
                    int tentativeGScore = gScore[current] + MovementCost(current, neighbour);
                    if (!gScore.ContainsKey(neighbour) || tentativeGScore < gScore[neighbour])
                    {
                        cameFrom[neighbour] = current;
                        gScore[neighbour] = tentativeGScore;
                        fScore[neighbour] = tentativeGScore + Heuristic(neighbour, goal);
                        if (!openSet.Contains(neighbour, out int priority)) openSet.Enqueue(neighbour, fScore[neighbour]);
                        else if (fScore[neighbour] < priority) openSet.Update(neighbour, fScore[neighbour]);
                    }
                }
            }
        }

        return null; // No path found
    }

    private MapTile[] ReconstructPath(Dictionary<(int q, int r), (int q, int r)> cameFrom, (int q, int r) current)
    {
        Stack<MapTile> path = new Stack<MapTile>();
        path.Push(MapHandler.Instance.GetGridTileFromGridXY(HexGrid.AxialToOddR(current)));
        while(cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Push(MapHandler.Instance.GetGridTileFromGridXY(HexGrid.AxialToOddR(current)));
        }
        return path.ToArray();
    }

    public static bool OnlyLand(TileType tileType)
    {
        return (tileType & TileType.GRASSLANDS) != 0 
            || (tileType & TileType.PLAINS) != 0
            || (tileType & TileType.DESERT) != 0;
    }

    private int Heuristic((int q, int r) from, (int q, int r) to)
    {
        return HexGrid.AxialDistance(from, to) * MovementCost(from, to);
    }

    private int MovementCost((int q, int r) from, (int q, int r) to)
    {
        // For simplicity, we assume all walkable tiles have the same movement cost
        return 1;
    }
}

