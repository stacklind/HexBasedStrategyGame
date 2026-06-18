using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static TileInfo[] TileInfos;
    public static MapSettings _mapSettings;

    [Header("World")]
    [SerializeField]
    private int width = 10;
    [SerializeField]
    private int height = 10;
    [SerializeField]
    private int cellSize = 10;
    [SerializeField]
    private TileInfo[] tileInfos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MapSettings mapSettings = new MapSettings(
            width,
            height,
            cellSize);

        TileInfos = tileInfos;
        MapHandler.Init(mapSettings);
    }

}
