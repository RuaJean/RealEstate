using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using NUnit.Framework;

namespace RealEstate.IntegrationTests.Controllers;

[TestFixture]
public class OwnerControllerTests
{
    private MongoDbFixture _mongo = null!;
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private string _token = string.Empty;
    private sealed class AuthResult { public string AccessToken { get; set; } = string.Empty; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _mongo = new MongoDbFixture();
        await _mongo.StartAsync();
        _factory = new CustomWebApplicationFactory(_mongo);
        _client = _factory.CreateClient();

        // Crear usuario y obtener token
        var email = $"ownerit+{Guid.NewGuid():N}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Passw0rd!" });
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Passw0rd!" });
        var loginPayload = await loginResp.Content.ReadFromJsonAsync<AuthResult>();
        _token = loginPayload!.AccessToken;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
        await _mongo.DisposeAsync();
    }

    [Test]
    public async Task Create_And_Get_Owner_Should_Succeed()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var createResp = await _client.PostAsJsonAsync("/api/owner", new { name = "John Doe", address = "Main St", photo = "" });
        Assert.That(createResp.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var created = await createResp.Content.ReadFromJsonAsync<OwnerCreated>();
        var id = created!.Id;

        var getResp = await _client.GetAsync($"/api/owner/{id}");
        Assert.That(getResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var owner = await getResp.Content.ReadFromJsonAsync<OwnerGet>();
        Assert.That(owner!.Name, Is.EqualTo("John Doe"));
    }

    private sealed class OwnerCreated { public Guid Id { get; set; } }
    private sealed class OwnerGet { public string Name { get; set; } = string.Empty; }
}

