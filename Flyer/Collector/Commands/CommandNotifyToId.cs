using System;
using System.Text;

namespace Flyer
{
    internal class CommandNotifyToId : CommandBase
    {
        #region Membri privati
        private string _id;
        private byte[] _info;
        #endregion

        #region Constructors
        public CommandNotifyToId(byte[] info, string id)
        {
            if (id.Length > 36)
                _id = id.Substring(0, 36);
            else if (id.Length < 36)
                _id = id + new string(' ', 36 - id.Length);
            else
                _id = id;

            _info = new byte[info.Length];
            if (_info.Length > 0)
                info.CopyTo(_info, 0);
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
