using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    public class CallResult<T>
    {
        /// <summary>
        /// The data returned by the call
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// An error if the call didn't succeed
        /// </summary>
        public Error Error { get; set; }
        /// <summary>
        /// Whether the call was successful
        /// </summary>
        public bool Success => Error == null;
    }
}
