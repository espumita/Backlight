using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;

namespace Backlight.OpenApi {
    public class OpenApiGenerator {

        public async Task GenerateAsync(HttpContext httpContext) {
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            var document = new OpenApiDocument();
            document.Generator = $"Backlight v{typeof(OpenApiGenerator).GetTypeInfo().Assembly.GetName().Version} NSwag v{OpenApiDocument.ToolchainVersion} (NJsonSchema v{JsonSchema.ToolchainVersion})";
            document.SchemaType = SchemaType.OpenApi3;
            document.Info = new OpenApiInfo {
                Title = "Backlight",
                Description = "Backlight Api Entity types definitions",
                Version = "1.0.0"
            };
            var openApiOperation = new OpenApiOperation {
                OperationId = $"test-put-1"
            };
            openApiOperation.Parameters.Add(new OpenApiParameter {
                Name = "fullName",
                Schema = new JsonSchema {
                    Type = JsonObjectType.String
                },
                Position = 1
            });
            openApiOperation.Responses.Add(new KeyValuePair<string, OpenApiResponse>("200", new OpenApiResponse()));
            var openApiRequestBody = new OpenApiRequestBody();
            openApiRequestBody.Content.Add(new KeyValuePair<string, OpenApiMediaType>("application/json", new OpenApiMediaType {
                Schema = JsonSchema.FromType<Test>()
            }));
            openApiRequestBody.IsRequired = true;
            openApiOperation.RequestBody = openApiRequestBody;
            document.Paths.Add("/api/type/{fullName}", new OpenApiPathItem {
                new KeyValuePair<string, OpenApiOperation>(OpenApiOperationMethod.Put, openApiOperation)
            });
            document.Components.Schemas.Add(new KeyValuePair<string, JsonSchema>("TEST2", JsonSchema.FromType<Test>()));
            //document.Paths.Add("/api/type/{fullName}/entity/{id}", new OpenApiPathItem {
            //    new KeyValuePair<string, OpenApiOperation>(OpenApiOperationMethod.Get, new OpenApiOperation {
            //        OperationId = $"test-get-1"
            //    }),
            //    new KeyValuePair<string, OpenApiOperation>(OpenApiOperationMethod.Post, new OpenApiOperation {
            //        OperationId = $"test-post-1"
            //    }),
            //    new KeyValuePair<string, OpenApiOperation>(OpenApiOperationMethod.Delete, new OpenApiOperation {
            //        OperationId = $"test-delete-1"
            //    })
            //});
            //document.GenerateOperationIds();
            var openApiSchemaResolver = new OpenApiSchemaResolver(document, new JsonSchemaGeneratorSettings());
            var jsonSchema = document.ToJson();
            await httpContext.Response.WriteAsync(jsonSchema, Encoding.UTF8);
        }
    }

    public class Test {
        public string WTF { get; set; }
        public DateTime WTF2 { get; set; }
    }
}