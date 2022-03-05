using System.ServiceProcess;
using System.Threading;

namespace AutoCount
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new AutoCountService()
            };

#if DEBUG
            AutoCountService myService = new AutoCountService();
            myService.OnDebug();
            Thread.Sleep(Timeout.Infinite);
#else

            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
