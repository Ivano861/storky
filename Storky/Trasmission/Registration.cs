using Storky.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky
{
    internal class Registration
    {
        #region Constructors
        public Registration(ISubscription subscription, bool strict)
        {
            Subscription = subscription;
            Strict = strict;
        }

        public Registration(ushort family, ushort application, ushort module, ushort functionality, bool strict)
        {
            Subscription = new Subscription(family, application, module, functionality);
            Strict = strict;
        }
        #endregion

        #region Properties
        public ISubscription Subscription { get; }
        public bool Strict { get; }
        #endregion
    }
}
