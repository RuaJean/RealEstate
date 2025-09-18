using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace RealEstate.Api.Configurations
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerWithFormSupport(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RealEstate API", Version = "v1" });
                c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
            });
            return services;
        }
    }
}
