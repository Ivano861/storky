using System;

namespace Storky
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
        public CommandNotifyToGroup(Message msg)
        {
            byte[] buffer = msg.Data;
            _family = BitConverter.ToUInt16(buffer, 0);
            _application = BitConverter.ToUInt16(buffer, 2);
            _module = BitConverter.ToUInt16(buffer, 4);
            _functionality = BitConverter.ToUInt16(buffer, 6);
            _strict = (buffer[8] != 0);
            _self = (buffer[9] != 0);
            _info = new byte[buffer.Length - 10];
            if (_info.Length > 0)
                Array.Copy(buffer, 10, _info, 0, buffer.Length - 10);
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
