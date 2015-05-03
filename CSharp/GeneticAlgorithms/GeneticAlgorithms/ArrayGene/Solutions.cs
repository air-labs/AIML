using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.GeneticAlgorithms
{
    public partial class ArrayGeneSolutions
    {
        public class Crossover
        {
            public static void Mix<G>(GeneticAlgorithm<G> alg)
                where G : ArrayChromosome
            {
                alg.PerformCrossover = delegate(G g1, G g2)
                {
                    var g = alg.CreateEmptyChromosome();
                    g.CheckLength(g1, g2);
                    g.SetObjectCode(i => alg.Rnd.RandomBool() ? g1.ObjectCode.GetValue(i) : g2.ObjectCode.GetValue(i));
                    return g;
                };
            }

            public static void Exchange<G>(GeneticAlgorithm<G> a)
                where G : ArrayChromosome
            {
                a.PerformCrossover = delegate(G g1, G g2)
                {
                    var g = a.CreateEmptyChromosome();
                    g.CheckLength(g1, g2);
                    var position = a.Rnd.RandomInt(g.ObjectCode.Length);
                    g.SetObjectCode(i => i < position ? g1.ObjectCode.GetValue(i) : g2.ObjectCode.GetValue(i));
                    return g;
                };
            }

            public static void Permutation<G>(GeneticAlgorithm<G> a)
                where G : ArrayChromosome<int>
            {
                a.PerformCrossover = delegate(G g1, G g2)
                {
                    var g = a.CreateEmptyChromosome();
                    g.CheckLength(g1, g2);
                    for (int i = 0; i < g.Code.Length; i++)
                        g.Code[i] = g1.Code[g2.Code[i]];
                    return g;
                };
            }
        }

        public class Mutators
        {
            public static void ByElement<G, T>(GeneticAlgorithm<G> alg, Func<Random, T, T> elementMutation)
                where G : ArrayChromosome<T>
                where T : IEquatable<T>
            {
                alg.PerformMutation = delegate(G source)
                {
                    var g = alg.CreateEmptyChromosome();
                    g.CheckLength(source);
                    g.SetCode(p => source.Code[p]);
                    var position = alg.Rnd.RandomInt(g.Code.Length);
                    g.Code[position] = elementMutation(alg.Rnd, g.Code[position]);
                    return g;
                };
            }

            public static void SwitchElements<G,T>(GeneticAlgorithm<G> alg)
                where G: ArrayChromosome<T>
                where T: IEquatable<T>
            {
                alg.PerformMutation = delegate(G source)
                {
                    var g = alg.CreateEmptyChromosome();
                    g.CheckLength(source);
                    g.SetCode(p => source.Code[p]);
                    var pos1 = alg.Rnd.RandomInt(g.Code.Length);
                    var pos2 = alg.Rnd.RandomInt(g.Code.Length);
                    var t = g.Code[pos1];
                    g.Code[pos1] = g.Code[pos2];
                    g.Code[pos2] = t;
                    return g;
                };
            }

            public static void Bool<G>(GeneticAlgorithm<G> a)
                where G : ArrayChromosome<bool>
            {
                ByElement<G, bool>(a, (rnd, c) => !c);
            }
        }

        public class Appearences
        {
            public static void ByElement<G, T>(GeneticAlgorithm<G> alg, Func<Random, T> elementGenerator)
                where G : ArrayChromosome<T>
                where T : IEquatable<T>
            {
                alg.PerformAppearence = delegate
                {
                    var g = alg.CreateEmptyChromosome();
                    g.SetCode(z => elementGenerator(alg.Rnd));
                    return g;
                };
            }

            public static void Bool<G>(GeneticAlgorithm<G> a)
                where G : ArrayChromosome<bool>
            {
                ByElement(a, rnd => rnd.Next(2) == 0);
            }

            public static void Permutation<G>(GeneticAlgorithm<G> alg)
                where G : ArrayChromosome<int>
            {
                alg.PerformAppearence = delegate
                {
                    var g = alg.CreateEmptyChromosome();
                    var c = new bool[g.Code.Length];
                    for (var i = 0; i < g.Code.Length; i++)
                    {
                        var place = alg.Rnd.RandomInt(c.Length);
                        while (c[place])
                        {
                            place++;
                            if (place >= c.Length)
                                place -= c.Length;
                        }
                        g.Code[place] = i;
                        c[place] = true;
                    }
                    return g;
                };
            }
        }
    }
}
