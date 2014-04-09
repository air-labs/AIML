using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfOrganizingNetworks
{
    class SelfOrganizingTaskV0
    {
        public int NetworkWidth=10;
        public int NetworkHeight=10;
        public int LearningRadius=2;
        public double Radius = 0.1;

        public Random Rnd = new Random(1);

        void GenerateInputs(List<double[]> points, double x, double y, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var r = Rnd.NextDouble() * Radius;
                var angle = Rnd.NextDouble()*Math.PI * 2;
                var xx = x + r * Math.Cos(angle);
                var yy = y + r * Math.Sin(angle);
                points.Add(new[] { xx, yy });
            }

        }

        public virtual List<double[]> GenerateInputs()
        {
            var list = new List<double[]>();
            GenerateInputs(list, 0.2, 0.2, 50);
            GenerateInputs(list, 0.8, 0.5, 50);
            GenerateInputs(list, 0.2, 0.8, 50);
            return list;
        }
    }
}
