using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Range
    {
        public double Minimum;
        public double Maximum;
        public double Delta;
        public double[][] GenerateSamples()
        {
            return Enumerable
                .Range(0, (int)((Maximum - Minimum) / Delta) + 1)
                .Select(z => z * Delta + Minimum)
                .Select(z => new[] { z })
                .ToArray();
        }
        public Range(double minimum, double maximum, double delta)
        {
            Maximum = maximum;
            Minimum = minimum;
            Delta = delta;
        }
    }
}
