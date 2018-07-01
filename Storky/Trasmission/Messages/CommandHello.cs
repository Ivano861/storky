using Storky.Structures;
using System;
using System.Text;

namespace Storky
{
    /// <summary>
    /// Handles the command of start of communication to recognize the client.
    /// </summary>
    internal class CommandHello : CommandBase
    {
        #region Constructors
        public CommandHello(Message msg)
        {
            byte[] buffer = msg.Data;

            if (buffer.Length < 44)
            {
                Member = new Member(string.Empty, 0, 0, 0, 0);
                return;
            }

            Member = new Member(Encoding.ASCII.GetString(buffer, 8, 36),    // reading the id
                                BitConverter.ToUInt16(buffer, 0),           // reading the family number
                                BitConverter.ToUInt16(buffer, 2),           // reading the application number
                                BitConverter.ToUInt16(buffer, 4),           // reading the module number
                                BitConverter.ToUInt16(buffer, 6));          // reading the functionality number
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
