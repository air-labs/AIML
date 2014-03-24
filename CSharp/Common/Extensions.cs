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
                l.ForEachWeight(modifier);
        }

        public static void ForEachWeight(this Layer layer, Func<double, double> modifier)
        {
            foreach (var n in layer.Neurons)
            {
                if (n is ActivationNeuron)
                    (n as ActivationNeuron).Threshold = modifier((n as ActivationNeuron).Threshold);
                for (int i = 0; i < n.Weights.Length; i++)
                    n.Weights[i] = modifier(n.Weights[i]);
            }
        }

        public static double[] GetWeightsVector(this Network network)
        {
            var list = new List<double>();
            network.ForEachWeight(z => { list.Add(z); return z; });
            return list.ToArray();
        }
    }
}
