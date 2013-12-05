using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuzzyArithmetic
{
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms.DataVisualization.Charting;


    static class Program
    {
        static double Max = 10;
        static double Delta = 0.01;
        static int MaxInt = (int)(Max / Delta);

        static Func<double, double, double> T = (x, y) => x*y;
        static Func<double, double, double> S = (x, y) => Math.Max(x,y);
        static double K = 1;
        

        static double[] Number(double near)
        {
            return Enumerable.Range(0, MaxInt)
                 .Select(z => Math.Exp(-Math.Pow(z * Delta - near, 2) * K))
                 .ToArray();
        }

        static double[] Operation(double[] A, double[] B, Func<double, double, double> operation)
        {
            var result = Enumerable.Range(0, MaxInt).Select(z => double.NaN).ToArray();
            
            for (int i=0;i<A.Length;i++)
                for (int j = 0; j < B.Length; j++)
                {
                    var arg = (int)(operation(i * Delta, j * Delta) / Delta);
                    if (arg < 0 || arg >= MaxInt) continue;
                    var val = T(A[i], B[j]);
                    if (double.IsNaN(result[arg])) result[arg] = val;
                    else result[arg] = S(result[arg], val);
                }

            for (int k=0;k<3;k++)
            for (int i=1;i<result.Length-1;i++)
                result[i]=(result[i]+result[i-1]+result[i+1])/3;

            return result;
        }

        static double[] Operation(double[] A, Func<double,double> operation)
        {
            var result = Enumerable.Range(0, MaxInt).Select(z => double.NaN).ToArray();

            for (int i = 0; i < A.Length; i++)
                 {
                    var arg = (int)(operation(i * Delta) / Delta);
                    if (arg < 0 || arg >= MaxInt) continue;
                    if (double.IsNaN(result[arg])) result[arg] = A[i];
                    else result[arg] = S(result[arg], A[i]);
                }

            int nonNan = 0;
            for (nonNan = 0; nonNan < result.Length; nonNan++)
                if (!double.IsNaN(result[nonNan])) break;
            for (int i = nonNan + 1; i < result.Length; i++)
                if (double.IsNaN(result[i])) result[i] = result[i - 1];
            for (int i = nonNan - 1; i >=0 ; i--)
                if (double.IsNaN(result[i])) result[i] = result[i + 1];



            for (int k = 0; k < 3; k++)
                for (int i = 1; i < result.Length - 1; i++)
                    result[i] = (result[i] + result[i - 1] + result[i + 1]) / 3;

            return result;
        }

        static double[] Rel(double[] A, double[] B, Func<double, double, double> op)
        {
            var result = new double[MaxInt];
            for (int i = 0; i < MaxInt; i++)
                result[i] = op(A[i], B[i]);
            return result;
        }

        static void Add(this SeriesCollection series, double[] A, Color color)
        {
            series.Add(Plot(A, color));
        }


        static Series Plot(double[] number, Color color)
        {
            var series = new Series();
            for (int i=0;i<number.Length;i++)
                if (number[i]>0.01)
                 series.Points.Add(new DataPoint(i*Delta, number[i]));
            series.ChartType = SeriesChartType.FastLine;
            series.MarkerBorderWidth = 5;
            series.Color = color;
            return series;
        }

        static string DTS(double d)
        {
            return d.ToString().Replace(',', '.');
        }

        static void SavePlot(string Name, double[] plot)
        {
            using (var file = new StreamWriter(@"..\..\..\..\LaTeX\Plots\"+Name+".txt"))
            {
                file.WriteLine("# N mu");
                for (int i = 0; i < plot.Length; i += 10)
                    if (plot[i]>0.01)
                        file.WriteLine("{0}\t{1}", DTS(i * Delta), DTS(plot[i]));
            }
            using (var file = new StreamWriter(@"..\..\..\..\LaTeX\Plots\" + Name + ".value.txt"))
            {
                var sum = Enumerable.Range(0, MaxInt).Select(z => z * Delta * plot[z]).Sum();
                var wei = plot.Sum();
                file.WriteLine("{0:0.000}", Math.Round(sum / wei,3));
            }
        }

        //static FuzzyNumber SumTest(double number, int count)
        //{
        //    FuzzyNumber result = null;
        //    for (int i = 0; i < count; i++)
        //    {
        //        var argument = Number(number);
        //        if (result == null) result = argument;
        //        else result = Operation(result, argument, (a,b)=>a+b);
        //    }
        //    return result;
        //}


        static void MakePlots(string suffix)
        {
            SavePlot("4_"+suffix, Number(4));
            SavePlot("2_mult_2_"+suffix, Operation(Number(2), Number(2), (a, b) => a * b));
            SavePlot("2_plus_2_"+suffix, Operation(Number(2), Number(2), (a, b) => a + b));
        }

        [STAThread]
        static void Main()
        {


            var chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.ChartAreas.Add(new ChartArea());


            var A2=Number(2);
            var A3=Number(3);
            SavePlot("2",A2);
            SavePlot("3", A3);
            SavePlot("2_cup1_3", Rel(A2, A3, (a, b) => Math.Max(a, b)));
            SavePlot("2_cap1_3", Rel(A2, A3, (a, b) => Math.Min(a, b)));
            SavePlot("2_cap2_3", Rel(A2, A3, (a, b) => a*b));
            SavePlot("2_cup2_3", Rel(A2, A3, (a, b) => a+b-a*b));
            SavePlot("2_plus_2", Operation(A2, A2, (a, b) => a + b));
            SavePlot("2_mult_2", Operation(A2, A2, (a, b) => a * b));
            SavePlot("8_div_2", Operation(Number(8), A2, (a, b) => a / b));
            SavePlot("4", Number(4));

            K = 0.3;
            MakePlots("K03");
            K = 3;
            MakePlots("K3");

            K = 1;
            T = (a, b) => Math.Min(a, b);
            MakePlots("min");

         //   chart.Series.Add(Operation(Number(8), Number(2), (a, b) => a / b), Color.Red);
          //  chart.Series.Add(Operation(Number(2), Number(2), (a, b) => a + b), Color.Green);
           // chart.Series.Add(Operation(Number(2), Number(2), (a, b) => a * b), Color.Blue);
            //   chart.Series.Add(Operation(Number(4), a => Math.Sqrt(a)), Color.Orange);
          //  chart.Series.Add(Operation(Number(2), a => a * a), Color.Orange);
          //  chart.Series.Add(Operation(Number(2), Number(2), (a,b) => a * b), Color.Orange);
            chart.Series.Add(Number(2), Color.Red);
            chart.Series.Add(Number(3), Color.Green);
            chart.Series.Add(Rel(Number(2), Number(3), (x, y) => x+y-x*y), Color.Orange);

            //chart.Series.Add(Operation(Number(4), Number(4), (a, b) => { if (b == 0) return 0; else return a / b; }), Color.Red);
            //chart.Series.Add(Operation(Number(2), Number(2), (a, b) => a + b), Color.Blue);
                

            var form = new Form();
            form.Controls.Add(chart);
            Application.Run(form);
        }
    }
}
