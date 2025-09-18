using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Infrastructure.Persistence.Context;

namespace RealEstate.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly MongoDbFixture _mongo;

    public CustomWebApplicationFactory(MongoDbFixture mongo)
    {
        _mongo = mongo;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var memConfig = new Dictionary<string, string?>
            {
                ["MongoDb:ConnectionString"] = _mongo.ConnectionString,
                ["MongoDb:Database"] = _mongo.DatabaseName,
                ["FileStorage:RootPath"] = "wwwroot-test",
                ["FileStorage:BaseRequestPath"] = "/",
                ["Jwt:Issuer"] = "RealEstateApi",
                ["Jwt:Audience"] = "RealEstateApiClients",
                ["Jwt:SecretKey"] = "integration-tests-secret-key-1234567890",
                ["Jwt:ExpirationMinutes"] = "60"
            };
            configBuilder.AddInMemoryCollection(memConfig!);
        });

        builder.ConfigureServices(services =>
        {
            // No reemplazamos servicios; DI ya se ajusta con configuración en memoria
        });
    }
}


