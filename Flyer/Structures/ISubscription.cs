using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Structures
{
    public interface ISubscription
    {
        ushort Family { get; }
        ushort Application { get; }
        ushort Module { get; }
        ushort Functionality { get; }
    }
}
