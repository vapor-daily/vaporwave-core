using Serilog;
using Serilog.Core;

namespace vaporwave_core
{
    internal class LogSing
    {
        private static Logger? logSing;

        private static readonly object locker = new();

        private LogSing()
        {
        }

        public static Logger GetInstance()
        {
            lock (locker) if (logSing == null) logSing = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File(DateTime.Now.ToString("d").Replace("/", "-"))
                        .CreateLogger();
            return logSing;
        }

        public static void LogWarn(string str)
        {
            GetInstance().Warning(str);
        }

        public static void LogInfo(string str)
        {
            GetInstance().Information(str);
        }

        public static void LogError(string str)
        {
            GetInstance().Error(str);
        }
    }
}
