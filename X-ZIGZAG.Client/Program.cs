using frameless.Properties;
using System;
using System.Threading;
using System.Windows.Forms;

namespace frameless
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.Sleep(new Random().Next(1_000, 1_000));
            Application.Run(new Form1());
        }
    }
}
