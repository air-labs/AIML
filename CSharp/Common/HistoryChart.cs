using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Common
{
 


    public class HistoryChart : Chart
    {
        public int DotsCount = 200;
        public List<double> values = new List<double>();
        public Queue<double> temp = new Queue<double>();
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
        public Series DataFunction;
        int averageCount = 1;

        public HistoryChart()
        {
            area = new ChartArea()
            {
                AxisY = { Minimum = 0 },
                AxisX = { Minimum = 0 }
            };
            ChartAreas.Add(area);
            DataFunction = new Series
            {
                ChartType = SeriesChartType.FastLine
            };
            Series.Add(DataFunction);
        }

        public void AddRange(IEnumerable<double> range)
        {
            foreach (var e in range) temp.Enqueue(e);

            while (temp.Count > averageCount)
            {
                double sum = 0;
                for (int i = 0; i < averageCount; i++) sum += temp.Dequeue();
                sum /= averageCount;
                values.Add(sum);
                if (values.Count >= DotsCount)
                {
                    for (int j = 0; j < values.Count - 1; j++)
                    {
                        values[j] = (values[j] + values[j + 1]) / 2;
                        values.RemoveAt(j + 1);
                    }

                    area.AxisX.Maximum = 2*DotsCount * averageCount;
                    averageCount *= 2;
                }
            }

            DataFunction.Points.Clear();
            for (int i = 0; i < values.Count; i++)
                DataFunction.Points.Add(new DataPoint(averageCount*i, Math.Min(values[i],max*0.9)));
        }
    }
}