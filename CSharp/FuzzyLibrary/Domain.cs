using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLibrary
{
    public class Domain
    {
        public readonly double Max;
        public readonly double Min;
        public readonly double Precision;
        public readonly int ArrayLength;
        public double DefaultSpread = 1;

        public int ToInt(double x)
        {
            return (int)((x - Min) / Precision);
        }

        public bool Contains(double x)
        {
            return x <= Max && x >= Min;
        }

        public Func<double, double, double> T = TMul;

        public Func<double, double, double> S = SMax;

        public Domain(double min, double max, double precision=0.01)
        {
            Max = max;
            Min = min;
            Precision = precision;
            ArrayLength = (int)((Max - Min) / Precision) + 10;
        }

        public static readonly Func<double, double, double> SMax = (x, y) => Math.Max(x, y);
        public static readonly Func<double, double, double> SSum = (x, y) => x + y - x * y;


        public static readonly Func<double, double, double> TMin = (x, y) => Math.Min(x, y);
        public static readonly Func<double, double, double> TMul = (x, y) => x * y;

        public FuzzyNumber Near(double near, double sigma = double.NaN)
        {
            if (double.IsNaN(sigma)) sigma = DefaultSpread;
            var result =  new FuzzyNumber(this);
            var min=Math.Max(Min,near - 3 * sigma);
            var max=Math.Min(Max,near + 3 * sigma);

            for (double x = min; x <= max; x += Precision)
                result[x] = Math.Exp(-Math.Pow(x - near, 2) * sigma);
            return result;
        }

        public IEnumerable<double> Arguments
        {
            get
            {
                for (double x = Min; x <= Max; x += Precision) yield return x;
            }
        }
    }
}
