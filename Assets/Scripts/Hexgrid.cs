using UnityEngine;

namespace HexagonUtils
{
    public class HexGrid
    {
        public static (int, int) OddRToAxial(int x, int y)
        {
            int q = x - (y - (y & 1)) / 2;
            int r = y;
            return (q, r);
        }

        public static (int, int) OddRToAxial((int x, int y) coords) => OddRToAxial(coords.x, coords.y);

        public static (int, int) AxialToOddR(int q, int r)
        {
            int x = q + (r - (r & 1)) / 2;
            int y = r;
            return (x, y);
        }

        public static (int, int) AxialToOddR((int q, int r) coords) => AxialToOddR(coords.q, coords.r);

        public static int AxialDistance((int q, int r) a, (int q, int r) b)
        {
            int dq = Mathf.Abs(a.q - b.q);
            int dr = Mathf.Abs(a.r - b.r);
            int ds = Mathf.Abs((-a.q - a.r) - (-b.q - b.r));
            return (dq + dr + ds) / 2;
        }

        public static int QMin(int r)
        {
            return 0 - (r - (r & 1)) / 2;
        }

        public static (int, int)[] GetNeighboursAxial(int q, int r)
        {
            (int, int)[] neighbours = new (int, int)[6]
            {
            (q + 1, r),     // Right
            (q, r + 1),     // Right-Top
            (q - 1, r + 1), // Left-Top
            (q - 1, r),     // Left
            (q, r - 1),     // Left-Bottom
            (q + 1, r - 1)  // Right-Bottom
            };
            return neighbours;
        }

        public static (int, int)[] GetNeighboursOddR((int x, int y) oddR)
        {
            (int q, int r) = OddRToAxial(oddR.x, oddR.y);
            (int, int)[] axialNeighbours = GetNeighboursAxial(q, r);
            (int, int)[] neighbours = new (int, int)[6];
            for (int i = 0; i < 6; i++)
            {
                neighbours[i] = AxialToOddR(axialNeighbours[i].Item1, axialNeighbours[i].Item2);
            }
            return neighbours;
        }
    }

    public struct AxialIndexArray2D<T>
    {
        private T[,] array;

        public AxialIndexArray2D(int width, int height)
        {
            array = new T[width, height];

        }

        public int GetLength(int dimension)
        {
            return array.GetLength(dimension);
        }

        public T this[int q, int r]
        {
            get
            {
                int offset = HexGrid.QMin(r);
                return array[q - offset, r];
            }

            set
            {
                int offset = HexGrid.QMin(r);
                array[q - offset, r] = value;
            }
        }

        public T[,] ToArray() => array;
    }
}