using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Storky
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
#if !DEBUG && !SERVICE_DEBUG
                throw new Exception("Release version required windows service mode. Please set 'Windows Application' into 'Output type' setting");
#endif
#if SERVICE_DEBUG
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new StorkyService()
                };
                ServiceBase.Run(ServicesToRun);
#else
                StorkyService service1 = new StorkyService();
                service1.TestStartupAndStop();
#endif
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new StorkyService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
