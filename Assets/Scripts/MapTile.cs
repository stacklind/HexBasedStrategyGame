using HexagonUtils;
using System;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

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

    public MapTile(int x, int y, MapNode mapNode, Transform parent)
    {
        int tileTypeIndex = BitwiseUtils.GetFirstActiveBitIndex(mapNode.TileType);
        TileInfo tileInfo = GameHandler.TileRules[tileTypeIndex].TileInfo;

        int z = mapNode.GetElevation();

        CreateGameObject(x, y, tileInfo.Prefab, parent);
        SetupGameObjectTransform(x, y, z);
        UpdateMeshWithElevation(x, y);
        _tileGOHandler = _gameObject.GetComponent<TileGOHandler>();
        Yield = tileInfo.Yield;
        TileType = (TileType)mapNode.TileType;
    }

    private void CreateGameObject(int x, int y, GameObject prefab, Transform parent)
    {
        _gameObject = GameObject.Instantiate(prefab, parent);
        _gameObject.name = $"{x}, {y}";
    }

    private void SetupGameObjectTransform(int x, int y, int z)
    {
        float worldXCoord = x * GameHandler.HexInfo.Width + (y % 2 * 0.5f * GameHandler.HexInfo.Width); // Offset the x-position of the tile every other row
        float worldYCoord = y * 0.75f * GameHandler.HexInfo.Height;
        _gameObject.transform.localPosition = new Vector3(worldXCoord, z, worldYCoord);
    }

    private void UpdateMeshWithElevation(int x, int y)
    {
        int outerVertStartIndex = 7;
        MeshFilter meshFilter = _gameObject.GetComponent<TileGOHandler>().GetMeshFilter();
        Vector3[] vertices = meshFilter.mesh.vertices;
        int[] outerElevation = GetElevationMidPoints(x, y);
        if (vertices.Length - outerElevation.Length != outerVertStartIndex)
        {
            throw new ArgumentOutOfRangeException($"Something wrong with vertices in object ({x}, {y}).");
        }
        for(int i = outerVertStartIndex; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] + new Vector3(0, outerElevation[i - outerVertStartIndex], 0);
        }
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();
    }

    private int[] GetOuterElevations(int x, int y)
    {
        (int q, int r) = HexGrid.OddRToAxial(x, y);
        MapHandler mh = MapHandler.Instance;
        int thisElevation = mh.GetMapNodeFromAxial(q, r).GetElevation();

        (int q, int r)[] neighbours = mh.GetNeighbourAxials(q, r);
        int[] outerElevations = new int[12];

        for(int i = 0; i < neighbours.Length; i++)
        {
            int corner;
            int edge;

            int neighbourElevation;
            if (mh.IsValidAxial(neighbours[i]))
            {
                neighbourElevation = mh.GetMapNodeFromAxial(neighbours[i].q, neighbours[i].r).GetElevation();

            }
            else
            {
                neighbourElevation = thisElevation;
            }

            int otherNeighbour = i > 0 ? i - 1 : neighbours.Length-1; // Index of neighbour one behind
            int otherNeighbourElevation;
            if (mh.IsValidAxial(neighbours[otherNeighbour]))
            {
                otherNeighbourElevation = mh.GetMapNodeFromAxial(neighbours[otherNeighbour].q, neighbours[otherNeighbour].r).GetElevation();

            }
            else
            {
                otherNeighbourElevation = thisElevation;
            }

            corner = Mathf.Min(neighbourElevation - thisElevation, otherNeighbourElevation - thisElevation, 0);
            edge = Mathf.Min(neighbourElevation - thisElevation, 0);
            outerElevations[i * 2] = corner;
            outerElevations[i * 2 + 1] = edge;
        }

        return outerElevations;
    }

    private int[] GetElevationMidPoints(int x, int y)
    {
        (int q, int r) = HexGrid.OddRToAxial(x, y);
        MapHandler mh = MapHandler.Instance;
        int thisElevation = mh.GetMapNodeFromAxial(q, r).GetElevation();

        (int q, int r)[] neighbours = mh.GetNeighbourAxials(q, r);
        int[] elevationMidPoints = new int[12];

        for (int i = 0; i < neighbours.Length; i++)
        {
            int corner;
            int edge;

            int neighbourElevation;
            if (mh.IsValidAxial(neighbours[i]))
            {
                neighbourElevation = mh.GetMapNodeFromAxial(neighbours[i].q, neighbours[i].r).GetElevation();

            }
            else
            {
                neighbourElevation = thisElevation;
            }

            int otherNeighbour = i > 0 ? i - 1 : 5; // Index of neighbour one behind
            int otherNeighbourElevation;
            if (mh.IsValidAxial(neighbours[otherNeighbour]))
            {
                otherNeighbourElevation = mh.GetMapNodeFromAxial(neighbours[otherNeighbour].q, neighbours[otherNeighbour].r).GetElevation();

            }
            else
            {
                otherNeighbourElevation = thisElevation;
            }
            corner = (thisElevation + neighbourElevation + otherNeighbourElevation) / 3 - thisElevation;
            edge = (neighbourElevation + thisElevation) / 2 - thisElevation;
            elevationMidPoints[i * 2] = corner;
            elevationMidPoints[i * 2 + 1] = edge;
        }

        return elevationMidPoints;
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