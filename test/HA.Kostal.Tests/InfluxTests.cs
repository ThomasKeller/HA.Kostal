using HA.Influx;

namespace HA.Kostal.Tests;

public class InfluxTests
{
    private string _token = $"zWIzCF8VlDmNcBkr-MujkIXCPiq11skFYEHfMEiNkrfA8yKOXxgwE1VX8G_EA1b3DP5D-0PgL3qby8PUBpFVfQ==";
    private string _bucket = "ha_test";
    private string _org = "Keller";
    private string _url = "http://192.168.111.237:8086";

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ping_influx_successfully()
    {
        var influxStore = new InfluxSimpleStore(
            _url, _bucket, _org, _token);
        var pong = influxStore.Ping();
        Assert.That(pong, Is.EqualTo(true));
    }

    [Test]
    public void test_that_measurement_is_stored_in_influx_successfully()
    {
        var influxStore = new InfluxSimpleStore(
            _url, _bucket, _org, _token);

        var measurement = new Measurement
        {
            Device = "HA.Test",
            Quality = QualityInfos.Good
        };
        measurement.Tags.Add("T1", "Tag1");
        measurement.Tags.Add("T2", "Tag2");
        measurement.Values.Add(MeasuredValue.Create("V1", 123));
        influxStore.WriteMeasurement(measurement);
        Assert.Pass();
        //Assert.That(success.IsSuccessful, Is.EqualTo(true));
        //Assert.That(success.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        //Assert.That(success.ErrorMessage, Is.Null);
    }

    [Test]
    public void check_health_of_influx_successfully()
    {
        var influxStore = new InfluxSimpleStore(
            _url, _bucket, _org, _token);
        var response = influxStore.CheckHealth();
        Assert.That(response, Is.EqualTo(true));
    }
}