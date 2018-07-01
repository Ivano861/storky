namespace Storky
{
    internal class CommandReady : CommandBase
    {
        #region Constructors
        public CommandReady()
        {
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
