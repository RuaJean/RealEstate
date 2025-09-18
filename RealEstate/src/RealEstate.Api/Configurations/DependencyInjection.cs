using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Infrastructure.Persistence.Repositories;
using RealEstate.Application.Interfaces;
using RealEstate.Infrastructure.Storage;
using AutoMapper;
using RealEstate.Application.Profiles;
using RealEstate.Application.Services;
using RealEstate.Application.Interfaces.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace RealEstate.Api.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Mongo
            MongoDbConfig.AddMongoDb(services, configuration);

            // Repositorios
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
            services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // File storage
            var fsOptions = new LocalFileStorageOptions();
            configuration.GetSection("FileStorage").Bind(fsOptions);
            services.AddSingleton(fsOptions);
            services.AddSingleton<IFileStorageService>(sp =>
            {
                var opt = sp.GetRequiredService<LocalFileStorageOptions>();
                var env = sp.GetRequiredService<IHostEnvironment>();
                return new LocalFileStorage(opt, env.ContentRootPath);
            });

            // AutoMapper (registro manual para evitar ambigüedad de extensiones)
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            services.AddSingleton<IMapper>(mapperConfig.CreateMapper());

            // Servicios de aplicación
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IPropertyImageService, PropertyImageService>();
            services.AddScoped<IPropertyTraceService, PropertyTraceService>();

            // FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RealEstate.Application.Validators.PropertyCreateValidator>();

            // Swagger custom (form support)
            services.AddSwaggerWithFormSupport();

            return services;
        }
    }
}
