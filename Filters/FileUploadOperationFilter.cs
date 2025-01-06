using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RecargaApi.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica si el método tiene parámetros de tipo IFormFile
            var fileParam = context.ApiDescription.ActionDescriptor.Parameters
                .FirstOrDefault(p => p.ParameterType == typeof(IFormFile));

            if (fileParam != null)
            {
                // Modifica la operación para agregar el parámetro de archivo correctamente
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "multipart/form-data",
                        new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    { "file", new OpenApiSchema { Type = "string", Format = "binary" } },
                                    { "userId", new OpenApiSchema { Type = "integer" } },
                                    { "monto", new OpenApiSchema { Type = "number", Format = "float" } },
                                    { "descripcion", new OpenApiSchema { Type = "string" } }
                                }
                            }
                        }
                    }
                }
                };
            }
        }
    }
}
