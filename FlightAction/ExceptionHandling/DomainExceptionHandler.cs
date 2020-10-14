﻿using System;
using System.Threading.Tasks;
using Framework.Extensions;
using Framework.IoC;
using Serilog;
using Unity;

namespace FlightAction.ExceptionHandling
{
    /// <summary>
    /// Unhandled event registrar.
    /// Registers and handles events from domain, tasks and dispatchers.
    /// </summary>
    public static class DomainExceptionHandler
    {
        public static void HandleDomainExceptions()
        {
            UnHandleDomainExceptions();

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledDomainException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private static void UnHandleDomainExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledDomainException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        }

        private static void OnUnhandledDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject as Exception);
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Observed)
                return;

            e.SetObserved();
            LogAggregatedException(e.Exception);
        }

        private static void LogAggregatedException(AggregateException exceptions)
        {
            if (exceptions?.InnerExceptions == null)
                return;

            foreach (var exception in exceptions.InnerExceptions)
            {
                LogException(exception);
            }
        }

        private static void LogException(Exception exception)
        {
            if (exception == null)
                return;

            var logger = DependencyUtility.Container.Resolve<ILogger>();
            logger.Fatal(exception, exception.GetExceptionDetailMessage());
        }
    }

}
