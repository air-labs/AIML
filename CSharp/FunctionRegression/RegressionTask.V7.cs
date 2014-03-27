using AForge.Neuro;
using AForge.Neuro.Learning;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionRegression
{
    public class RegressionTaskV7 : RegressionTaskV5
    {

        protected override void LearningIteration()
        {
            int RepetitionCount=5;
            for (int i = 0; i < Inputs.Length / RepetitionCount; i++)
            {
                var sample = rnd.Next(Inputs.Length);
                for (int j = 0; j < RepetitionCount; j++)
                    teacher.Run(Inputs[sample], Answers[sample]);
            }
        }

    }
}
