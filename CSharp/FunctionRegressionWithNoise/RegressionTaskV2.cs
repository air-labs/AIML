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
    class RegressionTaskV2 : RegressionTaskV1
    {
        double NoiseLevel=0.3;

        protected double Randomizator(double x)
        {
            var value = x + (rnd.NextDouble() * NoiseLevel - NoiseLevel / 2);
            if (value < -1) return -1;
            if (value > 1) return 1;
            return value;
        }


        protected override void PrepareData()
        {
            base.PrepareData();

            LearningAnswers = LearningInputs
                .Select(z => z[0])
                .Select(Function)
                .Select(Randomizator)
                .Select(z => new double[] { z })
                .ToArray();
        }

        
    }
}
