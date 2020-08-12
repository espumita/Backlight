using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Backlight.Services;
using Microsoft.AspNetCore.Http;
using NJsonSchema;
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
                if (service.Options.ProvidersForType[type].CanCreate()) document.Paths.Add($"/api/type/{type.FullName}", PathFor(type, new List<string>{ OpenApiOperationMethod.Put }));
                if (service.Options.ProvidersForType[type].CanRead()
                    || service.Options.ProvidersForType[type].CanUpdate()
                    || service.Options.ProvidersForType[type].CanDelete()
                ) document.Paths.Add($"/api/type/{type.FullName}/entity/{{id}}", PathFor(type, HttpMethodsFor(type)));
            });
            //document.Components.Schemas.Add(new KeyValuePair<string, JsonSchema>("TEST2", JsonSchema.FromType<Test>()));
            var jsonSchema = document.ToJson();
            await httpContext.Response.WriteAsync(jsonSchema, Encoding.UTF8);
        }

        private List<string> HttpMethodsFor(Type type) {
            var httpMethods = new List<string>();
            var providersForType = service.Options.ProvidersForType[type];
            if (providersForType.CanRead()) httpMethods.Add(OpenApiOperationMethod.Get);
            if (providersForType.CanUpdate()) httpMethods.Add(OpenApiOperationMethod.Post);
            if (providersForType.CanDelete()) httpMethods.Add(OpenApiOperationMethod.Delete);
            return httpMethods;
        }

        private static OpenApiInfo OpenApiDocumentInfo() {
            return new OpenApiInfo {
                Title = "Backlight",
                Description = "Backlight Api Entity types definitions",
                Version = "1.0.0"
            };
        }

        private OpenApiPathItem PathFor(Type type, List<string> httpMethod) {
            var openApiPathItem = new OpenApiPathItem();
            httpMethod.ForEach(method => openApiPathItem.Add(
                new KeyValuePair<string, OpenApiOperation>(method, OperationFor(method, type))));
            return openApiPathItem;
        }

        private OpenApiOperation OperationFor(string httpMethod, Type type) {
            if (httpMethod == OpenApiOperationMethod.Put || httpMethod == OpenApiOperationMethod.Post) {
                var openApiRequestBody = new OpenApiRequestBody();
                openApiRequestBody.IsRequired = true;
                openApiRequestBody.Content.Add(new KeyValuePair<string, OpenApiMediaType>("application/json", new OpenApiMediaType {
                    Schema = JsonSchema.FromType(typeof(string))
                }));
                var putOperation = new OpenApiOperation {
                    OperationId = OperationId(httpMethod, type),
                    RequestBody = openApiRequestBody,
                };
                putOperation.Responses.Add(new KeyValuePair<string, OpenApiResponse>("200", new OpenApiResponse()));
                return putOperation;
            }

            var getOperation = new OpenApiOperation {
                OperationId = OperationId(httpMethod, type)
            };
            getOperation.Responses.Add(new KeyValuePair<string, OpenApiResponse>("200", new OpenApiResponse()));
            return getOperation;


        }

        private static string OperationId(string httpMethod, Type type) {
            return $"{type.FullName}-{httpMethod}";
        }

    }

}