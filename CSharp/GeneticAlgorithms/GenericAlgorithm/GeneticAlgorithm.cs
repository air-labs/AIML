using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRLab.GeneticAlgorithms
{
    public class GeneticAlgorithm<T> : IGeneticAlgorithm where T : Chromosome
    {
        public Func<T> CreateEmptyChromosome;
        public Action<T> Evaluate;

        public Func<int> GetAppearenceCount;
        public Func<T> PerformAppearence;
        public Func<IEnumerable<Tuple<T,int>>> GetMutationOrigins;
        public Func<T, T> PerformMutation;
        public Func<IEnumerable<Tuple<T, T, int>>> GetCrossoverFamilies;
        public Func<T, T, T> PerformCrossover;
        public Action<List<T>, List<T>> PerformSelection;
        public Action PerformBanking;
        public event Action IterationCallBack;
        public event Action EvaluationCompleted;
        public event Action EvaluationBegins;
        public Random Rnd { get; private set; }

        public T RandomChromosomeFromPool()
        {
            return Pool[Rnd.Next(Pool.Count)];
        }

        public GeneticAlgorithm(Func<T> createEmptyChromosome)
        {
            Rnd = new Random();
            Pool = new List<T>();
            Buffer = new List<T>();
            Bank = new List<T>();
            GaParameters = new List<GaParameter>();
            CreateEmptyChromosome = createEmptyChromosome;
        }

        public GeneticAlgorithm(Func<T> createEmptyChromosome, int seed) 
            : this(createEmptyChromosome)
            
        {
            Rnd = new Random(seed);
        }

        public List<T> Pool { get; private set; }
        public List<T> Buffer { get; private set; }
        public List<T> Bank { get; private set; }

        public List<GaParameter> GaParameters { get; private set; }
        
        public int CurrentIteration { get; private set; }
        public int CurrentId { get; private set; }

        public bool CanKeepOldGenesInBuffer = true;
        public bool ReevaluateOldGenes = false;
        public bool DisableEqualGenes = true;

        public event EventHandler IterationBegins;

        public bool IsBanking
        {
            get { return PerformBanking != null; }
        }

        public bool IsParameterized
        {
            get { return GaParameters.Count != 0; }
        }

        public void MakeIteration()
        {
            if (IterationBegins != null)
                IterationBegins(this, EventArgs.Empty);

            if (GetAppearenceCount!=null && PerformAppearence != null)
            {
                var c=GetAppearenceCount();
                for (var i = 0; i < c; i++)
                    Buffer.Add(PerformAppearence());
                ++CurrentIteration;
            }

            if (Pool.Count!=0 && GetMutationOrigins != null && PerformMutation != null)
            {
                var mutationSource = GetMutationOrigins();
                foreach (var source in mutationSource)
                {
                    source.Item1.Inherited = true;
                    for (var i = 0; i < source.Item2; i++)
                    {
                        var m = PerformMutation(source.Item1);
                        if (m == null) continue;
                        m.Parent1 = source.Item1.Id;
                        Buffer.Add(m);
                    }
                }
            }

            if (Pool.Count != 0 && GetCrossoverFamilies != null && PerformCrossover != null)
            {
                var pairs = GetCrossoverFamilies();
                foreach (var pair in pairs)
                {
                    pair.Item1.Inherited = pair.Item2.Inherited = true;
                    for (var i=0;i<pair.Item3;i++)
                    {
                        var cross = PerformCrossover(pair.Item1, pair.Item2);
                        if (cross == null) continue;
                        cross.Parent1 = pair.Item1.Id;
                        cross.Parent2 = pair.Item2.Id;
                        Buffer.Add(cross);
                    }
                }
            }

            if (ReevaluateOldGenes)
                foreach (var e in Pool) e.Evaluated = false;

            if (CanKeepOldGenesInBuffer)
                Buffer.AddRange(Pool);

            Pool.Clear();

            if (DisableEqualGenes)
                for (var i = Buffer.Count - 1; i >= 0; i--)
                    for (var j = i - 1; j >= 0; j--)
                        if (Buffer[i].Equals(Buffer[j]))
                        {
                            Buffer.RemoveAt(j);
                            j--;
                            i--;
                        }

            if (EvaluationBegins != null)
                EvaluationBegins();

            foreach (var e in Buffer.Where(z=>!z.Evaluated))
                Evaluate(e);

            if (EvaluationCompleted != null)
                EvaluationCompleted();

            if (PerformSelection != null)
                PerformSelection(Buffer, Pool);
            else
                Pool.AddRange(Buffer);

            Buffer.Clear();

            foreach (var g in Pool)
            {
                g.Evaluated = true;
                g.Age++;
                if (g.Id == 0) g.Id = ++CurrentId;
                if (g.Generation == 0) g.Generation = CurrentIteration;
            }

            if (PerformBanking != null) PerformBanking();

            if (IterationCallBack != null) IterationCallBack();
        }

        public void Refresh()
        {
            Pool.Clear();
            CurrentIteration = 0;
        }

        public void Restart()
        {
            Refresh();
            Bank.Clear();
        }

        public IEnumerable<Chromosome> ChromosomePool { get { return Pool.OrderByDescending(c => c.Value); } }
        public IEnumerable<Chromosome> ChromosomeBank { get { return Bank.OrderByDescending(c => c.Value); } }
        public IEnumerable<GaParameter> Parameters { get { return GaParameters; } }
    }
}