using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Infrastructure.Persistence.Context;

namespace RealEstate.Api.Configurations
{
    public static class MongoDbConfig
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new MongoDbSettings();
            configuration.GetSection("MongoDb").Bind(settings);

            services.AddSingleton(settings);
            services.AddSingleton<IMongoDbContext, MongoDbContext>();
            return services;
        }
    }
}
