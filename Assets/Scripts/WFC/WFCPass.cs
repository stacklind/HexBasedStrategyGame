using HexagonUtils;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class WFCPass
{
    protected AxialIndexArray2D<MapNode> nodes;
    protected SpecialStack<(int, int)> searchStack;
    public WFCPass(AxialIndexArray2D<MapNode> initialNodes)
    {
        searchStack = new SpecialStack<(int, int)>();
        nodes = initialNodes;
    }

    public virtual PassResult DoPass(MapNode[] predetermined)
    {
        PrepareSearch(predetermined);
        CollapsePredetermined(predetermined);

        while(searchStack.Count > 0)
        {
            (int q, int r) = searchStack.GetRandomElement();
            CollapseNode(q, r);
        }

        return GenerateResult();
    }

    protected virtual uint GetAvailableAdjacentMask(uint available, uint allTypes, uint[] rules)
    {
        uint result = 0;
        int length = BitwiseUtils.GetBitLength(allTypes);
        for (int i = 0; i < length; i++)
        {
            if((available & BitwiseUtils.GetIsolatedBitIndex(allTypes, i)) != 0)
            {
                result |= rules[i];
            }
        }
        return result;
    }
    protected abstract void PrepareSearch(MapNode[] predetermined);
    protected abstract void CollapsePredetermined(MapNode[] predetermined);
    protected abstract void CollapseNode(int q, int r);
    protected abstract void Propagate(int q, int r);
    protected abstract PassResult GenerateResult();

    protected class SpecialStack<T>
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
            for (int i = 0; i < index; i++)
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
            while (tmp.Tail != null)
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

    protected class SpecialDictionary<T, V>
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

        public (T, V) Pop()
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
