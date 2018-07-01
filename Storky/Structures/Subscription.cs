using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky.Structures
{
    internal sealed class Subscription : ISubscription
    {
        #region Constructors
        public Subscription(ushort family, ushort application, ushort module, ushort functionality)
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

        //internal Subscription Copy()
        //{
        //    return new Subscription(Family, Application, Module, Functionality);
        //}
        #endregion
    }
}
