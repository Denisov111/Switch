using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;

namespace Switch
{
    public class Persona
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfilePath { get; set; }
        public string UserAgent { get; set; }
        public Proxy Proxy { get; set; }
    }
}
