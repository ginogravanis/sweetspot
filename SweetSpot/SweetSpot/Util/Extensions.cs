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

        public static void ShuffleSubset<T>(this IList<T> list, int subsetSize)
        {
            if (subsetSize <= 1)
                return;

            List<int> indexList = Enumerable.Range(0, list.Count).ToList();
            indexList.Shuffle();
            for (int i = 1; i < subsetSize; i++)
            {
                list.Swap(indexList[i - 1], indexList[i]);
            }
        }

        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }

        public static void ClearBetween<T>(this LinkedList<T> list, LinkedListNode<T> start, LinkedListNode<T> end)
        {
            if (start == end)
                return;

            while (start.NextOrFirst() != end)
                list.Remove(start.NextOrFirst());
        }

        public static String GetTimestamp(this DateTime value)
        {
            return value.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
