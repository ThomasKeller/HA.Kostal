namespace HA.Kostal.Service;

public class Worker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly Components _components;

    public Worker(ILogger<Worker> logger, Components components)
    {
        _logger = logger;
        _components = components;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastLog = DateTime.MinValue;
        _logger.LogInformation("Execute Background Task");
        if (_components.MeasurmentObservable != null)
        {
            _components.MeasurmentObservable.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                if ((DateTime.Now - lastLog).TotalSeconds > 30)
                {
                    lastLog = DateTime.Now;
                    _logger.LogInformation(_components.CurrentStatus());
                    if (_components.HealthMqttPublisher != null)
                    {
                        var componentStatus = _components.CurrentComponentsStatus();
                        if (componentStatus.Count > 0)
                        {
                            try
                            {
                                await _components.HealthMqttPublisher.Publish(componentStatus);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning("Cannot report status to MQTT broker. {0}", ex.Message);
                            }
                        }
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
            _components.MeasurmentObservable.Stop();
        }
    }
}