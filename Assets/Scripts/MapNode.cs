using System;
public class MapNode
{
    private static int _numberOfLayers;
    private static uint[] _defaults;
    public uint[] Layers;

    public MapNode()
    {
        Layers = new uint[_numberOfLayers];
        for(int i = 0; i < Layers.Length; i++)
        {
            Layers[i] = _defaults[i];
        }
    }

    public static void Init(int numberOfLayers, uint[] defaults)
    {
        if (defaults.Length != numberOfLayers)
        {
            throw new ArgumentException();
        }
        _numberOfLayers = numberOfLayers;
        _defaults = defaults;
    }
}
