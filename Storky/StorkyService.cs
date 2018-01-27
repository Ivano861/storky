using System;
using System.ServiceProcess;
using System.Threading;

namespace Storky
{
    public partial class StorkyService : ServiceBase
    {
        private Thread _threadDiscovery;
        private Thread _threadHandshake;

        public StorkyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _threadDiscovery = new Thread(Discovery.Start);
            _threadDiscovery.Start();

            _threadHandshake = new Thread(Handshake.Start);
            _threadHandshake.Start();
        }

        protected override void OnStop()
        {
            Discovery.Stop();
            Handshake.Stop();
        }

        // Debug start
        internal void TestStartupAndStop()
        {
            OnStart(new string[0]);
            Console.ReadLine();
            OnStop();
        }
    }
}
