using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky.Structures
{
    internal sealed class Member : IMember
    {
        #region Constructors
        public Member(string id, ushort family, ushort application, ushort module, ushort functionality)
        {
            Id = id;
            Subscription = new Subscription(family, application, module, functionality);
        }
        public Member(string id, Subscription subscription)
        {
            Id = id;
            Subscription = subscription;
        }
        #endregion

        #region Public properties
        public string Id { get; }
        public ISubscription Subscription { get;}
        #endregion
    }
}
