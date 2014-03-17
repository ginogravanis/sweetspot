using System;
using System.Collections.Generic;
using System.Linq;

namespace SweetSpot.Util
{
    public static class Extensions
    {
        // Implements http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            for (int n = list.Count; n > 1; n--)
            {
                n--;
                int k = rng.Next(n + 1);
                list.Swap(n, k);
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        public static void SwapPairs<T>(this IList<T> list, int numPairs)
        {
            List<int> indexList = Enumerable.Range(0, list.Count).ToList();
            indexList.Shuffle();
            for (int i = 0; i < numPairs; i++)
            {
                list.Swap(indexList[2*i], indexList[2*i + 1]);
            }
        }

        public static String GetTimestamp(this DateTime value)
        {
            return value.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
