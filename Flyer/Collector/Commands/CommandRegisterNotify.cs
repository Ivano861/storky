using System;
using System.Collections.Generic;
using Flyer.Structures;

namespace Flyer
{
    internal class CommandRegisterNotify : CommandBase
    {
        #region Constructors
        public CommandRegisterNotify(ISubscription subscription, bool strict)
        {
            Subscriptions = new Subscriptions
            {
                subscription
            };
            Strict = strict;
        }
        public CommandRegisterNotify(IEnumerable<ISubscription> subscriptions, bool strict)
        {
            Subscriptions = new List<ISubscription>(subscriptions);
            Strict = strict;
        }
        #endregion

        #region Public properties
        public IReadOnlyList<ISubscription> Subscriptions { get; }
        public bool Strict { get; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 4 + (2 + 2 + 2 + 2) * Subscriptions.Count + 1];
            result[0] = (byte)Message.CommandList.RegisterNotify;
            Array.Copy(BitConverter.GetBytes(Subscriptions.Count), 0, result, 1, 4);
            for (int i = 0; i < Subscriptions.Count; i++)
            {
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Family), 0, result, 5 + i * 8, 2);
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Application), 0, result, 7 + i * 8, 2);
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Module), 0, result, 9 + i * 8, 2);
                Array.Copy(BitConverter.GetBytes(Subscriptions[i].Functionality), 0, result, 11 + i * 8, 2);
            }
            result[Subscriptions.Count * 8 + 5] = (byte)(Strict ? 1 : 0);

            return result;
        }
        #endregion
    }
}
