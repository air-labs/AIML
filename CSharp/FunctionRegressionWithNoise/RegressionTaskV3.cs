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
    class RegressionTaskV3 : RegressionTaskV2
    {
        public RegressionTaskV3()
        {
            Sizes = new int[] { 1, 2,2,1 };
            IterationsCount = 1000000000;
        }

    }
}
