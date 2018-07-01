using System;
using System.Text;

namespace Flyer
{
    internal class Message
    {
        // The communication protocol is as follows:
        // Storky MSG-       11 byte -> A constant string that identifies the Protocol
        // version            1 byte -> Protocol version
        // len                4 byte -> Message size
        // data               n byte -> Message of size length

        // The date portion contains the following message are explained (More information can be found in the description of individual messages):
        // Unknow             -> used internally to initialize and to indicate a bad message
        // Hello              -> used during initial connection to register a client
        // Ready              -> used during the initial connection to report enrollment status
        // RegisterNotify     -> registers the client to receive notification specifies
        // DeregisterNotify   -> Unregisters the client to receive notification specifies
        // Notify             -> used for send a notification to registered nodes
        // NotifyToGroup      -> used for send a notication to group
        // NotifyToId         -> used for send a notication to client Id

        // Protocol messages:
        // - Hello -
        // Message code        1 byte -> Message ID
        // Family code         2 byte -> Family ID
        // Application code    2 byte -> Application ID
        // Module code         2 byte -> Module ID
        // Functionality code  2 byte -> Functionality ID
        // Client code        36 byte -> Client ID
        //
        // - Ready -
        // Message code        1 byte -> Message ID
        //
        // - RegisterNotify -
        // Message code        1 byte -> Message ID
        // Family code         2 byte -> Family ID
        // Application code    2 byte -> Application ID
        // Module code         2 byte -> Module ID
        // Functionality code  2 byte -> Functionality ID
        // Strict              1 byte -> Determines if you receive messages more interiors
        //
        // - DeregisterNotify -
        // Message code        1 byte -> Message ID
        // Family code         2 byte -> Family ID
        // Application code    2 byte -> Application ID
        // Module code         2 byte -> Module ID
        // Functionality code  2 byte -> Functionality ID
        //
        // - Notify -
        // Message code        1 byte -> Message ID
        // Self                1 byte -> Determines whether the message can be self received
        // Message             n byte -> Array of bytes containing the message
        //
        // - NotifyToGroup -
        // Message code        1 byte -> Message ID
        // Family code         2 byte -> Target Family ID
        // Application code    2 byte -> Target Application ID
        // Module code         2 byte -> Target Module ID
        // Functionality code  2 byte -> Target Functionality ID
        // Strict              1 byte -> Determines if you receive messages more interiors
        // Self                1 byte -> Determines whether the message can be self received
        // Message             n byte -> Array of bytes containing the message
        //
        // - NotifyToId -
        // Message code        1 byte -> Message ID
        // Client code        36 byte -> Client ID
        // Message             n byte -> Array of bytes containing the message

        #region Enumerate
        public enum CommandList
        {
            Unknown = 0,
            Hello,
            Ready,
            RegisterNotify,
            DeregisterNotify,
            Notify,
            NotifyToGroup,
            NotifyToId
        }
        #endregion

        #region Static constant
        public const string MessageHeader = "Storky MSG-";
        public const byte Version = 1;
        public const int MessageHeaderLen = 11; // MessageHeader.Length;
        public const int DataLen = 4;
        #endregion

        #region Private members
        private byte[] _data;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor used for reading messages from clients.
        /// </summary>
        internal Message(byte[] data)
        {
            if (data == null || data.Length < 1)
            {
                Command = CommandList.Unknown;
                _data = new byte[0];
                return;
            }

            // Create the new message
            Command = (CommandList)data[0];
            int lenMessage = data.Length - 1;
            _data = new byte[lenMessage];
            Array.Copy(data, 1, _data, 0, lenMessage);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Length of message
        /// </summary>
        public int DataLength { get => _data.Length; }

        /// <summary>
        /// Message ID
        /// </summary>
        public CommandList Command { get; }

        /// <summary>
        /// array of bytes that contains the data associated at the message
        /// </summary>
        public byte[] Data
        {
            get
            {
                byte[] result = new byte[_data.Length];

                _data.CopyTo(result, 0);

                return result;
            }
        }
        #endregion

        #region Internal static methods
        /// <summary>
        /// Check the accuracy of the header
        /// </summary>
        /// <param name="buffer">Array of bytes to verify</param>
        /// <returns>True if the buffer contains the text of the header, otherwise false.</returns>
        internal static bool TestHeader(byte[] buffer)
        {
            if (buffer.Length < MessageHeaderLen)
                return false;

            return (Encoding.ASCII.GetString(buffer, 0, MessageHeaderLen) == MessageHeader);
        }

        /// <summary>
        /// Check the accuracy of the version
        /// </summary>
        /// <param name="buffer">Array of bytes to verify</param>
        /// <returns>True if the buffer contains the valid version, otherwise false.</returns>
        internal static bool TestVersion(byte[] buffer)
        {
            if (buffer.Length < 1)
                return false;

            return (buffer[0] == 1);
        }

        /// <summary>
        /// Check the accuracy of the length
        /// </summary>
        /// <param name="buffer">Array of bytes to verify</param>
        /// <returns>The length if the buffer contains a valid value, otherwise 0.</returns>
        internal static int GetLength(byte[] buffer)
        {
            if (buffer.Length < DataLen)
                return 0;

            return (BitConverter.ToInt32(buffer, 0));
        }

        /// <summary>
        /// Create complete message from a command object
        /// </summary>
        /// <param name="cmd">Command object to send</param>
        /// <returns>Aarray of bytes that contains the entire message.</returns>
        internal static byte[] MessageToSend(ICommandSend cmd)
        {
            byte[] buffer = cmd.ToSend();
            byte[] result = new byte[MessageHeaderLen + 1 + DataLen + buffer.Length];

            // Inserts the header
            Encoding.ASCII.GetBytes(MessageHeader).CopyTo(result, 0);
            // Inserts the version
            result[MessageHeaderLen] = Version;
            // Inserts the length of message
            BitConverter.GetBytes(buffer.Length).CopyTo(result, MessageHeaderLen + 1);
            // Puts the message
            buffer.CopyTo(result, MessageHeaderLen + 1 + DataLen);

            return result;
        }
        #endregion
    }
}
