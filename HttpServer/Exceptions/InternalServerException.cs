using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Exceptions
{
    internal class InternalServerException : Exception
    {
        public InternalServerException()
        {
        }

        public InternalServerException(string message)
            : base(message)
        {
        }

        public InternalServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
