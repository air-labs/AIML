using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionRegression
{
    public class RegressionTaskV3 : RegressionTaskV2
    {
        

        public RegressionTaskV3()
        {
            LearningRange = new Range(0, 1, 0.025);
            Function = z => z * Math.Sin(15*z);
            MaxError = 0.5;
        }
    }
}
