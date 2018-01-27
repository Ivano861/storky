using System;

namespace Flyer
{
    internal class CommandNotifyToGroup : CommandBase
    {
        #region Membri privati
        private byte[] _info;
        private ushort _family;
        private ushort _application;
        private ushort _module;
        private ushort _functionality;
        private bool _strict;
        private bool _self;
        #endregion

        #region Constructors
        public CommandNotifyToGroup(byte[] info, ushort family, ushort application, ushort module, ushort functionality, bool strict, bool self)
        {
            _info = new byte[info.Length];
            if (_info.Length > 0)
                info.CopyTo(_info, 0);

            _family = family;
            _application = application;
            _module = module;
            _functionality = functionality;
            _strict = strict;
            _self = self;
        }
        #endregion

        #region Public properties
        public byte[] Info { get => _info; }
        public ushort Family { get => _family; }
        public ushort Application { get => _application; }
        public ushort Module { get => _module; }
        public ushort Functionality { get => _functionality; }
        public bool Strict { get => _strict; }
        public bool Self { get => _self; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 2 + 2 + 2 + 2 + 1 + 1 + _info.Length];
            result[0] = (byte)Message.CommandList.NotifyToGroup;
            Array.Copy(BitConverter.GetBytes(_family), 0, result, 1, 2);
            Array.Copy(BitConverter.GetBytes(_application), 0, result, 3, 2);
            Array.Copy(BitConverter.GetBytes(_module), 0, result, 5, 2);
            Array.Copy(BitConverter.GetBytes(_functionality), 0, result, 7, 2);
            result[9] = (byte)(_strict ? 1 : 0);
            result[10] = (byte)(_self ? 1 : 0);
            if (_info.Length > 0)
                _info.CopyTo(result, 11);

            return result;
        }
        #endregion
    }
}
