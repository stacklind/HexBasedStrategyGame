using System;
using Unity.GraphToolkit.Editor;

namespace MyCollections
{
    public class PriorityQueue<T>
    {
        private Node firstInQueue;
        private Node lastInQueue;

        public int Count { get; private set; }

        public void Enqueue(T item, int priority)
        {
            Count++;
            Node n = new Node(item, priority);
            if (firstInQueue == null)
            {
                firstInQueue = n;
                lastInQueue = firstInQueue;
                return;
            }
            if (firstInQueue.Tail == null)
            {
                firstInQueue.Tail = n;
                lastInQueue = n;
                return;
            }
            if (priority < firstInQueue.priority)
            {
                n.Tail = firstInQueue;
                firstInQueue = n;
                return;
            }

            Node target = firstInQueue.Tail;
            Node previous = firstInQueue;
            while (target != null)
            {
                if (priority < target.priority)
                {
                    previous.Tail = n;
                    n.Tail = target;
                    return;
                }
                previous = target;
                target = target.Tail;
            }

            lastInQueue.Tail = n;
            lastInQueue = n;
        }

        public T Pop()
        {
            if (Count == 0) throw new ArgumentOutOfRangeException();
            Node tmp = firstInQueue;
            firstInQueue = firstInQueue.Tail;
            Count--;
            return tmp.item;
        }

        public bool Contains(T item, out int priority)
        {
            priority = int.MaxValue;
            if (Count == 0) return false;

            (Node result, _) = Find(item);
            if (result != null)
            {
                priority = result.priority;
                return true;
            }

            return false;
        }

        public bool Update(T item, int priority)
        {
            if (Count == 0) return false;
            (Node result, Node previous) = Find(item);
            if (result != null)
            {
                if (previous != null) previous.Tail = result.Tail;
                else firstInQueue = result.Tail;
                if (result == lastInQueue) lastInQueue = previous;
                Enqueue(item, priority);
                return true;
            }

            return false;
        }

        private (Node, Node) Find(T item)
        {
            if (Count == 0) return (null, null);

            Node current = firstInQueue;
            Node previous = null;
            while (current != null)
            {
                if (current.item.Equals(item))
                {
                    return (current, previous);
                }
                previous = current;
                current = current.Tail;
            }

            return (null , null);
        }

        private class Node
        {
            public readonly T item;
            public readonly int priority;
            public Node Tail { get; set; }

            public Node(T item, int priority)
            {
                this.item = item;
                this.priority = priority;
            }
        }
    }
}
