using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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

                // Security scheme (JWT Bearer)
                var bearerScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Ingrese el token JWT como: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", bearerScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { bearerScheme, Array.Empty<string>() }
                });

                // Ejemplos básicos
                c.OperationFilter<RealEstate.Api.Configurations.DefaultExamplesOperationFilter>();
            });
            return services;
        }
    }
}
