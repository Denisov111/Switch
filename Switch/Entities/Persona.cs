using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Switch
{
    public class Persona
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProfilePath { get; set; }
        public string UserAgent { get; set; }
        public Proxy Proxy { get; set; }


        public override string ToString()
        {
            return Title + " " + Description + " " + Proxy.ToString();
        }
    }
}
