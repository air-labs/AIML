using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuzzyArithmetic
{
    using System.Drawing;
    using System.Windows.Forms.DataVisualization.Charting;
    using FuzzyNumber = Dictionary<double, double>;


    static class Program
    {
        static double Min = 0;
        static double Max = 10;
        static double Delta = 0.1;

        static FuzzyNumber Number(double near)
        {
            return Enumerable.Range(0, (int)((Max - Min) / Delta))
                 .Select(z => z * Delta)
                 .Select(z => new KeyValuePair<double, double>(z, Math.Exp(-Math.Pow(z - near,2))))
                 .ToDictionary(z => z.Key, z => z.Value);
        }

        static FuzzyNumber Operation(FuzzyNumber A, FuzzyNumber B, Func<double, double, double> operation)
        {
            FuzzyNumber result = new FuzzyNumber();
            foreach(var a in A)
                foreach (var b in B)
                {
                    var arg = operation(a.Key, b.Key);
                    var val= a.Value*b.Value;
                    if (!result.ContainsKey(arg)) result[arg] = val;
                    else result[arg] = Math.Max(result[arg], val);
                }
            return result;
        }

       

        static Series Plot(FuzzyNumber number, Color color)
        {
            var series = new Series();
            foreach (var e in number.OrderBy(z => z.Key))
                series.Points.Add(new DataPoint(e.Key, e.Value));
            series.ChartType = SeriesChartType.FastLine;
            series.Color = color;
            return series;
        }

        static FuzzyNumber SumTest(double number, int count)
        {
            FuzzyNumber result = null;
            for (int i = 0; i < count; i++)
            {
                var argument = Number(number);
                if (result == null) result = argument;
                else result = Operation(result, argument, (a,b)=>a+b);
            }
            return result;
        }


        [STAThread]
        static void Main()
        {


            var chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.ChartAreas.Add(new ChartArea());
            chart.Series.Add(Plot(SumTest(1, 2), Color.Red));
            
            var form = new Form();
            form.Controls.Add(chart);
            Application.Run(form);
        }
    }
}
