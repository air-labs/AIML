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

        public Func<double, double, double> NearFunction = NearGauss(1);

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

        public static Func<double, double, double> NearGauss(double sigma)
        {
            return (x, near) => Math.Exp(-Math.Pow(x - near, 2) / (sigma * sigma));
        }

        public static Func<double, double, double> NearQuadratic(double sigma)
        {
            return (x, near) => Math.Max(0,
                1 - Math.Pow((x- near)/sigma, 2));
        }



        public FuzzyNumber Near(double near)
        {
            var result =  new FuzzyNumber(this);
            for (double x = Min; x <= Max; x += Precision)
            {
                var measure = NearFunction(x, near);
                if (measure > 0.001)
                    result[x] = measure;
            }
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
