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
        static double TargetX = 5;
        static double TargetDeviation=0.5;

        static double Velocity = 0.05;
        static int StepCount = (int)(TargetX / Velocity);
        static int IntermediateStepCount = 50;

        static double BoxMuller(Random rnd, double M, double sigma)
        {
            var U = rnd.NextDouble();
            var V = rnd.NextDouble();
            var x = Math.Sqrt(-2 * Math.Log(U, Math.E)) * Math.Cos(2 * Math.PI * V);
            return x * sigma + M;
        }

        static Series[] RunCar(Func<double, double, FuzzyNumber> algorithm, Color color)
        {
            return RunCar((x, y) => algorithm(x, y).Average(), color);
        }

        static Series[] RunCar(Func<double, double, double> algorithm, Color color)
        {
            double x = 0;
            double y = 0;
            double angle=Math.PI/2;
            var rnd = new Random(1);
            var result = new[]
                {
                    new Series() { Color = color, ChartType = SeriesChartType.FastLine },
                    new Series() { Color = color, ChartType = SeriesChartType.FastLine },
                    new Series() { Color = color, ChartType = SeriesChartType.FastPoint },

                };
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
                    result[0].Points.Add(new DataPoint(x, y)); 
                }
                result[1].Points.Add(new DataPoint(i, dangle));
                result[2].Points.Add(new DataPoint(y, dangle));
            }
            return result;
        }

        static double ExactAlgorithm(double x, double y)
        {
            return Math.Atan2(y, x);
        }

        static Domain domain=new Domain(-7,7,0.05);

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
            var max= domain.Arguments.Max(z => result[z]);
            foreach (var e in domain.Arguments)
                result[e] /= max;
            return result;
        }

        static FuzzyNumber FuzzyLogic(double x, double y)
        {
            FuzzyNumber Positive = domain.CreateEmpty();
            
            FuzzyNumber Negative = domain.CreateEmpty();
            FuzzyNumber TurnLeft = domain.Near(Math.PI);
            FuzzyNumber TurnRIght = domain.Near(-Math.PI);
            
            
            foreach (var e in domain.Arguments)
            {
                Positive[e] = Math.Max(0, Math.Min(1, e));
                Negative[e] = 1 - Positive[e];
            }
            
            


            return null;
        }



        static Chart GetChart(params Series[] series)
        {
            var chart =
            new Chart
                    {
                        ChartAreas = { new ChartArea() },
                        Dock = DockStyle.Fill
                    };
            foreach (var s in series)
                chart.Series.Add(s);
            return chart;
        }


        static Chart pathChart;
        static Chart controlChart;
        static Chart valuesChart;

        static void AddAlgorithm(Func<double, double, double> algorithm, Color color)
        {
            var path = RunCar(algorithm, color);
            pathChart.Series.Add(path[0]);
            controlChart.Series.Add(path[1]);
            valuesChart.Series.Add(path[2]);
        }

        static void Compare(int x, int y)
        {
            var fuzzy = FuzzyAlgorithm(x, y);
            var probably = MostProbableAlgorithm(x, y);
            FuzzyNumber.ShowChart(fuzzy.ToPlot(Color.Red), probably.ToPlot(Color.Blue));
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {



            pathChart = new Chart { ChartAreas = { new ChartArea() }, Dock = DockStyle.Fill };
            controlChart = new Chart { ChartAreas = { new ChartArea() }, Dock = DockStyle.Fill };
            valuesChart = new Chart { ChartAreas = { new ChartArea() }, Dock = DockStyle.Fill };

            domain.NearFunction = Domain.NearQuadratic(2);

            FuzzyLogic(1, 1);


            AddAlgorithm(ExactAlgorithm, Color.Red);
            AddAlgorithm((x,y)=>FuzzyAlgorithm(x,y).Average(), Color.Green);
            AddAlgorithm((x,y)=>MostProbableAlgorithm(x,y).Average(), Color.Blue);



            var tableControl = new TableLayoutPanel { RowCount = 2, ColumnCount = 2, Dock= DockStyle.Fill };
            tableControl.Controls.Add(pathChart,0,0);
            tableControl.Controls.Add(controlChart,0,1);
            tableControl.Controls.Add(valuesChart, 1, 0);

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
    }
}
