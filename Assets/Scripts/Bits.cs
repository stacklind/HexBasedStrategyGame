namespace BitwiseUtils
{
    public class Bits
    {
        public static int CountActiveBits(uint n)
        {
            int count = 0;
            while (n != 0)
            {
                count++;
                n &= n - 1;
            }

            return count;
        }

        public static bool IsBitActive(uint n, int i)
        {
            return (n & (1u << i)) != 0;
        }
        public static uint SetActiveBits(int n)
        {
            return TwoToPowerOf(n) - 1;
        }

        public static int GetFirstActiveBitIndex(uint n, int total)
        {
            for (int i = 0; i < total; i++)
            {
                if ((n & (1 << i)) != 0) return i;
            }

            return -1;
        }

        public static uint TwoToPowerOf(int n)
        {
            return 1u << n;
        }
    }
}