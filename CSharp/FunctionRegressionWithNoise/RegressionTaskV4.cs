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
    class RegressionTaskV4 : RegressionTaskV3
    {
        public RegressionTaskV4()
        {
            Sizes = new int[] { 1, 3, 1 };
            IterationsCount = 100000;
        }


    }
}
