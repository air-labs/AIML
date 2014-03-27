using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionRegression
{
    public class RegressionTaskV1 : RegressionTaskV0
    {
        

        public RegressionTaskV1()
        {
            Function = z => z * Math.Sin(z)/10; //нормируем функцию, чтобы она была от 0 до 1
            MaxError = 2;
        }
    }
}
