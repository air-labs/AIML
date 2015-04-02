using AForge.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionRegression
{
    public class Tanh : IActivationFunction
    {
        public double Beta;

        public Tanh(double Beta = 1) { this.Beta = Beta; }

        public double Function(double x)
        {
            return Math.Tanh(x * Beta);
        }

        public double Derivative(double x)
        {
            var f=Function(x);
            return Beta * (1 - f * f);
        }

        public double Derivative2(double y)
        {
            return Beta * (1 - y * y);
        }
    }
}
