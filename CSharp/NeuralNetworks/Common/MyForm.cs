using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common
{
    public class MyForm : Form
    {
        public MyForm()
        {
            DoubleBuffered = true;
        }
    }

    public class MyUserControl : UserControl
    {
        public MyUserControl()
        {
            DoubleBuffered = true;
        }
    }
}
