using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static TileInfo[] TileTypes;
    public static MapSettings _mapSettings;

    [Header("World")]
    [SerializeField]
    private int width = 10;
    [SerializeField]
    private int height = 10;
    [SerializeField]
    private int cellSize = 10;
    [SerializeField]
    private TileInfo[] tileTypes;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MapSettings mapSettings = new MapSettings(
            width,
            height,
            cellSize);

        TileTypes = tileTypes;
        MapHandler.Init(mapSettings);
    }

}
