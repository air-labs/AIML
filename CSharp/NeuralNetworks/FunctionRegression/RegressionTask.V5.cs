using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionRegression
{
    public class RegressionTaskV5 : RegressionTaskV4
    {
        

        public RegressionTaskV5()
        {
            LearningRange = new Range(0, 1, 0.025);
            Function = z => z * Math.Sin(25*z);
            MaxError = 0.5;
            IterationsCount = 102400 + 51200;
        }
    }
}
