using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using NUnit.Framework;

namespace RealEstate.IntegrationTests.Controllers;

[TestFixture]
public class PropertyControllerTests
{
    private MongoDbFixture _mongo = null!;
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private string _token = string.Empty;
    private Guid _ownerId;
    private sealed class AuthResult { public string AccessToken { get; set; } = string.Empty; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _mongo = new MongoDbFixture();
        await _mongo.StartAsync();
        _factory = new CustomWebApplicationFactory(_mongo);
        _client = _factory.CreateClient();

        var email = $"propit+{Guid.NewGuid():N}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Passw0rd!" });
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Passw0rd!" });
        var loginPayload = await loginResp.Content.ReadFromJsonAsync<AuthResult>();
        _token = loginPayload!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var ownerResp = await _client.PostAsJsonAsync("/api/owner", new { name = "Owner1", address = "Addr", photo = "" });
        var owner = await ownerResp.Content.ReadFromJsonAsync<OwnerCreated>();
        _ownerId = owner!.Id;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
        await _mongo.DisposeAsync();
    }

    [Test]
    public async Task Get_List_Public_Should_Work_Without_Token()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var resp = await _client.GetAsync("/api/properties");
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Create_And_Get_Should_Require_Token_And_Work()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var createResp = await _client.PostAsJsonAsync("/api/properties", new
        {
            name = "Casa Centro",
            street = "Calle 1",
            city = "Bogotá",
            state = "Cundinamarca",
            country = "CO",
            zipCode = "110111",
            price = 200000m,
            currency = "USD",
            year = 2015,
            area = 120.5,
            ownerId = _ownerId,
            active = true
        });
        Assert.That(createResp.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var created = await createResp.Content.ReadFromJsonAsync<PropertyCreated>();
        var id = created!.Id;

        var getResp = await _client.GetAsync($"/api/properties/{id}");
        Assert.That(getResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var prop = await getResp.Content.ReadFromJsonAsync<PropertyGet>();
        Assert.That(prop!.Name, Is.EqualTo("Casa Centro"));
    }

    private sealed class OwnerCreated { public Guid Id { get; set; } }
    private sealed class PropertyCreated { public string Id { get; set; } = string.Empty; }
    private sealed class PropertyGet { public string Name { get; set; } = string.Empty; }
}

