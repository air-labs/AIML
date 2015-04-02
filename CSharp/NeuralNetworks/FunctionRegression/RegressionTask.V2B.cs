using AForge.Neuro;
using AForge.Neuro.Learning;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionRegression
{
    public class RegressionTaskV2B : RegressionTaskV1
    {
        public RegressionTaskV2B()
        {
            LearningRange = new Range(0, 1, 0.025);
            Function = z => z * Math.Sin(z*10);

            MaxError = 0.5;
            IterationsCount = 30720;
        }

        protected override void LearningIteration()
        {
            int RepetitionCount = 5;
            for (int i = 0; i < Inputs.Length / RepetitionCount; i++)
            {
                var sample = rnd.Next(Inputs.Length);
                for (int j = 0; j < RepetitionCount; j++)
                    teacher.Run(Inputs[sample], Answers[sample]);
            }
        }

        protected override void CreateNetwork()
        {
            network = new ActivationNetwork(new Tanh(1), 1, 5, 5, 1);
            network.ForEachWeight(z => rnd.NextDouble() * 2-1);

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;
        }
    }
}
