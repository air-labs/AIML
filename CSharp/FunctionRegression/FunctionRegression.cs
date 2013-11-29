using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using AForge.Neuro;
using AForge.Neuro.Learning;
using Common;

namespace FunctionRegression
{
    static class FunctionRegression
    {
        static Range LearningRange = new Range(0, 1, 0.01);
        static Func<double, double> Function = z => z * Math.Sin(10*z)*0.5+0.5;
        static int[] Sizes = new int[] { 1, 40, 40, 40, 1 };


        static double[][] Inputs;
        static double[] Outputs;
        static double[][] Answers;
        static ConcurrentQueue<double> Errors = new ConcurrentQueue<double>();


        static BackPropagationLearning teacher;
        static ActivationNetwork network;
        static Random rnd = new Random(1);

        static void Learning()
        {


            network = new ActivationNetwork(
                new SigmoidFunction(),
                Sizes[0],
                Sizes.Skip(1).ToArray()
                );
            network.ForEachWeight(z => rnd.NextDouble() * 2 - 1);

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 2;
            teacher.Momentum = 0.1;

            while(true)
            {
                var watch = new Stopwatch();
                watch.Start();
                while(watch.ElapsedMilliseconds<200)
                {
                    Errors.Enqueue(teacher.RunEpoch(Inputs, Answers));
                }
                watch.Stop();

                Outputs = Inputs
                            .Select(z => network.Compute(z)[0])
                            .ToArray();
                form.BeginInvoke(new Action(UpdateCharts));
            }
        }

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
            while (Errors.TryDequeue(out error));
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
                        ChartAreas = { new ChartArea() },
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
