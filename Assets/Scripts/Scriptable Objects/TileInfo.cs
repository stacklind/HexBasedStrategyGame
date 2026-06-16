using UnityEngine;

[CreateAssetMenu(fileName = "TileInfo", menuName = "Scriptable Objects/TileInfo")]
public class TileInfo : ScriptableObject
{
    [SerializeField]
    private Yield yield;
    [SerializeField]
    private GameObject prefab;

    public Yield Yield { get => yield; }
    public GameObject Prefab { get => prefab; }
}