using Serilog;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Infrastructure.Security;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// CORS global configurable por appsettings
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        var corsSection = builder.Configuration.GetSection("Cors");
        var allowAnyOrigin = corsSection.GetValue<bool?>("AllowAnyOrigin") ?? false;
        var allowCredentials = corsSection.GetValue<bool?>("AllowCredentials") ?? false;
        var allowAnyHeader = corsSection.GetValue<bool?>("AllowAnyHeader") ?? true;
        var allowAnyMethod = corsSection.GetValue<bool?>("AllowAnyMethod") ?? true;
        var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var allowedHeaders = corsSection.GetSection("AllowedHeaders").Get<string[]>() ?? Array.Empty<string>();
        var allowedMethods = corsSection.GetSection("AllowedMethods").Get<string[]>() ?? Array.Empty<string>();
        var exposedHeaders = corsSection.GetSection("ExposedHeaders").Get<string[]>() ?? Array.Empty<string>();

        // Orígenes
        if (allowAnyOrigin && !allowCredentials)
        {
            policy.AllowAnyOrigin();
        }
        else if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .SetIsOriginAllowedToAllowWildcardSubdomains();
            if (allowCredentials) policy.AllowCredentials();
            else policy.DisallowCredentials();
        }
        else
        {
            // Sin configuración explícita, usar localhost común para evitar bloqueos accidentales
            policy.WithOrigins("http://localhost", "http://127.0.0.1");
        }

        // Headers
        if (allowAnyHeader) policy.AllowAnyHeader();
        else if (allowedHeaders.Length > 0) policy.WithHeaders(allowedHeaders);

        // Métodos
        if (allowAnyMethod) policy.AllowAnyMethod();
        else if (allowedMethods.Length > 0) policy.WithMethods(allowedMethods);

        // Exponer headers si aplica
        if (exposedHeaders.Length > 0) policy.WithExposedHeaders(exposedHeaders);
    });
});

// AuthN/AuthZ (JWT Bearer)
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// Custom 400 model state response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
            );
        var error = new ErrorResponse
        {
            TraceId = context.HttpContext.TraceIdentifier,
            Error = "Validation",
            Message = "Se encontraron errores de validación",
            Errors = errors
        };
        return new BadRequestObjectResult(error);
    };
});

// DI custom
RealEstate.Api.Configurations.DependencyInjection.AddApiServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RealEstate.Api.Middlewares.ExceptionMiddleware>();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Necesario para WebApplicationFactory en tests de integración
public partial class Program { }


