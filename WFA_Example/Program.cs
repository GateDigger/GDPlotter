using System;
using System.Windows.Forms;

namespace GDPlotter
{
    internal static class Program
    {
        [MTAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HeadForm());
        }
    }
}