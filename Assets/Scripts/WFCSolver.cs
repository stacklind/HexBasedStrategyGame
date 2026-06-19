using BitwiseUtils;
using HexagonUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WFCSolver
{
    private readonly int NUM_LAYERS = 2;

    private AxialIndexArray2D<MapNode> _nodes;
    private SpecialStack<(int, int)> searchStack;

    public WFCSolver()
    {
        MapSettings mapSettings = GameHandler.MapSettings;
        uint[] defaults =
        {
            Bits.SetActiveBits(mapSettings.MaxElevation + 1),
            (uint) TileType.ALL
        };
        MapNode.Init(NUM_LAYERS, defaults);
        _nodes = new AxialIndexArray2D<MapNode>(mapSettings.Width, mapSettings.Height);
        searchStack = new SpecialStack<(int, int)>();
    }

    public AxialIndexArray2D<MapNode> Solve()
    {
        SpecialDictionary<(int, int), MapNode> predetermined = new SpecialDictionary<(int, int), MapNode>();
        MapNode ocean = new MapNode();
        ocean.Layers[0] = GetBitFromElevation(0);
        ocean.Layers[1] = (uint)TileType.OCEAN;
        predetermined.Add(HexGrid.OddRToAxial(0, 0), ocean);
        ocean = new MapNode();
        ocean.Layers[0] = GetBitFromElevation(0);
        ocean.Layers[1] = (uint)TileType.OCEAN;
        predetermined.Add(HexGrid.OddRToAxial(0, _nodes.GetLength(1) - 1), ocean);
        ocean = new MapNode();
        ocean.Layers[0] = GetBitFromElevation(0);
        ocean.Layers[1] = (uint)TileType.OCEAN;
        predetermined.Add(HexGrid.OddRToAxial(_nodes.GetLength(0) - 1, _nodes.GetLength(1) - 1), ocean);
        ocean = new MapNode();
        ocean.Layers[0] = GetBitFromElevation(0);
        ocean.Layers[1] = (uint)TileType.OCEAN;
        predetermined.Add(HexGrid.OddRToAxial(_nodes.GetLength(0) - 1, 0), ocean);
        PrepareNodes(predetermined);

        if (predetermined.Count > 0)
        {
            CollapsePredetermined(predetermined);
        }


        while (searchStack.Count > 0)
        {
            (int q, int r) = searchStack.GetRandomElement();
            for (int i = 0; i < NUM_LAYERS; i++)
            {
                CollapseNode(q, r, i);
            }
        }

        return _nodes;
    }

    //public void SolveContinuous()
    //{
    //    (int q, int r) = searchStack.GetRandomElement();
    //    Debug.Log($"Collapsing ({q}, {r}) with current 0-value {_nodes[q, r].Layers[0]} and 1-value {_nodes[q, r].Layers[0]}");
    //    for (int i = 0; i < NUM_LAYERS; i++)
    //    {
    //        CollapseNode(q, r, i);
    //    }
    //    (int x, int y) = HexGrid.AxialToOddR(q, r);
    //    MapHandler.Instance.UpdateTile(x, y, _nodes[q, r]);
    //}

    private void PrepareNodes(SpecialDictionary<(int, int), MapNode> predetermined)
    {
        for(int r = 0; r < _nodes.GetLength(1); r++)
        {
            int qMin = HexGrid.QMin(r);
            for (int q = 0 + qMin; q < qMin + _nodes.GetLength(0); q++)
            {
                _nodes[q, r] = new MapNode();
                if (predetermined.ContainsKey((q, r)))
                {
                    continue;
                }
                searchStack.Push((q, r));
            }
        }
    }

    private void CollapseNode(int q, int r, int layer)
    {
        //Debug.Log($"Collapsing ({q}, {r}) in layer " + layer + " node has value " + _nodes[q, r].Layers[layer]);
        uint available = _nodes[q, r].Layers[layer];
        if (Bits.CountActiveBits(available) == 1) return;
        uint result = GetRandomAvailable(available, layer);
        CollapseNodeSpecific(q, r, result, layer);
    }

    private void CollapseNodeSpecific(int q, int r, uint value, int layer)
    {
        //Debug.Log($"Collapse ({q}, {r}) with value " + value + " in node with " + _nodes[q, r].Layers[layer] + " available in layer " + layer);
        if ((_nodes[q, r].Layers[layer] & value) == 0) throw new ArgumentOutOfRangeException("Cant propogate that tile type.");

        _nodes[q, r].Layers[layer] = value;
        if (!UpdateRelatedLayer(q, r, layer))
        {
            throw new Exception("Could not update related layer");
        }
        Propagate(q, r, layer);
    }

    private void CollapsePredetermined(SpecialDictionary<(int, int), MapNode> predetermined)
    {
        while (predetermined.Count > 0)
        {
            ((int q, int r), MapNode mapNode) = predetermined.Pop();
            for(int i = 0; i < NUM_LAYERS; i++)
            {
                CollapseNodeSpecific(q, r, mapNode.Layers[i], i);
            }
            
        }
    }

    private void Propagate(int q, int r, int layer)
    {
        Stack<(int, int)> nodesToEvaluate = new Stack<(int, int)>();
        nodesToEvaluate.Push((q, r));

        while (nodesToEvaluate.Count != 0)
        {
            (q, r) = nodesToEvaluate.Pop();
            //Debug.Log($"({q}, {r}) in layer {layer} has value {_nodes[q, r].Layers[layer]}");
            uint validAdjacent = ComputeValidAdjacent(_nodes[q, r].Layers[layer], layer);
            (int, int)[] neighBours = HexGrid.GetNeighboursAxial(q, r);

            foreach ((int q, int r) neighbour in neighBours)
            {
                int qMin = HexGrid.QMin(neighbour.r);
                if (MapHandler.Instance.IsValidAxial(neighbour.q, neighbour.r))
                {
                    PropagateNeighbour(neighbour.q, neighbour.r, validAdjacent);
                }
            }
        }

        void PropagateNeighbour(int q, int r, uint mask)
        {
            //Debug.Log("Propagate neighbour " + $"({q}, {r})");
            uint available = _nodes[q, r].Layers[layer];
            uint maskedAvailable = available & mask;
            //Debug.Log("Mask: " + mask + " | Available: " + available + " | Masked available: " + maskedAvailable);
            if (available != maskedAvailable)
            {
                _nodes[q, r].Layers[layer] = maskedAvailable;
                if (!UpdateRelatedLayer(q, r, layer))
                {
                    throw new Exception("Could not update related layer");
                }
                nodesToEvaluate.Push((q, r));
            }
        }
    }

    private uint ComputeValidAdjacent(uint available, int layer)
    {
        uint result = 0;
        
        for (int i = 0; i < GetTotalInLayer(layer); i++)
        {
            if (Bits.IsBitActive(available, i))
            {
                result |= GetNeighbourRuleFromBitIndex(i, layer);
            }
        }
        //Debug.Log("Compute valid adjacent for available " + available + " in layer " + layer + " with result " + result);
        return result;
    }

    private uint GetRandomAvailable(uint n, int layer)
    {
        SpecialStack<int> stack = new SpecialStack<int>();
        for(int i = 0; i < GetTotalInLayer(layer); i++)
        {
            if ((n & 1u << i) != 0) { stack.Push(i); }
        }
        int index = stack.GetRandomElement();
        return Bits.TwoToPowerOf(index);
    }

    private int GetTotalInLayer(int layer)
    {
        switch(layer)
        {
            case 0:
                return GameHandler.MapSettings.MaxElevation + 1;
            case 1:
                return Enum.GetNames(typeof(TileType)).Length - 1;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private uint GetElevationRule(int elevation)
    {
        uint asBitIndex = Bits.TwoToPowerOf(elevation);
        return (asBitIndex << 2
            | asBitIndex << 1
            | asBitIndex
            | asBitIndex >> 1
            | asBitIndex >> 2) 
            & (Bits.TwoToPowerOf(GameHandler.MapSettings.MaxElevation + 1) - 1);
    }

    private uint GetBitFromElevation(int elevation)
    {
        return Bits.TwoToPowerOf(elevation);
    }

    private uint GetValidTilesFromElevationMask(uint elevationMask)
    {

        uint result = 0;
        for (int i = 0; i < GameHandler.MapSettings.MaxElevation + 1; i++)
        {
            if(Bits.IsBitActive(elevationMask, i))
            {
                result |= (uint)GameHandler.ElevationRules[i];
            }
        }
        //Debug.Log("Valid tiles from mask " + elevationMask + " gave result " + result);
        return result;
    }

    private uint GetValidElevationsFromTileMask(uint tileMask)
    {
        uint result = 0;
        for(int i = 0; i < GameHandler.ElevationRules.Length; i++)
        {
            if (((uint)GameHandler.ElevationRules[i] & tileMask) != 0)
            {
                result |= GetBitFromElevation(i);
            }
        }
        //Debug.Log("Valid Elevations from mask " + tileMask + " gave result " + result);
        return result;
    }

    private uint GetNeighbourRuleFromBitIndex(int index, int layer)
    {
        switch (layer)
        {
            case 0:
                return GetElevationRule(index);
            case 1:
                return (uint) GameHandler.TileRules[index].ValidNeighbours;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool UpdateRelatedLayer(int q, int r, int updatedLayer)
    {
        //Debug.Log("Update related in " + $"({q}, {r}) in layer " + updatedLayer);
        uint updatedLayerAvailable = _nodes[q, r].Layers[updatedLayer];
        uint layerToUpdateAvailable;
        switch(updatedLayer)
        {
            case 0:
                layerToUpdateAvailable = _nodes[q, r].Layers[1];
                _nodes[q, r].Layers[1] &= GetValidTilesFromElevationMask(updatedLayerAvailable);
                if (_nodes[q, r].Layers[1] != layerToUpdateAvailable)
                {
                    //Debug.Log("Propagate");
                    Propagate(q, r, 1);
                }
                return true;
            case 1:
                layerToUpdateAvailable = _nodes[q, r].Layers[0];
                _nodes[q, r].Layers[0] &= GetValidElevationsFromTileMask(updatedLayerAvailable);
                if (_nodes[q, r].Layers[0] != layerToUpdateAvailable)
                {
                    //Debug.Log("Propagate");
                    Propagate(q, r, 0);
                }
                return true;
            default:
                return false;
        }
    }

    private class SpecialStack<T>
    {
        Node _head;
        public int Count { get; private set; } = 0;

        public SpecialStack() { _head = null; }

        public void Push(T element) 
        { 
            _head = new Node(element, _head); 
            Count++;
        }

        public T Pop()
        {
            if (_head == null) throw new ArgumentOutOfRangeException();
            T result = _head.Element;
            _head = _head.Tail;
            Count--;
            return result;
        }

        public T Peek()
        {
            return _head != null ? _head.Element : throw new ArgumentOutOfRangeException();
        }

        public T GetRandomElement()
        {
            if (_head == null) throw new ArgumentOutOfRangeException();
            if (Count == 1) return Pop();

            System.Random rnd = new System.Random();
            int index = rnd.Next(0, Count);
            Node tmp = _head;
            Node prev = _head;
            for(int i = 0; i < index; i++)
            {
                prev = tmp;
                tmp = tmp.Tail;
            }

            if (tmp == _head) _head = _head.Tail;
            prev.Tail = tmp.Tail;
            Count--;
            return tmp.Element;
        }

        public bool Contains(T element)
        {
            if (_head == null) throw new ArgumentOutOfRangeException();
            if (_head.Element.GetHashCode() == element.GetHashCode()) return true;

            Node tmp = _head;
            while (tmp.Tail != null)
            {
                tmp = tmp.Tail;
                if (tmp.Element.GetHashCode() == element.GetHashCode()) return true;
            }

            return false;
        }

        public T Take(T element)
        {
            if (_head == null) throw new ArgumentOutOfRangeException();
            if (_head.Element.GetHashCode() == element.GetHashCode())
            {
                Count--;
                return Pop();
            }

            Node tmp = _head;
            Node prev = _head;
            while(tmp.Tail != null)
            {
                prev = tmp;
                tmp = tmp.Tail;
                if (tmp.Element.GetHashCode() == element.GetHashCode())
                {
                    prev.Tail = tmp.Tail;
                    Count--;
                    return tmp.Element;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        public T Find(T element)
        {
            if (_head == null) throw new ArgumentOutOfRangeException();
            if (_head.Element.GetHashCode() == element.GetHashCode()) return _head.Element;

            Node tmp = _head;
            while (tmp.Tail != null)
            {
                tmp = tmp.Tail;
                if (tmp.Element.GetHashCode() == element.GetHashCode())
                {
                    return tmp.Element;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        public bool HasNext()
        {
            return _head != null && _head.Tail != null;
        }

        private class Node
        {
            public readonly T Element;
            public Node Tail;
            public Node(T element, Node tail) 
            { 
                Element = element;
                Tail = tail;
            }
        }
    }

    private class SpecialDictionary<T, V>
    {
        SpecialStack<Node> dictionary;

        public int Count { get => dictionary.Count; }

        public SpecialDictionary()
        {
            dictionary = new SpecialStack<Node>();
        }

        public void Add(T key, V value)
        {
            dictionary.Push(new Node(key, value));
        }

        public (T,V) Pop()
        {
            Node tmp = dictionary.Pop();
            return (tmp.Key, tmp.Value);
        }

        public bool ContainsKey(T key)
        {
            return dictionary.Contains(new Node(key, default));
        }

        public V this[T key]
        {
            get => dictionary.Find(new Node(key, default)).Value;
        }

        private class Node
        {
            public readonly T Key;
            public readonly V Value;

            public Node(T key, V value)
            {
                Key = key;
                Value = value;
            }

            public override int GetHashCode() => Key.GetHashCode();
        }
    }
}