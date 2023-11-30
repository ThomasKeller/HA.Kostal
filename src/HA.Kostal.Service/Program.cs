namespace HA.Kostal.Service;

public class Program
{
    private static ILogger? _logger;

    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;
        var envWorkDir = Environment.GetEnvironmentVariable("WORKDIR", EnvironmentVariableTarget.Process) ?? "";
        var envLoggingLevel = Environment.GetEnvironmentVariable("LOGGINGLEVEL", EnvironmentVariableTarget.Process) ?? "Information";
        var appLoggingLevel = LogLevel.Information;
        if (Enum.TryParse(envLoggingLevel, out LogLevel level))
            appLoggingLevel = level;

        var loggerFactory = LoggerFactory.Create(builder => builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("ha", appLoggingLevel)
            .SetMinimumLevel(appLoggingLevel)
            .AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "yy-MM-dd HH:mm:ss.fff ";
            }));
        _logger = loggerFactory.CreateLogger<Program>();
        _logger.LogInformation("start service");

        try
        {
            var components = new Components(loggerFactory);
            components.Init(envWorkDir);
            components.EnableConsoleObserver();

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
            _logger?.LogCritical(ex, "Stopped program because of exception");
            throw;
        }
    }

    private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        _logger?.LogCritical(ex, ex?.Message);
    }
}