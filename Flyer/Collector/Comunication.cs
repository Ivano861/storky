﻿using System;
using System.Net.Sockets;
using System.Threading;

namespace Flyer
{
    internal class Comunication : IDisposable
    {
        #region AsyncReadState class definition
        /// <summary>
        /// Class used to communicate with the asynchronous read method
        /// </summary>
        private class AsyncReadState
        {
            public int LenRead;
            public int LastRead;
        }
        #endregion

        #region Constants
        private const string ConnectionClosed = "Connection Closed";
        #endregion

        #region Private member
        private Socket _socket;
        private ManualResetEvent _mre;
        private bool _exit;
        #endregion

        #region Contructors
        internal Comunication(Socket socket)
        {
            _socket = socket;
            _mre = new ManualResetEvent(false);
            _exit = false;
        }
        #endregion

        #region Pubblic method used for the comunication
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="msg">The message to be sent.</param>
        /// <returns>True if the message is sent, otherwise false.</returns>
        public void SendCommand(ICommandSend cmd)
        {
            byte[] data = Message.MessageToSend(cmd);

            // Send the message
            _socket.Send(data, data.Length, SocketFlags.None);
        }

        /// <summary>
        /// Waits for an asynchronous message.
        /// </summary>
        /// <returns>The received message.</returns>
        public Message ReceiveMessage()
        {
            #region Message header reading
            // Start reading the message header
            if (_exit || ReadAsync(out byte[] buffer, Message.MessageHeaderLen) == 0)
                return null;

            // Test the message header
            if (!Message.TestHeader(buffer))
                return null;
            #endregion

            #region Version comunication
            // Reading version comunication
            if (_exit || ReadAsync(out buffer, 1) == 0)
                return null;

            // Test the version
            if (!Message.TestVersion(buffer))
                return null;
            #endregion

            #region Length of the message
            // Reading length message
            if (_exit || ReadAsync(out buffer, Message.DataLen) == 0)
                return null;

            // Gets the length
            int length = Message.GetLength(buffer);
            if (length <= 0)
                return null;
            #endregion

            #region Reading message
            // Reading current message
            if (_exit || ReadAsync(out buffer, length) == 0 || buffer.Length < length)
                return null;

            // Return the message
            return new Message(buffer);
            #endregion
        }
        #endregion

        #region Private methods used for the comunication
        /// <summary>
        /// Asynchronous read method.
        /// Waits until receipt of the expected number of bytes, or until the last reading has gotten no bytes.
        /// </summary>
        /// <param name="buffer">Container of bytes received</param>
        /// <param name="lenReading">Expected number of bytes</param>
        /// <returns>
        /// Number of bytes received.
        /// If the latest reading didn't get bytes, returns 0.
        /// </returns>
        private int ReadAsync(out byte[] buffer, int lenReading)
        {
            // questo oggetto serve per la lettura asincrona degli stream di rete
            IAsyncResult asyncResult;

            AsyncReadState readState = new AsyncReadState
            {
                LenRead = 0,
                LastRead = 0,
            };

            // crea un buffer grande a sufficenza per i dati che ci interessa leggere
            buffer = new byte[lenReading];

            while (readState.LenRead < lenReading)
            {
                _mre.Reset();

                // inizia la lettura asincrona
                asyncResult = _socket.BeginReceive(buffer, readState.LenRead, lenReading - readState.LenRead, SocketFlags.None, new AsyncCallback(EndReadAsync), readState);

                // attende il completamento della lettura
                _mre.WaitOne();

                // Se la lettura è stata interrotta, oppure se ritorna 0 vuol dire che la connessione è stata chiusa, quindi esce dal ciclo
                if (_exit || readState.LastRead == 0)
                    break;
            }

            // restituisce il numero totale di byte letti, oppure restituisce 0
            // per indicare che la connessione è stata chiusa
            return (readState.LastRead == 0 ? readState.LastRead : readState.LenRead);
        }

        /// <summary>
        /// Method that receives asynchronous communications.
        /// </summary>
        /// <param name="asyncResult">The result of the asynchronous operation.</param>
        private void EndReadAsync(IAsyncResult asyncResult)
        {
            AsyncReadState readState = null;

            try
            {
                // prendiamo un puntatore allo stream di rete
                readState = (AsyncReadState)asyncResult.AsyncState;

                // eseguiamo la lettura
                readState.LastRead = _socket.EndReceive(asyncResult);
                readState.LenRead += readState.LastRead;
                if (!_socket.Connected)
                {
                    _exit = true;
                    _socket.Close();
                }
            }
            catch (ObjectDisposedException /*ex*/)
            {
                // the socket in invalid
                _exit = true;
            }
            catch (SocketException /*ex*/)
            {
                // the socket in invalid
                _exit = true;
            }
            catch (Exception /*ex*/)
            {
                // the socket in invalid
                _exit = true;
            }
            finally
            {
                _mre.Set();
            }
        }
        #endregion

        #region IDisposable interface implementation
        public void Dispose()
        {
            if (_socket != null && _socket.Connected)
                _socket.Close();

            _socket = null;
        }
        #endregion

        #region Method to terminate the comunication
        internal void End()
        {
            _exit = true;
            _mre.Set();
        }
        #endregion
    }
}
