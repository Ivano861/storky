using System;

namespace Flyer
{
    internal class CommandDeregisterNotify : CommandBase
    {
        #region Membri privati
        private ushort _family;
        private ushort _application;
        private ushort _module;
        private ushort _functionality;
        #endregion

        #region Constructors
        public CommandDeregisterNotify(ushort family, ushort application, ushort module, ushort functionality)
        {
            _family = family;
            _application = application;
            _module = module;
            _functionality = functionality;
        }
        #endregion

        #region Public properties
        public ushort Family { get => _family; }
        public ushort Application { get => _application; }
        public ushort Module { get => _module; }
        public ushort Functionality { get => _functionality; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 2 + 2 + 2 + 2];
            result[0] = (byte)Message.CommandList.DeregisterNotify;
            Array.Copy(BitConverter.GetBytes(_family), 0, result, 1, 2);
            Array.Copy(BitConverter.GetBytes(_application), 0, result, 3, 2);
            Array.Copy(BitConverter.GetBytes(_module), 0, result, 5, 2);
            Array.Copy(BitConverter.GetBytes(_functionality), 0, result, 7, 2);

            return result;
        }
        #endregion
    }
}
