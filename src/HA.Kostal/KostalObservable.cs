using HA;
using HA.Observable;
using Microsoft.Extensions.Logging;

namespace HA.Kostal;

public class KostalObservable : ObservableBase<Measurement>
{
    private readonly IKostalClient _kostalClient;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _tokenSource = new();
    private Task? _task;

    public KostalObservable(ILogger logger, IKostalClient kostalClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _kostalClient = kostalClient ?? throw new ArgumentNullException(nameof(kostalClient));
    }

    public TimeSpan MeasureInterval { get; set; } = TimeSpan.FromSeconds(60);

    public TimeSpan SleepInterval { get; set; } = TimeSpan.FromMinutes(5);

    public double Latitude { get; set; } = 51.1853900;

    public double Longtitude { get; set; } = 6.4417200;

    public bool StopDuringSunset { get; set; } = true;

    public DateTime LastMeasurementSentAt { get; private set; } = DateTime.MinValue;

    public void Start()
    {
        _logger.LogInformation("start Kostal Observable.");
        _task = new Task(() => ReadFromKostal(), _tokenSource.Token);
        _task.Start();
    }

    public void Stop()
    {
        _logger.LogInformation("stop Kostal Observable.");
        _tokenSource.Cancel();
        foreach (var observer in _observers.ToArray())
        {
            if (observer != null)
            {
                _logger.LogDebug("send on complete event to observer #{0}.", observer.GetHashCode());
                observer.OnCompleted();
            }
        }
        _observers.Clear();
    }

    private void ReadFromKostal()
    {
        while (!_tokenSource.Token.IsCancellationRequested)
        {
            if (StopDuringSunset && HasTheSunGoneDown())
            {
                _logger.LogDebug("the sun went down. Wait {0} s.", SleepInterval.TotalSeconds);
                Thread.Sleep((int)SleepInterval.TotalMilliseconds);
                continue;
            }
            try
            {
                _logger.LogDebug("request Kostal page.");
                var kostalClientResult = _kostalClient.readPage();
                if (kostalClientResult.IsSuccessStatusCode)
                {
                    _logger.LogDebug("read Kostal page successfully.");
                    var page = kostalClientResult.Page;
                }
                else
                {
                    _logger.LogDebug("read Kostal page not successfully. {0}", kostalClientResult.StatusCode.ToString());
                    foreach (var observer in _observers)
                        observer.OnError(new Exception(kostalClientResult.StatusCode.ToString()));
                }
                _logger.LogDebug("parse Kostal page.");
                var parser = new KostalParser();
                var kostalValues = parser.Parse(kostalClientResult.Page, kostalClientResult.DownloadTimeMilliSec);
                _logger.LogInformation("Current Power: {0} W Daily Energy: {1} kWh Download Time {3} ms",
                    kostalValues.CurrentACPower_W, kostalValues.DailyEnergy_kWh, kostalValues.DownloadTime_ms);
                LastMeasurementSentAt = DateTime.Now;
                foreach (var observer in _observers)
                    observer.OnNext(kostalValues.ToMeasurement());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                foreach (var observer in _observers)
                    observer.OnError(ex);
            }
            _logger.LogDebug("wait {0} s.", MeasureInterval.TotalSeconds);
            Thread.Sleep((int)MeasureInterval.TotalMilliseconds);
        }
    }

    private bool HasTheSunGoneDown()
    {
        var now = DateTime.Now;
        var sunValues = Sun.CalculatePvTime(Latitude, Longtitude);
        var sunriseHour = sunValues.Sunrise;
        var sunsetHour = sunValues.Sunset;
        _logger.LogDebug("Sunrise: {0}, Sunset: {1}", sunriseHour, sunsetHour);
        return now.Hour > sunsetHour.Hour || now.Hour < sunriseHour.Hour;
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<Measurement>> m_Observers;
        private readonly IObserver<Measurement> m_Observer;

        public Unsubscriber(List<IObserver<Measurement>> observers, IObserver<Measurement> observer)
        {
            m_Observers = observers;
            m_Observer = observer;
        }

        public void Dispose()
        {
            if (!(m_Observer == null))
                m_Observers.Remove(m_Observer);
        }
    }
}