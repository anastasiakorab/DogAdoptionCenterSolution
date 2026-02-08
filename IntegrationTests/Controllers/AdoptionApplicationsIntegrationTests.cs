using System.Net;
using DopAdoption.Domain.DomainModels;
using DopAdoptipon.Web.Data;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.Controllers;

[TestFixture]
public class AdoptionApplicationsIntegrationTests
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
    public async Task GET_AdoptionApplications_Index_Returns200()
    {
        var res = await _client.GetAsync("/AdoptionApplications");
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GET_AdoptionApplications_Create_Returns200()
    {
        
        var res = await _client.GetAsync("/AdoptionApplications/Create");
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task POST_AdoptionApplications_Create_Valid_Redirects_And_Persists_InDb()
    {
        
        Guid dogId;
        Guid adopterId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var breed = new Breed { Name = "SeedBreed" };
            db.Breeds.Add(breed);

            var dog = new Dog { Name = "SeedDog", Age = 2, Sex = "M", Status = "Available", Breed = breed };
            db.Dogs.Add(dog);

            var adopter = new Adopter { FullName = "Seed Adopter", Email = "seed@adopter.com", Phone = "070000000" };
            db.Adopters.Add(adopter);

            await db.SaveChangesAsync();

            dogId = dog.Id;
            adopterId = adopter.Id;
        }

       
        var getCreate = await _client.GetAsync("/AdoptionApplications/Create");
        Assert.That(getCreate.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var html = await getCreate.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(html);
        Assert.That(token, Is.Not.Null.And.Not.Empty);

        CopySetCookieHeader(getCreate, _client);

        
        var form = new Dictionary<string, string>
        {
            ["DogId"] = dogId.ToString(),
            ["AdopterId"] = adopterId.ToString(),
            ["Status"] = "Pending",
            ["Notes"] = "integration test",
            ["__RequestVerificationToken"] = token!
        };

        var post = await _client.PostAsync("/AdoptionApplications/Create", new FormUrlEncodedContent(form));
        Assert.That(post.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));

        
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Assert.That(db.Applications.Any(a => a.DogId == dogId && a.AdopterId == adopterId), Is.True);
        }
    }

    [Test]
    public async Task POST_AdoptionApplications_Create_InvalidModel_Returns200_View()
    {
        
        var getCreate = await _client.GetAsync("/AdoptionApplications/Create");
        Assert.That(getCreate.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var html = await getCreate.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(html);
        CopySetCookieHeader(getCreate, _client);

        
        var form = new Dictionary<string, string>
        {
            ["DogId"] = "",
            ["AdopterId"] = "",
            ["Status"] = "Pending",
            ["Notes"] = "x",
            ["__RequestVerificationToken"] = token!
        };

        var post = await _client.PostAsync("/AdoptionApplications/Create", new FormUrlEncodedContent(form));
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
