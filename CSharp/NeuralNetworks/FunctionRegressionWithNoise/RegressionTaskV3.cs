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

        double[][] ControlInputs;
        double[][] ControlAnswers;
        ConcurrentQueue<double> ControlError = new ConcurrentQueue<double>();



        protected override void PrepareData()
        {
            base.PrepareData();

            var newLearningInputs = new List<double[]>();
            var newLearningAnswers = new List<double[]>();
            var controlInputs = new List<double[]>();
            var controlAnsers = new List<double[]>();

            for (int i = 0; i < LearningInputs.Length; i++)
            {
                if (i % 2 == 0)
                {
                    newLearningInputs.Add(LearningInputs[i]);
                    newLearningAnswers.Add(LearningAnswers[i]);
                }
                else
                {
                    controlInputs.Add(LearningInputs[i]);
                    controlAnsers.Add(LearningAnswers[i]);
                }

            }

            LearningInputs = newLearningInputs.ToArray();
            LearningAnswers = newLearningAnswers.ToArray();
            ControlInputs = controlInputs.ToArray();
            ControlAnswers = controlAnsers.ToArray();
        }

        protected override void PrepareCharts()
        {
            base.PrepareCharts();
            var controlPoints = new Series() { ChartType = SeriesChartType.Point, Color = Color.Blue, MarkerSize = 10 };
            for (int i = 0; i < ControlInputs.Length; i++)
            {
                controlPoints.Points.Add(new DataPoint(ControlInputs[i][0], ControlAnswers[i][0]));
            }
            AreaChart.Series.Add(controlPoints);
            HistoryChart.Lines[0].DataFunction.Color = Color.Red;
            HistoryChart.Lines.Add(new HistoryChartValueLine { DataFunction = { Color = Color.Blue} });
        }


        protected override void AccountError()
        {
            LearningErrors.Enqueue(GetError(LearningInputs, LearningAnswers));
            ControlError.Enqueue(GetError(ControlInputs, ControlAnswers));
        }

        protected override void UpdateCharts()
        {
            computedFunction.Points.Clear();
            for (int i = 0; i < FunctionInputs.Length; i++)
            {
                computedFunction.Points.Add(new DataPoint(FunctionInputs[i][0], FunctionOutput[i]));
            }

            HistoryChart.AddRange(LearningErrors,ControlError);
            double error;
            while (LearningErrors.TryDequeue(out error)) ;
            while (ControlError.TryDequeue(out error)) ;
        }
        
    }
}
