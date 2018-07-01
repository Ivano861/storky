using System;
using System.Collections.Generic;
using Storky.Structures;

namespace Storky
{
    internal class CommandDeregisterNotify : CommandBase
    {
        #region Constructors
        public CommandDeregisterNotify(Message msg)
        {
            byte[] buffer = msg.Data;
            int subscriptionsCount = BitConverter.ToInt32(buffer, 0);

            Subscriptions subscriptions = new Subscriptions();
            for (int i = 0; i < subscriptionsCount; i++)
            {
                subscriptions.Add(new Subscription(BitConverter.ToUInt16(buffer, 4 + i * 8),
                                                   BitConverter.ToUInt16(buffer, 6 + i * 8),
                                                   BitConverter.ToUInt16(buffer, 8 + i * 8),
                                                   BitConverter.ToUInt16(buffer, 10 + i * 8)));
            }
            Subscriptions = subscriptions;
        }
        #endregion

        #region Public properties
        public IReadOnlyList<ISubscription> Subscriptions { get; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 2 + (2 + 2 + 2 + 2) * Subscriptions.Count];
            result[0] = (byte)Message.CommandList.DeregisterNotify;
            for (int i = 0; i < Subscriptions.Count; i++)
            {
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Family), 0, result, 5 + i * 8, 2);
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Application), 0, result, 7 + i * 8, 2);
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Module), 0, result, 9 + i * 8, 2);
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Functionality), 0, result, 11 + i * 8, 2);
            }

            return result;
        }
        #endregion
    }
}
