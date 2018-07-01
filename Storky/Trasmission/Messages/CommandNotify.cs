using System;

namespace Storky
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
        public CommandNotify(CommandNotifyToGroup notifyToGroup)
        {
            Info = new byte[notifyToGroup.Info.Length];
            if (Info.Length > 0)
                notifyToGroup.Info.CopyTo(Info, 0);

            Self = notifyToGroup.Self;
        }
        public CommandNotify(CommandNotifyToId notifyToId)
        {
            Info = new byte[notifyToId.Info.Length];
            if (Info.Length > 0)
                notifyToId.Info.CopyTo(Info, 0);

            Self = true;
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
