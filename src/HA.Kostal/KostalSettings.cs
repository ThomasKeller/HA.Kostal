using HA.AppTools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HA.Kostal
{
    public class KostalSettings : AppSettingsBase
    {
        public KostalSettings(IConfiguration? configuration)
        {
            if (configuration != null)
            {
                ReadAppConfigFile(configuration);
            }
            ReadEnvironmentVariables();
        }

        [EnvParameter("KOSTAL_URL")]
        [ConfigParameter("kostal", "url", Required = true)]
        public string KostalUrl { get; set; } = "http://192.168.x.x";

        [EnvParameter("KOSTAL_USER")]
        [ConfigParameter("kostal", "user", Required = true)]
        public string KostalUser { get; set; } = "pvserver";

        [EnvParameter("KOSTAL_PASSWORD")]
        [ConfigParameter("kostal", "password", Required = true)]
        public string KostalPassword { get; set; } = string.Empty;

        [EnvParameter("KOSTAL_STOP_DURING_SUNSET")]
        [ConfigParameter("kostal", "stopDuringSunset")]
        public bool KostalStopDuringSunset { get; set; } = true;

        public void CheckRequiredProperties()
        {
            CheckSettings();
        }
    }
}
