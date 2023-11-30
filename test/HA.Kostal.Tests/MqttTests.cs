using HA.Mqtt;
using Microsoft.Extensions.Logging;

namespace HA.Kostal.Tests;

public class MqttTests
{
    private ILoggerFactory _loggerFactory;
    private string _mqttHost = $"192.168.111.50";

    [SetUp]
    public void Setup()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
            builder.AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("ha", LogLevel.Debug)
                .AddDebug()
                .AddConsole());
    }

    [TearDown]
    public void Teardown()
    {
        _loggerFactory.Dispose();
    }


    [Test]
    public async Task publish_payload_to_MQTT_topic_successfully()
    {
        var mqttClient = new MqttPublisher(_loggerFactory.CreateLogger<MqttPublisher>(), _mqttHost);

        await mqttClient.Publish("measurements/test/value1", "12");
        await mqttClient.Disconnect();
        Assert.Pass();
    }
}