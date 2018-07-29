using Flyer.Errors;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Flyer
{
    #region Delegates and events arguments
    public delegate void TerminateDiscoveryEventHandler(TerminateDiscoveryEventArgs e);
    public delegate void FoundDiscoveryEventHandler(FoundDiscoveryEventArgs e);

    public class TerminateDiscoveryEventArgs
    {
        public TerminateDiscoveryEventArgs(IEnumerable<IPEndPoint> endPoints) { EndPoints = endPoints; }
        public IEnumerable<IPEndPoint> EndPoints { get; private set; }
    }

    public class FoundDiscoveryEventArgs
    {
        public FoundDiscoveryEventArgs(IPEndPoint endPoint) { EndPoint = endPoint; }
        public IPEndPoint EndPoint { get; private set; }
    }
    #endregion

    public class Discovery
    {
        #region Private members for synchronous method
        private ManualResetEvent _mre;

        private List<IPEndPoint> _endPoints;

        private bool _isClosed = false;

        private bool _inProcess = false;
        #endregion

        #region Events for asyncronous
        public event TerminateDiscoveryEventHandler Terminate;
        public event FoundDiscoveryEventHandler Found;
        #endregion

        #region Public properties
        /// <summary>
        /// Timeout if for milliseconds is specified -1 or 0 after receiving the last endpoint.
        /// </summary>
        public int Timeout { get; set; } = 10000;
        #endregion

        #region Synchronous method
        /// <summary>
        /// Search the servers on the local network synchronously.
        /// </summary>
        /// <param name="milliseconds">
        /// Milliseconds to wait for the answer.
        /// <para>For more information, see the note in the remarks.</para>
        /// </param>
        /// <returns>
        /// List of IpEndPoin found.
        /// </returns>
        /// <remarks>
        /// The milliseconds parameter can take the following values:
        /// <para>If millisecond &lt; 0 the wait is the Timeout value.</para>
        /// <para>If millisecond = 0 is returned the first endpoint found (Timeout wait if no servers respond).</para>
        /// <para>If millisecond &gt; 0 are returned all endpoints that responded in the specified time.</para>
        /// </remarks>
        public IEnumerable<IPEndPoint> Search(int milliseconds = 0)
        {
            // To avoid multiple call
            if (_inProcess)
                return new List<IPEndPoint>();

            try
            {
                _inProcess = true;
                return InnerSearch(milliseconds);
            }
            catch (Exception ex)
            {
                throw new DiscoveryException("Error in Discovery Search, for more information see the InnerException.", ex);
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
        /// Search the servers on the local network asynchronously. The result is returned by the Terminate event. 
        /// </summary>
        /// <param name="milliseconds">
        /// Milliseconds to wait for the answer.
        /// <para>For more information, see the note in the remarks.</para>
        /// </param>
        /// <returns>
        /// List of IpEndPoin found.
        /// </returns>
        /// <remarks>
        /// The milliseconds parameter can take the following values:
        /// <para>If millisecond &lt; 0 the wait is the Timeout value.</para>
        /// <para>If millisecond = 0 is returned the first endpoint found (Timeout wait if no servers respond).</para>
        /// <para>If millisecond &gt; 0 are returned all endpoints that responded in the specified time.</para>
        /// </remarks>
        public void SearchAsync(int milliseconds = 0)
        {
            // To avoid multiple call
            if (_inProcess)
                return;

            _thread = new Thread(StartSearch)
            {
                IsBackground = true
            };
            _thread.Start(milliseconds);
        }

        private void StartSearch(object times)
        {
            // To avoid multiple call
            if (_inProcess)
                return;

            try
            {
                _inProcess = true;
                IEnumerable<IPEndPoint> result = InnerSearch((int)times);
                Terminate?.Invoke(new TerminateDiscoveryEventArgs(result));
            }
            catch (Exception ex)
            {
                throw new DiscoveryException("Error in asynchronous Discovery Search, for more information see the InnerException.", ex);
            }
            finally
            {
                _inProcess = false;
                _thread = null;
            }

            return;
        }
        #endregion

        #region Search method
        private IEnumerable<IPEndPoint> InnerSearch(int milliseconds)
        {
            try
            {
                _endPoints = new List<IPEndPoint>();

                _mre = new ManualResetEvent(false);

                using (UdpClient udp = new UdpClient())
                {
                    _isClosed = false;

                    //socket.Connect(new IPEndPoint(IPAddress.Broadcast, 5315));
                    byte[] sending = Encoding.ASCII.GetBytes("I search Storky");
                    udp.Send(sending, sending.Length, new IPEndPoint(IPAddress.Broadcast, 5315));

                    DateTime start = DateTime.Now;
                    do
                    {
                        _mre.Reset();
                        IAsyncResult resultAsync = udp.BeginReceive(new AsyncCallback(ReceiveAsync), udp);

                        if (milliseconds < 0)
                            _mre.WaitOne((Timeout > 0 ? Timeout : 1));      // To avoid infinite timeout
                        else if (milliseconds == 0)
                            _mre.WaitOne((Timeout > 0 ? Timeout : 1));      // To avoid infinite timeout
                        else
                            _mre.WaitOne(milliseconds - (int)(DateTime.Now - start).TotalMilliseconds);
                    } while (milliseconds < 0 || (milliseconds != 0 && (DateTime.Now - start).TotalMilliseconds < milliseconds));

                    _isClosed = true;
                }
            }
            catch (Exception ex)
            {
                throw new DiscoveryException("Error in internal asynchronous Discovery Search, for more information see the InnerException.", ex);
            }

            return _endPoints;
        }

        private void ReceiveAsync(IAsyncResult asyncResult)
        {
            try
            {
                if (_isClosed)
                    return;

                UdpClient udp = (UdpClient)(asyncResult.AsyncState);

                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                // Read acknowledgment message
                byte[] buffer = udp.EndReceive(asyncResult, ref remote);

                if (Encoding.ASCII.GetString(buffer) == "I am Storky, you have discovered me")
                {
                    _endPoints.Add(remote);
                    if (_thread != null)
                        Found?.Invoke(new FoundDiscoveryEventArgs(remote));
                }
            }
            catch (Exception ex)
            {
                throw new DiscoveryException("Error in receive asynchronous Discovery Search, for more information see the InnerException.", ex);
            }
            finally
            {
                _mre.Set();
            }
        }
        #endregion
    }
}
