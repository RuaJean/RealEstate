using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RealEstate.Api.Configurations
{
    public sealed class DefaultExamplesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody?.Content is null) return;

            foreach (var (contentType, media) in operation.RequestBody.Content)
            {
                if (contentType.Contains("application/json") && media.Schema?.Reference?.Id is string schemaId)
                {
                    // Agrega ejemplos simples para algunos DTOs conocidos
                    if (schemaId.EndsWith("PropertyCreateDto"))
                    {
                        media.Example = new OpenApiObject
                        {
                            ["name"] = new OpenApiString("House X"),
                            ["street"] = new OpenApiString("Main 123"),
                            ["city"] = new OpenApiString("Bogota"),
                            ["state"] = new OpenApiString("Cundinamarca"),
                            ["country"] = new OpenApiString("CO"),
                            ["zipCode"] = new OpenApiString("110111"),
                            ["price"] = new OpenApiDouble(100000),
                            ["currency"] = new OpenApiString("USD"),
                            ["year"] = new OpenApiInteger(2019),
                            ["area"] = new OpenApiDouble(120.5),
                            ["ownerId"] = new OpenApiString("00000000-0000-0000-0000-000000000001"),
                            ["active"] = new OpenApiBoolean(true)
                        };
                    }
                    else if (schemaId.EndsWith("PropertyPriceUpdateDto"))
                    {
                        media.Example = new OpenApiObject
                        {
                            ["amount"] = new OpenApiDouble(150000),
                            ["currency"] = new OpenApiString("USD")
                        };
                    }
                    else if (schemaId.EndsWith("OwnerCreateDto"))
                    {
                        media.Example = new OpenApiObject
                        {
                            ["name"] = new OpenApiString("Jane Doe"),
                            ["address"] = new OpenApiString("Av. Siempre Viva 742"),
                            ["photo"] = new OpenApiString("/img/owner.png")
                        };
                    }
                }
                else if (contentType.Contains("multipart/form-data"))
                {
                    // ejemplo de form-data
                    media.Example = new OpenApiObject
                    {
                        ["description"] = new OpenApiString("front view"),
                        ["enabled"] = new OpenApiBoolean(true)
                    };
                }
            }
        }
    }
}


