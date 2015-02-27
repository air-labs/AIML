using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FuzzyLibrary;

namespace FuzzyView
{
    static class FuzzyView
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var domain = new Domain(0, 20);
            domain.NearFunction = Domain.NearQuadratic(2);
            var chart = new Chart()
            {
                Dock = DockStyle.Fill,
                ChartAreas = { new ChartArea() },
                Series = 
                {
                    ((domain.Near(2)+domain.Near(3))/domain.Near(2)).ToPlot(Color.Red),
                    FuzzyNumber.BinaryOperation(domain.Near(2),domain.Near(3),(a,b)=>(a+b)/b).ToPlot(Color.Green)
                }
            };


            var form = new Form();
            form.Controls.Add(chart);
            Application.Run(form);
        }
    }
}
