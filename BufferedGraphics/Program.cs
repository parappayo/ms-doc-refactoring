using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BufferedGraphicsExample
{
    public class BufferedGraphicsExample
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new ExampleForm());
        }
    }
}
