namespace HA.Kostal;

public class KostalValues
{
    public DateTime MeasureTime { get; set; } = DateTime.Now;

    public long DownloadTime_ms { get; set; }

    // aktuell 1130
    public int CurrentACPower_W { get; set; }

    // Gesamtenergie 75221
    public int? ProducedEnergy_kWh { get; set; }

    // Tagesenergie 3.81
    public double DailyEnergy_kWh { get; set; }

    // Status Einspeisen
    public string Status { get; set; } = "Fehler";

    // String1_Voltage_V
    public int String1_Voltage_V { get; set; }

    // String1_Current_A
    public double String1_Current_A { get; set; }

    // String2_Voltage_V
    public int String2_Voltage_V { get; set; }

    // String2_Current_A
    public double String2_Current_A { get; set; }

    // L1_Voltage_V
    public int L1_Voltage_V { get; set; }

    // L1_Power_W
    public int L1_Power_W { get; set; }

    // L2_Voltage_V
    public int L2_Voltage_V { get; set; }

    // L2_Power_W
    public int L2_Power_W { get; set; }

    // L3_Voltage_V 229
    public int L3_Voltage_V { get; set; }

    // L3_Power_W 379
    public int L3_Power_W { get; set; }

    public Measurement ToMeasurement()
    {
        var measurement = new Measurement(MeasureTime)
        {
            Device = "KostalPiko",
            Quality = QualityInfos.Good
        };
        measurement.Tags.Add("Status", Status);
        AddValue(measurement.Values, "DownloadTime_ms", DownloadTime_ms);
        AddValue(measurement.Values, "ProducedEnergy_kWh", ProducedEnergy_kWh);
        AddValue(measurement.Values, "DailyEnergy_kWh", DailyEnergy_kWh);
        AddValue(measurement.Values, "CurrentACPower_W", CurrentACPower_W);
        AddValue(measurement.Values, "String1.Voltage_V", String1_Voltage_V);
        AddValue(measurement.Values, "String1.Current_A", String1_Current_A);
        var string1_Power_W = String1_Current_A * String1_Voltage_V;
        AddValue(measurement.Values, "String1.Power_W", string1_Power_W);
        AddValue(measurement.Values, "String2.Voltage_V", String2_Voltage_V);
        AddValue(measurement.Values, "String2.Current_A", String2_Current_A);
        var string2_Power_W = String2_Current_A * String2_Voltage_V;
        AddValue(measurement.Values, "String2.Power_W", string2_Power_W);
        var string_Power_W = string1_Power_W + string2_Power_W;
        AddValue(measurement.Values, "String.Power_W", string_Power_W);
        var efficiency = 0.0;
        if (string_Power_W > 0)
        {
            efficiency = CurrentACPower_W / string_Power_W * 100;
        }
        AddValue(measurement.Values, "Efficiency_%", efficiency);
        AddValue(measurement.Values, "L1.Voltage_V", L1_Voltage_V);
        AddValue(measurement.Values, "L1.Power_W", L1_Power_W);
        AddValue(measurement.Values, "L2.Voltage_V", L2_Voltage_V);
        AddValue(measurement.Values, "L2.Power_W", L2_Power_W);
        AddValue(measurement.Values, "L3.Voltage_V", L3_Voltage_V);
        AddValue(measurement.Values, "L3.Power_W", L3_Power_W);
        return measurement;
    }

    public override string ToString()
    {
        return $"{MeasureTime} Status:{Status} | Current:{CurrentACPower_W}W | Daily:{DailyEnergy_kWh}kWh | All:{ProducedEnergy_kWh}kWh";
    }

    private void AddValue<T>(List<MeasuredValue> values, string name, T value)
    {
        if (value != null)
            values.Add(MeasuredValue.Create(name, value));
    }
}