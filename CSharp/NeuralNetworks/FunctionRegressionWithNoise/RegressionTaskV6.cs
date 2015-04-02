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
    class RegressionTaskV6 : RegressionTaskV5
    {

        public RegressionTaskV6()
        {
            LearningRange = new Range(-1, 1, 0.01);
        }

        protected override void PrepareCharts()
        {
            base.PrepareCharts();
            computedFunction.BorderWidth = 5;
        }


    }
}
