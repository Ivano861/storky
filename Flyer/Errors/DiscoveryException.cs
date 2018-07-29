using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Errors
{
    public class DiscoveryException : Exception
    {
        public DiscoveryException()
            : base()
        {
        }

        public DiscoveryException(string message)
            : base(message)
        {
        }

        public DiscoveryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
