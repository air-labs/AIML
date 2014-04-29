using System;
using System.Collections.Generic;

namespace AIRLab.GeneticAlgorithms
{
    public static class GaHelper
    {
        public static double Round(this double number, int frac)
        {
            return Math.Round(number, frac);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var i in enumerable)
            {
                action(i);
            }
        }
    }
}