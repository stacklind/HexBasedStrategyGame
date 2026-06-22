using UnityEngine;

[CreateAssetMenu(fileName = "TileRule", menuName = "Scriptable Objects/TileRule")]
public class TileRule : ScriptableObject
{
    [SerializeField]
    private TileInfo tileInfo;
    [SerializeField]
    private TileType validNeighbours;
    [SerializeField, Range(0, 6)]
    private int minElevation;
    [SerializeField, Range(0, 6)]
    private int maxElevation;

    public TileInfo TileInfo { get => tileInfo; }
    public TileType ValidNeighbours { get => validNeighbours; }
    public uint ValidElevations { get => BitwiseUtils.SetActiveBetween(minElevation, maxElevation); }
}
