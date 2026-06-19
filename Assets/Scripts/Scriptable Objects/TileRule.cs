using UnityEngine;

[CreateAssetMenu(fileName = "TileRule", menuName = "Scriptable Objects/TileRule")]
public class TileRule : ScriptableObject
{
    [SerializeField]
    private TileInfo tileInfo;
    [SerializeField]
    private TileType validNeighbours;

    public TileInfo TileInfo { get => tileInfo; }
    public TileType ValidNeighbours { get => validNeighbours; }
}
