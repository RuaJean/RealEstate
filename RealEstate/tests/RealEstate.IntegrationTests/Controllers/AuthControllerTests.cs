using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;

namespace RealEstate.IntegrationTests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private MongoDbFixture _mongo = null!;
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private sealed class AuthResult { public string AccessToken { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Role { get; set; } = string.Empty; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _mongo = new MongoDbFixture();
        await _mongo.StartAsync();
        _factory = new CustomWebApplicationFactory(_mongo);
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
        await _mongo.DisposeAsync();
    }

    [Test]
    public async Task Register_Then_Login_Should_Return_Token()
    {
        var email = $"ituser+{Guid.NewGuid():N}@example.com";
        var registerResp = await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Passw0rd!" });
        Assert.That(registerResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var regPayload = await registerResp.Content.ReadFromJsonAsync<AuthResult>();
        Assert.That(regPayload, Is.Not.Null);
        Assert.That(regPayload!.AccessToken, Is.Not.Empty);

        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Passw0rd!" });
        Assert.That(loginResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var loginPayload = await loginResp.Content.ReadFromJsonAsync<AuthResult>();
        Assert.That(loginPayload, Is.Not.Null);
        Assert.That(loginPayload!.AccessToken, Is.Not.Empty);
    }
}

