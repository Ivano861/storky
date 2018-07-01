using Flyer;
using Flyer.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace TestFlayer
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<IPEndPoint> _ipEndPoints;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Discovery_Click(object sender, RoutedEventArgs e)
        {
            _ipEndPoints = null;
            Discovery discovery = new Discovery();

            //_ipEndPoints = discovery.Search(3000);
            //listIP.ItemsSource = _ipEndPoints;

            discovery.Terminate += Discovery_Terminate;
            discovery.Found += Discovery_Found;
            discovery.SearchAsync(3330);

            //System.Threading.Thread.Sleep(20000);
            //discovery.SearchAsync(3330);
        }

        private void Discovery_Found(FoundDiscoveryEventArgs e)
        {
            IPEndPoint ep = e.EndPoint;
        }

        private void Discovery_Terminate(TerminateDiscoveryEventArgs e)
        {
            _ipEndPoints = e.EndPoints;

            if (listIP.Dispatcher.CheckAccess())
                listIP.ItemsSource = _ipEndPoints;
            else
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    listIP.ItemsSource = _ipEndPoints;
                }));
        }

        private Collector _collector;
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (_ipEndPoints == null)
            {
                MessageBox.Show("Search first with discovery");
                return;
            }

            IPAddress ip = (from x in _ipEndPoints select x.Address).FirstOrDefault();

            if (ip == null)
            {
                MessageBox.Show("IP not found");
                return;
            }

            _collector = new Collector(1, 1, 1, 1);
            _collector.Connected += Collector_Connected;
            _collector.Terminated += Collector_Terminated;
            _collector.Notify += Collector_Notify;

            /*bool b = */
            _collector.ConnectAsync(ip.ToString(), 3000);
        }

        private void Collector_Connected(ConnectedEventArgs e)
        {
            if (e.Result)
            {
                _collector.Register(Subscription.Create(1,2,3,4));
                _collector.Send(new byte[] { 1, 2 }, self: true);
                _collector.Send(new byte[] { 3, 4 }, 1, strict: false, self: true);
            }
        }

        private void Collector_Notify(NotifyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Collector_Terminated(TerminatedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            _collector.Register(2);
            _collector.Register(3, 4);
        }

        private void Deregister_Click(object sender, RoutedEventArgs e)
        {
            _collector.Deregister(2);
        }

        private void MultiRegister_Click(object sender, RoutedEventArgs e)
        {
            ISubscription[] subscriptions = new ISubscription[2];

            subscriptions[0] = Subscription.Create(4);
            subscriptions[1] = Subscription.Create(5, 6);
            _collector.Register(subscriptions);
        }

        private void MultiDeregister_Click(object sender, RoutedEventArgs e)
        {
            ISubscription[] subscriptions = new ISubscription[2];

            subscriptions[0] = Subscription.Create(4);
            subscriptions[1] = Subscription.Create(5, 6);
            _collector.Deregister(subscriptions);
        }
    }
}
