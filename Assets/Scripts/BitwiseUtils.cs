public class BitwiseUtils
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

    public static int GetFirstActiveBitIndex(uint n, int total)
    {
        for(int i = 0; i < total; i++)
        {
            if ((n & (1 << i)) != 0) return i;
        }

        return -1;
    }
}
