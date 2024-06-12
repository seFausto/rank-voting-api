using System;
using System.Collections.Generic;

namespace RankVotingApi.Common
{

    public static class Helper
    {
        private static readonly Random rng = new();

        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }

            return list;
        }

    }
}
