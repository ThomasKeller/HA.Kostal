using HA;
using HA.Influx;
using HA.Mqtt;
using HA.Store;
using System.Text;

namespace HA.Kostal.Service
{
    public class Components
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private ConsoleObserver? _consoleObserver;

        public Components(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<Components>();
        }

        public KostalObservable? MeasurmentObservable { get; private set; }

        public InfluxResilientStore? InfluxResilientStore { get; set; }

        public MeasurementObserver? InfluxMeasurementObserver { get; set; }

        public MeasurementObserver? MqttMeasurementObserver { get; set; }

        public MqttPublisher? HealthMqttPublisher { get; private set; }

        public void Init(string workDir = "")
        {
            var appSettings = new AppSettings(workDir);
            appSettings.Read();

            InitIKostalComponent(appSettings);
            InitInfluxComponent(appSettings);
            InitMqttComponent(appSettings);
        }

        public void EnableConsoleObserver()
        {
            _consoleObserver = new ConsoleObserver();
            MeasurmentObservable?.Subscribe(_consoleObserver);
        }

        public string CurrentStatus()
        {
            var sb = new StringBuilder();
            if (MeasurmentObservable != null && InfluxResilientStore != null && InfluxMeasurementObserver != null)
            {
                // initialized
                sb.Append($"Last Event sent: {(DateTime.Now - MeasurmentObservable.LastMeasurementSentAt).TotalSeconds} s ");
                sb.Append($"[Measurement] Last Proccessed: {(DateTime.Now - InfluxMeasurementObserver.LastMeasurementProccessed).TotalSeconds} s ");
                sb.Append($"[Influx] Queue Count: {InfluxResilientStore.QueueCount} Error Count: {InfluxResilientStore.InfluxErrorCount} ");
            }
            else
            {
                sb.AppendLine("Status: Not Ready, components un-initialized");
            }
            return sb.ToString();
        }

        public IDictionary<string, string> CurrentComponentsStatus()
        {
            var status = new Dictionary<string, string>();
            if (MeasurmentObservable != null && InfluxResilientStore != null && InfluxMeasurementObserver != null)
            {
                var root = "health/kostalservice/";
                status.Add($"{root}lastHeartBeat", DateTime.Now.ToString("o"));
                status.Add($"{root}observable/lastMeasurementSec",
                    (DateTime.Now - MeasurmentObservable.LastMeasurementSentAt).TotalSeconds.ToString("#.000"));
                status.Add($"{root}influx/lastMeasurementStoredSec",
                    (DateTime.Now - InfluxMeasurementObserver.LastMeasurementProccessed).TotalSeconds.ToString("#.000"));
                status.Add($"{root}influx/queueCount", InfluxResilientStore.QueueCount.ToString());
                status.Add($"{root}influx/totalErrorCount", InfluxResilientStore.InfluxErrorCount.ToString());
            }
            return status;
        }

        private void InitIKostalComponent(AppSettings appSettings)
        {
            MeasurmentObservable = new KostalObservable(
                _loggerFactory.CreateLogger<KostalObservable>(),
                new KostalClient(appSettings.KostalUrl, appSettings.KostalUser, appSettings.KostalPassword));
            MeasurmentObservable.StopDuringSunset = appSettings.KostalStopDuringSunset;// envKostalStopDuringNight == "true";
        }

        private void InitInfluxComponent(AppSettings appSettings)
        {
            InfluxResilientStore = new InfluxResilientStore(
                _loggerFactory.CreateLogger<InfluxResilientStore>(),
                new InfluxSimpleStore(
                    appSettings.InfluxUrl,
                    appSettings.InfluxBucket,
                    appSettings.InfluxOrg,
                    appSettings.InfluxToken),
                new MeasurementStore(appSettings.MeasurmentStoreFilePath));
            InfluxMeasurementObserver = new MeasurementObserver(
                    _loggerFactory.CreateLogger<MeasurementObserver>(),
                    InfluxResilientStore);
            if (MeasurmentObservable != null)
                InfluxMeasurementObserver.Subscribe(MeasurmentObservable);
        }

        private void InitMqttComponent(AppSettings appSettings)
        {
            var mqttPublisher = new MqttPublisher(
                _loggerFactory.CreateLogger<MqttPublisher>(),
                appSettings.MqttHost,
                appSettings.MqttPort,
                "ha.kostal.service");
            HealthMqttPublisher = mqttPublisher;
            MqttMeasurementObserver = new MeasurementObserver(
                   _loggerFactory.CreateLogger<MeasurementObserver>(),
                   mqttPublisher);
            if (MeasurmentObservable != null)
                MqttMeasurementObserver.Subscribe(MeasurmentObservable);
        }
    }
}