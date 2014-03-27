using AForge.Neuro;
using AForge.Neuro.Learning;
using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionRegression
{
    public class RegressionTaskV0
    {
        protected Range LearningRange = new Range(0, 10, 0.25);
        protected Func<double, double> Function = z => z * Math.Sin(z);

        public double[][] Inputs;
        public double[] Outputs;
        public double[][] Answers;
        public ConcurrentQueue<double> Errors = new ConcurrentQueue<double>();
        public double MaxError = 4;
        public int IterationsCount = 51200;

        protected ActivationNetwork network;
        protected BackPropagationLearning teacher;
        protected Random rnd = new Random(1);

        public void Prepare()
        {
            Inputs = LearningRange.GenerateSamples();

            Answers = Inputs
                        .Select(z => z[0])
                        .Select(Function)
                        .Select(z => new[] { z })
                        .ToArray();

            CreateNetwork();
        }


        protected virtual void CreateNetwork()
        {
            network = new ActivationNetwork(new Tanh(1), 1, 5, 1);
            network.ForEachWeight(z => rnd.NextDouble() * 2 - 1);

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;
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
                    Errors.Enqueue(GetError());
                    counter++;
                    if (counter > IterationsCount) break;
                }
                watch.Stop();

                Outputs = Inputs
                            .Select(z => network.Compute(z)[0])
                            .ToArray();
                if (UpdateCharts != null) UpdateCharts(this, EventArgs.Empty);
                if (counter > IterationsCount) break;
            }
        }

        double GetError()
        {
            double sum=0;
            for (int i=0;i<Inputs.Length;i++)
            {
                sum+=Math.Abs(network.Compute(Inputs[i])[0]-Answers[i][0]);
            }
            sum/=Inputs.Length;
            return sum;
        }

        protected virtual void LearningIteration()
        {
            teacher.RunEpoch(Inputs, Answers);
        }

        public event EventHandler UpdateCharts;

        


    }
}
