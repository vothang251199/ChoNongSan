using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Utilities.Exceptions
{
    public class ChoNSException : Exception
    {
        public ChoNSException()
        {
        }

        public ChoNSException(string message)
            : base(message)
        {
        }

        public ChoNSException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}