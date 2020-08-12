using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Backlight.Services;
using Microsoft.AspNetCore.Http;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;

namespace Backlight.OpenApi {
    public class OpenApiGenerator {
        private readonly BacklightService service;

        public OpenApiGenerator(BacklightService service) {
            this.service = service;
        }

        public async Task GenerateAsync(HttpContext httpContext) {
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            var document = new OpenApiDocument();
            document.SchemaType = SchemaType.OpenApi3;
            document.Info = OpenApiDocumentInfo();
            service.Options.ProvidersForType.Keys.ToList().ForEach(type => {
                if (service.Options.ProvidersForType[type].CanCreate()) document.Paths.Add($"/api/type/{type.FullName}", PathFor(OpenApiOperationMethod.Put, type));
            });
            //document.Components.Schemas.Add(new KeyValuePair<string, JsonSchema>("TEST2", JsonSchema.FromType<Test>()));
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
            //var openApiSchemaResolver = new OpenApiSchemaResolver(document, new JsonSchemaGeneratorSettings());
            var jsonSchema = document.ToJson();
            await httpContext.Response.WriteAsync(jsonSchema, Encoding.UTF8);
        }

        private static OpenApiInfo OpenApiDocumentInfo() {
            return new OpenApiInfo {
                Title = "Backlight",
                Description = "Backlight Api Entity types definitions",
                Version = "1.0.0"
            };
        }

        private OpenApiPathItem PathFor(string httpMethod, Type type) {
            return new OpenApiPathItem {
                new KeyValuePair<string, OpenApiOperation>(httpMethod, OperationFor(httpMethod, type))
            };
        }

        private OpenApiOperation OperationFor(string httpMethod, Type type) {
            var openApiRequestBody = new OpenApiRequestBody();
            openApiRequestBody.IsRequired = true;
            openApiRequestBody.Content.Add(new KeyValuePair<string, OpenApiMediaType>("application/json", new OpenApiMediaType {
                Schema = JsonSchema.FromType(typeof(string))
            }));
            var openApiOperation = new OpenApiOperation {
                OperationId = $"{type.FullName}-{httpMethod}",
                RequestBody = openApiRequestBody,
            };
            openApiOperation.Responses.Add(new KeyValuePair<string, OpenApiResponse>("200", new OpenApiResponse()));
            return openApiOperation;
        }

    }

}