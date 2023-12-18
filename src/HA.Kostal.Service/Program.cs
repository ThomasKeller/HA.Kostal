using HA.AppTools;
using HA.Influx;
using HA.Mqtt;

namespace HA.Kostal.Service;

public class Program
{
    private static ILogger? _logger;

    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;
        var appInitSettings = new AppInitSettings();
        var loggerFactory = LoggerFactory.Create(builder => builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("ha", appInitSettings.LoggingLevel)
            .SetMinimumLevel(appInitSettings.LoggingLevel)
            .AddSimpleConsole(options => {
                options.SingleLine = true;
                options.TimestampFormat = "yy-MM-dd HH:mm:ss.fff ";
            }));
        var appSettings = new AppSettings(
            loggerFactory.CreateLogger<AppSettings>(),
            appInitSettings);
        appInitSettings.CheckSettings();

        try
        {
            var components = new Components(loggerFactory, appSettings);
            components.EnableConsoleObserver();

            _logger = loggerFactory.CreateLogger<Program>();
            _logger.LogInformation("start Kostal service");
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddSingleton(components)
                    .AddSingleton(loggerFactory.CreateLogger<Worker>())
                    .AddHostedService<Worker>())
                .Build();
            host.Run();
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, $"Stopped program because of exception: {ex.Message}");
            throw;
        }
    }

    private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        _logger?.LogCritical(ex, ex?.Message);
    }
}