using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Errors
{
    public class CollectorException : Exception
    {
        public CollectorException()
            : base()
        {
        }

        public CollectorException(string message)
            : base(message)
        {
        }

        public CollectorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
