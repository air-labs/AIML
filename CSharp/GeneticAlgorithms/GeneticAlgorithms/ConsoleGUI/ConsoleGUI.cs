using System;
using System.Linq;

namespace AIRLab.GeneticAlgorithms
{
    public class ConsoleGui
    {   
        /*
        public static void Run<T>(GeneticAlgorithm<T> alg, int iterationShowRate)
            where T : Chromosome
        {
            Run(alg, iterationShowRate);
        }*/

        public static void Run<T>(GeneticAlgorithm<T> alg, int iterationShowRate)
            where T: Chromosome
        {
            for (var cnt=0;;cnt++)
            {                
                alg.MakeIteration();
                if (cnt%iterationShowRate != 0) continue;
                Console.Clear();
                var bound = Math.Min(alg.Pool.Count, 15);                
                for (var i = 0; i < bound; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("{0}\t{1}", alg.Pool[i].Value, alg.Pool[i]);
                }
                Console.SetCursorPosition(0, 16);                
                Console.WriteLine("Iterations:     " + alg.CurrentIteration);
                Console.WriteLine("Average value:  " + alg.Pool.Select(z => z.Value).Average());
                Console.WriteLine("Average age:    " + alg.Pool.Select(z => z.Age).Average());                       

//                if(!alg.IsParameterized) return;
//                Console.SetCursorPosition(0, 20);
               // alg.Parameters.ForEach(p => Console.WriteLine("{0}\t{1}", p.Name, p.Value));
               // if (stop()) break;
            }
        }
    }
}
