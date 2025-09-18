using System.Net;
using System.Text.Json;
using RealEstate.Shared.Exceptions;
using RealEstate.Shared.Responses;

namespace RealEstate.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException vex)
            {
                _logger.LogWarning(vex, "Validation error");
                await WriteError(context, HttpStatusCode.BadRequest, "Validation", vex.Message, vex.Errors);
            }
            catch (NotFoundException nfex)
            {
                _logger.LogInformation(nfex, "Not found");
                await WriteError(context, HttpStatusCode.NotFound, "NotFound", nfex.Message);
            }
            catch (BusinessException bex)
            {
                _logger.LogWarning(bex, "Business error");
                await WriteError(context, HttpStatusCode.Conflict, "Business", bex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                await WriteError(context, HttpStatusCode.InternalServerError, "ServerError", "Ha ocurrido un error inesperado");
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode status, string error, string message, IDictionary<string, string[]>? errors = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            var payload = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Error = error,
                Message = message,
                Errors = errors
            };
            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
    }
}


