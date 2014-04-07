using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Neuro;
using AForge.Neuro.Learning;
using Common;

namespace SymbolRecognition
{
    static class Learning
    {

        static LearningArguments arguments;
        static BaseMaker baseMaker;
        static double[,] percentage;
        static double totalErrors;
     
        static Random rnd = new Random();

        static void ForEachWeight(ActivationNetwork network, Func<double, double> modifier)
        {
            foreach (var l in network.Layers)
                foreach (var n in l.Neurons)
                    for (int i = 0; i < n.Weights.Length; i++)
                        n.Weights[i] = modifier(n.Weights[i]);

        }


        static void Learn()
        {


            var network = new ActivationNetwork(
                new SigmoidFunction(),
                baseMaker.InputSize,
                arguments.NeuronsCount,
                baseMaker.OutputSize
                );
            network.Randomize();
            foreach (var l in network.Layers)
                foreach (var n in l.Neurons)
                    for (int i = 0; i < n.Weights.Length; i++)
                        n.Weights[i] = rnd.NextDouble() * 2 - 1;

            var teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;
            teacher.Momentum = 0;

            while (true)
            {
                var watch = new Stopwatch();
                watch.Start();
                while (watch.ElapsedMilliseconds < 500)
                {
                    teacher.RunEpoch(baseMaker.Inputs, baseMaker.Answers);
                                        
                }
                watch.Stop();
                var count = 0;
                percentage = new double[baseMaker.OutputSize, baseMaker.OutputSize];
                for (int i = 0; i < baseMaker.OutputSize; i++)
                    for (int j = 0; j < baseMaker.OutputSize * 5; j++)
                    {
                        var task = baseMaker.GenerateRandom(i);
                        var output = network.Compute(task);
                        var max = output.Max();
                        var maxIndex = Enumerable.Range(0, output.Length).Where(z => output[z] == max).First();
                        percentage[i, maxIndex]++;
                        if (i != maxIndex) totalErrors++;
                        count++;
                    }
                var maxPercentage = percentage.Cast<double>().Max();
                for (int i = 0; i < baseMaker.OutputSize; i++)
                    for (int j = 0; j < baseMaker.OutputSize; j++)
                        percentage[i, j] /= maxPercentage;
                totalErrors /= count;
                form.BeginInvoke(new Action(Update));


            }

        }
        static Form form;
        static UserControl table;
        static HistoryChart success;



        static void Update()
        {
            var g = table.CreateGraphics();
            g.Clear(Color.White);
            var W = table.ClientSize.Width / baseMaker.OutputSize;
            var H = table.ClientSize.Height / baseMaker.OutputSize;
            for (int i = 0; i < baseMaker.OutputSize; i++)
                for (int j = 0; j < baseMaker.OutputSize; j++)
                {
                    var p = (int)(percentage[i, j] * 255);
                    var color = Color.FromArgb(255, 255 - p, 255-p);
                    if (i==j)
                        color = Color.FromArgb(255-p, 255, 255 - p);
                    g.FillRectangle(
                        new SolidBrush(color),
                        W * i,
                        H * j,
                        W,
                        H);
                    if (i==j)
                    {
                        g.DrawString(
                            baseMaker.Symbols[i].ToString(),
                            baseMaker.Font,
                            percentage[i,i]>0.5?Brushes.White:Brushes.Black,
                            new Rectangle(W*i,H*j,W,H),
                            new StringFormat { LineAlignment= StringAlignment.Center, Alignment= StringAlignment.Center });
                    }

                }
            success.AddRange(new[] { totalErrors });
        }



        public static void Run(LearningArguments _arguments)
        {
            arguments = _arguments;
            baseMaker = _arguments.BaseMaker;
            baseMaker.Generate();

            table = new UserControl
            {
                Dock= DockStyle.Left,
                Size=new Size(400,400)
            };

            success = new HistoryChart
            {
                Lines = {
                    new HistoryChartValueLine
                    {

                    DataFunction = { Color = Color.Blue },
                    }
                },
                Dock = DockStyle.Right,
                Size = new Size(400, 400)
            };

            form = new Form()
            {
                Text = "Symbols recognition",
                ClientSize=new Size(800,400),
                Controls=
                {
                    table,
                    success
                }
            };
            new Action(Learn).BeginInvoke(null, null);
            Application.Run(form);
            
        }
    }
}
