using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UsefulThings
{
    public interface IAcc
    {
        Proxy Proxy { get; set; }
        XElement Cookies { get; set; }
    }
}
