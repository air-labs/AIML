using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FuzzyLibrary;

namespace FuzzyArithmetic
{

    static class Program
    {


        static string DTS(double d)
        {
            return d.ToString().Replace(',', '.');
        }

        static void SavePlot(string Name, FuzzyNumber num)
        {
            using (var file = new StreamWriter(@"..\..\..\..\LaTeX\Plots\" + Name + ".txt"))
            {
                file.WriteLine("# N mu");
                foreach(var e in num.Domain.Arguments)
                    if (num[e] > 0.01)
                        file.WriteLine("{0}\t{1}", DTS(e), DTS(num[e]));
            }
            using (var file = new StreamWriter(@"..\..\..\..\LaTeX\Plots\" + Name + ".value.txt"))
            {
                var sum = num.Domain.Arguments.Select(z => z * num[z]).Sum();
                var wei = num.Domain.Arguments.Select(z => num[z]).Sum();
                file.WriteLine("{0:0.000}", Math.Round(sum / wei, 3));
            }
        }

        static FuzzyNumber SumTest(double number, int count)
        {
            var result = domain.Near(number);
            for (int i = 0; i < count-1; i++)
            {
                var argument = domain.Near(number);
                result += argument;
            }
            return result;
        }


        static void MakePlots(string suffix)
        {
            SavePlot("4_" + suffix, domain.Near(4));
            SavePlot("2_mult_2_" + suffix, domain.Near(2) * domain.Near(2));
            SavePlot("2_plus_2_" + suffix, domain.Near(2)+domain.Near(2));
        }

        static Domain domain;

        [STAThread]
        static void Main()
        {


            domain = new Domain(0, 10);

            domain.T = Domain.TMin;
            domain.S = Domain.SMax;

            var A2=domain.Near(2);
            var A3=domain.Near(3);
            SavePlot("2", A2);
            SavePlot("3", A3);
            SavePlot("2_cup1_3", A2 | A3);
            SavePlot("2_cap1_3", A2 & A3);

            domain.T = Domain.TMul;
            domain.S = Domain.SSum;

            SavePlot("2_cap2_3", A2 & A3 );
            SavePlot("2_cup2_3", A2 | A3);

            domain.T = Domain.TMul;
            domain.S = Domain.SMax;

            SavePlot("2_plus_2", A2+A2);
            SavePlot("2_mult_2", A2*A2);
            SavePlot("8_div_2", domain.Near(8)/A2);
            SavePlot("4", domain.Near(4));

            domain.NearFunction = Domain.NearGauss(0.3);
            MakePlots("K03");

            domain.NearFunction = Domain.NearGauss(3);
            MakePlots("K3");

            domain.NearFunction = Domain.NearQuadratic(1);
            MakePlots("Q");

            domain.NearFunction = Domain.NearGauss(1);
           
            domain.T = Domain.TMin;
            MakePlots("min");

            domain.T = Domain.TMul;
            domain.S = Domain.SSum;
            MakePlots("nomax");

            domain.T = Domain.TMul;
            domain.S = Domain.SMax;

            SavePlot("3_times_2", SumTest(3, 2));
            SavePlot("2_times_3", SumTest(2, 3));
            SavePlot("1_times_6", SumTest(1, 6));

            SavePlot("6_mult_1", domain.Near(6)*domain.Near(1));
            SavePlot("3_mult_2", domain.Near(3)*domain.Near(2));

            SavePlot("F_direct", ((domain.Near(2)+domain.Near(3))/domain.Near(2)));
            SavePlot("F_ext", FuzzyNumber.BinaryOperation(domain.Near(2), domain.Near(3), (a, b) => (a + b) / b));
      }
    }
}
