using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    public class Error
    {
        /// <summary>
        /// The error code
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// The message for the error that occured
        /// </summary>
        public string Message { get; set; }

        public Error() { }

        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public Error(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{Code}: {Message}";
        }
    }
}
