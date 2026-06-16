using HexagonUtils;
using System;
using UnityEngine;

/*
     * A type of tile is represented by a bit
     * Ocean = First bit        (0b00000001)
     * Coast = Second bit       (0b00000010)
     * etc.
     */
[Flags]
public enum TileTypeEnum : uint
{
    ALL = 0b00011111,
    OCEAN = 0b00000001,
    COAST = 0b00000010,
    GRASSLANDS = 0b00000100,
    PLAINS = 0b00001000,
    DESERT = 0b00010000
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
    private MapHandler(MapSettings mapSettings)
    {
        Instance = this;
        pathfinding = new AStarSolver();
        map = new MapTile[mapSettings.Width, mapSettings.Height];
        WFCSolver wfc = new WFCSolver(mapSettings.Width, mapSettings.Height);
        GenerateGrid(mapSettings.Width, mapSettings.Height, mapSettings.CellSize, wfc.Solve());
    }

    /// <summary>
    /// Creates a new Grid.
    /// </summary>
    /// <param name="mapSettings">Settings that define the map.</param>
    public static bool Init(MapSettings mapSettings)
    {
        if(Instance == null)
        {
            new MapHandler(mapSettings);
            return true;
        }
        Debug.Log("There is already an instance of MapHandler");
        return false;
    }

    /// <summary>
    /// Returns the <see cref="TileTypeEnum"/> at the given XY-coordinates using odd-r layout.
    /// </summary>
    /// <param name="x">X coordinate (row).</param>
    /// <param name="y">Y coordinate (column).</param>
    /// <returns>The tile type at the specified offset coordinates.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the coordinates are outside the map bounds.</exception>
    public TileTypeEnum GetTileTypeOddR(int x, int y)
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
    public bool GetTileTypeAxial(int q, int r, out TileTypeEnum tileType)
    {
        if(IsValidAxial(q, r))
        {
            tileType = map[q, r].TileType;
            return true;
        }
        tileType = 0;
        return false;
    }

    /// <summary>
    /// Returns the instantiated <see cref="GridTile"/> at the given XY grid coordinates.
    /// </summary>
    /// <param name="x">X coordinate (row).</param>
    /// <param name="y">Y coordinate (column).</param>
    /// <returns>The <see cref="GridTile"/> instance stored at [x,y].</returns>
    public MapTile GetGridTileFromGridXY(int x, int y) => map[x, y];
    public MapTile GetGridTileFromGridXY((int x, int y) coords) => map[coords.x, coords.y];

    public MapTile[] FindShortestPath((int x, int y) from, (int x, int y) to, Func<TileTypeEnum, bool> isWalkable)
    {
        return pathfinding.Solve(from, to, isWalkable);
    }

    public bool IsValidAxial(int q, int r)
    {
        int qMin = HexGrid.QMin(r);
        return (q - qMin) < map.GetLength(0) && (q - qMin) >= 0 && r < map.GetLength(1) && r >= 0;
    }

    /// <summary>
    /// Instantiates the grid GameObject parent and creates GridTile objects for each cell.
    /// </summary>
    /// <param name="width">Grid width in cells.</param>
    /// <param name="height">Grid height in cells.</param>
    /// <param name="cellSize">Size of each cell in map units.</param>
    private void GenerateGrid(int width, int height, int cellSize, AxialIndexArray2D<TileTypeEnum> tileTypes)
    {
        // Create a parent GameObject to hold all tile GameObjects in the scene hierarchy.
        GameObject parent = new GameObject("Grid");

        TileTypeEnum[,] xyMap = tileTypes.ToArray();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Determine the TileType index from the bitmask stored in xyMap.
                int tileTypeIndex = BitwiseUtils.GetFirstActiveBitIndex((uint)xyMap[x, y], GameHandler.TileTypes.Length);

                // Instantiate a GridTile with the resolved TileType and attach it to the parent transform.
                MapTile gridTile = new MapTile(x, y, cellSize, GameHandler.TileTypes[tileTypeIndex], parent.transform);
                map[x, y] = gridTile;
            }
        }
    }
}

public struct MapSettings
{
    public MapSettings(int width, int height, int cellSize)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
    }

    public int Width { get; }
    public int Height { get; }
    public int CellSize { get; }
}