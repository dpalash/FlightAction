using Framework.IoC;
using Serilog;
using Unity;

namespace Framework.Logger
{
    /// <summary>
    /// Log based on NLog
    /// </summary>
    public static class Log
    {
        private static ILogger _logger;
        public static ILogger Logger
        {
            get
            {
                if (_logger != null)
                    return _logger;

                _logger = DependencyUtility.Container?.Resolve<ILogger>();
                return _logger;
            }
        }
    }
}
