using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Neuro;

namespace Common
{
    public static class Extensions
    {
        public static void ForEachWeight(this Network network, Func<double, double> modifier)
        {     
            foreach (var l in network.Layers)
                foreach (var n in l.Neurons)
                    for (int i = 0; i < n.Weights.Length; i++)
                        n.Weights[i] = modifier(n.Weights[i]);
        }
    }
}
