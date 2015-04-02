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
    public class RegressionTaskV8 : RegressionTaskV5
    {

        public RegressionTaskV8()
        {
            IterationsCount = 51200;
        }
        protected override void CreateNetwork()
        {
            network = new ActivationNetwork(new Tanh(0.1), 1, 7,7,6, 1);
            network.ForEachWeight(z => rnd.NextDouble() * 2 - 1);

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;

            teacher.Momentum = 0.3;
        }

    }
}
