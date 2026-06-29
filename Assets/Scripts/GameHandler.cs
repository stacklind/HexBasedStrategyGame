using HexagonUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class GameHandler : MonoBehaviour
{
    public static TileRule[] TileRules;
    public static TileType[] ElevationRules;
    public static MapSettings MapSettings;
    public static HexInfo HexInfo;
    public static Material[] TileMaterials;

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
    [SerializeField]
    private Material[] tileMaterials;
    [Header("Players")]
    [SerializeField]
    private GameObject playerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MapSettings = new MapSettings(
            width,
            height,
            cellSize,
            maxElevation);

        TileRules = tileRules;
        HexInfo = new HexInfo(cellSize);
        TileMaterials = tileMaterials;
        MapHandler.Init();
    }

    [ContextMenu("Generate Map")]
    private void GenerateEditorMap()
    {
        if(Application.isEditor && !Application.isPlaying)
        {
            MapSettings = new MapSettings(
            width,
            height,
            cellSize,
            maxElevation);

            TileRules = tileRules;
            HexInfo = new HexInfo(cellSize);
            TileMaterials = tileMaterials;
            MapHandler.Reinit();
        }
    }
}
