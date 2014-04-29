using System;
using System.Collections.Generic;

namespace AIRLab.GeneticAlgorithms
{
    public interface IGeneticAlgorithm
    {
        IEnumerable<Chromosome> ChromosomePool { get; }
        IEnumerable<Chromosome> ChromosomeBank { get; }
        IEnumerable<GaParameter> Parameters { get; }
        int CurrentIteration { get; }
        bool IsBanking { get; }
        bool IsParameterized { get; }
        void MakeIteration();
        void Refresh();
        void Restart();
        event Action IterationCallBack;
    }
}