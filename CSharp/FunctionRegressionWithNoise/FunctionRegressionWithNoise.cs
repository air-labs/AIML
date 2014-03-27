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
        static Range LearningRange = new Range(-1, 1, 0.25);
        static Range FunctionRange = new Range(-1, 1, 0.01);


        static Func<double, double> Function = z => 0.8 * (2 * z * z - 1);
        static int[] Sizes = new int[] { 1, 5, 5, 1 };
        static double Noise=0.5;

        static double[][] LearningInputs;
        static double[][] LearningAnswers;
        static double[][] FunctionInputs;
        static double[] FunctionOutputs;

  
        static ConcurrentQueue<double> LearningErrors = new ConcurrentQueue<double>();
        
        static BackPropagationLearning teacher;
        static ActivationNetwork network;
        static Random rnd = new Random(1);

        #region Контрольная выборка
        static Range ControlRange = new Range(0.5, 9.5, 0.1);
        static double[][] ControlInputs;
        static double[][] ControlAnswers;
        static ConcurrentQueue<double> ControlErrors = new ConcurrentQueue<double>();
        #endregion

        static double GetError(double[][] inputs, double[][] answers)
        {
            double sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                var output = network.Compute(inputs[i]);
                sum += Math.Abs(answers[i][0] - output[0]);
            }
            return sum / inputs.Length;
        }

        static void Learning()
        {
            network = new ActivationNetwork(
                new Tanh(0.1),
                Sizes[0],
                Sizes.Skip(1).ToArray()
                );

            foreach (var layer in network.Layers)
                layer.ForEachWeight(z => (rnd.NextDouble() * 2 - 1) / layer.InputsCount);

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;
            teacher.Momentum = 0.1;

            while(true)
            {
                var watch = new Stopwatch();
                watch.Start();
                while(watch.ElapsedMilliseconds<200)
                {
                    //LearningErrors.Enqueue(GetError(LearningInputs,LearningAnswers));
                    //ControlErrors.Enqueue(GetError(ControlInputs, ControlAnswers));

                    var sample = rnd.Next(LearningInputs.Length);
                    for (int i = 0; i < 3; i++)
                        teacher.Run(LearningInputs[sample], LearningAnswers[sample]);
                    

                }
                
                watch.Stop();

                FunctionOutputs = FunctionInputs
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
            for (int i = 0; i < FunctionInputs.Length; i++)
               computedFunction.Points.Add(new DataPoint(FunctionInputs[i][0], FunctionOutputs[i]));
       
            history.AddRange(LearningErrors);

            double error;
            while (LearningErrors.TryDequeue(out error));
        }


        [STAThread]
        static void Main()
        {
            LearningInputs = LearningRange.GenerateSamples();
            LearningAnswers = LearningInputs
                        .Select(z => z[0])
                        .Select(z=>Function(z)*(1+rnd.NextDouble()*Noise-Noise/2))
                        .Select(z => new[] { z })
                        .ToArray();

            ControlInputs = ControlRange.GenerateSamples();
            ControlAnswers = ControlInputs
                        .Select(z => z[0])
                        .Select(z => Function(z) * (1 + rnd.NextDouble() * Noise - Noise / 2))
                        .Select(z => new[] { z })
                        .ToArray();

            var learningPoints = new Series()
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
                MarkerSize=5
            };
            for (int i = 0; i < LearningInputs.Length; i++)
                learningPoints.Points.Add(new DataPoint(LearningInputs[i][0], LearningAnswers[i][0]));

            var controlPoints = new Series()
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Blue,
                MarkerSize = 5
            };


            FunctionInputs = FunctionRange.GenerateSamples();


            var targetFunction = new Series()
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Orange,
                BorderWidth = 2
            };
            for (int i = 0; i < FunctionInputs.Length; i++)
                targetFunction.Points.Add(new DataPoint(FunctionInputs[i][0], Function(FunctionInputs[i][0])));



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
                        ChartAreas = { new ChartArea()
                        {
                            AxisY = {Maximum=1, Minimum=-1 }
                        }},
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
