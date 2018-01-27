namespace Storky
{
    internal abstract class CommandBase : ICommandSend
    {
        public abstract byte[] ToSend();
    }
}
