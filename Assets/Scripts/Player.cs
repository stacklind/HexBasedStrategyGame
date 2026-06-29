using HexagonUtils;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private GameObject _gameObject;

    private LineRenderer borderLine;
    private List<MapTile> ownedTiles;

    public GameObject gameObject { get => _gameObject; }

    public Player(int playerID, GameObject playerObject)
    {
        _gameObject = playerObject;
        borderLine = _gameObject.GetComponent<LineRenderer>();
        ownedTiles = new List<MapTile>();
    }

    public void UpdateBorder(Vector3[] verts)
    {
        borderLine.positionCount = verts.Length;
        borderLine.SetPositions(verts);
    }

    public void ClaimTile(MapTile tile)
    {
        ownedTiles.Add(tile);
        Vector3[] tileVerts = Hexagon.GetHexVerts(tile.X, tile.Y, tile.Z, GameHandler.MapSettings.CellSize);
    }

    private void TakeTileFromPlayer(int playerID, MapTile tile)
    {
        ownedTiles.Remove(tile);
        Vector3[] tileVerts = Hexagon.GetHexVerts(tile.X, tile.Y, tile.Z, GameHandler.MapSettings.CellSize);
    }
}
