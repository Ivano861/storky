using Flyer.Structures;
using System;
using System.Text;

namespace Flyer
{
    /// <summary>
    /// Handles the command of start of communication to recognize the client module.
    /// </summary>
    internal class CommandHello : CommandBase
    {
        #region Constructors
        public CommandHello(IMember member)
        {
            Member = member;
        }
        #endregion

        #region Public properties
        public IMember Member { get; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[45];

            result[0] = (byte)Message.CommandList.Hello;
            Array.Copy(BitConverter.GetBytes(Member.Subscription.Family), 0, result, 1, 2);
            Array.Copy(BitConverter.GetBytes(Member.Subscription.Application), 0, result, 3, 2);
            Array.Copy(BitConverter.GetBytes(Member.Subscription.Module), 0, result, 5, 2);
            Array.Copy(BitConverter.GetBytes(Member.Subscription.Functionality), 0, result, 7, 2);
            Array.Copy(Encoding.ASCII.GetBytes(Member.Id), 0, result, 9, 36);

            return result;
        }
        #endregion
    }
}
