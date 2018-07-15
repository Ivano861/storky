using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Storky.LogManage;

namespace Storky
{
    internal static class Discovery
    {
        #region Private constant
        private const string MessageHeader = "I search Storky";
        private const int DefaultPort = 5315;
        #endregion

        #region Private members
        private static int portConnect = DefaultPort;

        private static ManualResetEvent _mre;

        internal static bool _exit = false;
        #endregion

        #region Startup method
        /// <summary>
        /// Input method for thread management
        /// </summary>
        public static void Start()
        {
#if SERVICE_DEBUG
            System.Threading.Thread.Sleep(20000);
#endif
            Log.Instance.Write("Discovery - Start service.", EventLogEntryType.Information);

            portConnect = DefaultPort;

            _mre = new ManualResetEvent(false);

            try
            {
                // Create the socket
                using (UdpClient listenUDP = new UdpClient(portConnect))
                {
                    // Infinite loop.
                    // To stop everything we think the carrier the caller by throwing an exception specific to the end of the thread
                    while (!_exit)
                    {
                        _mre.Reset();

                        IAsyncResult resultAsync = listenUDP.BeginReceive(new AsyncCallback(ReceiveAsync), listenUDP);

                        // Waits for a connection
                        _mre.WaitOne();
                    }
                }

                Log.Instance.Write("Discovery - End service.", EventLogEntryType.Information);
            }
            catch (ThreadAbortException ex)
            {
#if DEBUG
                Log.Instance.Write("Discovery - End service: thread aborted." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Discovery - End service: thread aborted." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
                // Does nothing
                _exit = true;
            }
            catch (Exception ex)
            {
                // Catch all errors
#if DEBUG
                Log.Instance.Write("Discovery - End service: general exception." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Discovery - End service: general exception." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
                // Does nothing
                _exit = true;
            }
        }
#endregion

#region Closing method
        internal static void Stop()
        {
            _exit = true;
            _mre.Set();
        }
#endregion

#region Method of receiving events regarding the application or receipt
        /// <summary>
        /// Method called when the socket is awakened by a connection request.
        /// </summary>
        /// <param name="asyncResult">Contains the status of the asynchronous operation: AsyncState must contain socket information source.</param>
        private static void ReceiveAsync(IAsyncResult asyncResult)
        {
            if (_exit)
                return;

            try
            {
                UdpClient udp = (UdpClient)(asyncResult.AsyncState);

                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

                // Read acknowledgment message
                byte[] buffer = udp.EndReceive(asyncResult, ref remote);

                // Check the correctness of information received
                if (buffer.Length != 15)
                    return;
                if (Encoding.ASCII.GetString(buffer) != MessageHeader)
                    return;

                byte[] response = Encoding.ASCII.GetBytes("I am Storky, you have discovered me");
                udp.Send(response, response.Length, remote);
            }
            catch (ObjectDisposedException ex)
            {
#if DEBUG
                Log.Instance.Write("Discovery - Object disposed." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Discovery - Object disposed." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
            }
            catch (SocketException ex)
            {
#if DEBUG
                Log.Instance.Write("Discovery - Socket exception." + Environment.NewLine +
                                   "Native code: (" + ex.NativeErrorCode.ToString() + ")" + Environment.NewLine +
                                   ex.Message + Environment.NewLine +
                                   ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Discovery - Socket exception." + Environment.NewLine +
                                   "Native code: (" + ex.NativeErrorCode.ToString() + ")" + Environment.NewLine +
                                   ex.Message, EventLogEntryType.Error);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Log.Instance.Write("Discovery - General exception." + Environment.NewLine +
                                   ex.Message + Environment.NewLine +
                                   ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Discovery - General exception." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
            }
            finally
            {
                _mre.Set();
            }
        }
#endregion
    }
}
