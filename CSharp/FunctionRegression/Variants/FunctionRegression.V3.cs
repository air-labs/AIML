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

namespace FunctionRegression.V3
{
    static partial class FunctionRegression
    {
        static Range LearningRange = new Range(0, 1, 0.025);
        static Func<double, double> Function = z => ((z * 10) * Math.Sin(z * 10)) / 10;
        static int[] Sizes = new int[] { 1, 20, 20, 1 };


        static double[][] Inputs;
        static double[] Outputs;
        static double[][] Answers;
        static ConcurrentQueue<double> Errors = new ConcurrentQueue<double>();


        static BackPropagationLearning teacher;
        static ActivationNetwork network;
        static Random rnd = new Random(2);

        static void Learning()
        {


            network = new ActivationNetwork(
                new Tanh(1),
                Sizes[0],
                Sizes.Skip(1).ToArray()
                );
            network.ForEachWeight(z => rnd.NextDouble() * 2 - 1);



            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;
            teacher.Momentum = 0.1;

            while (true)
            {
                var watch = new Stopwatch();
                watch.Start();
                while (watch.ElapsedMilliseconds < 200)
                {
                   var sample = rnd.Next(Inputs.Length);
                    for (int i = 0; i < 10; i++)
                    {
                        var e = teacher.Run(Inputs[sample], Answers[sample]); ;
                        if (i == 0)
                            Errors.Enqueue(e);
                    }

               
                }
                watch.Stop();

                Outputs = Inputs
                            .Select(z => network.Compute(z)[0])
                           .ToArray();
            //     form.BeginInvoke(new Action(UpdateCharts));
            }
        }




    }
}
