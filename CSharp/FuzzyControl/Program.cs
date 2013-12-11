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
    class Algorithm
    {
        public Func<double, double, FuzzyNumber> AlgorithmToFuzzy;
        public Func<double, double, double> AlgorithmToNumber;
        public Color Color;
    }


    static class Program
    {
        static double TargetX = 5;
        static double TargetDeviation=0.5;

        static double Velocity = 0.05;
        static int StepCount = (int)(TargetX / Velocity);
        static int IntermediateStepCount = 50;


        #region Запуск

        static double BoxMuller(Random rnd, double M, double sigma)
        {
            var U = rnd.NextDouble();
            var V = rnd.NextDouble();
            var x = Math.Sqrt(-2 * Math.Log(U, Math.E)) * Math.Cos(2 * Math.PI * V);
            return x * sigma + M;
        }


        static Series[] RunCar(Func<double, double, double> algorithm, Color color)
        {
            double x = 0;
            double y = 0;
            double angle=Math.PI/2;
            var rnd = new Random(1);
            

            var path=new Series() { Color = color, ChartType = SeriesChartType.FastLine };
            var dangles=new Series() { Color = color, ChartType = SeriesChartType.FastLine };
            var values=new Series() { Color = color, ChartType = SeriesChartType.FastPoint };


            for (int i=0;i<StepCount;i++)
            {
                var tX = BoxMuller(rnd, TargetX-x, TargetDeviation);
                var ty = BoxMuller(rnd, -y, TargetDeviation);
                var tr = Math.Sqrt(tX * tX + ty * ty);
                var ta = Math.Atan2(ty, tX);
                ta -= angle;
                var rx = tr * Math.Cos(ta);
                var ry = tr * Math.Sin(ta);

                var dangle = algorithm(rx, ry);
                dangle = Math.Sin(dangle) * Math.Min(Math.Abs(dangle), Math.PI/4);
                dangle/=IntermediateStepCount;
                for (int j = 0; j < IntermediateStepCount; j++)
                {
                    angle += dangle;
                    x += Velocity * Math.Cos(angle)/IntermediateStepCount;
                    y += Velocity * Math.Sin(angle) / IntermediateStepCount;
                    path.Points.Add(new DataPoint(x, y)); 
                }
                dangles.Points.Add(new DataPoint(i, dangle));
                values.Points.Add(new DataPoint(y, dangle));
            }
            return new Series[] { path, dangles };
        }

        static void RunAll()
        {
            var charts = Enumerable.Range(0, 2).Select(z => new Chart { ChartAreas = { new ChartArea() }, Dock = DockStyle.Fill }).ToArray();
            foreach (var e in algorithms)
            {
                var series = RunCar(e.AlgorithmToNumber, e.Color);
                for (int i = 0; i < series.Length; i++)
                    charts[i].Series.Add(series[i]);
            }

            int rowCount = 2;
            int columnCount = 2;

            var tableControl = new TableLayoutPanel { RowCount = rowCount, ColumnCount = columnCount, Dock = DockStyle.Fill };
            for (int i = 0; i < charts.Length; i++)
            {
                tableControl.Controls.Add(charts[i], i % rowCount, i / rowCount);
            }
          
            tableControl.RowStyles.Add(new RowStyle { SizeType = System.Windows.Forms.SizeType.Percent, Height = 50 });
            tableControl.RowStyles.Add(new RowStyle { SizeType = System.Windows.Forms.SizeType.Percent, Height = 50 });
            tableControl.ColumnStyles.Add(new ColumnStyle { SizeType = System.Windows.Forms.SizeType.Percent, Width = 50 });
            tableControl.ColumnStyles.Add(new ColumnStyle { SizeType = System.Windows.Forms.SizeType.Percent, Width = 50 });

            var form = new Form
            {
                WindowState = FormWindowState.Maximized,
                Controls = 
                {
                   tableControl
                }
            };

            Application.Run(form);
        }
        #endregion
        #region Алгоритмы

        static double ExactAlgorithm(double x, double y)
        {
            return Math.Atan2(y, x);
        }


        static Domain domain=new Domain(-7,7,0.1);

        static FuzzyNumber FuzzyAlgorithm(double x, double y)
        {
            return FuzzyNumber.BinaryOperation(domain.Near(x), domain.Near(y), ExactAlgorithm);
        }

        static FuzzyNumber MostProbableAlgorithm(double x, double y)
        {
            var gaussFunction = Domain.NearGauss(TargetDeviation);
            int pointCount = 100;
            var halfRange = 2 * TargetDeviation;
            var step = (halfRange* 2) / pointCount;
            var result = domain.CreateEmpty();
            for (double dx=-halfRange;dx<=x+halfRange;dx+=step)
                for (double dy = -halfRange; dy <= +halfRange; dy += step)
                {
                    var value = ExactAlgorithm(x + dx, y + dy);
                    var probability = gaussFunction(dx, 0) * gaussFunction(dy, 0);
                    result[value] += probability;
                }
            var total = result.Domain.Arguments.Sum(z => result[z]);
            foreach (var e in result.Domain.Arguments)
                result[e] /= total;
            return result;
        }

        static FuzzyNumber FuzzyLogic(double x, double y)
        {
            var positive = domain.NumberFromLambda(z => { if (z <0 ) return 0; else return z/domain.Max; });
            var turnRight = domain.Near(Math.PI);

            var result1 = FuzzyNumber.Relation(Implication(positive, turnRight), domain.Near(y));

            var negative = domain.NumberFromLambda(z => { if (z >0) return 0; else return -z/domain.Max; });
            var turnLeft = domain.Near(-Math.PI);
            var result2 = FuzzyNumber.Relation(Implication(negative, turnLeft), domain.Near(y));

            FuzzyNumber.ShowChart(result1.ToPlot(Color.Red), result2.ToPlot(Color.Blue));
            FuzzyNumber.ShowChart(positive.ToPlot(Color.Red), negative.ToPlot(Color.Blue));
            //  FuzzyNumber.ShowChart((result1 & result2).ToPlot(Color.Orange));
            
            return result1 & result2;
        }

        #endregion



       static  List<Algorithm> algorithms = new List<Algorithm>();

      

        static void Compare(double x, double y)
        {
            var args = algorithms.Where(z => z.AlgorithmToFuzzy != null).Select(z => z.AlgorithmToFuzzy(x, y).ToPlot(z.Color)).ToArray();
            FuzzyNumber.ShowChart(args);
        }

        static void Compare(double x)
        {
            var chart = new Chart { ChartAreas = { new ChartArea() }, Dock = DockStyle.Fill };
            foreach (var e in algorithms)
            {
                var serie = new Series { Color = e.Color, ChartType = SeriesChartType.FastLine};
                foreach (var y in domain.Arguments)
                {
                    var value = e.AlgorithmToNumber(x, y);
                    serie.Points.Add(new DataPoint(y, value));
                }
                chart.Series.Add(serie);
            }
            Application.Run(new Form
            {
                Controls = { chart }
            });
        }

        static Func<double, double, double> Implication(FuzzyNumber from, FuzzyNumber to)
        {
            return (x, y) => Math.Max(1 - from[x], from[y]);
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {



       
            domain.NearFunction = Domain.NearQuadratic(2);

            algorithms.Add(new Algorithm { AlgorithmToFuzzy = null, AlgorithmToNumber = ExactAlgorithm, Color = Color.Red });
           algorithms.Add(new Algorithm 
            { 
                AlgorithmToFuzzy = FuzzyAlgorithm, 
                AlgorithmToNumber = (x,y)=>FuzzyAlgorithm(x,y).Average(),
                Color = Color.Green 
            });

            algorithms.Add(new Algorithm
            {
                AlgorithmToFuzzy = MostProbableAlgorithm,
                AlgorithmToNumber = (x, y) => FuzzyAlgorithm(x, y).ArgMax(),
                Color = Color.Blue
            });

            Compare(0.01, 0.8);
            Compare(0.01);

            //RunAll();
            return;

        
        }
    }
}
