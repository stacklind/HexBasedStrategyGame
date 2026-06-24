using HexagonUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
     * A type of tile is represented by a bit
     * Ocean = First bit        (0b00000001)
     * Coast = Second bit       (0b00000010)
     * etc.
     */
[Flags]public enum TileType : uint
{
    OCEAN       = 0b00000001,
    COAST       = 0b00000010,
    GRASSLANDS  = 0b00000100,
    PLAINS      = 0b00001000,
    DESERT      = 0b00010000,
    ALL         = 0b00011111
}

public class MapHandler
{
    /// <summary>
    /// Singleton instance of the grid manager.
    /// </summary>
    public static MapHandler Instance { get; private set; }

    private AxialIndexArray2D<MapNode> mapNodes;
    private MapTile[,] map;
    private AStarSolver pathfinding;

    /// <summary>
    /// Constructor for singleton instance.
    /// </summary>
    /// <param name="mapSettings"Settings that define the map></param>
    private MapHandler()
    {
        MapSettings mapSettings = GameHandler.MapSettings;
        Instance = this;
        map = new MapTile[mapSettings.Width, mapSettings.Height];
        WFCSolver wfc = new WFCSolver();
        mapNodes = wfc.Solve();
        //pathfinding = new AStarSolver(mapNodes);
        //MeshGenerator.GenerateAndSaveMesh(Hexagon.GetMeshInfo(GameHandler.HexInfo.Width, GameHandler.HexInfo.Height), "Assets/Meshes");
        GenerateMap(mapSettings.Width, mapSettings.Height);
    }

    /// <summary>
    /// Creates a new Grid.
    /// </summary>
    /// <param name="mapSettings">Settings that define the map.</param>
    public static bool Init()
    {
        if(Instance == null)
        {
            new MapHandler();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the <see cref="TileType"/> at the given XY-coordinates using odd-r layout.
    /// </summary>
    /// <param name="x">X coordinate (row).</param>
    /// <param name="y">Y coordinate (column).</param>
    /// <returns>The tile type at the specified offset coordinates.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the coordinates are outside the map bounds.</exception>
    public TileType GetTileTypeOddR(int x, int y)
    {
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
        {
            throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
        }

        return map[x, y].TileType;
    }

    /// <summary>
    /// Attempts to get the tile type at the given axial coordinates.
    /// </summary>
    /// <param name="q">Axial q coordinate.</param>
    /// <param name="r">Axial r coordinate.</param>
    /// <param name="tileType">Outputs the tile type when found.</param>
    /// <returns>True if the axial coordinates are valid and <paramref name="tileType"/> is set; otherwise false.</returns>
    public TileType GetTileTypeAxial(int q, int r)
    {
        (int x, int y) = HexGrid.AxialToOddR(q, r);
        return GetTileTypeOddR(x, y);
    }

    /// <summary>
    /// Returns the instantiated <see cref="GridTile"/> at the given XY grid coordinates.
    /// </summary>
    /// <param name="x">X coordinate (row).</param>
    /// <param name="y">Y coordinate (column).</param>
    /// <returns>The <see cref="GridTile"/> instance stored at [x,y].</returns>
    public MapTile GetMapTileFromXY(int x, int y) => map[x, y];
    public MapTile GetMapTileFromXY((int x, int y) coords) => map[coords.x, coords.y];
    public MapNode GetMapNodeFromAxial(int q, int r) => mapNodes[q, r];

    public bool FindShortestPath((int x, int y) from, (int x, int y) to, Func<TileType, bool> isWalkable, out MapTile[] path)
    {
        return pathfinding.Solve(from, to, isWalkable, out path);
    }

    public bool IsValidAxial(int q, int r)
    {
        int qMin = HexGrid.QMin(r);
        return (q - qMin) < map.GetLength(0) && (q - qMin) >= 0 && r < map.GetLength(1) && r >= 0;
    }

    public bool IsValidAxial((int q, int r) axial)
    {
        return IsValidAxial(axial.q, axial.r);
    }

    public (int q, int r)[] GetNeighbourAxials(int q, int r)
    {
        (int, int)[] neighbourModifiers = new (int, int)[6]
        {
            (0, -1),
            (-1, 0),
            (-1, 1),
            (0, 1),
            (1, 0),
            (1, -1)
        };
        Queue<(int q, int r)> neighbours = new Queue<(int q, int r)>();
        foreach ((int q, int r) direction in neighbourModifiers)
        {
            neighbours.Enqueue((q + direction.q, r + direction.r));
        }

        return neighbours.ToArray();
    }

    /// <summary>
    /// Instantiates the grid GameObject parent and creates GridTile objects for each cell.
    /// </summary>
    /// <param name="width">Grid width in cells.</param>
    /// <param name="height">Grid height in cells.</param>
    /// <param name="cellSize">Size of each cell in map units.</param>
    private void GenerateMap(int width, int height)
    {
        // Create a parent GameObject to hold all tile GameObjects in the scene hierarchy.
        GameObject parent = new GameObject("Map");

        MapNode[,] xyMap = mapNodes.ToArray();

        Dictionary<TileType,List<MeshFilter>> meshLists = new Dictionary<TileType, List<MeshFilter>>();
        for(uint i = 1u; i < (uint)TileType.ALL; i<<=1)
        {
            meshLists.Add((TileType)i, new List<MeshFilter>());
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Instantiate a GridTile with the resolved PossibleTileTypes and attach it to the parent transform.
                MapTile mapTile = new MapTile(x, y, xyMap[x, y], parent.transform);
                map[x, y] = mapTile;
                meshLists[mapTile.TileType].Add(mapTile.gameObject.GetComponent<TileGOHandler>().GetMeshFilter());
            }
        }
        CombineMeshes(parent, meshLists.Values.ToArray());
    }

    private void CombineMeshes(GameObject parent, List<MeshFilter>[] meshFiltersByTileType)
    {
        CombineInstance[] combineInstances;
        MeshFilter[] meshFilters;
        Debug.Log(meshFiltersByTileType.Length);
        CombineInstance[] finalCombineInstances = new CombineInstance[meshFiltersByTileType.Length];

        for (int i = 0; i < meshFiltersByTileType.Length; i++)
        {
            meshFilters = meshFiltersByTileType[i].ToArray();
            combineInstances = new CombineInstance[meshFilters.Length];
            for (int j = 0; j < meshFilters.Length; j++)
            {
                combineInstances[j] = new CombineInstance()
                {
                    mesh = meshFilters[j].mesh,
                    transform = meshFilters[j].gameObject.transform.localToWorldMatrix
                };
                meshFilters[j].gameObject.SetActive(false);
            }
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances);
            finalCombineInstances[i] = new CombineInstance()
            {
                mesh = combinedMesh,
            };
        }

        MeshFilter parentMeshFilter = parent.AddComponent<MeshFilter>();
        Mesh parentMesh = new Mesh();
        parentMesh.CombineMeshes(finalCombineInstances, false, false);
        parentMeshFilter.mesh = parentMesh;

        MeshRenderer parentMeshRenderer = parent.AddComponent<MeshRenderer>();
        parentMeshRenderer.materials = GameHandler.TileMaterials;

        Debug.Log(parentMeshFilter.mesh.vertexCount);
        Debug.Log(parentMeshFilter.mesh.GetSubMesh(0).vertexCount);
    }
}

public struct MapSettings
{
    public MapSettings(int width, int height, int cellSize, int maxElevation)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        MaxElevation = maxElevation;
    }

    public int Width { get; }
    public int Height { get; }
    public int CellSize { get; }
    public int MaxElevation { get; }
}