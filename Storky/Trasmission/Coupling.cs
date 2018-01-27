using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Storky
{
    internal class Coupling : IDisposable
    {
        #region Private members
        private bool _exit;
        private Comunication _comunication;
        private ushort _family;
        private ushort _application;
        private ushort _module;
        private ushort _functionality;
        private string _id;

        private IList<Registration> _registrations;

        private Thread _threadSocket;
        #endregion

        #region Constructors
        public Coupling(Comunication comunication, CommandHello hello)
        {
            _exit = false;
            _comunication = comunication;
            _family = hello.Family;
            _application = hello.Application;
            _module = hello.Module;
            _functionality = hello.Functionality;
            _id = hello.Id;

            _registrations = new List<Registration>
            {
                new Registration(_family, _application, _module, _functionality, false)
            };

            // We create the thread for comunication with remote
            _threadSocket = new Thread(CouplingThread);
            _threadSocket.Start();
        }
        #endregion

        #region Properties
        public ushort Family { get => _family; }
        public ushort Application { get => _application; }
        public ushort Module { get => _module; }
        public ushort Functionality { get => _functionality; }
        public string Id { get => _id; }
        public IList<Registration> Registrations { get => _registrations; }
        #endregion

        #region Working thread for comunication
        private void CouplingThread()
        {
            try
            {
                while (!_exit)
                {
                    try
                    {
                        Message msg = _comunication.ReceiveMessage();
                        switch (msg?.Command ?? Message.CommandList.Unknown)
                        {
                            case Message.CommandList.Hello:
                            case Message.CommandList.Ready:
                                break;
                            case Message.CommandList.RegisterNotify:
                                AddRegistration(new CommandRegisterNotify(msg));
                                break;
                            case Message.CommandList.DeregisterNotify:
                                RemoveRegistration(new CommandDeregisterNotify(msg));
                                break;
                            case Message.CommandList.Notify:
                                Handshake.SendNotify(new CommandNotify(msg), this);
                                break;
                            case Message.CommandList.NotifyToGroup:
                                Handshake.SendNotify(new CommandNotifyToGroup(msg), this);
                                break;
                            case Message.CommandList.NotifyToId:
                                Handshake.SendNotify(new CommandNotifyToId(msg), this);
                                break;
                            case Message.CommandList.Unknown:
                            default:
                                break;
                        }
                    }
                    catch (ThreadAbortException /*ex*/) { throw; }
                    catch (ObjectDisposedException /*ex*/) { throw; }
                    catch (SocketException ex)
                    {
                        switch (ex.NativeErrorCode)
                        {
                            case 10053: // WSAECONNABORTED: An established connection was aborted by the software in your host machine.
                            case 10054: // WSAECONNRESET: An existing connection was forcibly closed by the remote host.
                                break;
                            default:
                                break;
                        }
                        throw;
                    }
                    catch (Exception /*ex*/) { /*We try to listen */ }
                }
            }
            catch (ThreadAbortException /*ex*/) { }
            catch (ObjectDisposedException /*ex*/) { }
            catch (SocketException ex)
            {
                switch (ex.NativeErrorCode)
                {
                    case 10053: // WSAECONNABORTED: An established connection was aborted by the software in your host machine.
                    case 10054: // WSAECONNRESET: An existing connection was forcibly closed by the remote host.
                        break;
                    default:
                        break;
                }
            }
            catch (Exception /*ex*/) { }
            finally
            {
                // Remove object from container
                Handshake.RemoveCoupling(this);
            }
        }
        #endregion

        #region Private methods to register and unregister the communication with a node
        private void AddRegistration(CommandRegisterNotify registerNotify)
        {
            bool isPresent = false;
            foreach (Registration registration in _registrations)
            {
                if (registration.Family == registerNotify.Family &&
                    registration.Application == registerNotify.Application &&
                    registration.Module == registerNotify.Module &&
                    registration.Functionality == registerNotify.Functionality)
                {
                    if (registration.Strict != registerNotify.Strict)
                        _registrations.Remove(registration);
                    else
                        isPresent = true;
                    break;
                }
            }
            if (!isPresent)
            {
                _registrations.Add(new Registration(registerNotify.Family, registerNotify.Application, registerNotify.Module, registerNotify.Functionality, registerNotify.Strict));
            }
        }

        private void RemoveRegistration(CommandDeregisterNotify deregisterNotify)
        {
            foreach (Registration registration in _registrations)
            {
                if (registration.Family == deregisterNotify.Family &&
                    registration.Application == deregisterNotify.Application &&
                    registration.Module == deregisterNotify.Module &&
                    registration.Functionality == deregisterNotify.Functionality)
                {
                    _registrations.Remove(registration);
                    break;
                }
            }
        }
        #endregion

        #region Internal method for send commands
        internal void SendNotify(CommandNotify commandNotify)
        {
            _comunication.SendCommand(commandNotify);
        }
        #endregion

        #region Closing the thread
        internal void CloseThread()
        {
            _exit = true;
            _comunication.End();
            _threadSocket.Join();
        }
        #endregion

        #region IDisposable interface implementation
        public void Dispose()
        {
            ((IDisposable)_comunication).Dispose();
        }
        #endregion
    }
}
