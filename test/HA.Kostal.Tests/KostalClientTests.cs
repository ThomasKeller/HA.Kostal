using System.Net;

namespace HA.Kostal.Tests;

public class KostalClientTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void test_that_client_read_page_successfully()
    {
        var sunValues =   Sun.CalculatePvTime();
        sunValues = Sun.Calculate(51.194256, 6.400471, 2022, 10, 26);

        var sut = new KostalClient("http://192.168.111.4", "pvserver", "EMsiWgsus63pv");
        var result = sut.readPage();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.DownloadTimeMilliSec, Is.GreaterThan(0));
    }

    [Test]
    public void client_should_return_401_by_using_wrong_credentials()
    {
        var sut = new KostalClient("http://192.168.111.4", "pvserver1", "EMsiWgsus63pv");
        var result = sut.readPage();
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(result.DownloadTimeMilliSec, Is.GreaterThan(0));
    }

    [Test]
    public void client_should_throw_exception_by_using_wrong_endpoint()
    {
        var sut = new KostalClient("http://192.168.111.40", "pvserver", "EMsiWgsus63pv");
        Assert.Throws<HttpRequestException>(() => sut.readPage());
    }
}