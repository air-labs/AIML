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
        List<double> data = new List<double>();
        int beginning;
        ChartArea area;
        public Series DataFunction { get; private set; }
        Series supportingSerie;
        public int HistoryLength { get; set; }
        public HistoryChart()
        {
            area = new ChartArea()
            {

            };
            ChartAreas.Add(area);
            DataFunction = new Series
            {
                ChartType = SeriesChartType.FastLine
            };
            Series.Add(DataFunction);
            supportingSerie = new Series { Color = Color.Transparent };
            Series.Add(supportingSerie);
        }

        public void AddRange(IEnumerable<double> range)
        {
            data.AddRange(range);
            int adjustment = data.Count - HistoryLength;
            if (adjustment > 0)
            {
                data.RemoveRange(0, adjustment);
                beginning += adjustment;
                area.AxisX.Minimum = beginning;
                area.AxisX.Maximum = beginning + HistoryLength;
            }

            DataFunction.Points.Clear();
            for (int i = 0; i < data.Count; i++)
                DataFunction.Points.Add(new DataPoint(i + beginning, data[i]));
            supportingSerie.Points.Clear();
            supportingSerie.Points.Add(new DataPoint(HistoryLength+beginning, data[0]));
            

        }

    }
}
