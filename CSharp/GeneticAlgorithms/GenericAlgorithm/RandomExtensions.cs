using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.GeneticAlgorithms
{
    public class Roulette
    {
        double[] cumulativeWidthes;
        Random rnd;
        public Roulette(Random rnd, IEnumerable<double> values, double minimalSectorWidth)
        {
            this.rnd = rnd;
            var min = values.Min();
            var addition = min >= minimalSectorWidth ? 0 : -(min - minimalSectorWidth);
            var prob = values.Select(z => z + addition).ToArray();
            for (int i = 1; i < prob.Length; i++) prob[i] += prob[i - 1];
            cumulativeWidthes = prob;
        }
        public int RandomSector()
        {
            var val = rnd.RandomDouble(0, cumulativeWidthes.Last());
            for (int i = 0; i < cumulativeWidthes.Length; i++)
                if (val < cumulativeWidthes[i]) return i;
            return cumulativeWidthes.Length-1;
        }
    }

    public static class RandomExtensions
    {

        public static Roulette CreateRoulette(this Random rnd, IEnumerable<double> values, double minimalSectorWidth)
        {
            return new Roulette(rnd, values, minimalSectorWidth);
        }

        public static int RandomInt(this Random Rnd, int max)
        {
            return Rnd.Next(max);
        }
        public static int RandomInt(this Random Rnd, int min, int max)
        {
            return min+Rnd.Next(max-min);
        }

        public static bool RandomBool(this Random Rnd)
        {
            return Rnd.Next(2) == 0;
        }

        public static  double RandomDouble(this Random Rnd, double min, double max)
        {
            return min + (max - min) * Rnd.NextDouble();
        }


        public static  F RandomElement<F>(this Random Rnd, IEnumerable<F> coll)
        {
            var cnt = coll.Count();
            if (cnt == 0) return default(F);
            var num = Rnd.RandomInt(cnt);
            return coll.ElementAt(num);
        }
    }
}
