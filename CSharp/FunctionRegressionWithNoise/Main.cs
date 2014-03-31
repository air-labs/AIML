using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FunctionRegressionWithNoise
{
    class Program
    {
        static RegressionTaskV0 task = new RegressionTaskV0();

        [STAThread]
        static void Main()
        {
            task.Prepare();
            

            new Action(task.Learn).BeginInvoke(null, null);

            Application.Run(task.Form);
        }
    }
}
