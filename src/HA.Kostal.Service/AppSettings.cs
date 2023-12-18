using HA.AppTools;
using HA.Influx;
using HA.Mqtt;

namespace HA.Kostal.Service;

public class AppSettings
{
    public AppInitSettings Application { get; private set; }

    public KostalSettings Kostal { get; private set; }

    public InfluxSettings Influx { get; private set; }

    public MqttSettings Mqtt { get; private set; }

    public AppSettings(ILogger logger, AppInitSettings appInitSettings)
    {
        Application = appInitSettings;
        Kostal = new KostalSettings(Application.Configuration);
        Influx = new InfluxSettings(Application.Configuration);
        Mqtt = new MqttSettings(Application.Configuration);
    }

    public void CheckSettings()
    {
        Application.CheckSettings();
        Kostal.CheckSettings();
        Influx.CheckSettings();
        Mqtt.CheckSettings();
    }
}