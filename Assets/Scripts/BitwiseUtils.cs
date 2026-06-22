using System;
using System.Collections.Generic;

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

    public static int GetLastActiveBitIndex(uint n)
    {
        int count = -1;
        while (n > 0)
        {
            count++;
            n >>= 1;
        }

        return count;
    }

    public static uint SetAllActiveBits(int numberOfBits)
    {
        return TwoToPowerOf(numberOfBits) - 1;
    }

    public static uint SetActiveBetween(int minInclusive, int maxInclusive)
    {
        uint result = 0;
        for(int i = minInclusive; i <= maxInclusive; i++)
        {
            result |= 1u << i;
        }

        return result;
    }

    public static uint TwoToPowerOf(int i)
    {
        return 1u << i;
    }

    public static int GetBitLength(uint n)
    {
        return GetLastActiveBitIndex(n) + 1;
    }

    public static int GetFirstActiveBitIndex(uint n)
    {
        int length = GetBitLength(n);
        for (int i = 0; i < length; i++)
        {
            if ((n & (1 << i)) != 0) return i;
        }

        return -1;
    }

    public static int GetRandomActiveIndex(uint n)
    {
        List<int> result = new List<int>();
        int length = GetBitLength(n);
        for (int i = 0; i < length; i++)
        {
            if ((n & 1u << i) != 0) { result.Add(i); }
        }
        Random rand = new Random();
        int randomIndex = rand.Next(result.Count);
        return result[randomIndex];
    }

    public static uint GetIsolatedBitIndex(uint n, int index)
    {
        return n & (1u << index);
    }

    public static uint[] FromBitToArray(uint n)
    {
        uint[] result = new uint[GetBitLength(n)];
        for(int i = 0; i < result.Length; i++)
        {
            result[i] = GetIsolatedBitIndex(n, i);
        }

        return result;
    }
}
