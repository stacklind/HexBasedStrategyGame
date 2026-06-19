using UnityEngine;

[CreateAssetMenu(fileName = "ElevationRule", menuName = "Scriptable Objects/ElevationRule")]
public class ElevationRule : ScriptableObject
{
    [SerializeField]
    private TileType validTileTypes;
    [SerializeField, Range(0, 6)]
    private int minElevation;
    [SerializeField, Range(0, 6)]
    private int maxElevation;

    public TileType ValidTileTypes { get => validTileTypes; }
    public int MinElevation { get => minElevation; }
    public int MaxElevation { get => maxElevation; }
}
