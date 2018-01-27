using System;

namespace Storky
{
    internal class CommandNotify : CommandBase
    {
        #region Membri privati
        private byte[] _info;
        private bool _self;
        #endregion

        #region Constructors
        public CommandNotify(Message msg)
        {
            byte[] buffer = msg.Data;
            _self = (buffer[0] != 0);
            _info = new byte[buffer.Length - 1];
            if (_info.Length > 0)
                Array.Copy(buffer, 1, _info, 0, buffer.Length - 1);
        }
        public CommandNotify(CommandNotifyToGroup notifyToGroup)
        {
            _info = new byte[notifyToGroup.Info.Length];
            if (_info.Length > 0)
                notifyToGroup.Info.CopyTo(_info, 0);

            _self = notifyToGroup.Self;
        }
        public CommandNotify(CommandNotifyToId notifyToId)
        {
            _info = new byte[notifyToId.Info.Length];
            if (_info.Length > 0)
                notifyToId.Info.CopyTo(_info, 0);

            _self = true;
        }
        #endregion

        #region Public properties
        public byte[] Info { get => _info; }
        public bool Self { get => _self; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 1 + _info.Length];
            result[0] = (byte)Message.CommandList.Notify;
            result[1] = (byte)(_self ? 1 : 0);
            if (_info.Length > 0)
                _info.CopyTo(result, 2);

            return result;
        }
        #endregion
    }
}
