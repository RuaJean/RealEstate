using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RealEstate.Application.DTOs.Owner;
using RealEstate.Application.DTOs.Property;
using RealEstate.Application.DTOs.PropertyImage;
using RealEstate.Application.DTOs.PropertyTrace;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Persistence.Context;
using RealEstate.Infrastructure.Persistence.Repositories;
using RealEstate.Application.Profiles;
using AutoMapper;
using System.IO;

var services = new ServiceCollection();

// Cargar configuración desde src/RealEstate.Api/appsettings.json para reusar MongoDb y FileStorage
var configBuilder = new ConfigurationBuilder();

// Intentar resolver la ruta del appsettings.json del Api desde varias ubicaciones
string baseDir = AppContext.BaseDirectory;
string currentDir = Directory.GetCurrentDirectory();
string candidate1 = Path.GetFullPath(Path.Combine(baseDir, "../../../../../src/RealEstate.Api/appsettings.json"));
string candidate2 = Path.Combine(currentDir, "src/RealEstate.Api/appsettings.json");
string? appsettingsPath = File.Exists(candidate1) ? candidate1 : (File.Exists(candidate2) ? candidate2 : null);

if (appsettingsPath != null)
{
    configBuilder.AddJsonFile(appsettingsPath, optional: true, reloadOnChange: false);
}

// Defaults por si no hay archivo
var defaults = new Dictionary<string, string?>
{
    ["MongoDb:ConnectionString"] = "mongodb://localhost:27017",
    ["MongoDb:Database"] = "RealEstateDb"
};
configBuilder.AddInMemoryCollection(defaults);

var configuration = configBuilder.Build();

// Registrar MongoDb
var mongoSettings = new MongoDbSettings();
configuration.GetSection("MongoDb").Bind(mongoSettings);
services.AddSingleton(mongoSettings);
services.AddSingleton<IMongoDbContext, MongoDbContext>();

// Repositorios
services.AddScoped<IOwnerRepository, OwnerRepository>();
services.AddScoped<IPropertyRepository, PropertyRepository>();
services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();

// AutoMapper minimal (solo MappingProfile)
var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
services.AddSingleton<IMapper>(mapperConfig.CreateMapper());

// Servicios
services.AddScoped<IOwnerService, OwnerService>();
services.AddScoped<IPropertyService, PropertyService>();
services.AddScoped<IPropertyImageService, PropertyImageService>();
services.AddScoped<IPropertyTraceService, PropertyTraceService>();

var provider = services.BuildServiceProvider();

var ownerSvc = provider.GetRequiredService<IOwnerService>();
var propSvc = provider.GetRequiredService<IPropertyService>();
var imgSvc = provider.GetRequiredService<IPropertyImageService>();
var traceSvc = provider.GetRequiredService<IPropertyTraceService>();

Console.WriteLine("Seed iniciado...");

var rnd = new Random();
string[] cities = new[] { "Bogotá", "Medellín", "Cali", "Barranquilla", "Cartagena", "Bucaramanga", "Pereira", "Manizales", "Santa Marta", "Cúcuta" };
string[] states = new[] { "Cundinamarca", "Antioquia", "Valle", "Atlántico", "Bolívar", "Santander", "Risaralda", "Caldas", "Magdalena", "Norte de Santander" };
string[] countries = new[] { "CO", "US", "MX", "AR", "CL", "PE", "BR", "ES" };
string[] currencies = new[] { "USD", "COP", "EUR" };

// 1) Owners
var ownerIds = new List<Guid>();
for (int i = 0; i < 50; i++)
{
    var o = await ownerSvc.CreateAsync(new OwnerCreateDto
    {
        Name = $"Owner {i + 1}",
        Address = $"Calle {rnd.Next(1, 300)} #{rnd.Next(1, 99)}-{rnd.Next(1, 99)}",
        Photo = "/uploads/sample-owner.png"
    });
    ownerIds.Add(o.Id);
}
Console.WriteLine($"Owners creados: {ownerIds.Count}");

// 2) Properties
var propertyIds = new List<Guid>();
for (int i = 0; i < 50; i++)
{
    var ownerId = ownerIds[rnd.Next(ownerIds.Count)];
    var city = cities[rnd.Next(cities.Length)];
    var state = states[rnd.Next(states.Length)];
    var country = countries[rnd.Next(countries.Length)];
    var currency = currencies[rnd.Next(currencies.Length)];

    var p = await propSvc.CreateAsync(new PropertyCreateDto
    {
        Name = $"Propiedad {i + 1}",
        Street = $"Av {rnd.Next(1, 500)}",
        City = city,
        State = state,
        Country = country,
        ZipCode = $"{rnd.Next(10000, 99999)}",
        Price = rnd.Next(60_000, 900_000),
        Currency = currency,
        Year = rnd.Next(1990, DateTime.UtcNow.Year + 1),
        Area = Math.Round(rnd.NextDouble() * 200 + 40, 2),
        OwnerId = ownerId,
        Active = rnd.Next(0, 10) != 0
    });
    propertyIds.Add(p.Id);
}
Console.WriteLine($"Properties creadas: {propertyIds.Count}");

// 3) PropertyImages - usaremos URLs apuntando a archivos reales en wwwroot/uploads si existen
// Tomamos algunos nombres conocidos del repo o usamos fallback
string baseUrl = "http://localhost:5106"; // para formar URLs absolutas válidas
string[] knownUploads = new[]
{
    "/uploads/2025/09/18/4361d1dd2fde4755af272fbc03985583.png",
    "/uploads/2025/09/18/1d228d881a47428a858dc3eba5ac8815.txt",
    "/uploads/2025/09/18/24dc2787cf5c4b05ad79dfdcbbddb7bd.txt",
    "/uploads/2025/09/18/f251f3b44f544d8ba18c4e8cfca88738.txt",
    "/uploads/2025/09/18/ed0f8214438d405f8b43d9ab5256513a.txt"
};

var imageIds = new List<Guid>();
for (int i = 0; i < 50; i++)
{
    var propertyId = propertyIds[rnd.Next(propertyIds.Count)];
    var relative = knownUploads[rnd.Next(knownUploads.Length)];
    // Asegurar URL absoluta válida para validación de dominio
    var url = $"{baseUrl}{relative}";
    var img = await imgSvc.CreateAsync(new PropertyImageCreateDto
    {
        PropertyId = propertyId,
        Url = url,
        Description = "Vista frontal",
        Enabled = rnd.Next(0, 10) > 1
    });
    imageIds.Add(img.Id);
}
Console.WriteLine($"PropertyImages creadas: {imageIds.Count}");

// 4) PropertyTraces
var traceIds = new List<Guid>();
for (int i = 0; i < 50; i++)
{
    var propertyId = propertyIds[rnd.Next(propertyIds.Count)];
    var when = DateTime.UtcNow.AddDays(-rnd.Next(0, 365));
    var currency = currencies[rnd.Next(currencies.Length)];
    var t = await traceSvc.CreateAsync(new PropertyTraceCreateDto
    {
        PropertyId = propertyId,
        DateUtc = when,
        Description = "Ajuste de valor",
        Value = rnd.Next(50_000, 950_000),
        Currency = currency
    });
    traceIds.Add(t.Id);
}
Console.WriteLine($"PropertyTraces creadas: {traceIds.Count}");

Console.WriteLine("Seed completado.");
