﻿using System;

namespace Storky
{
    internal class CommandRegisterNotify : CommandBase
    {
        #region Membri privati
        private ushort _family;
        private ushort _application;
        private ushort _module;
        private ushort _functionality;
        private bool _strict;
        #endregion

        #region Constructors
        public CommandRegisterNotify(Message msg)
        {
            byte[] buffer = msg.Data;
            _family = BitConverter.ToUInt16(buffer, 0);
            _application = BitConverter.ToUInt16(buffer, 2);
            _module = BitConverter.ToUInt16(buffer, 4);
            _functionality = BitConverter.ToUInt16(buffer, 6);
            _strict = (buffer[8] != 0);
        }
        #endregion

        #region Public properties
        public ushort Family { get => _family; }
        public ushort Application { get => _application; }
        public ushort Module { get => _module; }
        public ushort Functionality { get => _functionality; }
        public bool Strict { get => _strict; }
        #endregion

        #region Public methods
        public override byte[] ToSend()
        {
            byte[] result = new byte[1 + 2 + 2 + 2 + 2 + 1];
            result[0] = (byte)Message.CommandList.RegisterNotify;
            Array.Copy(BitConverter.GetBytes(_family), 0, result, 1, 2);
            Array.Copy(BitConverter.GetBytes(_application), 0, result, 3, 2);
            Array.Copy(BitConverter.GetBytes(_module), 0, result, 5, 2);
            Array.Copy(BitConverter.GetBytes(_functionality), 0, result, 7, 2);
            result[9] = (byte)(_strict ? 1 : 0);

            return result;
        }
        #endregion
    }
}