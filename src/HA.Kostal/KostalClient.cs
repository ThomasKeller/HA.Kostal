using System.Diagnostics;
using System.Net;

namespace HA.Kostal;

public class KostalClient : IKostalClient
{
    private static readonly TaskFactory _taskFactory = new(
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

    private CredentialCache _credentialCache = new();
    private readonly string _url;

    public KostalClient(string url, string user, string? password)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        if (password == null)
            throw new ArgumentNullException(nameof(password));
        _url = url;
        _credentialCache.Add(new Uri(url), "Basic", new NetworkCredential(user, password));
    }

    public async Task<KostalClientResult> readPageAsync()
    {
        using var handler = new HttpClientHandler { Credentials = _credentialCache };
        using var client = new HttpClient(handler);
        var before = DateTime.UtcNow;
        var response = await client.GetAsync(_url);
        var page = response.Content.ReadAsStringAsync().Result;
        var after = DateTime.UtcNow;
        return new KostalClientResult
        {
            Page = page,
            DownloadTimeMilliSec = (long)(after - before).TotalMilliseconds,
            StatusCode = response.StatusCode,
            IsSuccessStatusCode = response.IsSuccessStatusCode
        };
    }

    public KostalClientResult readPage()
    {
        return _taskFactory
            .StartNew(readPageAsync)
            .Unwrap()
            .GetAwaiter()
            .GetResult();
    }
}