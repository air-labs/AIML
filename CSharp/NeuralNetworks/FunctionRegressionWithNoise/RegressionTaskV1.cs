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
    class RegressionTaskV1 : RegressionTaskV0
    {
       
        public Range FunctionRange=new Range(-1,1,0.01);
        public double[][] FunctionInputs;
        public double[] FunctionAnswers;
        public double[] FunctionOutput;


        protected override void PrepareData()
        {
            base.PrepareData();

            FunctionInputs = FunctionRange.GenerateSamples();
            FunctionAnswers = FunctionInputs
                .Select(z => z[0])
                .Select(Function)
                .ToArray();
        }

        protected override void PrepareCharts()
        {
            base.PrepareCharts();
            var originalFunction = new Series() { ChartType = SeriesChartType.FastLine, Color = Color.Orange };
            for (int i = 0; i < FunctionInputs.Length; i++)
            {
                originalFunction.Points.Add(new DataPoint(FunctionInputs[i][0], FunctionAnswers[i]));
            }
            AreaChart.Series.Add(originalFunction);
            computedFunction.ChartType = SeriesChartType.FastLine;
            computedFunction.MarkerSize = 1;
            computedFunction.BorderWidth = 2;
        }

        protected override void LearningEnds()
        {
            FunctionOutput = FunctionInputs.Select(z => network.Compute(z)[0]).ToArray();
        }

        protected override void UpdateCharts()
        {
            computedFunction.Points.Clear();
            for (int i = 0; i < FunctionInputs.Length; i++)
            {
                computedFunction.Points.Add(new DataPoint(FunctionInputs[i][0], FunctionOutput[i]));
            }

            HistoryChart.AddRange(LearningErrors);
            double error;
            while (LearningErrors.TryDequeue(out error)) ;
        }

        
    }
}
