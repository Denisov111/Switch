using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Switch
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            InitializeComponent();
        }

        [STAThread]
        static void Main()
        {
            App app = new App();
            //MainWindow window = new MainWindow();
            Global gl = new Global();
            gl.Run();
            app.Run(gl.view);
            
        }
    }
}
