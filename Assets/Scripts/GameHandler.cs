using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField]
    private ElevationRule[] elevationRules;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MapSettings = new MapSettings(
            width,
            height,
            cellSize,
            maxElevation);

        TileRules = tileRules;
        SetupElevationRules();
        MapHandler.Init();
    }

    //private void Update()
    //{
    //    if (Keyboard.current.kKey.wasReleasedThisFrame)
    //    {
    //        MapHandler.Instance.Solve();
    //    }
    //}

    private void SetupElevationRules()
    {
        ElevationRules = new TileType[maxElevation + 1];
        for (int i = 0; i < elevationRules.Length; i++)
        {
            int minEle = elevationRules[i].MinElevation;
            int maxEle = elevationRules[i].MaxElevation > maxElevation ? maxElevation : elevationRules[i].MaxElevation;
            while (minEle <= maxEle)
            {
                ElevationRules[minEle] |= elevationRules[i].ValidTileTypes;
                minEle++;
            }
        }
    }
}
