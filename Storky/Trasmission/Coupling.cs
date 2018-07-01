using Storky.Structures;
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
        private Thread _threadSocket;
        #endregion

        #region Constructors
        public Coupling(Comunication comunication, CommandHello hello)
        {
            _exit = false;
            _comunication = comunication;
            Member = new Member(hello.Member.Id,
                                 hello.Member.Subscription.Family,
                                 hello.Member.Subscription.Application,
                                 hello.Member.Subscription.Module,
                                 hello.Member.Subscription.Functionality);

            Registrations = new List<Registration>
            {
                new Registration(Member.Subscription, false)
            };

            // We create the thread for comunication with remote
            _threadSocket = new Thread(CouplingThread);
            _threadSocket.Start();
        }
        #endregion

        #region Properties
        public IMember Member { get; }
        public IList<Registration> Registrations { get; }
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
                    catch (SocketException  /*ex*/) { throw; }
                    catch (Exception /*ex*/) { /*We try to listen */ }
                }
            }
            catch (ThreadAbortException /*ex*/) { }
            catch (ObjectDisposedException /*ex*/) { }
            catch (SocketException /*ex*/) { }
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
            for (int i = 0; i < registerNotify.Subscriptions.Count; i++)
            {
                foreach (Registration registration in Registrations)
                {
                    if (registration.Subscription.Family == registerNotify.Subscriptions[i].Family &&
                        registration.Subscription.Application == registerNotify.Subscriptions[i].Application &&
                        registration.Subscription.Module == registerNotify.Subscriptions[i].Module &&
                        registration.Subscription.Functionality == registerNotify.Subscriptions[i].Functionality)
                    {
                        if (registration.Strict != registerNotify.Strict)
                            Registrations.Remove(registration);
                        else
                            isPresent = true;
                        break;
                    }
                }
                if (!isPresent)
                {
                    Registrations.Add(new Registration(registerNotify.Subscriptions[i].Family,
                                                       registerNotify.Subscriptions[i].Application,
                                                       registerNotify.Subscriptions[i].Module,
                                                       registerNotify.Subscriptions[i].Functionality, registerNotify.Strict));
                }
            }
        }

        private void RemoveRegistration(CommandDeregisterNotify deregisterNotify)
        {
            for (int i = 0; i < deregisterNotify.Subscriptions.Count; i++)
            {
                foreach (Registration registration in Registrations)
                {
                    if (registration.Subscription.Family == deregisterNotify.Subscriptions[i].Family &&
                        registration.Subscription.Application == deregisterNotify.Subscriptions[i].Application &&
                        registration.Subscription.Module == deregisterNotify.Subscriptions[i].Module &&
                        registration.Subscription.Functionality == deregisterNotify.Subscriptions[i].Functionality)
                    {
                        Registrations.Remove(registration);
                        break;
                    }
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
