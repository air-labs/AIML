using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfOrganizingNetworks
{
    class SelfOrganizingTaskV4 : SelfOrganizingTaskV0
    {

        public SelfOrganizingTaskV4()
        {
            NetworkWidth = 10;
            NetworkHeight = 1;
            LearningRadius = 2;
        }

        public override List<double[]> GenerateInputs()
        {
            var list = new List<double[]>();
            for (int i = 0; i < 100; i++)
            {
                var x = Rnd.NextDouble();
                var y = Math.Pow(x * 2 - 1,2);
                var xx = x + Rnd.NextDouble() * 2 * Radius - Radius;
                var yy = y + Rnd.NextDouble() * 2 * Radius - Radius;
                list.Add(new[] { xx, yy });
             }
            return list;
        }
        
    }
}
