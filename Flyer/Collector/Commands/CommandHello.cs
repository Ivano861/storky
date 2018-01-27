using System;
using System.Text;

namespace Flyer
{
    /// <summary>
    /// Handles the command of start of communication to recognize the client module.
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
        public CommandHello(ushort family, ushort application, ushort module, ushort functionality, string id)
        {
            if (id.Length != 36)
            {
                // TODO: insert exception
            }
            _family = family;
            _application = application;
            _module = module;
            _functionality = functionality;

            if (id.Length > 36)
                _id = id.Substring(0, 36);
            else if (id.Length < 36)
                _id = id + new string(' ', 36 - id.Length);
            else
                _id = id;
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
            byte[] result = new byte[45];

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
