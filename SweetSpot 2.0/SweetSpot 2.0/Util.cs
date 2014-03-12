﻿using System;
using System.Collections.Generic;

namespace SweetSpot_2._0
{
    public static class Util
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
    }
}
