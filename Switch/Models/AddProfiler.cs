using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch
{
    class AddProfiler
    {
        internal static void Add(Global global)
        {
            
            Views.AddProfile f = new Views.AddProfile();
            f.Show();
            Persona persona=f.persona;
        }
    }
}
