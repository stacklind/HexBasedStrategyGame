using HexagonUtils;
using System;
using System.Collections.Generic;

public class WFCSolver
{
    
    private readonly int NUMBER_OF_TILETYPES = 5;

    /*
     * Each tile-type has rules governing what tiles
     * can spawn adjacent to them. An active bit
     * represents that the corresponding tile is a legal
     * adjacent tile.
     * 
     * Since the Ocean-tile is 0b00000001 (first bit active)
     * that means any rule that has the first bit active
     * allows an Ocean-tile to spawn next to the tile.
     */
    private readonly uint OCEAN_RULES =         0b00000011;
    private readonly uint COAST_RULES =         0b00000111;
    private readonly uint GRASSLANDS_RULES =    0b00001110;
    private readonly uint PLAINS_RULES =        0b00011100;
    private readonly uint DESERT_RULES =        0b00011000;

    private AxialIndexArray2D<TileType> _nodes;
    SpecialStack<(int, int)> searchStack;

    public WFCSolver(int width, int height)
    {
        _nodes = new AxialIndexArray2D<TileType>(width, height);
        searchStack = new SpecialStack<(int, int)>();
    }

    public AxialIndexArray2D<TileType> Solve()
    {
        SpecialDictionary<(int, int), TileType> predetermined = new SpecialDictionary<(int, int), TileType>();
        predetermined.Add(HexGrid.OddRToAxial(0, 0), TileType.OCEAN);
        predetermined.Add(HexGrid.OddRToAxial(0, _nodes.GetLength(1) - 1), TileType.OCEAN);
        predetermined.Add(HexGrid.OddRToAxial(_nodes.GetLength(0) - 1, _nodes.GetLength(1) - 1), TileType.OCEAN);
        predetermined.Add(HexGrid.OddRToAxial(_nodes.GetLength(0) - 1, 0), TileType.OCEAN);
        PrepareNodes(predetermined);

        if (predetermined.Count > 0)
        {
            CollapsePredetermined(predetermined);
        }

        while (searchStack.Count > 0)
        {
            (int x, int y) = searchStack.GetRandomElement();
            CollapseNode(x, y);
        }
        
        return _nodes;
    }

    private void PrepareNodes(SpecialDictionary<(int, int), TileType> predetermined)
    {
        for(int r = 0; r < _nodes.GetLength(1); r++)
        {
            int qMin = HexGrid.QMin(r);
            for (int q = 0 + qMin; q < qMin + _nodes.GetLength(0); q++)
            {
                _nodes[q, r] = TileType.ALL;
                if (predetermined.ContainsKey((q, r)))
                {
                    continue;
                }
                searchStack.Push((q, r));
            }
        }
    }

    private void CollapseNode(int q, int r)
    {
        uint available = (uint)_nodes[q, r];
        if (BitwiseUtils.CountActiveBits(available) == 1) return;
        int index = GetRandomAvailableTileType(available);
        uint result = available & 1u << index;
        _nodes[q, r] = (TileType)result;
        Propagate(q, r);
    }

    private void CollapseNodeSpecific(int q, int r, TileType tileType)
    {
        if ((_nodes[q, r] & tileType) == 0) throw new ArgumentOutOfRangeException("Cant propogate that tile type.");

        _nodes[q, r] = tileType;
        Propagate(q, r);
    }

    private void CollapsePredetermined(SpecialDictionary<(int, int), TileType> predetermined)
    {
        while (predetermined.Count > 0)
        {
            ((int q, int r), TileType tileType) = predetermined.Pop();
            CollapseNodeSpecific(q, r, tileType);
        }
    }

    private void Propagate(int q, int r)
    {
        Stack<(int, int)> nodesToEvaluate = new Stack<(int, int)>();
        nodesToEvaluate.Push((q, r));

        while (nodesToEvaluate.Count != 0)
        {
            (q, r) = nodesToEvaluate.Pop();
            TileType validAdjacentTiles = ComputeValidAdjacentTiles(_nodes[q, r]);
            (int, int)[] neighBours = HexGrid.GetNeighboursAxial(q, r);

            foreach((int q, int r) neighbour in neighBours)
            {
                int qMin = HexGrid.QMin(neighbour.r);
                if (MapHandler.Instance.IsValidAxial(neighbour.q, neighbour.r))
                {
                    PropagateNeighbour(neighbour.q, neighbour.r, validAdjacentTiles);
                }
            }
        }

        void PropagateNeighbour(int q, int r, TileType tileMask)
        {
            TileType availableTiles = _nodes[q, r];
            TileType maskedAvailableTiles = availableTiles & tileMask;

            if(availableTiles != maskedAvailableTiles)
            {
                _nodes[q, r] = (TileType)maskedAvailableTiles;
                nodesToEvaluate.Push((q, r));
            }
        }
    }

    private TileType ComputeValidAdjacentTiles(TileType availableTiles)
    {
        uint result = 0b00000000;
        if ((availableTiles & TileType.OCEAN) != 0)      result |= OCEAN_RULES;
        if ((availableTiles & TileType.COAST) != 0)      result |= COAST_RULES;
        if ((availableTiles & TileType.GRASSLANDS) != 0) result |= GRASSLANDS_RULES;
        if ((availableTiles & TileType.PLAINS) != 0)     result |= PLAINS_RULES;
        if ((availableTiles & TileType.DESERT) != 0)     result |= DESERT_RULES;
        return (TileType)result;
    }

    private int GetRandomAvailableTileType(uint n)
    {
        SpecialStack<int> stack = new SpecialStack<int>();
        for(int i = 0; i < NUMBER_OF_TILETYPES; i++)
        {
            if ((n & 1u << i) != 0) { stack.Push(i); }
        }

        return stack.GetRandomElement();
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

            Random rnd = new Random();
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