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
    class RegressionTaskV5 : RegressionTaskV3
    {


        protected override void LearningIteration()
        {
            network.ForEachWeight(z => 0.9995 * z);
            teacher.RunEpoch(LearningInputs, LearningAnswers);
           
        }


    }
}
