using System;
using System.Text;

namespace Storky
{
    /// <summary>
    /// Handles the command of start of communication to recognize the client.
    /// </summary>
    internal class CommandHello : CommandBase
    {
        #region Private members
        private string _id;
        private ushort _family;
        private ushort _application;
        private ushort _module;
        private ushort _functionality;
        #endregion

        #region Constructors
        public CommandHello(Message msg)
        {
            byte[] buffer = msg.Data;

            if (buffer.Length < 40)
            {
                _family = 0;
                _application = 0;
                _module = 0;
                _functionality = 0;
                _id = string.Empty;
                return;
            }

            // reading the family number
            _family = BitConverter.ToUInt16(buffer, 0);
            // reading the application number
            _application = BitConverter.ToUInt16(buffer, 2);
            // reading the module number
            _module = BitConverter.ToUInt16(buffer, 4);
            // reading the functionality number
            _functionality = BitConverter.ToUInt16(buffer, 6);
            // reading the id
            _id = Encoding.ASCII.GetString(buffer, 8, 36);
        }
        #endregion

        #region Public properties
        public ushort Family { get => _family; }
        public ushort Application { get => _application; }
        public ushort Module { get => _module; }
        public ushort Functionality { get => _functionality; }
        public string Id { get => _id; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[41];

            result[0] = (byte)Message.CommandList.Hello;
            Array.Copy(BitConverter.GetBytes(_family), 0, result, 1, 2);
            Array.Copy(BitConverter.GetBytes(_application), 0, result, 3, 2);
            Array.Copy(BitConverter.GetBytes(_module), 0, result, 5, 2);
            Array.Copy(BitConverter.GetBytes(_functionality), 0, result, 7, 2);
            Array.Copy(Encoding.ASCII.GetBytes(_id), 0, result, 9, 36);

            return result;
        }
        #endregion
    }
}
