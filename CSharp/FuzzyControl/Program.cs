using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FuzzyLibrary;

namespace FuzzyControl
{
    static class Program
    {

        static double AMin = 5;
        static double AMax = 10;
        static double ADeviation = 2;

        static double BValue = 10;
        static double BDeviation = 2;


        static Domain domain = new Domain(0, 20,0.1);

        static Dictionary<double, double> MakeComputations(IEnumerable<double> ARange, Func<double, double> algorithm)
        {
            var result = new Dictionary<double, double>();
            foreach(var a in ARange)
                result[a] = algorithm(a);
            return result;
        }

        static Series GetTSerie(Dictionary<double,double> result, Color color)
        {
            var serie = new Series() { Color = color, ChartType = SeriesChartType.FastLine, BorderWidth = 2 };
            foreach(var e in result)
            serie.Points.Add(new DataPoint(e.Key, e.Value));
            return serie; 
        }

        static Series GetPSerie(IEnumerable<double> AValues, Dictionary<double,double> result, Color color)
        {
            var serie = new Series { Color = color, ChartType = SeriesChartType.FastLine, BorderWidth=2 };
            var BPoints = (2 * BDeviation) / AValues.Count();
            foreach(var AMeasured in AValues)
            {
                double totalProbability=0;
                double weightedResult=0;
                double successProbability = 0;
                foreach (var AReal in AValues)
                    for (double BReal=BValue-BDeviation;BReal<=BValue+BDeviation;BReal+=BPoints)
                    {
                        var probability=
                            Math.Exp(-Math.Pow(AReal-AMeasured,2)/(2*ADeviation*ADeviation))*
                            Math.Exp(-Math.Pow(BReal-BValue,2)/(2*BDeviation*BDeviation));
                        totalProbability+=probability;
                        double value=0;
                        if (result!=null)
                            value = result[AMeasured];
                        else 
                            value=RealFunction(AReal,BReal);
                        weightedResult += value*probability;
                        successProbability += (Math.Abs(value - RealFunction(AReal, BReal)) < 2)?probability:0;
                    }
                serie.Points.Add(new DataPoint(AMeasured, weightedResult / totalProbability));
            }
            return serie;
        }


       // [STAThread]
       // static void Main()
       // {
       //     var fuzzyResult = MakeComputations(FuzzyAlgorithm);
       //     var exactResult = MakeComputations(ExactAlgorithm);

       //     Func<Dictionary<double, double>, Color, Series> graph = GetPSerie;

       //     var chart = new Chart();
       //     chart.Dock = DockStyle.Fill;
       //     chart.ChartAreas.Add(new ChartArea());
       //     chart.Series.Add(graph(exactResult,Color.Red));
       //     chart.Series.Add(graph(fuzzyResult,Color.Green));
       //     var form = new Form();
       //     form.Controls.Add(chart);
       //     Application.Run(form);
       //} 

        

        static double RealFunction(double AReal, double BReal)
        {
            return AReal / BReal;
        }

        static double FuzzyAlgorithm(double AValue)
        {
            var fuzzyA = domain.Near(AValue);
            var fuzzyB = domain.Near(BValue);
            return FuzzyNumber.BinaryOperation(fuzzyA, fuzzyB, RealFunction).Average();
        }

        [STAThread]
        static void Main()
        {
            var chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.ChartAreas.Add(new ChartArea());

            int pointsCount = 10;

            var AValues = Enumerable.Range(0, pointsCount).Select(z => AMin + (AMax - AMin) * z / pointsCount).ToArray();

            var exact = MakeComputations(AValues, a => RealFunction(a,BValue));

            var fuzzy = new Dictionary<double, double>[10];
            for (int i = 0; i < fuzzy.Length; i++)
            {
                domain.NearFunction = Domain.NearQuadratic(1+i*0.5);
                fuzzy[i] = MakeComputations(AValues, FuzzyAlgorithm);
            }

            chart.Series.Add(GetPSerie(AValues, null ,Color.Green));
            chart.Series.Add(GetPSerie(AValues, exact, Color.Red));
            for (int i = 0; i < fuzzy.Length; i++)
                chart.Series.Add(GetPSerie(AValues, fuzzy[i], Color.FromArgb(255 - (i * 255) / (fuzzy.Length + 1), Color.Blue)));
            
            var form = new Form();
            form.Controls.Add(chart);
            Application.Run(form);


        }
    }
}
