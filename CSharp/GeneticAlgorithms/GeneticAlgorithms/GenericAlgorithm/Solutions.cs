using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.GeneticAlgorithms
{
    public partial class Solutions
    {
        public class AppearenceCount
        {
            public static void MinimalPoolSize<G>(GeneticAlgorithm<G> alg, int minPoolSize)
                where G : Chromosome
            {
                alg.GetAppearenceCount = () => minPoolSize - alg.Pool.Count;
            }
        }

        public class MutationOrigins
        {
            public static void Random<G>(GeneticAlgorithm<G> alg, double mutationPercentage)
            where G : Chromosome
            {
                alg.GetMutationOrigins = delegate
                {
                    var res = new Tuple<G, int>[(int)(alg.Pool.Count * mutationPercentage)];
                    for (int i = 0; i < res.Length; i++)
                        res[i] = new Tuple<G, int>(alg.RandomChromosomeFromPool(), 1);
                    return res;
                };
            }

        }

        public class CrossFamilies
        {
            public static void Random<G>(GeneticAlgorithm<G> alg, Func<int,double> familiesCount)
                where G : Chromosome
            {
                alg.GetCrossoverFamilies = delegate
                {
                    var res = new Tuple<G, G, int>[(int)familiesCount(alg.Pool.Count)];
                    for (int i = 0; i < res.Length; i++)
                        res[i] = new Tuple<G, G, int>(alg.RandomChromosomeFromPool(), alg.RandomChromosomeFromPool(), 1);
                    return res;
                };
            }

            public static void Proportional<G>(GeneticAlgorithm<G> alg,Func<int,double> familiesCount)
                where G: Chromosome
            {
                alg.GetCrossoverFamilies = delegate
                {
                    var roulette = alg.Rnd.CreateRoulette(alg.Pool.Select(z => z.Value), 1);
                    var res = new Tuple<G, G, int>[(int)familiesCount(alg.Pool.Count)];
                    for (int i = 0; i < res.Length; i++)
                        res[i] = new Tuple<G, G, int>(
                            alg.Pool[roulette.RandomSector()],
                            alg.Pool[roulette.RandomSector()],
                            1);
                    return res;
                };
            }
        }

        public class Selections
        {
            public static void Threashold<G>(GeneticAlgorithm<G> alg, int maxPoolSize)
             where G : Chromosome
            {
                alg.PerformSelection = delegate(List<G> from, List<G> to)
                {
                    to.AddRange(from.OrderBy(z => -z.Value).Take(maxPoolSize));
                };
            }
        }

        

    }
}