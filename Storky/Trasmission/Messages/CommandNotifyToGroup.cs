using Storky.Structures;
using System;

namespace Storky
{
    internal class CommandNotifyToGroup : CommandBase
    {
        #region Constructors
        public CommandNotifyToGroup(Message msg)
        {
            byte[] buffer = msg.Data;

            Subscription = new Subscription(BitConverter.ToUInt16(buffer, 0),
                                            BitConverter.ToUInt16(buffer, 2),
                                            BitConverter.ToUInt16(buffer, 4),
                                            BitConverter.ToUInt16(buffer, 6));
            Strict = (buffer[8] != 0);
            Self = (buffer[9] != 0);
            Info = new byte[buffer.Length - 10];
            if (Info.Length > 0)
                Array.Copy(buffer, 10, Info, 0, buffer.Length - 10);
        }
        #endregion

        #region Public properties
        public byte[] Info { get; }
        public ISubscription Subscription { get; }
        public bool Strict { get; }
        public bool Self { get; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 2 + 2 + 2 + 2 + 1 + 1 + Info.Length];
            result[0] = (byte)Message.CommandList.NotifyToGroup;
            Array.Copy(BitConverter.GetBytes(Subscription.Family), 0, result, 1, 2);
            Array.Copy(BitConverter.GetBytes(Subscription.Application), 0, result, 3, 2);
            Array.Copy(BitConverter.GetBytes(Subscription.Module), 0, result, 5, 2);
            Array.Copy(BitConverter.GetBytes(Subscription.Functionality), 0, result, 7, 2);
            result[9] = (byte)(Strict ? 1 : 0);
            result[10] = (byte)(Self ? 1 : 0);
            if (Info.Length > 0)
                Info.CopyTo(result, 11);

            return result;
        }
        #endregion
    }
}
