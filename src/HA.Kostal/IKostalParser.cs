namespace HA.Kostal;

public interface IKostalParser
{
    KostalValues Parse(string htlmPage, long downloadTime_ms = 0);
}