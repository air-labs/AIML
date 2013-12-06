using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FuzzyLibrary;

namespace FuzzyControl
{
    class CannonCommand
    {
        public double R;
        public double Angle;
    }

    static class Program
    {

        static Random rnd = new Random(1);
        static double deviation = 10;
        static double noise = 10;
        static Point Target = new Point(100, 100);

        static int Radius = 20;

        static double Randomize(double x)
        {
            return x + (rnd.NextDouble() - 0.5) * noise * 2;
        }

        static int HitIndex(double hit, int target)
        {
            var result=(int)(Radius*(hit-target)/(1.5*deviation))+Radius;
            if (result<0) result=0;
            if (result>=2*Radius+1) result=2*Radius;
            return result;
        }

        static double[,] TestAlgorithm(Func<double,double,CannonCommand> alg)
        {
            var matrix = new double[2 * Radius + 2, 2 * Radius + 2];
            for (int i = 0; i < 100; i++)
            {
                var x = Randomize(Target.X);
                var y = Randomize(Target.Y);
                var result = alg(x, y);
                var newX = result.R * Math.Cos(result.Angle);
                var newY = result.R * Math.Sin(result.Angle);
                matrix[HitIndex(newX, Target.X), HitIndex(newY, Target.Y)]++;
            }
            
            var max = matrix.Cast<double>().Max();
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    matrix[i, j] /= max;
            return matrix;
        }

        static void DrawMatrix(Graphics g, double[,] matrix)
        {
            for (int x=0;x<matrix.GetLength(0);x++)
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    var val=(int)(matrix[x, y] * 255);
                    var color = Color.FromArgb(val, Color.Red);
                    g.FillRectangle(new SolidBrush(color), 5 * x, 5 * y, 5, 5);
                }

        }

        static CannonCommand TrivialAlgorithm(double x, double y)
        {
            var cmd = new CannonCommand();
            cmd.R = Math.Sqrt(x * x + y * y);
            cmd.Angle = Math.Atan(y/x);
            return cmd;
        }

        static CannonCommand FuzzyAlgorithm(double x, double y)
        {
            x /= 100;
            y /= 100;
            var domain = new Domain(0, 3, 0.1);
            var X = domain.Near(x,0.1);
            var Y = domain.Near(y,0.1);
            var R = X * X + Y * Y;
            var Tan = Y / X;
            return new CannonCommand { R = 100*Math.Sqrt(R.Ceiling()), Angle = Math.Atan(Tan.Ceiling()) };
        }

 
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var matrix = TestAlgorithm(TrivialAlgorithm);
            var matrix = TestAlgorithm(FuzzyAlgorithm);
            var form = new Form();
            form.Paint += (s, a) => { DrawMatrix(a.Graphics, matrix);  };
            Application.Run(form);
        }
    }
}
