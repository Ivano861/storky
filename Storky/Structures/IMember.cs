using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky.Structures
{
    public interface IMember
    {
        string Id { get; }
        ISubscription Subscription { get; }
    }
}
