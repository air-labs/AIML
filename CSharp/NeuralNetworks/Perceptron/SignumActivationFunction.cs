using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Neuro;

namespace Perceptron
{
    public class SignumActivationFunction : IActivationFunction
    {
        public double Function(double x)
        {
            return Math.Sign(x);
        }

        public double Derivative(double x)
        {
            return 0;
        }

        public double Derivative2(double y)
        {
            return 0;
        }
    }
}
