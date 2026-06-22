using UnityEngine;
using UnityEngine.UIElements;

public class GameHandler : MonoBehaviour
{
    public static TileRule[] TileRules;
    public static TileType[] ElevationRules;
    public static MapSettings MapSettings;

    [Header("World")]
    [SerializeField]
    private int width = 10;
    [SerializeField]
    private int height = 10;
    [SerializeField]
    private int cellSize = 10;
    [SerializeField, Range(1, 6)]
    private int maxElevation = 3;
    [SerializeField]
    private TileRule[] tileRules;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MapSettings = new MapSettings(
            width,
            height,
            cellSize,
            maxElevation);

        TileRules = tileRules;
        MapHandler.Init();
    }
}
