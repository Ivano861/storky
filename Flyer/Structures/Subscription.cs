using Flyer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Structures
{
    public sealed class Subscription : ISubscription
    {
        #region Constructors
        private Subscription(ushort family, ushort application, ushort module, ushort functionality)
        {
            Family = family;
            Application = application;
            Module = module;
            Functionality = functionality;
        }
        #endregion

        #region Static methods
        public static ISubscription Create(ushort family, ushort application = 0, ushort module = 0, ushort functionality = 0)
        {
            if ((family == 0) ||
                (application == 0 && module != 0) ||
                (module == 0 && functionality != 0))
            {
                throw new CollectorArgumentException("The family parameter cannot be equal to 0 and the other parameters cannot be equal to 0 if a subsequent parameter is greater than 0.");
            }

            return new Subscription(family, application, module, functionality);
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
