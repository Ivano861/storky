using System;
using System.Text;

namespace Storky
{
    internal class CommandNotifyToId : CommandBase
    {
        #region Membri privati
        private string _id;
        private byte[] _info;
        #endregion

        #region Constructors
        public CommandNotifyToId(Message msg)
        {
            byte[] buffer = msg.Data;
            Array.Copy(Encoding.ASCII.GetBytes(_id), 0, buffer, 0, 36);
            _info = new byte[buffer.Length - 36];
            if (_info.Length > 0)
                Array.Copy(buffer, 36, _info, 0, buffer.Length - 36);
        }
        #endregion

        #region Public properties
        public string Id { get => _id; }
        public byte[] Info { get => _info; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 36 + _info.Length];
            result[0] = (byte)Message.CommandList.NotifyToId;
            Array.Copy(Encoding.ASCII.GetBytes(_id), 0, result, 1, 36);
            if (_info.Length > 0)
                _info.CopyTo(result, 37);

            return result;
        }
        #endregion
    }
}
