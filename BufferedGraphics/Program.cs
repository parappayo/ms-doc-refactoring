using System;
using System.Windows.Forms;

namespace BufferedGraphicsExample
{
    public class BufferedGraphicsExample
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new ExampleForm
            {
                Width = 1024,
                Height = 768,
            });
        }
    }
}
