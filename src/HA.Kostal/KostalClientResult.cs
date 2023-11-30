using System.Net;

namespace HA.Kostal;

public class KostalClientResult
{
    public string Page { get; set; } = string.Empty;
    public long DownloadTimeMilliSec { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }
}
