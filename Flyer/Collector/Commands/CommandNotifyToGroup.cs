using Flyer.Structures;
using System;

namespace Flyer
{
    internal class CommandNotifyToGroup : CommandBase
    {
        #region Constructors
        public CommandNotifyToGroup(byte[] info, ISubscription subscription, bool strict, bool self)
        {
            Info = new byte[info.Length];
            if (Info.Length > 0)
                info.CopyTo(Info, 0);

            Subscription = subscription;
            Strict = strict;
            Self = self;
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
