using Serilog;

namespace system_sprava_skladu
{
    internal static class ErrorLog
    {
        public static void Inicializace()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
