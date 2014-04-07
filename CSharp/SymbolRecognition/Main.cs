using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolRecognition
{
    class LearningArguments
    {
        public BaseMaker BaseMaker;
        public int NeuronsCount;
    }


    class Program
    {
        static LearningArguments task1 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 60,
                DeltaAngle = 10,
                ShowWhenCreated = true,
            },
            NeuronsCount = 30
        };

        static LearningArguments task2 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 60,
                DeltaAngle = 5,
                ShowWhenCreated = true,
            },
            NeuronsCount = 30
        };

        static LearningArguments task3 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 60,
                DeltaAngle = 5,
                NoiseLevel=0.05,
                ShowWhenCreated = true,
            },
            NeuronsCount = 30
        };

        static LearningArguments task4 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 60,
                DeltaAngle = 5,
                NoiseLevel = 0.05,
                ShowWhenCreated = true,
            },
            NeuronsCount = 60
        };
         


        public static void Main()
        {
            Learning.Run(task4);

        }
    }
}
