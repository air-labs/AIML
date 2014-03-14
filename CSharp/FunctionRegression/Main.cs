using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FunctionRegression
{
    static partial class FunctionRegression
    {

        static Form form;
        static Series computedFunction;
        static HistoryChart history;


        static void UpdateCharts()
        {
            computedFunction.Points.Clear();
            for (int i = 0; i < Inputs.Length; i++)
                computedFunction.Points.Add(new DataPoint(Inputs[i][0], Outputs[i]));

            history.AddRange(Errors);
            double error;
            while (Errors.TryDequeue(out error)) ;
        }

        [STAThread]
        static void Main()
        {
            Inputs = LearningRange.GenerateSamples();
            Answers = Inputs
                        .Select(z => z[0])
                        .Select(Function)
                        .Select(z => new[] { z })
                        .ToArray();

            var targetFunction = new Series()
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2
            };
            for (int i = 0; i < Inputs.Length; i++)
                targetFunction.Points.Add(new DataPoint(Inputs[i][0], Answers[i][0]));

            computedFunction = new Series()
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2
            };

            history = new HistoryChart
                    {
                        HistoryLength = 1000,
                        DataFunction =
                        {
                            Color = Color.Blue
                        },
                        Dock = DockStyle.Bottom
                    };

            form = new Form()
            {
                Text = "Function regression",
                Size = new Size(800, 600),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Controls =
                {
                    new Chart
                    {
                        ChartAreas = 
                        {
                            new ChartArea
                            {
                                AxisX=
                                {
                                    Minimum = LearningRange.Minimum,
                                    Maximum = LearningRange.Maximum
                                },
                                AxisY=
                                {
                                    Minimum = Math.Min(-1.5,Answers.Min(z=>z[0])),
                                    Maximum = Math.Max(1.5, Answers.Max(z=>z[0]))
                                }
                            }
                        },
                        Series = { targetFunction, computedFunction },
                        Dock= DockStyle.Top
                    },
                    history
                }
            };

            new Action(Learning).BeginInvoke(null, null);

            Application.Run(form);
        }
    }
}
