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
        static double TargetDeviation=1;

        static double Velocity = 0.05;
        static int StepCount = (int)(TargetX / Velocity);
        static int IntermediateStepCount = 50;

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
                };
            for (int i=0;i<StepCount;i++)
            {
                var tX = TargetX + ((rnd.NextDouble() - 0.5) * 2 * TargetDeviation)-x;
                var ty = ((rnd.NextDouble() - 0.5) * 2 * TargetDeviation)-y;
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
            }
            return result;
        }

        static double ExactAlgorithm(double x, double y)
        {
            return Math.Atan2(y, x);
        }

        static Domain domain=new Domain(-7,7,0.1);

        static double FuzzyAlgorithm(double x, double y)
        {
            return FuzzyNumber.BinaryOperation(domain.Near(x), domain.Near(y), ExactAlgorithm).Average();
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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            domain.NearFunction = Domain.NearQuadratic(2);
            var exactPath = RunCar(ExactAlgorithm,Color.Red);
            var fuzzyPath = RunCar(FuzzyAlgorithm, Color.Blue);

            var tableControl = new TableLayoutPanel { RowCount = 2, ColumnCount = 0, Dock= DockStyle.Fill };
            tableControl.Controls.Add(GetChart(exactPath[0], fuzzyPath[0]),0,0);
            tableControl.Controls.Add(GetChart(exactPath[1], fuzzyPath[1]),0,1);

            tableControl.RowStyles.Add(new RowStyle { SizeType = System.Windows.Forms.SizeType.Percent, Height = 50 });
            tableControl.RowStyles.Add(new RowStyle { SizeType = System.Windows.Forms.SizeType.Percent, Height = 50 });


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
