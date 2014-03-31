using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Common
{


    public class HistoryChartValueLine
    {
        public int DotsCount = 200;
        public int AverageCount { get; private set; }
        public int ShrinkCount { get; private set; }
        List<double> values = new List<double>();
        Queue<double> temp = new Queue<double>();

        public HistoryChartValueLine()
        {
            DotsCount = 200;
            AverageCount = 1;
            ShrinkCount = 0;
            DataFunction = new Series()
            {
                ChartType = SeriesChartType.FastLine
            };
        }

        public void Shrink()
        {
            for (int j = 0; j < values.Count - 1; j++)
            {
                values[j] = (values[j] + values[j + 1]) / 2;
                values.RemoveAt(j + 1);
            }

            AverageCount *= 2;
            ShrinkCount++;
        }

        public void Take(IEnumerable<double> data)
        {
            foreach (var e in data) temp.Enqueue(e);
        }

        public int GetTotalShrinksAfterPull()
        {
            var totalPoints = (temp.Count / AverageCount) + values.Count;
            int result = 0;
            while (totalPoints > DotsCount) { totalPoints /= 2; result++; }
            return result + ShrinkCount;
        }

        public void Pull(double maximum)
        {
            while (temp.Count > AverageCount)
            {
                double sum = 0;
                for (int i = 0; i < AverageCount; i++) sum += temp.Dequeue();
                sum /= AverageCount;
                values.Add(sum);
            }

            DataFunction.Points.Clear();
            for (int i = 0; i < values.Count; i++)
                DataFunction.Points.Add(new DataPoint(AverageCount * i, Math.Min(values[i], maximum * 0.9)));

        }

        public Series DataFunction { get; set; }

    }


    public class HistoryChart : Chart
    {
        public int DotsCount = 200;
        public List<HistoryChartValueLine> Lines { get; private set; }
        double max = 1;
        public double Max
        {
            get { return max; }
            set
            {
                area.AxisY.Maximum = value;
                max = value;
            }
        }
        ChartArea area;
        int averageCount = 1;
        bool initialized = false;

        public HistoryChart()
        {
            Lines = new List<HistoryChartValueLine>();
            area = new ChartArea()
            {
                AxisY = { Minimum = 0 },
                AxisX = { Minimum = 0 }
            };
            ChartAreas.Add(area);
        }

        public void AddRange(params IEnumerable<double>[] ranges)
        {
            if (!initialized)
            {
                foreach (var s in Lines)
                {
                    Series.Add(s.DataFunction);
                    s.DotsCount = DotsCount;
                }
                initialized = true;
            }

            if (Lines.Count != ranges.Length) throw new Exception();
            for (int i = 0; i < ranges.Length; i++)
                Lines[i].Take(ranges[i]);
            var maxShrink = Lines.Select(z => z.GetTotalShrinksAfterPull()).Max();
            for (int i = 0; i < Lines.Count; i++)
            {
                while (Lines[i].ShrinkCount < maxShrink) Lines[i].Shrink();
                Lines[i].Pull(max);
            }
            area.AxisX.Maximum = Math.Pow(2,maxShrink)*DotsCount;
        }
    }
}