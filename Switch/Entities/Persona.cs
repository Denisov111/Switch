using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChromeModForNet;
using System.Drawing;

namespace Switch
{
    public class Persona
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProfilePath { get; set; }
        public string UserAgent { get; set; }
        public Proxy Proxy { get; set; }
        public ChromeMod ChromeInstance { get; set; }
        public string HashString { get; set; }
        public Bitmap Avatar { get; set; }


        public override string ToString()
        {
            string proxy = Proxy == null ? "" : Proxy.ToString();
            return Title + " " + Description + " " + proxy;
        }
    }
}
