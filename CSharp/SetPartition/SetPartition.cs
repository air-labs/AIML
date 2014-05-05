using AIRLab.GeneticAlgorithms;
using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetPartition
{
    static class SetPartition
    {
        static int Count = 5000;
        static int MaxWeight = 5000;
        static int[] Weights;
        static Random rnd = new Random(1);
        static Form form;
        static ConcurrentQueue<double> averageValuations = new ConcurrentQueue<double>();
        static ConcurrentQueue<double> maxValuations = new ConcurrentQueue<double>();
        static ConcurrentQueue<double> ages = new ConcurrentQueue<double>();

        static HistoryChart valuationsChart = new HistoryChart();
        static HistoryChart agesChart = new HistoryChart();


        static void Algorithm()
        {
            var ga = new GeneticAlgorithm<ArrayChromosome<bool>>(
                ()=>new ArrayChromosome<bool>(Count)
                ,rnd);
            
            Solutions.AppearenceCount.MinimalPoolSize(ga, 40);
            Solutions.MutationOrigins.Random(ga, 0.5);
            Solutions.CrossFamilies.Random(ga, z => z * 0.5);
            Solutions.Selections.Threashold(ga, 40);

            

            ArrayGeneSolutions.Appearences.Bool(ga);
            ArrayGeneSolutions.Mutators.Bool(ga);
            ArrayGeneSolutions.Crossover.Mix(ga);

            

            ga.Evaluate = chromosome =>
                {
                    chromosome.Value = 
                        1.0 / (1+Math.Abs(Enumerable.Range(0, Count).Sum(z => Weights[z]*(chromosome.Code[z]?-1:1))));
                };

            while (true)
            {
                var watch = new Stopwatch();
                watch.Start();
                while (watch.ElapsedMilliseconds < 200)
                {
                    ga.MakeIteration();
                    averageValuations.Enqueue(ga.Pool.Average(z=>z.Value));
                    maxValuations.Enqueue(ga.Pool.Max(z=>z.Value));
                    ages.Enqueue(ga.Pool.Average(z=>z.Age));

                }
                watch.Stop();
                if (!form.IsDisposed)
                    form.BeginInvoke(new Action(UpdateCharts));
            }
        }

        static void UpdateCharts()
        {
            valuationsChart.AddRange(maxValuations, averageValuations);
          
            maxValuations.Clear();
            averageValuations.Clear();
            agesChart.AddRange(ages);
            ages.Clear();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Weights=Enumerable.Range(0,Count).Select(z=>rnd.Next(MaxWeight)).ToArray();
            if (Weights.Sum() % 2 != 0) Weights[0]++;

            form = new Form();
            var table = new TableLayoutPanel() { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };


 
            valuationsChart = new HistoryChart
            {
                Lines = 
                {
                    new HistoryChartValueLine { DataFunction = { Color = Color.Green, BorderWidth=2 }},
                    new HistoryChartValueLine { DataFunction = { Color = Color.Orange, BorderWidth=2 }},
                },
                Max = 1,
                Dock = DockStyle.Fill
            };
            agesChart = new HistoryChart
            {
                Lines = { new HistoryChartValueLine { DataFunction = { Color = Color.Blue, BorderWidth=2 } } },
                Dock = DockStyle.Fill,
                Max=100
            };

            table.Controls.Add(valuationsChart,0,0);
            table.Controls.Add(agesChart,0,1);

            for (int i = 0; i < 2; i++)
            {
                table.RowStyles.Add(new RowStyle { SizeType = SizeType.Percent, Height = 50 });
            }
            
            form.Controls.Add(table);
            form.WindowState = FormWindowState.Maximized;

            new Thread(Algorithm) { IsBackground = true }.Start();
            Application.Run(form);

        }
    }
}
