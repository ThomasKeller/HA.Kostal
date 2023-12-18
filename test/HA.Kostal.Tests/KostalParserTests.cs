namespace HA.Kostal.Tests;

public class KostalParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void parser_should_parse_page_when_piko_is_off()
    {
        var page = File.ReadAllText("PageNoValues.html");
        var result = new KostalParser().Parse(page, 123);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.DownloadTime_ms, Is.EqualTo(123));
        Assert.That(result.Status, Is.EqualTo("Aus"));
        Assert.That(result.CurrentACPower_W, Is.EqualTo(0));
        Assert.That(result.DailyEnergy_kWh, Is.EqualTo(3.70));
        Assert.That(result.L1_Power_W, Is.EqualTo(0));
        Assert.That(result.L1_Voltage_V, Is.EqualTo(0));
        Assert.That(result.L2_Power_W, Is.EqualTo(0));
        Assert.That(result.L2_Voltage_V, Is.EqualTo(0));
        Assert.That(result.L3_Power_W, Is.EqualTo(0));
        Assert.That(result.L3_Voltage_V, Is.EqualTo(0));
        Assert.That(result.ProducedEnergy_kWh, Is.EqualTo(75217));
        Assert.That(result.String1_Current_A, Is.EqualTo(0.0));
        Assert.That(result.String1_Voltage_V, Is.EqualTo(0));
        Assert.That(result.String2_Current_A, Is.EqualTo(0.0));
        Assert.That(result.String2_Voltage_V, Is.EqualTo(0));
        var measurement = result.ToMeasurement();
        Assert.That(measurement.Quality, Is.EqualTo(QualityInfos.Good));
        Assert.That(measurement.Tags["Status"], Is.EqualTo("Aus"));
        Assert.That(measurement.Values.First(v => v.Name == "DownloadTime_ms").Value, Is.EqualTo(123));
        Assert.That(measurement.Values.First(v => v.Name == "DailyEnergy_kWh").Value, Is.EqualTo(3.70));
        Assert.That(measurement.Values.First(v => v.Name == "ProducedEnergy_kWh").Value, Is.EqualTo(75217));
        Assert.That(measurement.Tags.Count, Is.EqualTo(1));
        Assert.That(measurement.Values.Count, Is.EqualTo(18));
        var lineprotokoll = measurement.ToLineProtocol();
    }

    [Test]
    public void parser_should_parse_page_when_piko_is_on()
    {
        var page = File.ReadAllText("PageWithValues.html");
        var result = new KostalParser().Parse(page, 123);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.DownloadTime_ms, Is.EqualTo(123));
        Assert.That(result.Status, Is.EqualTo("Einspeisen"));
        Assert.That(result.CurrentACPower_W, Is.EqualTo(1130));
        Assert.That(result.DailyEnergy_kWh, Is.EqualTo(3.81));
        Assert.That(result.L1_Power_W, Is.EqualTo(373));
        Assert.That(result.L1_Voltage_V, Is.EqualTo(229));
        Assert.That(result.L2_Power_W, Is.EqualTo(378));
        Assert.That(result.L2_Voltage_V, Is.EqualTo(228));
        Assert.That(result.L3_Power_W, Is.EqualTo(379));
        Assert.That(result.L3_Voltage_V, Is.EqualTo(225));
        Assert.That(result.ProducedEnergy_kWh, Is.EqualTo(75221));
        Assert.That(result.String1_Current_A, Is.EqualTo(0.62));
        Assert.That(result.String1_Voltage_V, Is.EqualTo(524));
        Assert.That(result.String2_Current_A, Is.EqualTo(1.38));
        Assert.That(result.String2_Voltage_V, Is.EqualTo(613));
        var measurement = result.ToMeasurement();
        Assert.That(measurement.Quality, Is.EqualTo(QualityInfos.Good));
        Assert.That(measurement.Tags["Status"], Is.EqualTo("Einspeisen"));
        Assert.That(measurement.Values.First(v => v.Name == "DownloadTime_ms").Value, Is.EqualTo(123));
        Assert.That(measurement.Values.First(v => v.Name == "DailyEnergy_kWh").Value, Is.EqualTo(3.81));
        Assert.That(measurement.Values.First(v => v.Name == "ProducedEnergy_kWh").Value, Is.EqualTo(75221));
        Assert.That(measurement.Values.First(v => v.Name == "CurrentACPower_W").Value, Is.EqualTo(1130));

        Assert.That(measurement.Values.First(v => v.Name == "L1.Power_W").Value, Is.EqualTo(373));
        Assert.That(measurement.Values.First(v => v.Name == "L1.Voltage_V").Value, Is.EqualTo(229));
        Assert.That(measurement.Values.First(v => v.Name == "L2.Power_W").Value, Is.EqualTo(378));
        Assert.That(measurement.Values.First(v => v.Name == "L2.Voltage_V").Value, Is.EqualTo(228));
        Assert.That(measurement.Values.First(v => v.Name == "L3.Power_W").Value, Is.EqualTo(379));
        Assert.That(measurement.Values.First(v => v.Name == "L3.Voltage_V").Value, Is.EqualTo(225));

        Assert.That(measurement.Values.First(v => v.Name == "String1.Current_A").Value, Is.EqualTo(0.62));
        Assert.That(measurement.Values.First(v => v.Name == "String1.Voltage_V").Value, Is.EqualTo(524));
        Assert.That(measurement.Values.First(v => v.Name == "String2.Current_A").Value, Is.EqualTo(1.38));
        Assert.That(measurement.Values.First(v => v.Name == "String2.Voltage_V").Value, Is.EqualTo(613));

        Assert.That(measurement.Tags.Count, Is.EqualTo(1));
        Assert.That(measurement.Values.Count, Is.EqualTo(18));
        var lineprotokoll = measurement.ToLineProtocol(TimeResolution.ms);
    }

    [Test]
    public void parser_should_parse_empty_page()
    {
        var result = new KostalParser().Parse("", 123);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.DownloadTime_ms, Is.EqualTo(123));
        Assert.That(result.Status, Is.EqualTo("Fehler"));
        Assert.That(result.CurrentACPower_W, Is.EqualTo(0));
        Assert.That(result.DailyEnergy_kWh, Is.EqualTo(0.0));
        Assert.That(result.L1_Power_W, Is.EqualTo(0));
        Assert.That(result.L1_Voltage_V, Is.EqualTo(0));
        Assert.That(result.L2_Power_W, Is.EqualTo(0));
        Assert.That(result.L2_Voltage_V, Is.EqualTo(0));
        Assert.That(result.L3_Power_W, Is.EqualTo(0));
        Assert.That(result.L3_Voltage_V, Is.EqualTo(0));
        Assert.That(result.ProducedEnergy_kWh, Is.Null);
        Assert.That(result.String1_Current_A, Is.EqualTo(0.0));
        Assert.That(result.String1_Voltage_V, Is.EqualTo(0));
        Assert.That(result.String2_Current_A, Is.EqualTo(0.0));
        Assert.That(result.String2_Voltage_V, Is.EqualTo(0));
    }
}