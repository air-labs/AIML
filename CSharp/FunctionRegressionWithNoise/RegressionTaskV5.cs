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
    class RegressionTaskV5 : RegressionTaskV4
    {
        public RegressionTaskV5()
        {
            Sizes = new int[] { 1, 10, 10, 1 };
            IterationsCount = 100000;
        }

        
    }
}
