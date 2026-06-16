using HexagonUtils;
using System;
using UnityEngine;

public class MapTile
{
    private GameObject _gameObject;
    private TileGOHandler _tileHandler;
    private Yield _yield;

    public GameObject gameObject { get => _gameObject; }
    public TileType TileType { get; private set; }
    public Yield Yield 
    { 
        get => _yield;
        set
        {
            _yield += value;
            _tileHandler.UpdateYieldText(_yield);
        }
    }

    public MapTile(int x, int y, int cellSize, TileInfo tileType, Transform parent)
    {
        CreateGameObject(x, y, tileType.Prefab, parent);
        SetupGameObjectTransform(x, y, new HexInfo(cellSize));

        _tileHandler = _gameObject.GetComponent<TileGOHandler>();
        Yield = tileType.Yield;
    }

    private void CreateGameObject(int x, int y, GameObject prefab, Transform parent)
    {
        _gameObject = GameObject.Instantiate(prefab, parent);
        _gameObject.name = $"{x}, {y}";
    }

    private void SetupGameObjectTransform(int x, int y, HexInfo hex)
    {
        float worldXCoord = x * hex.Width + (y % 2 * 0.5f * hex.Width); // Offset the x-position of the tile every other row
        float worldYCoord = y * 0.75f * hex.Height;
        _gameObject.transform.localPosition = new Vector3(worldXCoord, 0, worldYCoord);
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