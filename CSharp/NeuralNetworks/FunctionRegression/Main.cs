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
        static RegressionTaskV0 task = new RegressionTaskV1();

        static Form form;
        static Series computedFunction;
        static HistoryChart history;


        static void UpdateCharts()
        {
            computedFunction.Points.Clear();
            for (int i = 0; i < task.Inputs.Length; i++)
                computedFunction.Points.Add(new DataPoint(task.Inputs[i][0], task.Outputs[i]));

            history.AddRange(task.Errors);
            double error;
            while (task.Errors.TryDequeue(out error)) ;
        }

        [STAThread]
        static void Main()
        {
            task.Prepare();            

            var targetFunction = new Series()
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2
            };
            for (int i = 0; i < task.Inputs.Length; i++)
                targetFunction.Points.Add(new DataPoint(task.Inputs[i][0], task.Answers[i][0]));

            computedFunction = new Series()
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2
            };

            history = new HistoryChart
                    {
                        Max = task.MaxError,
                        DotsCount=200,
                        Lines =
                        {
                            new HistoryChartValueLine
                            {
                                DataFunction = { Color = Color.Blue }
                            }
                        },
                        Dock = DockStyle.Bottom
                    };

            form = new Form()
            {
                Text = task.GetType().Name,
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
                                    Minimum = task.Inputs.Min(z=>z[0]),
                                    Maximum = task.Inputs.Max(z=>z[0])
                                },
                                AxisY=
                                {
                                    Minimum = Math.Min(-1.5,task.Answers.Min(z=>z[0])),
                                    Maximum = Math.Max(1.5, task.Answers.Max(z=>z[0]))
                                }
                            }
                        },
                        Series = { targetFunction, computedFunction },
                        Dock= DockStyle.Top
                    },
                    history
                }
            };

            task.UpdateCharts += (s, a) => form.BeginInvoke(new Action(UpdateCharts));
            new Action(task.Learn).BeginInvoke(null, null);
            Application.Run(form);
        }
    }
}
