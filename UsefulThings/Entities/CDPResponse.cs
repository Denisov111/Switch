using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    public class CDPResponse
    {
        public bool Success
        {
            get
            {
                return (ErrorMessage == null) ? true : false;
            }
        }
        public string ErrorMessage { get; set; }

        public object Response { get; set; }
    }
}
