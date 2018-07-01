using System;
using System.Text;

namespace Storky
{
    internal class CommandNotifyToId : CommandBase
    {
        #region Constructors
        public CommandNotifyToId(Message msg)
        {
            if (msg.DataLength < 36)
            {
                // TODO: error
            }
            byte[] buffer = msg.Data;
            Array.Copy(Encoding.ASCII.GetBytes(Id), 0, buffer, 0, 36);
            Info = new byte[buffer.Length - 36];
            if (Info.Length > 0)
                Array.Copy(buffer, 36, Info, 0, buffer.Length - 36);
        }
        #endregion

        #region Public properties
        public string Id { get; }
        public byte[] Info { get; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 36 + Info.Length];
            result[0] = (byte)Message.CommandList.NotifyToId;
            Array.Copy(Encoding.ASCII.GetBytes(Id), 0, result, 1, 36);
            if (Info.Length > 0)
                Info.CopyTo(result, 37);

            return result;
        }
        #endregion
    }
}
