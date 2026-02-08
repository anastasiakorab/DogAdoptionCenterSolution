using System.Net;
using NUnit.Framework;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.Controllers;

[TestFixture]
public class DogsIntegrationTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GET_Dogs_Index_Returns200()
    {
        var response = await _client.GetAsync("/Dogs");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
