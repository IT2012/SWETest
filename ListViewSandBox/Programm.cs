using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListViewSandBox
{
    class Programm
    {
        [STAThread]
        public static void Main()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }
    }
}
