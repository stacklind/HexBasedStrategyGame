using TMPro;
using UnityEngine;

public class TileGOHandler : MonoBehaviour
{
    private readonly string FOOD_SPRITE_STRING = "<sprite index=0>";
    private readonly string PROD_SPRITE_STRING = "<sprite index=1>";
    private readonly string GOLD_SPRITE_STRING = "<sprite index=2>";

    [SerializeField]
    TextMeshPro yieldText;
    [SerializeField]
    MeshFilter meshFilter;

    public MeshFilter GetMeshFilter()
    {
        return meshFilter;
    }

    public void UpdateYieldText(Yield yield)
    {
        string text = "";
        if (yield.food != 0)
        {
            text += $"{yield.food}{FOOD_SPRITE_STRING}";
        }
        if (yield.production != 0)
        {
            text += $"{yield.production}{PROD_SPRITE_STRING}";
        }
        if (yield.gold != 0)
        {
            text += $"{yield.gold}{GOLD_SPRITE_STRING}";
        }
        yieldText.text = text;
    }
}
