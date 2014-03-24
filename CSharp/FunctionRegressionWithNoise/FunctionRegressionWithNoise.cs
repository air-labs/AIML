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
        static Range LearningRange = new Range(0, 1, 0.1);
        static Range ControlRange = new Range(0, 1, 0.01);
        static Func<double, double> Function = z =>  -(z-0.5)*(z-0.5)+0.25;
        static int[] Sizes = new int[] { 1, 30, 30, 1 };
        static double Noise=0.4;

        static double[][] LearningInputs;
        static double[][] ControlInputs;
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
                    Errors.Enqueue(teacher.RunEpoch(LearningInputs, Answers));
                }
                watch.Stop();

                Outputs = ControlInputs
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
            for (int i = 0; i < ControlInputs.Length; i++)
               computedFunction.Points.Add(new DataPoint(ControlInputs[i][0], Outputs[i]));
       
            history.AddRange(Errors);
            double error;
            while (Errors.TryDequeue(out error));
        }


        [STAThread]
        static void Main()
        {
            LearningInputs = LearningRange.GenerateSamples();
            Answers = LearningInputs
                        .Select(z => z[0])
                        .Select(z=>Function(z)*(1+rnd.NextDouble()*Noise-Noise/2))
                        .Select(z => new[] { z })
                        .ToArray();

            var learningPoints = new Series()
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
                MarkerSize=5
            };
            for (int i = 0; i < LearningInputs.Length; i++)
                learningPoints.Points.Add(new DataPoint(LearningInputs[i][0], Answers[i][0]));

            ControlInputs = ControlRange.GenerateSamples();


            var targetFunction = new Series()
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Orange,
                BorderWidth = 2
            };
            for (int i = 0; i < ControlInputs.Length; i++)
                targetFunction.Points.Add(new DataPoint(ControlInputs[i][0], Function(ControlInputs[i][0])));



            computedFunction = new Series()
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2
            };

            history = new HistoryChart
                    {
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
                        Series = { learningPoints, targetFunction, computedFunction },
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
