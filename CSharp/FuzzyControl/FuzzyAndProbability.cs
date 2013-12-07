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
    static class FuzzyAndProbability
    {

        static double AMin = 2;
        static double AMax = 5;
        static double ADeviation = 1;

        static double BValue = 2;
        static double BDeviation = 1;


        static Domain domain = new Domain(0, 30,0.1);

        static Dictionary<double, double> MakeComputations(IEnumerable<double> ARange, Func<double, double> algorithm)
        {
            var result = new Dictionary<double, double>();
            foreach(var a in ARange)
                result[a] = algorithm(a);
            return result;
        }

        static Series MostProbableResult(IEnumerable<double> AValues, Color color)
        {
            var serie = new Series { Color = color, ChartType = SeriesChartType.FastLine, BorderWidth = 2 };
            var BPoints = (6 * BDeviation) / AValues.Count();
            foreach (var AMeasured in AValues)
            {
                double totalProbability = 0;
                double weightedResult = 0;
                foreach (var AReal in AValues)
                    for (double BReal = BValue - 3*BDeviation; BReal <= BValue + 3*BDeviation; BReal += BPoints)
                    {
                        var probability =
                            Math.Exp(-Math.Pow(AReal - AMeasured, 2) / (2 * ADeviation * ADeviation)) *
                            Math.Exp(-Math.Pow(BReal - BValue, 2) / (2 * BDeviation * BDeviation));
                        totalProbability += probability;
                        weightedResult += RealFunction(AReal, BReal) * probability;
                    }
                var value=weightedResult / totalProbability;
                serie.Points.Add(new DataPoint(AMeasured,value ));
            }
            return serie;
        }


        static Series GetPSerie(Dictionary<double,double> result, Color color)
        {
            var serie = new Series { Color = color, ChartType = SeriesChartType.FastLine, BorderWidth = 2 };
            foreach(var AMeasured in result.Keys)
                serie.Points.Add(new DataPoint(AMeasured,result[AMeasured]));
            return serie;
        }



        static double RealFunction(double AReal, double BReal)
        {
            return AReal*BReal;
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
            var fuzMin=0.5;
            var fuzMax=1.5;
            for (int i = 0; i < fuzzy.Length; i++)
            {
                var sigma = fuzMin + i*(fuzMax - fuzMin)/fuzzy.Length;
                domain.NearFunction = Domain.NearGauss(sigma);
                fuzzy[i] = MakeComputations(AValues, FuzzyAlgorithm);
            }

            chart.Series.Add(MostProbableResult(AValues, Color.Green));
            chart.Series.Add(GetPSerie(exact, Color.Red));
            for (int i = 0; i < fuzzy.Length; i++)
                chart.Series.Add(GetPSerie(fuzzy[i], Color.FromArgb(255 - (i * 255) / (fuzzy.Length + 1), Color.Blue)));
            
            var form = new Form();
            form.Controls.Add(chart);
            Application.Run(form);


        }
    }
}
