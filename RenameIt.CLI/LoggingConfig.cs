using Serilog;
using Serilog.Events;

namespace RenameIt.CLI
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging()
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            
            // Create log path with year/month/day structure
            var now = DateTime.Now;
            var year = now.Year.ToString();
            var month = now.Month.ToString("D2");
            var day = now.Day.ToString("D2");
            var period = now.Hour < 12 ? "AM" : "PM";
            
            var logPath = Path.Combine(logDirectory, year, month, $"{day}-{period}.log");
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(logPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    logPath,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();
            
            Log.Information("RenameIt CLI started");
            Log.Information("Logging to: {LogPath}", logPath);
        }
        
        public static void CloseLogging()
        {
            Log.Information("RenameIt CLI shutting down");
            Log.CloseAndFlush();
        }
    }
}
