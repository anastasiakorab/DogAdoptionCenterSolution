using System.Net;
using DopAdoptipon.Web.Data;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.Controllers;

[TestFixture]
public class AdoptersIntegrationTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GET_Adopters_Index_Returns200()
    {
        var res = await _client.GetAsync("/Adopters");
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task POST_Adopters_Create_Valid_Redirects_And_Persists_InDb()
    {
        var getCreate = await _client.GetAsync("/Adopters/Create");
        Assert.That(getCreate.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var html = await getCreate.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(html);
        Assert.That(token, Is.Not.Null.And.Not.Empty);

        CopySetCookieHeader(getCreate, _client);

        var form = new Dictionary<string, string>
        {
            ["FullName"] = "Test Adopter",
            ["Email"] = "test@adopter.com",
            ["Phone"] = "070123456",
            ["__RequestVerificationToken"] = token!
        };

        var post = await _client.PostAsync("/Adopters/Create", new FormUrlEncodedContent(form));
        Assert.That(post.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Assert.That(db.Adopters.Any(a => a.Email == "test@adopter.com"), Is.True);
    }

    [Test]
    public async Task POST_Adopters_Create_InvalidModel_Returns200_View()
    {
        var getCreate = await _client.GetAsync("/Adopters/Create");
        Assert.That(getCreate.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var html = await getCreate.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(html);
        CopySetCookieHeader(getCreate, _client);

        
        var form = new Dictionary<string, string>
        {
            ["FullName"] = "X",
            ["Email"] = "", 
            ["Phone"] = "1",
            ["__RequestVerificationToken"] = token!
        };

        var post = await _client.PostAsync("/Adopters/Create", new FormUrlEncodedContent(form));
        Assert.That(post.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    

    private static string? ExtractAntiForgeryToken(string html)
    {
        const string needle = "name=\"__RequestVerificationToken\"";
        var idx = html.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return null;

        var valueIdx = html.IndexOf("value=\"", idx, StringComparison.OrdinalIgnoreCase);
        if (valueIdx < 0) return null;

        valueIdx += "value=\"".Length;
        var endIdx = html.IndexOf("\"", valueIdx, StringComparison.OrdinalIgnoreCase);
        if (endIdx < 0) return null;

        return html.Substring(valueIdx, endIdx - valueIdx);
    }

    private static void CopySetCookieHeader(HttpResponseMessage response, HttpClient client)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies)) return;

        var cookieHeader = string.Join("; ", cookies.Select(c => c.Split(';')[0]));
        client.DefaultRequestHeaders.Remove("Cookie");
        client.DefaultRequestHeaders.Add("Cookie", cookieHeader);
    }
}
