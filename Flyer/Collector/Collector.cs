using Flyer.Errors;
using Flyer.Structures;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Flyer
{
    #region Delegates and events arguments
    public delegate void ConnectedEventHandler(ConnectedEventArgs e);
    public delegate void TerminatedEventHandler(TerminatedEventArgs e);
    public delegate void NotifyEventHandler(NotifyEventArgs e);

    public class ConnectedEventArgs
    {
        public ConnectedEventArgs(bool result) { Result = result; }
        public bool Result { get; private set; }
    }

    public class TerminatedEventArgs
    {
        public TerminatedEventArgs() { }
    }

    public class NotifyEventArgs
    {
        public NotifyEventArgs(byte[] info) { Info = info; }
        public byte[] Info { get; private set; }
    }
    #endregion

    public class Collector
    {
        #region Events for notifications
        public event NotifyEventHandler Notify;
        #endregion

        #region Private members for synchronous method
        private bool _inProcess = false;

        private readonly IMember _member;

        private Thread _connection;
        private Comunication _comunication;
        #endregion

        #region Constructors
        public Collector(ushort family, ushort application, ushort module, ushort functionality)
        {
            _member = Member.Create(family, application, module, functionality);

            _connection = null;
        }
        public Collector(IMember member)
        {
            _member = member;

            _connection = null;
        }
        #endregion

        #region Static methods
        public static Collector CreateAndConnectAsync(string hostName, ushort family, ushort application, ushort module, ushort functionality,
                                                      ConnectedEventHandler connectedEvent,
                                                      TerminatedEventHandler terminatedEvent,
                                                      NotifyEventHandler notifyEvent)
        {
            Collector result = new Collector(Member.Create(family, application, module, functionality))
            {
                _connection = null
            };
            result.Connected += connectedEvent;
            result.Terminated += terminatedEvent;
            result.Notify += notifyEvent;

            result.ConnectAsync(hostName, 3000);

            return result;
        }
        #endregion

        #region Events for asyncronous
        public event ConnectedEventHandler Connected;
        #endregion
        #region Events for syncronous and asyncronous
        public event TerminatedEventHandler Terminated;
        #endregion

        #region Public properties
        /// <summary>
        /// Timeout if for milliseconds is specified -1 or 0 after receiving the last endpoint.
        /// </summary>
        public int Timeout { get; set; } = 10000;
        #endregion

        #region Synchronous method
        /// <summary>
        /// Connect to server on the local network synchronously.
        /// </summary>
        /// <param name="hostName"> The name of the host./// </param>
        /// <param name="milliseconds">
        /// Milliseconds to wait for the answer.
        /// <para>For more information, see the note in the remarks.</para>
        /// </param>
        /// <returns>
        /// True if the connection was successful, otherwise false.
        /// </returns>
        /// <remarks>
        /// The milliseconds parameter can take the following values:
        /// <para>If millisecond &lt; 0 the wait is the Timeout value.</para>
        /// <para>If millisecond = 0 the wait is the Timeout value.</para>
        /// <para>If millisecond &gt; 0 Try to connect waiting until the specified time.</para>
        /// </remarks>
        public bool Connect(string hostName, int milliseconds)
        {
            // To avoid multiple call
            if (_inProcess)
                return false;

            _inProcess = true;

            try
            {
                return InnerConnect(hostName, milliseconds);
            }
            finally
            {
                _inProcess = false;
            }
        }
        #endregion

        #region Asynchronous method
        #region Private members for asynchronous method
        private Thread _thread = null;
        #endregion

        /// <summary>
        /// Connect to server on the local network asynchronously. The result is returned by the Connected event. 
        /// </summary>
        /// <param name="hostName"> The name of the host./// </param>
        /// <param name="milliseconds">
        /// Milliseconds to wait for the answer.
        /// <para>For more information, see the note in the remarks.</para>
        /// </param>
        /// <remarks>
        /// The milliseconds parameter can take the following values:
        /// <para>If millisecond &lt; 0 the wait is the Timeout value.</para>
        /// <para>If millisecond = 0 the wait is the Timeout value.</para>
        /// <para>If millisecond &gt; 0 Try to connect waiting until the specified time.</para>
        /// </remarks>
        public void ConnectAsync(string hostName, int milliseconds)
        {
            // To avoid multiple call
            if (_inProcess)
                return;

            _thread = new Thread(StartConnect)
            {
                IsBackground = true
            };
            _thread.Start(new Tuple<string, int>(hostName, milliseconds));
        }

        private void StartConnect(object param)
        {
            // To avoid multiple call
            if (_inProcess)
                return;

            try
            {
                _inProcess = true;
                Tuple<string, int> info = (Tuple<string, int>)param;
                bool result = InnerConnect(info.Item1, info.Item2);
                Connected?.Invoke(new ConnectedEventArgs(result));
            }
            finally
            {
                _inProcess = false;
                _thread = null;
            }

            return;
        }
        #endregion

        #region Connection method
        private bool InnerConnect(string hostName, int milliseconds)
        {
            // Create a new connection
            Socket tcp = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream,
                                    ProtocolType.Tcp);
            try
            {
                // Tries to connect to the server
                tcp.Connect(hostName, 5315);
            }
            catch (SocketException ex)
            {
                throw new CollectorException("Error in Collector Connect, for more information see the InnerException.", ex);
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Connect, for more information see the InnerException.", ex);
            }

            // Try to communicate to open a direct connection
            try
            {
                // New socket for direct communication with the new point that requires connection
                _comunication = new Comunication(tcp);

                _comunication.SendCommand(new CommandHello(_member));

                // Read acknowledgment message
                Message msg = _comunication.ReceiveMessage();
                // The only command awaited is Hello containing information of recognition to add it to the list of active socket.
                if ((msg?.Command ?? Message.CommandList.Unknown) == Message.CommandList.Ready)
                {
                    _connection = new Thread(ConnectionThread)
                    {
                        IsBackground = true
                    };
                    _connection.Start();
                }
                else
                {
                    throw new CollectorException("Unable to communicate with the server.");
                }
            }
            catch (Exception ex)
            {
                _comunication.Dispose();
                _comunication = null;

                throw new CollectorException("Error in Collector, for more information see the InnerException.", ex);
            }

            return true;
        }
        #endregion

        #region Connect methods
        private void ConnectionThread()
        {
            try
            {
                while (true)
                {
                    Message msg = _comunication.ReceiveMessage();
                    if (msg == null)
                        break;

                    switch (msg.Command)
                    {
                        case Message.CommandList.Hello:
                        case Message.CommandList.Ready:
                        case Message.CommandList.RegisterNotify:
                        case Message.CommandList.DeregisterNotify:
                            // Ignore
                            break;
                        case Message.CommandList.Notify:
                            {
                                CommandNotify notify = new CommandNotify(msg);

                                // Raise event
                                if (Notify != null)
                                {
                                    byte[] info = new byte[notify.Info.Length];
                                    notify.Info.CopyTo(info, 0);

                                    Notify(new NotifyEventArgs(info));
                                }
                            }
                            break;
                        case Message.CommandList.Unknown:
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector, for more information see the InnerException.", ex);
            }
            finally
            {
                _comunication.Dispose();
                _comunication = null;

                Terminated?.Invoke(new TerminatedEventArgs());

                _connection = null;
            }
        }

        public void Disconnect()
        {
            if (_comunication != null)
            {
                _comunication.End();
            }
        }

        public void Send(byte[] message, bool self = false)
        {
            try
            {
                _comunication.SendCommand(new CommandNotify(message, self));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Send, for more information see the InnerException.", ex);
            }
        }
        public void Send(byte[] message, ushort family, ushort application = 0, ushort module = 0, ushort functionality = 0, bool strict = true, bool self = false)
        {
            try
            {
                _comunication.SendCommand(new CommandNotifyToGroup(message, Subscription.Create(family, application, module, functionality), strict, self));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Send, for more information see the InnerException.", ex);
            }
        }
        public void Send(byte[] message, string id)
        {
            try
            {
                _comunication.SendCommand(new CommandNotifyToId(message, id));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Send, for more information see the InnerException.", ex);
            }
        }

        public void Register(ushort family, ushort application = 0, ushort module = 0, ushort functionality = 0, bool strict = true)
        {
            try
            {
                _comunication.SendCommand(new CommandRegisterNotify(Subscription.Create(family, application, module, functionality), strict));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Register, for more information see the InnerException.", ex);
            }
        }
        public void Register(ISubscription subscription, bool strict = true)
        {
            try
            {
                _comunication.SendCommand(new CommandRegisterNotify(subscription, strict));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Register, for more information see the InnerException.", ex);
            }
        }
        public void Register(IEnumerable<ISubscription> subscriptions, bool strict = true)
        {
            try
            {
                _comunication.SendCommand(new CommandRegisterNotify(subscriptions, strict));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Register, for more information see the InnerException.", ex);
            }
        }

        public void Deregister(ushort family, ushort application = 0, ushort module = 0, ushort functionality = 0)
        {
            try
            {
                _comunication.SendCommand(new CommandDeregisterNotify(Subscription.Create(family, application, module, functionality)));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Deregister, for more information see the InnerException.", ex);
            }
        }
        public void Deregister(ISubscription subscription)
        {
            try
            {
                _comunication.SendCommand(new CommandDeregisterNotify(subscription));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Deregister, for more information see the InnerException.", ex);
            }
        }
        public void Deregister(IEnumerable<ISubscription> subscriptions)
        {
            try
            {
                _comunication.SendCommand(new CommandDeregisterNotify(subscriptions));
            }
            catch (Exception ex)
            {
                throw new CollectorException("Error in Collector Deregister, for more information see the InnerException.", ex);
            }
        }
        #endregion
    }
}
