namespace Flyer
{
    /// <summary>
    /// Handles the command of start of communication to recognize the client.
    /// </summary>
    internal class CommandReady : CommandBase
    {
        #region Private members
        #endregion

        #region Constructors
        public CommandReady(Message msg)
        {
            byte[] buffer = msg.Data;
        }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1];

            result[0] = (byte)Message.CommandList.Ready;

            return result;
        }
        #endregion
    }
}
