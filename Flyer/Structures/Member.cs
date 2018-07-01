using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Structures
{
    internal sealed class Member : IMember
    {
        #region Constructors
        private Member(string id, ushort family, ushort application, ushort module, ushort functionality)
        {
            Id = id;
            Subscription = MemberSubscription.Create(family, application, module, functionality);
        }
        #endregion

        #region Static methods
        public static IMember Create(ushort family, ushort application, ushort module, ushort functionality)
        {
            if (family == 0 || application == 0 || module == 0 || functionality == 0)
            {
                // TODO: insert correct exception
                throw new ArgumentException("All parameters require a value greater than 0.");
            }

            return new Member(Guid.NewGuid().ToString(), family, application, module, functionality);
        }
        #endregion

        #region Public properties
        public string Id { get; }
        public ISubscription Subscription { get; }
        #endregion
    }
}
