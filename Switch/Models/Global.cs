using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch
{
    public class Global
    {
        public Views.MainWindow view;

        public void Run()
        {
            view = new Views.MainWindow();
            Console.WriteLine("HW");
        }
    }
}
