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
    public class RegressionTaskV5A : RegressionTaskV5
    {

        protected override void CreateNetwork()
        {
            network = new ActivationNetwork(new Tanh(0.1), 1, 20, 1);
            network.ForEachWeight(z => rnd.NextDouble() * 2-1);

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;

            teacher.Momentum = 0.3;
        }

        public RegressionTaskV5A()
        {
            LearningRange = new Range(0, 1, 0.025);
            Function = z => z * Math.Sin(25*z);
            MaxError = 0.5;
            IterationsCount = 102400 + 51200;
        }
    }
}
