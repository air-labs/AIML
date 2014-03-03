using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Neuro;
using AForge.Neuro.Learning;
using Common;

namespace Perceptron
{
    static class Program
    {
        static Func<bool, bool, bool> Function = (x, y) => (x ^ y);
        
        static List<int> LearningOrder = new List<int>
        {
             1,  1,
             -1,  1,
             1,  -1,
             -1,  -1,
        };


        static int LearningOrderPointer = 0;

        static ActivationNetwork network;
        static PerceptronLearning learning;

        static Form form;

        [STAThread]
        static void Main()
        {
            weights = new double[3];
            network = new ActivationNetwork(new SignumActivationFunction(), 2, 1);
            weights[0] = ((ActivationNeuron)network.Layers[0].Neurons[0]).Threshold = 0;
            weights[1] = network.Layers[0].Neurons[0].Weights[0] = 0.9;
            weights[2] = network.Layers[0].Neurons[0].Weights[1] = 0.2;


            learning = new PerceptronLearning(network);
            learning.LearningRate = 0.005;

            form = new MyForm() { WindowState = FormWindowState.Maximized };
            form.Paint += (s, a) => Redraw(a.Graphics);

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += NextSample;
            timer.Start();
            Application.Run(form);
        }

        static void NextSample(object sender, EventArgs e)
        {
            var x = LearningOrder[LearningOrderPointer];
            var y = LearningOrder[LearningOrderPointer + 1];
            LearningOrderPointer += 2;
            if (LearningOrderPointer >= LearningOrder.Count) LearningOrderPointer = 0;

            var input = new double[] { x, y };
            var output = new double[] { Function(x > 0, y > 0) ? 1 : -1 };

            learning.Run(input, output);


            weights[0] = ((ActivationNeuron)network.Layers[0].Neurons[0]).Threshold;
            weights[1] = network.Layers[0].Neurons[0].Weights[0];
            weights[2] = network.Layers[0].Neurons[0].Weights[1];

            form.Invalidate();
           
        }

        static double[] weights;
        static float S;
        static int MarginX;
        static int MarginY;
        
        
        static PointF Plot(double x, double y)
        {
            return new PointF(MarginX+(float)(S*(2+x)),MarginY+(float)(S*(2-y)));
        }


        static void Redraw(Graphics g)
        {
            if (weights == null) return;
            var min = Math.Min(form.ClientSize.Width, form.ClientSize.Height);
            var margin = min / 10;
            S = (min - 2 * margin) / 4;
            MarginX = (form.Width - (min - margin)) / 2;
            MarginY = (form.Height - (min - margin)) / 2;



            g.Clear(Color.White);

            var arrowPen = new Pen(Color.Black, 2) { EndCap = LineCap.ArrowAnchor };
            g.DrawLine(arrowPen, Plot(-2, 0), Plot(2, 0));
            g.DrawLine(arrowPen, Plot(0, -2), Plot(0, 2));

            float A = (float)weights[1];
            float B = (float)weights[2];
            float C = (float)weights[0];

            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                {
                    var result = A * x + B * y + C;
                    var brush = result < 0 ? Brushes.Black : Brushes.White;
                    var center = Plot(x, y);
                    var ellipse = new RectangleF(center.X - 10, center.Y - 10, 20, 20);
                    g.FillEllipse(brush, ellipse);
                    g.DrawEllipse(Pens.Black, ellipse);
                }


            var linePen = new Pen(Color.Black, 2) { DashStyle = DashStyle.Dash };

            var sum = (float)Math.Sqrt(A * A + B * B);
            if (sum == 0) return;
            A /= sum;
            B /= sum;
            C /= sum;

            var lineCenter = Plot(-C * A, -C * B);
            var begin = new PointF(lineCenter.X - S * B, lineCenter.Y - S * A);
            var end = new PointF(lineCenter.X + S * B, lineCenter.Y + S * A);

            g.DrawLine(linePen, begin, end);
          
            var directionPen = new Pen(Color.Red, 2) { EndCap = LineCap.ArrowAnchor };
            g.DrawLine(directionPen, lineCenter.X, lineCenter.Y, lineCenter.X + (float)(50 * A), lineCenter.Y - (float)(50 * B));

        }



  



    }
}

