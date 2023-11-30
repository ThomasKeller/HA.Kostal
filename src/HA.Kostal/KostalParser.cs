using System.Globalization;
using System.Text.RegularExpressions;

namespace HA.Kostal;

public class KostalParser : IKostalParser
{
    private const string m_ColumnPattern = @"<td\b[^>]*?>(?<V>[\s\S]*?)</\s*td>";

    public KostalValues Parse(string htlmPage, long downloadTime_ms = 0)
    {
        var line = 1;
        var result = new KostalValues { DownloadTime_ms = downloadTime_ms };
        if (string.IsNullOrEmpty(htlmPage))
            return result;

        foreach (Match match in Regex.Matches(htlmPage, m_ColumnPattern, RegexOptions.IgnoreCase))
        {
            string value = match.Groups["V"].Value;
            switch (line)
            {
                case 15:
                    result.CurrentACPower_W = ToInteger(value, 0);
                    break;
                case 18:
                    result.ProducedEnergy_kWh = ToInteger(value);
                    break;
                case 27:
                    result.DailyEnergy_kWh = ToDouble(value, 0.0);
                    break;
                case 33:
                    result.Status = value.Replace("(MPP)", "").Trim();
                    break;
                case 56:
                    result.String1_Voltage_V = ToInteger(value, 0);
                    break;
                case 59:
                    result.L1_Voltage_V = ToInteger(value, 0);
                    break;
                case 65:
                    result.String1_Current_A = ToDouble(value, 0.0);
                    break;
                case 68:
                    result.L1_Power_W = ToInteger(value, 0);
                    break;
                case 82:
                    result.String2_Voltage_V = ToInteger(value, 0);
                    break;
                case 85:
                    result.L2_Voltage_V = ToInteger(value, 0);
                    break;
                case 91:
                    result.String2_Current_A = ToDouble(value, 0.0);
                    break;
                case 94:
                    result.L2_Power_W = ToInteger(value, 0);
                    break;
                case 111:
                    result.L3_Voltage_V = ToInteger(value, 0);
                    break;
                case 120:
                    result.L3_Power_W = ToInteger(value, 0);
                    break;
            }
            line++;
        }
        return result;
    }

    internal static int? ToInteger(string value)
    {
        if (int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        return null;
    }

    internal static int ToInteger(string value, int defaultValue)
    {
        return ToInteger(value) ?? defaultValue;
    }


    internal static double? ToDouble(string value)
    {
        if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        return null;
    }

    internal static double ToDouble(string value, double defaultValue)
    {
        return ToDouble(value) ?? defaultValue;
    }

}
