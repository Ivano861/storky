using System;

namespace Flyer
{
    internal class CommandNotify : CommandBase
    {
        #region Constructors
        public CommandNotify(Message msg)
        {
            byte[] buffer = msg.Data;
            Self = (buffer[0] != 0);
            Info = new byte[buffer.Length - 1];
            if (Info.Length > 0)
                Array.Copy(buffer, 1, Info, 0, buffer.Length - 1);
        }
        public CommandNotify(byte[] info, bool self)
        {
            Info = new byte[info.Length];
            if (Info.Length > 0)
                info.CopyTo(Info, 0);

            Self = self;
        }
        #endregion

        #region Public properties
        public byte[] Info { get; }
        public bool Self { get; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 1 + Info.Length];
            result[0] = (byte)Message.CommandList.Notify;
            result[1] = (byte)(Self ? 1 : 0);
            if (Info.Length > 0)
                Info.CopyTo(result, 2);

            return result;
        }
        #endregion
    }
}
