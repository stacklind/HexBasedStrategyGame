using HexagonUtils;
using BitwiseUtils;
using System;
using UnityEngine;

public class MapTile
{
    private GameObject _gameObject;
    private TileGOHandler _tileGOHandler;
    private Yield _yield;

    public GameObject gameObject { get => _gameObject; }
    public TileType TileType { get; private set; }
    public Yield Yield 
    { 
        get => _yield;
        set
        {
            _yield += value;
            _tileGOHandler.UpdateYieldText(_yield);
        }
    }

    public MapTile(int x, int y, int cellSize, MapNode mapNode, Transform parent)
    {
        // Determine the PossibleTileTypes index from the bitmask stored in xyMap.
        int tileTypeIndex = Bits.GetFirstActiveBitIndex((uint)mapNode.Layers[1], GameHandler.TileRules.Length);
        TileInfo tileInfo = GameHandler.TileRules[tileTypeIndex].TileInfo;

        CreateGameObject(x, y, tileInfo.Prefab, parent);
        SetupGameObjectTransform(x, y, Bits.GetFirstActiveBitIndex(mapNode.Layers[0], GameHandler.MapSettings.MaxElevation + 1), new HexInfo(cellSize));

        _tileGOHandler = _gameObject.GetComponent<TileGOHandler>();
        Yield = tileInfo.Yield;
        TileType = (TileType)mapNode.Layers[0];
    }

    private void CreateGameObject(int x, int y, GameObject prefab, Transform parent)
    {
        _gameObject = GameObject.Instantiate(prefab, parent);
        _gameObject.name = $"{x}, {y}";
    }

    private void SetupGameObjectTransform(int x, int y, int z, HexInfo hex)
    {
        float worldXCoord = x * hex.Width + (y % 2 * 0.5f * hex.Width); // Offset the x-position of the tile every other row
        float worldYCoord = y * 0.75f * hex.Height;
        _gameObject.transform.localPosition = new Vector3(worldXCoord, z, worldYCoord);
    }
}

[Serializable]
public struct Yield
{
    public int food;
    public int production;
    public int gold;

    public Yield(int food, int prod, int gold)
    {
        this.food = food;
        production = prod;
        this.gold = gold;
    }

    public static Yield operator +(Yield a, Yield b) => new Yield(a.food + b.food, a.production + b.production, a.gold + b.gold);
}