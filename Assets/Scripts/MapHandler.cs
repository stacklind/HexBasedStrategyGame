using BitwiseUtils;
using HexagonUtils;
using System;
using UnityEditor;
using UnityEngine;

/*
     * A type of tile is represented by a bit
     * Ocean = First bit        (0b00000001)
     * Coast = Second bit       (0b00000010)
     * etc.
     */
[Flags]
public enum TileType : uint
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

    // 2D array of instantiated MapTile objects indexed by XY grid coordinates.
    private MapTile[,] map;
    private AStarSolver pathfinding;

    /// <summary>
    /// Constructor for singleton instance.
    /// </summary>
    /// <param name="mapSettings"Settings that define the map></param>
    private MapHandler()
    {
        Instance = this;
        MapSettings mapSettings = GameHandler.MapSettings;
        map = new MapTile[mapSettings.Width, mapSettings.Height];
        WFCSolver wfc = new WFCSolver();
        AxialIndexArray2D<MapNode> mapNodes = wfc.Solve();
        //pathfinding = new AStarSolver(mapNodes);
        GenerateGrid(mapSettings.Width, mapSettings.Height, mapSettings.CellSize, mapNodes);
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

    //public void Solve()
    //{
    //    wfcsolver.SolveContinuous();
    //}

    //public void UpdateTile(int x, int y, MapNode mapNode)
    //{
    //    MapTile mapTile = map[x, y];
    //    MapTile newMapTile = new MapTile(x, y, GameHandler.MapSettings.CellSize, (TileType)mapNode.Layers[1], mapTile.gameObject.transform.parent);
    //    GameObject tmObject = mapTile.gameObject.GetComponentInChildren<TextMesh>().gameObject;
    //    tmObject.transform.parent = newMapTile.gameObject.transform;
    //    tmObject.GetComponent<TextMesh>().text = mapNode.Layers[0].ToString();
    //    GameObject.Destroy(mapTile.gameObject);
    //    map[x, y] = newMapTile;

    //}

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
    public MapTile GetGridTileFromGridXY(int x, int y) => map[x, y];
    public MapTile GetGridTileFromGridXY((int x, int y) coords) => map[coords.x, coords.y];

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

    /// <summary>
    /// Instantiates the grid GameObject parent and creates GridTile objects for each cell.
    /// </summary>
    /// <param name="width">Grid width in cells.</param>
    /// <param name="height">Grid height in cells.</param>
    /// <param name="cellSize">Size of each cell in map units.</param>
    private void GenerateGrid(int width, int height, int cellSize, AxialIndexArray2D<MapNode> mapNodes)
    {
        // Create a parent GameObject to hold all tile GameObjects in the scene hierarchy.
        GameObject parent = new GameObject("Grid");

        MapNode[,] xyMap = mapNodes.ToArray();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Instantiate a GridTile with the resolved PossibleTileTypes and attach it to the parent transform.
                MapTile gridTile = new MapTile(x, y, cellSize, xyMap[x, y], parent.transform);
                map[x, y] = gridTile;
            }
        }
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