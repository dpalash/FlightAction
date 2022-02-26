using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FlightAction.Core;
using FlightAction.ExceptionHandling;

namespace AutoCount
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

            DomainExceptionHandler.HandleDomainExceptions();

            InitialSetup.GlobalConfigurationSetup();

            var unityContainer = InitialSetup.ConfigureUnityContainer();

            //INFO: Don't move this method from here.
            InitialSetup.ConfigureFlurlHttpClient(unityContainer);
        }

        protected override void OnStart(string[] args)
        {
            DomainExceptionHandler.HandleDomainExceptions();

            InitialSetup.GlobalConfigurationSetup();

            var unityContainer = InitialSetup.ConfigureUnityContainer();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            //INFO: Don't move this method from here.
            InitialSetup.ConfigureFlurlHttpClient(unityContainer);
        }

        protected override void OnStop()
        {
        }
    }
}
