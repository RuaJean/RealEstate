using System;
using System.Threading.Tasks;
using Mongo2Go;

namespace RealEstate.IntegrationTests;

public sealed class MongoDbFixture : IAsyncDisposable
{
    public string ConnectionString { get; }
    public string DatabaseName { get; } = $"itdb_{Guid.NewGuid():N}";
    private readonly MongoDbRunner? _runner;

    public MongoDbFixture()
    {
        var fromEnv = Environment.GetEnvironmentVariable("MONGO_URL");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            ConnectionString = fromEnv!;
            _runner = null;
        }
        else
        {
            _runner = MongoDbRunner.Start(singleNodeReplSet: true, additionalMongodArguments: "--quiet");
            ConnectionString = _runner.ConnectionString;
        }
    }

    public async Task StartAsync()
    {
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _runner?.Dispose();
        await Task.CompletedTask;
    }
}

