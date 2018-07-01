using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky.Structures
{
    internal class MemberSubscription : ISubscription
    {
        #region Constructors
        public MemberSubscription(ushort family, ushort application, ushort module, ushort functionality)
        {
            Family = family;
            Application = application;
            Module = module;
            Functionality = functionality;
        }
        #endregion

        #region Public properties
        public ushort Family { get; }
        public ushort Application { get; }
        public ushort Module { get; }
        public ushort Functionality { get; }
        #endregion
    }
}
