using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storky
{
    internal class Registration
    {
        #region Private members
        private ushort _family;
        private ushort _application;
        private ushort _module;
        private ushort _functionality;
        private bool _strict;
        #endregion

        #region Constructors
        public Registration(ushort family, ushort application, ushort module, ushort functionality, bool strict)
        {
            _family = family;
            _application = application;
            _module = module;
            _functionality = functionality;
            _strict = strict;
        }
        #endregion

        #region Properties
        public ushort Family { get => _family; }
        public ushort Application { get => _application; }
        public ushort Module { get => _module; }
        public ushort Functionality { get => _functionality; }
        public bool Strict { get => _strict; }
        #endregion
    }
}
