using Storky.LogManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Storky
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            foreach (Installer installer in serviceInstaller1.Installers)
            {
                EventLogInstaller eventLogInstaller = installer as EventLogInstaller;
                if (eventLogInstaller != null)
                {
                    eventLogInstaller.Log = Log.LogName;
                    eventLogInstaller.Source = Log.Source;
                    eventLogInstaller.UninstallAction = UninstallAction.NoAction;
                    break;
                }
            }

        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            Log.Instance.Write("Storky service installed");
        }
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            Log.Instance.Write("Storky service uninstalled");
        }
    }
}
