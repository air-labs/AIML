using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FuzzyLibrary
{


    
    public class FuzzyNumber
    {
        double[] values;


        public readonly Domain Domain;


        public FuzzyNumber(Domain domain)
        {
            this.Domain = domain;
            values = new double[domain.ArrayLength];
        }
       
        public double this[double x]
        {
            get
            {
                if (x < Domain.Min || x > Domain.Max) return 0;                
                return values[Domain.ToInt(x)];
            }
            set
            {
               
                values[Domain.ToInt(x)] = value;
            }
        }

   

        public static FuzzyNumber BinaryOperation(FuzzyNumber A, FuzzyNumber B, Func<double,double,double> op)
        {

            if (A.Domain != B.Domain) throw new ArgumentException("Домены должны совпадать");
            var dom = A.Domain;
            var result = new FuzzyNumber(dom);
            
            foreach(var a in dom.Arguments)
                foreach (var b in dom.Arguments)
                {
                    var value = op(a, b);
                    if (!dom.Contains(value)) continue;
                    result[value] = dom.S(result[value], dom.T(A[a], B[b]));
                }

            for (int k = 0; k < 3; k++)
                foreach (var e in dom.Arguments)
                {
                    if (!dom.Contains(e + dom.Precision) || !dom.Contains(e - dom.Precision)) continue;
                    result[e] = (result[e] + result[e + dom.Precision] + result[e - dom.Precision]) / 3;
                }
            return result;
        }

        public static FuzzyNumber SetOperation(FuzzyNumber A, FuzzyNumber B, Func<double,double,double> op)
        {
            if (A.Domain != B.Domain) throw new ArgumentException("Домены должны совпадать");
            var dom = A.Domain;
            var result=new FuzzyNumber(dom);
            foreach(var x in dom.Arguments)
                result[x] = op(A[x], B[x]);
            return result;
        }

        public static FuzzyNumber operator +(FuzzyNumber a, FuzzyNumber b)
        {
            return BinaryOperation(a, b, (x, y) => x + y);
        }
        public static FuzzyNumber operator -(FuzzyNumber a, FuzzyNumber b)
        {
            return BinaryOperation(a, b, (x, y) => x - y);
        }
        public static FuzzyNumber operator *(FuzzyNumber a, FuzzyNumber b)
        {
            return BinaryOperation(a, b, (x, y) => x * y);
        }
        public static FuzzyNumber operator /(FuzzyNumber a, FuzzyNumber b)
        {
            return BinaryOperation(a, b, (x, y) => x / y);
        }
        public static FuzzyNumber operator |(FuzzyNumber a, FuzzyNumber b)
        {
            return SetOperation(a,b,a.Domain.T);
        }
        public static FuzzyNumber operator &(FuzzyNumber a, FuzzyNumber b)
        {
            return SetOperation(a,b,a.Domain.S);
        }

        public Series ToPlot(Color color)
        {
            var series = new Series();
            foreach(var e in Domain.Arguments) 
                if (this[e] > 0.01)
                    series.Points.Add(new DataPoint(e, this[e]));
            series.ChartType = SeriesChartType.FastLine;
            series.MarkerBorderWidth = 5;
            series.Color = color;
            return series;
        }
    }
}