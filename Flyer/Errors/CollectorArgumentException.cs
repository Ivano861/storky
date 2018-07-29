using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Errors
{
    class CollectorArgumentException : ArgumentException
    {
        public CollectorArgumentException()
            : base()
        { }

        public CollectorArgumentException(string message)
            : base(message)
        { }

        public CollectorArgumentException(string message, Exception innerException)
                : base(message, innerException)
        { }

        public CollectorArgumentException(string message, string paramName)
                    : base(message, paramName)
        { }

        public CollectorArgumentException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException)
        { }
    }
}
