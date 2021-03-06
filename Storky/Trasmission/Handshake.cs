﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Storky.LogManage;

namespace Storky
{
    internal static class Handshake
    {
        #region Private constants
        private const string MessageHeader = "STORKY_FROM_FLYER";

        private const int backlog = 100;
        private const int DefaultPort = 5315;
        #endregion

        #region Private members
        private static int portConnect = DefaultPort;

        private static Couplings _couplings = null;
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
            Log.Instance.Write("Handshake - Start service.", EventLogEntryType.Information);

            portConnect = DefaultPort;

            _mre = new ManualResetEvent(false);

            try
            {
                // Creation of the object responsible for containing the connections made
                _couplings = new Couplings();

                // Create the socket
                using (Socket listenTCP = new Socket(AddressFamily.InterNetwork,
                                                     SocketType.Stream,
                                                     ProtocolType.Tcp))
                {
                    // Binds this socket to the specified port on any IP address
                    listenTCP.Bind(new IPEndPoint(new IPAddress(0), portConnect));

                    // Start your listening with a maximum of connections queue specified by backlog
                    // putting the socket in a listening state
                    listenTCP.Listen(backlog);

                    // Infinite loop.
                    // To stop everything we think the carrier the caller by throwing an exception specific to the end of the thread
                    while (!_exit)
                    {
                        _mre.Reset();

                        IAsyncResult resultAsync = listenTCP.BeginAccept(new AsyncCallback(AcceptAsync), listenTCP);

                        // Waits for a connection
                        _mre.WaitOne();
                    }
                }

                Log.Instance.Write("Handshake - End service.", EventLogEntryType.Information);
            }
            catch (ThreadAbortException ex)
            {
#if DEBUG
                Log.Instance.Write("Handshake - End connection: thread aborted." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Handshake - End connection: thread aborted." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
                // Does nothing and leaves the cleaning in the finally section
                _exit = true;
            }
            catch (Exception ex)
            {
                // Catch all errors
#if DEBUG
                Log.Instance.Write("Handshake - End connection: generic exception." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Handshake - End connection: generic exception." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
                // Does nothing and leaves the cleaning in the finally section
                _exit = true;
            }
            finally
            {
                // Try to close all active connections in an orderly manner
                if (_couplings != null)
                {
                    // It cleans up after itself from the list of modules
                    while (_couplings.Count > 0)
                        _couplings[0].CloseThread();

                    _couplings.Clear();
                    _couplings = null;
                }
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
        private static void AcceptAsync(IAsyncResult asyncResult)
        {
            if (_exit)
                return;

            try
            {
                // New socket for direct communication with the new point that requires connection
                Comunication comunication = new Comunication(((Socket)asyncResult.AsyncState).EndAccept(asyncResult));

                // Read acknowledgment message
                Message msg = comunication.ReceiveMessage();
                // The only command awaited is Hello containing information of recognition to add it to the list of active socket.
                if ((msg?.Command ?? Message.CommandList.Unknown) == Message.CommandList.Hello)
                {
                    _couplings.Add(new Coupling(comunication, new CommandHello(msg)));
                    comunication.SendCommand(new CommandReady());
                }
            }
            catch (ObjectDisposedException ex)
            {
#if DEBUG
                Log.Instance.Write("Handshake - Object disposed." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Handshake - Object disposed." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
            }
            catch (SocketException ex)
            {
#if DEBUG
                Log.Instance.Write("Handshake - Socket exception." + Environment.NewLine +
                                   "Native code: (" + ex.NativeErrorCode.ToString() + ")" + Environment.NewLine +
                                   ex.Message + Environment.NewLine +
                                   ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Handshake - Socket exception." + Environment.NewLine +
                                   "Native code: (" + ex.NativeErrorCode.ToString() + ")" + Environment.NewLine +
                                   ex.Message, EventLogEntryType.Error);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Log.Instance.Write("Handshake - General exception." + Environment.NewLine +
                                    ex.Message + Environment.NewLine +
                                    ex.StackTrace, EventLogEntryType.Error);
#else
                Log.Instance.Write("Handshake - General exception." + Environment.NewLine + ex.Message, EventLogEntryType.Error);
#endif
            }
            finally
            {
                _mre.Set();
            }
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Allows you to delete a connection element from list
        /// </summary>
        /// <param name="item"></param>
        internal static void RemoveCoupling(Coupling item)
        {
            if (_couplings != null)
                _couplings.Remove(item);

            item.Dispose();
        }
        #endregion

        #region Internal methods to send
        internal static void SendNotify(CommandNotify notify, Coupling sender)
        {
            foreach (Coupling coupling in _couplings)
            {
                foreach (Registration registration in coupling.Registrations)
                {
                    if ((registration.Subscription.Family == sender.Member.Subscription.Family) &&
                        (registration.Subscription.Application == sender.Member.Subscription.Application || (registration.Subscription.Application == 0 && !registration.Strict) ) &&
                        (registration.Subscription.Module == sender.Member.Subscription.Module || (registration.Subscription.Module == 0 && !registration.Strict)) &&
                        (registration.Subscription.Functionality == sender.Member.Subscription.Functionality || (registration.Subscription.Functionality == 0 && !registration.Strict)) &&
                        (coupling.Member.Id != sender.Member.Id || notify.Self))
                    {
                        coupling.SendNotify(notify);
                    }
                }
            }
        }
        internal static void SendNotify(CommandNotifyToGroup notifyToGroup, Coupling sender)
        {
            foreach (Coupling coupling in _couplings)
            {
                foreach (Registration registration in coupling.Registrations)
                {
                    if ((registration.Subscription.Family == notifyToGroup.Subscription.Family) &&
                        (registration.Subscription.Application == notifyToGroup.Subscription.Application || (registration.Subscription.Application == 0 && !registration.Strict)) &&
                        (registration.Subscription.Module == notifyToGroup.Subscription.Module || (registration.Subscription.Module == 0 && !registration.Strict)) &&
                        (registration.Subscription.Functionality == notifyToGroup.Subscription.Functionality || (registration.Subscription.Functionality == 0 && !registration.Strict)) &&
                        (coupling.Member.Id != sender.Member.Id || notifyToGroup.Self))
                    {
                        coupling.SendNotify(new CommandNotify(notifyToGroup));
                    }
                }
            }
        }
        internal static void SendNotify(CommandNotifyToId notifyToId, Coupling sender)
        {
            foreach (Coupling coupling in _couplings)
            {
                if (coupling.Member.Id == notifyToId.Id)
                {
                    coupling.SendNotify(new CommandNotify(notifyToId));
                }
            }
        }
        #endregion
    }
}
