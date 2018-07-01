using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Structures
{
    internal sealed class MemberSubscription : ISubscription
    {
        #region Constructors
        private MemberSubscription(ushort family, ushort application, ushort module, ushort functionality)
        {
            Family = family;
            Application = application;
            Module = module;
            Functionality = functionality;
        }
        #endregion

        #region Static methods
        internal static ISubscription Create(ushort family, ushort application, ushort module, ushort functionality)
        {
            if (family == 0 || application == 0 || module == 0 || functionality == 0)
            {
                // TODO: insert correct exception
                throw new ArgumentException("All parameters require a value greater than 0.");
            }

            return new MemberSubscription(family, application, module, functionality);
        }
        #endregion

        #region Public properties
        public ushort Family { get; }
        public ushort Application { get; }
        public ushort Module { get; }
        public ushort Functionality { get; }

        //internal MemberSubscription Copy()
        //{
        //    return new MemberSubscription(Family, Application, Module, Functionality);
        //}
        #endregion
    }
}
