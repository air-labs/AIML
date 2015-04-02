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

namespace FunctionRegressionWithNoise
{
    class RegressionTaskV0
    {
        public Func<double, double> Function = z => 1.8*z*z-0.9;

        public int IterationsCount = 30000;
        public double MaxError = 0.5;

        protected int[] Sizes = new int[] { 1, 5, 5,  1 };

        protected Range LearningRange = new Range(-1, 1, 0.1);
        public double[][] LearningInputs;
        public double[][] LearningAnswers;
        public double[] LearningOutputs;
        public ConcurrentQueue<double> LearningErrors = new ConcurrentQueue<double>();

        public BackPropagationLearning teacher;
        public ActivationNetwork network;
        public Random rnd = new Random(2);

        public Form Form;
        protected Chart AreaChart;
        protected HistoryChart HistoryChart;
        protected Series computedFunction;

        public virtual void Prepare()
        {
            PrepareData();
            PrepareCharts();

            network = new ActivationNetwork(new Tanh(0.2), 
                Sizes[0],
                Sizes.Skip(1).ToArray());

            network.ForEachWeight(z => rnd.NextDouble() * 2 - 1);

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;

         

            Form = new Form()
            {
                Text = GetType().Name,
                Size = new Size(800, 600),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Controls =
                {
                    AreaChart,
                    HistoryChart
                }
            };
        }

        protected  virtual void PrepareData()
        {
            LearningInputs = LearningRange.GenerateSamples();

            LearningAnswers = LearningInputs
                        .Select(z => z[0])
                        .Select(Function)
                        .Select(z => new[] { z })
                        .ToArray();

        }

        protected virtual void PrepareCharts()
        {
            var learningPoints = new Series()
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
                MarkerSize = 10
            };
            for (int i = 0; i < LearningInputs.Length; i++)
                learningPoints.Points.Add(new DataPoint(LearningInputs[i][0], LearningAnswers[i][0]));

            computedFunction = new Series() { MarkerSize = 10, Color = Color.Green, ChartType = SeriesChartType.Point };

            AreaChart = new Chart
            {
                ChartAreas = { new ChartArea()
                        {
                            AxisY = {Maximum=1, Minimum=-1 }
                        }},
                Series = { learningPoints, computedFunction },
                Dock = DockStyle.Top
            };

            HistoryChart = new HistoryChart
            {
                Lines = 
                {
                    new HistoryChartValueLine { DataFunction = { Color = Color.Blue }},
                },
                Dock = DockStyle.Bottom,
                Max=MaxError
                
            };
        }

        


        public void Learn()
        {

            int counter = 0;
            while (true)
            {
                var watch = new Stopwatch();
                watch.Start();
                while (watch.ElapsedMilliseconds < 200)
                {
                    LearningIteration();
                    AccountError();
                    counter++;
                    if (counter > IterationsCount) break;
                    
                }
                watch.Stop();
                LearningEnds();
                Form.BeginInvoke(new Action(UpdateCharts));
                if (counter > IterationsCount) break;

            }
        }

        protected virtual void LearningIteration()
        {
            teacher.RunEpoch(LearningInputs, LearningAnswers);
        }

        protected virtual void AccountError()
        {
            LearningErrors.Enqueue(GetError(LearningInputs, LearningAnswers));
        }


        protected virtual void LearningEnds()
        {
            LearningOutputs = LearningInputs.Select(z => network.Compute(z)[0]).ToArray();
        }

        protected virtual void UpdateCharts()
        {

            computedFunction.Points.Clear();
            for (int i=0;i<LearningInputs.Length;i++)
            {
                computedFunction.Points.Add(new DataPoint(LearningInputs[i][0], LearningOutputs[i]));
            }

            HistoryChart.AddRange(LearningErrors);
            double error;
            while (LearningErrors.TryDequeue(out error)) ;
        }

    

        protected double GetError(double[][] Inputs, double[][] Answers)
        {
            double sum = 0;
            for (int i = 0; i < Inputs.Length; i++)
            {
                sum += Math.Abs(network.Compute(Inputs[i])[0] - Answers[i][0]);
            }
            sum /= Inputs.Length;
            return sum;
        }




        

        
    }
}
