using System;
using System.Text;

namespace Flyer
{
    internal class CommandNotifyToId : CommandBase
    {
        #region Constructors
        public CommandNotifyToId(byte[] info, string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 36)
            {
                // TODO: insert error
                throw new ArgumentException("the id parameter is invalid.");
            }
            //if (id.Length > 36)
            //    Id = id.Substring(0, 36);
            //else if (id.Length < 36)
            //    Id = id + new string(' ', 36 - id.Length);
            //else
            Id = id;
            Info = new byte[info.Length];
            if (Info.Length > 0)
                info.CopyTo(Info, 0);
        }
        #endregion

        #region Public properties
        public string Id { get; }
        public byte[] Info { get; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 36 + Info.Length];
            result[0] = (byte)Message.CommandList.NotifyToId;
            Array.Copy(Encoding.ASCII.GetBytes(Id), 0, result, 1, 36);
            if (Info.Length > 0)
                Info.CopyTo(result, 37);

            return result;
        }
        #endregion
    }
}
