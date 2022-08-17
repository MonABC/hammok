using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hammock.AssetView.Platinum.Tools
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.serviceInstaller.ServiceName = Assembly.GetExecutingAssembly().GetName().Name;
            this.serviceInstaller.DisplayName = "AssetView Autoupdate Service";
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
        }
    }
}